using Manager.Domain.Requests.ConfiguracionGlobal;
using Manager.Domain.Responses.ConfiguracionGlobalResponses;
using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class ConfiguracionGlobalService : IConfiguracionGlobalService
    {
        private readonly IMapper _ConfiguracionGlobalMapper;
        private readonly IConfiguracionGlobalRepository _ConfiguracionGlobalRepository;
        private readonly ILogger<ConfiguracionGlobalService> _logger;

        public ConfiguracionGlobalService(IConfiguracionGlobalRepository ConfiguracionGlobalRepository, IMapper ConfiguracionGlobalMapper)
        {
            _ConfiguracionGlobalRepository = ConfiguracionGlobalRepository;
            _ConfiguracionGlobalMapper = ConfiguracionGlobalMapper;
        }

        public ConfiguracionGlobalService(IConfiguracionGlobalRepository ConfiguracionGlobalRepository, IMapper ConfiguracionGlobalMapper, ILogger<ConfiguracionGlobalService> logger)
        {
            _ConfiguracionGlobalRepository = ConfiguracionGlobalRepository;
            _ConfiguracionGlobalMapper = ConfiguracionGlobalMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ConfiguracionGlobalResponse>> GetConfiguracionGlobalsAsync()
        {
            var result = await _ConfiguracionGlobalRepository.GetAsync();
            return result
                .Select(x => _ConfiguracionGlobalMapper.Map<ConfiguracionGlobalResponse>(x));
        }

        public async Task<ConfiguracionGlobalResponse> GetConfiguracionGlobalAsync(GetConfiguracionGlobalRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _ConfiguracionGlobalRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _ConfiguracionGlobalMapper.Map<ConfiguracionGlobalResponse>(entity);
        }

        public async Task<ConfiguracionGlobalResponse> AddConfiguracionGlobalAsync(AddConfiguracionGlobalRequest request)
        {
            var ConfiguracionGlobal = _ConfiguracionGlobalMapper.Map<ConfiguracionGlobal>(request);

            var result = _ConfiguracionGlobalRepository.AddAsync(ConfiguracionGlobal);
            await _ConfiguracionGlobalRepository.UnitOfWork.SaveChangesAsync();

            return _ConfiguracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result.Result);
        }

        public async Task<ConfiguracionGlobalResponse> EditConfiguracionGlobalAsync(EditConfiguracionGlobalRequest request)
        {
            var existingRecord = await _ConfiguracionGlobalRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _ConfiguracionGlobalMapper.Map<ConfiguracionGlobal>(request);
            var result = _ConfiguracionGlobalRepository.UpdateAsync(entity);

            await _ConfiguracionGlobalRepository.UnitOfWork.SaveChangesAsync();
            return _ConfiguracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result.Result);
        }

        public async Task<ConfiguracionGlobalResponse> DeleteConfiguracionGlobalAsync(DeleteConfiguracionGlobalRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _ConfiguracionGlobalRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _ConfiguracionGlobalRepository.UpdateAsync(result);
            await _ConfiguracionGlobalRepository.UnitOfWork.SaveChangesAsync();

            return _ConfiguracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result);
        }
    }
}
