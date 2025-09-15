using Manager.Domain.Services.Interfaces;

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

        public async Task<TicketResponse> GetTicketAsync(GetTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.clienteId == null)
                throw new ArgumentNullException(nameof(request.clienteId));
            if (string.IsNullOrWhiteSpace(request.codProceso))
                throw new ArgumentNullException(nameof(request.codProceso));
            if (string.IsNullOrWhiteSpace(request.perTributario))
                throw new ArgumentNullException(nameof(request.perTributario));

            var entity = await _repo.GetAsync(
                request.clienteId,
                request.codProceso,
                request.perTributario);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _mapper.Map<TicketResponse>(entity);
        }

        public async Task<TicketResponse> AddTicketAsync(AddTicketRequest request)
        {
            var ticket = _mapper.Map<Ticket>(request);

            var result = await _repo.AddAsync(ticket);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<TicketResponse>(result);
        }

        public async Task<TicketResponse> EditTicketAsync(EditTicketRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _mapper.Map<Ticket>(request);
            var result = await _repo.UpdateAsync(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map<TicketResponse>(result);
        }

        public async Task<TicketResponse> GetOrGenerateActiveTicketAsync(string token, GetTicketRequest request, bool Error2244 = false)
        {
            var ticket = await GetTicketAsync(request);

            if (ticket is not null)
                return await ProcesarTicketExistenteAsync(token, ticket, request, Error2244);

            return await GenerarNuevoTicketAsync(token, request);
        }

        private async Task<TicketResponse> ProcesarTicketExistenteAsync(string token, TicketResponse ticketEncontrado, GetTicketRequest request, bool Error2244)
        {
            bool ticketAntiguo = EsTicketAntiguo(ticketEncontrado);

            if (ticketEncontrado.CodEstadoEnvio == "06" && !ticketAntiguo && !Error2244)
                return ticketEncontrado;

            string numTicket = ticketEncontrado.NumTicket;

            if (ticketAntiguo || Error2244)
            {
                // 1.Generar ticket
                var generarTicket = await _sireComprasService.DescargarPropuestaRCEAsync(
                    new DescargarPropuestaRequest
                    {
                        Token = token,
                        PerTributario = request.perTributario
                    });

                if (generarTicket == null || string.IsNullOrEmpty(generarTicket.Result.NumTicket))
                    throw new ApplicationException("No se pudo generar un nuevo ticket en SUNAT.");

                numTicket = generarTicket.Result.NumTicket;
            }

            var estadoTicket = await ConsultarEstadoTicketAsync(token, numTicket, request);
            var registro = ObtenerPrimerRegistro(estadoTicket);

            var updatedTicket = await ActualizarTicketAsync(ticketEncontrado.Id, registro, request);

            // Reglas de negocio sobre estado
            string estado = registro.DetalleTicket?.CodEstadoEnvio;
            if (estado == "06")
                return updatedTicket;

            if (ticketAntiguo)
                throw new InvalidOperationException($"El ticket está en estado {estado} después de 15 días.");
            if (estado == "05")
                throw new InvalidOperationException($"El ticket está en un estado {estado}.");
            if (estado != "06")
                throw new InvalidOperationException($"El ticket está en un estado no manejado: {estado}");

            return updatedTicket;
        }

        #region Métodos Privados

        private bool EsTicketAntiguo(TicketResponse ticket)
        {
            return !string.IsNullOrEmpty(ticket.FecCargaImportacion)
                && DateTime.TryParse(ticket.FecCargaImportacion, out var fechaTicket)
                && fechaTicket.AddDays(15) < DateTime.Now;
        }

        private async Task<ConsultarEstadoTicketResponse> ConsultarEstadoTicketAsync(
            string token, string numTicket, GetTicketRequest request)
        {
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

            if (estadoTicket == null ||
                !estadoTicket.EsExito ||
                estadoTicket.Result?.Registros == null ||
                !estadoTicket.Result.Registros.Any())
            {
                var mensajeError = estadoTicket?.Errores != null && estadoTicket.Errores.Any()
                    ? string.Join(" | ", estadoTicket.Errores.Select(e => $"[{e.Cod}] {e.Msg}"))
                    : "Error al consultar ticket en SUNAT.";

                throw new ApplicationException(mensajeError);
            }

            return estadoTicket;
        }

        private Registro ObtenerPrimerRegistro(ConsultarEstadoTicketResponse estadoTicket)
        {
            return estadoTicket.Result.Registros.First();
        }

        private async Task<TicketResponse> ActualizarTicketAsync(
            Guid ticketId, Registro registro, GetTicketRequest request)
        {
            var detalle = registro.DetalleTicket;
            var archivo = registro.ArchivoReporte?.FirstOrDefault();

            return await EditTicketAsync(new EditTicketRequest
            {
                Id = ticketId,
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
        }

        #endregion

        private async Task<TicketResponse> GenerarNuevoTicketAsync(string token, GetTicketRequest request)
        {
            // 1. Generar ticket
            var generarTicket = await _sireComprasService.DescargarPropuestaRCEAsync(
                new DescargarPropuestaRequest
                {
                    Token = token,
                    PerTributario = request.perTributario
                });

            if (generarTicket == null || string.IsNullOrEmpty(generarTicket.Result.NumTicket))
                throw new ApplicationException("No se pudo generar un nuevo ticket en SUNAT.");

            // 2. Consultar estado del ticket generado
            var estadoTicket = await _sireComprasService.ConsultarEstadoTicketAsync(
                new ConsultarEstadoTicketRequest
                {
                    AccessToken = token,
                    PerIni = request.perTributario,
                    PerFin = request.perTributario,
                    Page = request.Page,
                    PerPage = request.PerPage,
                    NumTicket = generarTicket.Result.NumTicket
                });

            if (estadoTicket?.EsExito != true || estadoTicket.Result?.Registros == null || !estadoTicket.Result.Registros.Any())
            {
                throw new ApplicationException("No se pudo consultar el estado del ticket en SUNAT.");
            }

            // 3. Tomar el primer registro
            var registro = estadoTicket.Result.Registros.First();
            var detalle = registro.DetalleTicket;
            var archivo = registro.ArchivoReporte?.FirstOrDefault();
            var estadoEnvio = detalle?.CodEstadoEnvio;

            // 4. Crear ticket en BD
            var nuevoTicket = await AddTicketAsync(new AddTicketRequest
            {
                CodProceso = registro.CodProceso,
                CodEstadoProceso = registro.CodEstadoProceso,
                DesProceso = registro.DesProceso,
                PerTributario = registro.PerTributario,

                NumTicket = registro.NumTicket,
                FecCargaImportacion = detalle?.FecCargaImportacion,
                HoraCargaImportacion = detalle?.HoraCargaImportacion,
                CodEstadoEnvio = estadoEnvio,
                DesEstadoEnvio = detalle?.DesEstadoEnvio,

                CodTipoAchivoReporte = archivo?.CodTipoAchivoReporte,
                NomArchivoReporte = archivo?.NomArchivoReporte,

                ClienteId = request.clienteId,
            });

            return nuevoTicket;
        }
    }
}