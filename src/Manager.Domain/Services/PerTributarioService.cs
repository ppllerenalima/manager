using Manager.Domain.Repositories;
using System.Data.SqlTypes;
using System.Threading;

namespace Manager.Domain.Services
{
    public class PerTributarioService : IPerTributarioService
    {
        private readonly IMapper _perTributarioMapper;
        private readonly IPerTributarioRepository _perTributarioRepository;
        private readonly IComprobanteRepository _comprobanteRepository;

        private readonly ILogger<PerTributarioService> _logger;
        private readonly IZipFileParser _zipFileParser;

        public PerTributarioService(IPerTributarioRepository perTributarioRepository, IMapper perTributarioMapper)
        {
            _perTributarioRepository = perTributarioRepository;
            _perTributarioMapper = perTributarioMapper;
        }

        public PerTributarioService(IPerTributarioRepository perTributarioRepository, IComprobanteRepository comprobanteRepository, IMapper perTributarioMapper, ILogger<PerTributarioService> logger, IZipFileParser zipFileParser)
        {
            _perTributarioRepository = perTributarioRepository;
            _comprobanteRepository = comprobanteRepository;
            _perTributarioMapper = perTributarioMapper;
            _logger = logger;
            _zipFileParser = zipFileParser;
        }

        public async Task<IEnumerable<PerTributarioResponse>> GetPerTributariosAsync()
        {
            var result = await _perTributarioRepository.GetAsync();
            return result
                .Select(x => _perTributarioMapper.Map<PerTributarioResponse>(x));
        }

        public async Task<PerTributarioResponse> GetPerTributarioAsync(GetPerTributarioRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _perTributarioRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _perTributarioMapper.Map<PerTributarioResponse>(entity);
        }

        public async Task<PerTributarioResponse> GetPerTributarioAsync(GetPerTributarioByPeriodoRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await _perTributarioRepository.GetByPredicateAsync(
                predicate: z => z.anio == request.Anio
                && z.mes == request.Mes
                && z.ClienteId == request.ClienteId);

            if (entity == null)
            {
                _logger.LogWarning("📌 No se encontró PerTributario para Cliente {ClienteId}, Año {Anio}, Mes {Mes}",
                    request.ClienteId, request.Anio, request.Mes);

                return null; // o lanzar una excepción controlada
            }

            _logger.LogInformation("✅ PerTributario encontrado: {PerTributarioId}", entity.Id);

            return _perTributarioMapper.Map<PerTributarioResponse>(entity);
        }

        public async Task<PerTributarioResponse> AddPerTributarioAsync(AddPerTributarioRequest request, CancellationToken cancellationToken)
        {
            // iniciamos una transacción en el UnitOfWork
            using var transaction = await _perTributarioRepository.UnitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Crear PerTributario
                var perTributario = _perTributarioMapper.Map<PerTributario>(request);
                var resultPerTributario = await _perTributarioRepository.AddAsync(perTributario, cancellationToken);

                // 2. Procesar comprobantes
                var comprobantes = await ProcesarZipAsync(request.archivoZip, resultPerTributario.Id);
                await _comprobanteRepository.AddAsync(comprobantes, cancellationToken);

                // 3. Guardar cambios en la BD (todos juntos)
                await _perTributarioRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                // 4. Confirmar transacción
                await transaction.CommitAsync(cancellationToken);

                return _perTributarioMapper.Map<PerTributarioResponse>(resultPerTributario);
            }
            catch
            {
                // si ocurre cualquier excepción, revertimos todo
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<ICollection<Comprobante>> ProcesarZipAsync(byte[] archivoZip, Guid Id)
        {
            var lineas = await _zipFileParser.ExtractLinesAsync(archivoZip);

            return lineas.Select(campos => new Comprobante
            {
                Ruc = campos.ElementAtOrDefault(0),
                RazonSocial = campos.ElementAtOrDefault(1),
                Periodo = campos.ElementAtOrDefault(2),
                CarSunat = campos.ElementAtOrDefault(3),
                FechaEmision = campos.ElementAtOrDefault(4),
                FechaVencimiento = campos.ElementAtOrDefault(5),
                TipoComprobante = campos.ElementAtOrDefault(6),
                Serie = campos.ElementAtOrDefault(7),
                Anio = campos.ElementAtOrDefault(8),
                Numero = campos.ElementAtOrDefault(9),
                NumeroFinalRango = campos.ElementAtOrDefault(10),
                TipoDocIdentidad = campos.ElementAtOrDefault(11),
                NumeroDocIdentidad = campos.ElementAtOrDefault(12),
                NombreProveedor = campos.ElementAtOrDefault(13),
                BiGravadoDG = TryDecimal(campos.ElementAtOrDefault(14)),
                IgvDG = TryDecimal(campos.ElementAtOrDefault(15)),
                BiGravadoDGNG = TryDecimal(campos.ElementAtOrDefault(16)),
                IgvDGNG = TryDecimal(campos.ElementAtOrDefault(17)),
                BiGravadoDNG = TryDecimal(campos.ElementAtOrDefault(18)),
                IgvDNG = TryDecimal(campos.ElementAtOrDefault(19)),
                ValorAdqNG = TryDecimal(campos.ElementAtOrDefault(20)),
                Isc = TryDecimal(campos.ElementAtOrDefault(21)),
                Icbper = TryDecimal(campos.ElementAtOrDefault(22)),
                OtrosTributos = TryDecimal(campos.ElementAtOrDefault(23)),
                Total = TryDecimal(campos.ElementAtOrDefault(24)),
                Moneda = campos.ElementAtOrDefault(25),
                TipoCambio = TryDecimal(campos.ElementAtOrDefault(26)),
                FechaEmisionMod = campos.ElementAtOrDefault(27),
                TipoCPMod = campos.ElementAtOrDefault(28),
                SerieCPMod = campos.ElementAtOrDefault(29),
                CodDam = campos.ElementAtOrDefault(30),
                NumeroCPMod = campos.ElementAtOrDefault(31),
                Clasificacion = campos.ElementAtOrDefault(32),
                IdProyecto = campos.ElementAtOrDefault(33),
                PorcPart = TryDecimal(campos.ElementAtOrDefault(34)),
                Imb = TryDecimal(campos.ElementAtOrDefault(35)),
                CarOrigen = campos.ElementAtOrDefault(36),
                Detraccion = campos.ElementAtOrDefault(37),
                TipoNota = campos.ElementAtOrDefault(38),
                EstadoComprobante = campos.ElementAtOrDefault(39),
                Incal = campos.ElementAtOrDefault(40),
                Clus = campos.Skip(41).ToList(), // todos los CLU desde la posición 41

                PerTributarioId = Id
            }).ToList();
        }

        private decimal? TryDecimal(string? value) =>
            decimal.TryParse(value, out var result) ? result : (decimal?)null;
    }
}
