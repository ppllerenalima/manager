namespace Manager.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly IMapper _mapper;
        private readonly ITicketRepository _repo;
        private readonly ILogger<TicketService> _logger;
        
        private readonly ISireComprasService _sireComprasService;

        public TicketService(ITicketRepository tokenClienteRepository, IMapper tokenClienteMapper, ISireComprasService sireComprasService, ILogger<TicketService> logger)
        {
            _repo = tokenClienteRepository;
            _mapper = tokenClienteMapper;
            _logger = logger;

            _sireComprasService = sireComprasService;
        }

        public async Task<BaseResponseGeneric<TicketResponse>> GetOrGenerateActiveTicketAsync(
            string token,
            GetTicketRequest request,
            bool error2244 = false)
        {
            try
            {
                // 1️⃣ Buscar ticket actual
                var ticket = await GetTicketAsync(request);

                // Si no existe, generamos uno nuevo
                if (!ticket.Success || ticket.Data == null)
                    return await GenerarNuevoTicketAsync(token, request);

                // 2️⃣ Verificar si el ticket está vencido o con error
                bool ticketAntiguo = !string.IsNullOrEmpty(ticket.Data.FecCargaImportacion)
                                     && DateTime.TryParse(ticket.Data.FecCargaImportacion, out var fechaTicket)
                                     && fechaTicket.AddDays(15) < DateTime.Now;

                if (ticket.Data.CodEstadoEnvio == "06" && !ticketAntiguo && !error2244)
                    return ticket; // Ticket válido y reciente

                string numTicket = ticket.Data.NumTicket;

                // 3️⃣ Si el ticket está vencido o hay error 2244 → generar nuevo
                if (ticketAntiguo || error2244)
                {
                    var generarTicket = await _sireComprasService.DescargarPropuestaRCEAsync(
                        new DescargarPropuestaRequest
                        {
                            Token = token,
                            PerTributario = request.perTributario
                        });

                    if (!generarTicket.Success || string.IsNullOrEmpty(generarTicket.Data?.NumTicket))
                        return ResponseFactory.Error<TicketResponse>(
                            generarTicket.Message!,
                            generarTicket.ErrorCode,
                            generarTicket.StatusCode,
                            generarTicket.Details
                        );

                    numTicket = generarTicket.Data.NumTicket;
                }

                // 4️⃣ Consultar estado del ticket en SUNAT
                var estadoTicket = await _sireComprasService.ConsultarEstadoTicketAsync(
                    new ConsultarEstadoTicketRequest
                    {
                        AccessToken = token,
                        PerIni = request.perTributario,
                        PerFin = request.perTributario,
                        Page = request.Page,
                        PerPage = request.PerPage,
                        NumTicket = numTicket
                    });

                if (!estadoTicket.Success || estadoTicket.Data?.Registros == null || !estadoTicket.Data.Registros.Any())
                    return ResponseFactory.Error<TicketResponse>(
                        estadoTicket.Message!,
                        estadoTicket.ErrorCode,
                        estadoTicket.StatusCode,
                        estadoTicket.Details
                    );

                var registro = estadoTicket.Data.Registros.First();
                var detalle = registro.DetalleTicket;
                var archivo = registro.ArchivoReporte?.FirstOrDefault();

                // 5️⃣ Actualizar el ticket con la nueva información
                var updatedTicket = await EditTicketAsync(new EditTicketRequest
                {
                    Id = ticket.Data.Id,
                    CodProceso = registro.CodProceso,
                    CodEstadoProceso = registro.CodEstadoProceso,
                    DesProceso = registro.DesProceso,
                    PerTributario = registro.PerTributario,

                    NumTicket = registro.NumTicket,
                    FecCargaImportacion = detalle?.FecCargaImportacion,
                    HoraCargaImportacion = detalle?.HoraCargaImportacion,
                    CodEstadoEnvio = detalle?.CodEstadoEnvio,
                    DesEstadoEnvio = detalle?.DesEstadoEnvio,

                    CodTipoAchivoReporte = archivo?.CodTipoAchivoReporte,
                    NomArchivoReporte = archivo?.NomArchivoReporte,

                    ClienteId = request.clienteId
                });

                if (!updatedTicket.Success)
                    return ResponseFactory.Error<TicketResponse>(
                        updatedTicket.Message,
                        updatedTicket.ErrorCode,
                        updatedTicket.StatusCode,
                        updatedTicket.Details
                    );

                // 6️⃣ Reglas de negocio según estado
                string estado = registro.DetalleTicket?.CodEstadoEnvio ?? "00";

                return estado switch
                {
                    "06" => updatedTicket, // OK
                    "05" => ResponseFactory.Error<TicketResponse>(
                                $"El ticket está en estado {estado} (rechazado o en error).",
                                "TICKET_ESTADO_05",
                                409),
                    _ when ticketAntiguo => ResponseFactory.Error<TicketResponse>(
                                $"El ticket está en estado {estado} después de 15 días.",
                                "TICKET_EXPIRADO",
                                410),
                    _ => ResponseFactory.Error<TicketResponse>(
                                $"El ticket está en un estado no manejado: {estado}.",
                                "TICKET_ESTADO_DESCONOCIDO",
                                422)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetOrGenerateActiveTicketAsync para cliente {ClienteId}", request?.clienteId);

                return ResponseFactory.Error<TicketResponse>(
                    "Error interno al procesar el ticket.",
                    "GENERAL_EXCEPTION",
                    500,
                    ex.Message
                );
            }
        }

        private async Task<BaseResponseGeneric<TicketResponse>> GetTicketAsync(GetTicketRequest request)
        {
            var response = new BaseResponseGeneric<TicketResponse>();

            try
            {
                // 1️⃣ Consultar ticket en base de datos
                var entity = await _repo.GetAsync(request.clienteId, request.codProceso, request.perTributario);

                if (entity == null)
                    return ResponseFactory.Error<TicketResponse>("No se encontró ningún ticket con los criterios especificados.", "TICKET_NO_ENCONTRADO", 404);

                // 2️⃣ Log y mapeo
                _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity.Id);

                return ResponseFactory.Success<TicketResponse>(_mapper.Map<TicketResponse>(entity), "Ticket recuperado correctamente.", 200);
            }
            catch (Exception ex)
            {
                // 4️⃣ Manejo de excepciones
                _logger.LogError(ex, "Error al obtener el ticket para el cliente {ClienteId}", request?.clienteId);

                return ResponseFactory.Error<TicketResponse>("Ocurrió un error al obtener el ticket.", "EX", 500, ex.Message);
            }
        }

        private async Task<BaseResponseGeneric<TicketResponse>> AddTicketAsync(AddTicketRequest request)
        {
            try
            {
                // 1️⃣ Mapeo del request a la entidad
                var entity = _mapper.Map<Ticket>(request);

                // 2️⃣ Inserción en base de datos
                var result = await _repo.AddAsync(entity);
                await _repo.UnitOfWork.SaveChangesAsync();

                // 3️⃣ Log informativo
                _logger.LogInformation("Ticket creado correctamente con ID {TicketId}", result.Id);

                // 4️⃣ Retornar respuesta exitosa
                return ResponseFactory.Success<TicketResponse>(
                    _mapper.Map<TicketResponse>(result),
                    "Ticket creado correctamente.",
                    201
                );
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error al insertar el ticket en la base de datos.");

                return ResponseFactory.Error<TicketResponse>(
                    "Error al guardar el ticket en la base de datos.",
                    "DB_INSERT_ERROR",
                    500,
                    dbEx.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al agregar el ticket.");

                return ResponseFactory.Error<TicketResponse>(
                    "Ocurrió un error inesperado al crear el ticket.",
                    "EX",
                    500,
                    ex.Message
                );
            }
        }

        private async Task<BaseResponseGeneric<TicketResponse>> EditTicketAsync(EditTicketRequest request)
        {
            try
            {
                // 1️⃣ Obtener registro existente
                var existingRecord = await _repo.GetAsync(request.Id);
                if (existingRecord == null)
                    return ResponseFactory.Error<TicketResponse>(
                        $"No se encontró un ticket con el Id {request.Id}.",
                        "TICKET_NO_ENCONTRADO",
                        404
                    );

                // 2️⃣ Mapear cambios
                var entity = _mapper.Map<Ticket>(request);
                var updatedEntity = await _repo.UpdateAsync(entity);

                // 3️⃣ Guardar cambios
                await _repo.UnitOfWork.SaveChangesAsync();

                // 5️⃣ Log informativo
                _logger.LogInformation("Ticket con ID {TicketId} actualizado correctamente.", updatedEntity.Id);

                // 4️⃣ Respuesta exitosa
                return ResponseFactory.Success<TicketResponse>(
                    _mapper.Map<TicketResponse>(updatedEntity),
                    "Ticket actualizado correctamente.",
                    200
                );
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar el ticket {TicketId}", request?.Id);

                return ResponseFactory.Error<TicketResponse>(
                    "Error de base de datos al actualizar el ticket.",
                    "DB_UPDATE_ERROR",
                    500,
                    dbEx.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el ticket {TicketId}", request?.Id);

                return ResponseFactory.Error<TicketResponse>(
                    "Ocurrió un error inesperado al actualizar el ticket.",
                    "GENERAL_EXCEPTION",
                    500,
                    ex.Message
                );
            }
        }

        private async Task<BaseResponseGeneric<TicketResponse>> GenerarNuevoTicketAsync(string token, GetTicketRequest request)
        {
            try
            {
                // 1️⃣ Generar ticket en SUNAT
                var generarTicket = await _sireComprasService.DescargarPropuestaRCEAsync(
                    new DescargarPropuestaRequest
                    {
                        Token = token,
                        PerTributario = request.perTributario
                    });

                if (generarTicket == null || !generarTicket.Success || string.IsNullOrEmpty(generarTicket.Data?.NumTicket))
                {
                    return ResponseFactory.Error<TicketResponse>(
                        generarTicket?.Message ?? "Error al generar nuevo ticket.",
                        generarTicket?.ErrorCode ?? "GENERAR_TICKET_ERROR",
                        generarTicket?.StatusCode ?? 500,
                        generarTicket?.Details
                    );
                }

                // 2️⃣ Consultar estado del ticket generado
                var estadoTicket = await _sireComprasService.ConsultarEstadoTicketAsync(
                    new ConsultarEstadoTicketRequest
                    {
                        AccessToken = token,
                        PerIni = request.perTributario,
                        PerFin = request.perTributario,
                        Page = request.Page,
                        PerPage = request.PerPage,
                        NumTicket = generarTicket.Data.NumTicket
                    });

                if (estadoTicket == null || !estadoTicket.Success || estadoTicket.Data?.Registros == null || !estadoTicket.Data.Registros.Any())
                {
                    return ResponseFactory.Error<TicketResponse>(
                        estadoTicket?.Message ?? "Error al consultar el estado del ticket.",
                        estadoTicket?.ErrorCode ?? "CONSULTAR_TICKET_ERROR",
                        estadoTicket?.StatusCode ?? 500,
                        estadoTicket?.Details
                    );
                }

                // 3️⃣ Tomar el primer registro
                var registro = estadoTicket.Data.Registros.First();
                var detalle = registro.DetalleTicket!;
                var archivo = registro.ArchivoReporte?.FirstOrDefault();
                var estadoEnvio = detalle.CodEstadoEnvio;

                // 4️⃣ Guardar el ticket en la base de datos
                var nuevoTicket = await AddTicketAsync(new AddTicketRequest
                {
                    CodProceso = registro.CodProceso,
                    CodEstadoProceso = registro.CodEstadoProceso,
                    DesProceso = registro.DesProceso,
                    PerTributario = registro.PerTributario,

                    NumTicket = registro.NumTicket,
                    FecCargaImportacion = detalle.FecCargaImportacion,
                    HoraCargaImportacion = detalle.HoraCargaImportacion,
                    CodEstadoEnvio = estadoEnvio,
                    DesEstadoEnvio = detalle.DesEstadoEnvio,

                    CodTipoAchivoReporte = archivo?.CodTipoAchivoReporte,
                    NomArchivoReporte = archivo?.NomArchivoReporte,

                    ClienteId = request.clienteId
                });

                if (!nuevoTicket.Success)
                {
                    return ResponseFactory.Error<TicketResponse>(
                        nuevoTicket.Message ?? "Error al registrar el nuevo ticket en la base de datos.",
                        nuevoTicket.ErrorCode ?? "ADD_TICKET_ERROR",
                        nuevoTicket.StatusCode,
                        nuevoTicket.Details
                    );
                }

                // 5️⃣ Devolver ticket creado exitosamente
                return ResponseFactory.Success(
                    nuevoTicket.Data!,
                    "Ticket generado y registrado correctamente.",
                    201
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GenerarNuevoTicketAsync para cliente {ClienteId}", request?.clienteId);

                return ResponseFactory.Error<TicketResponse>(
                    "Error interno al generar el nuevo ticket.",
                    "GENERAL_EXCEPTION",
                    500,
                    ex.Message
                );
            }
        }

    }
}