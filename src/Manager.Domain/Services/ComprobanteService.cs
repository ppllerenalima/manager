using Manager.Domain.Repositories;
using Manager.Domain.Responses.ComprobanteResponses;
using System.Data.SqlTypes;
using System.Threading;

namespace Manager.Domain.Services
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IMapper _comprobanteMapper;
        private readonly IComprobanteRepository _comprobanteRepository;

        private readonly ILogger<ComprobanteService> _logger;

        public ComprobanteService(IComprobanteRepository comprobanteRepository, IMapper comprobanteMapper, ILogger<ComprobanteService> logger)
        {
            _comprobanteRepository = comprobanteRepository;
            _comprobanteMapper = comprobanteMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync()
        {
            var result = await _comprobanteRepository.GetAsync();
            return result
                .Select(x => _comprobanteMapper.Map<ComprobanteResponse>(x));
        }

    }
}
