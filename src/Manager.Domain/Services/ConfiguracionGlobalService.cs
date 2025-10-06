namespace Manager.Domain.Services
{
    public class ConfiguracionGlobalService : IConfiguracionGlobalService
    {
        private readonly IMapper _configuracionGlobalMapper;
        private readonly IConfiguracionGlobalRepository _configuracionGlobalRepository;
        private readonly ILogger<ConfiguracionGlobalService> _logger;

        public ConfiguracionGlobalService(IConfiguracionGlobalRepository configuracionGlobalRepository, IMapper configuracionGlobalMapper)
        {
            _configuracionGlobalRepository = configuracionGlobalRepository;
            _configuracionGlobalMapper = configuracionGlobalMapper;
        }

        public ConfiguracionGlobalService(IConfiguracionGlobalRepository configuracionGlobalRepository, IMapper configuracionGlobalMapper, ILogger<ConfiguracionGlobalService> logger)
        {
            _configuracionGlobalRepository = configuracionGlobalRepository;
            _configuracionGlobalMapper = configuracionGlobalMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ConfiguracionGlobalResponse>> GetConfiguracionGlobalsAsync()
        {
            var result = await _configuracionGlobalRepository.GetAsync();
            return result
                .Select(x => _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(x));
        }

        public async Task<ConfiguracionGlobalResponse> GetConfiguracionGlobalFirstOrDefaultAsync()
        {
            var entity = await _configuracionGlobalRepository.GetFirstOrDefaultAsync();

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(entity);
        }

        public async Task<ConfiguracionGlobalResponse> GetConfiguracionGlobalAsync(GetConfiguracionGlobalRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _configuracionGlobalRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(entity);
        }

        public async Task<ConfiguracionGlobalResponse> AddConfiguracionGlobalAsync(AddConfiguracionGlobalRequest request)
        {
            var ConfiguracionGlobal = _configuracionGlobalMapper.Map<ConfiguracionGlobal>(request);

            var result = _configuracionGlobalRepository.AddAsync(ConfiguracionGlobal);
            await _configuracionGlobalRepository.UnitOfWork.SaveChangesAsync();

            return _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result.Result);
        }

        public async Task<ConfiguracionGlobalResponse> EditConfiguracionGlobalAsync(EditConfiguracionGlobalRequest request)
        {
            var existingRecord = await _configuracionGlobalRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _configuracionGlobalMapper.Map<ConfiguracionGlobal>(request);
            var result = _configuracionGlobalRepository.UpdateAsync(entity);

            await _configuracionGlobalRepository.UnitOfWork.SaveChangesAsync();
            return _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result.Result);
        }

        public async Task<ConfiguracionGlobalResponse> DeleteConfiguracionGlobalAsync(DeleteConfiguracionGlobalRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _configuracionGlobalRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _configuracionGlobalRepository.UpdateAsync(result);
            await _configuracionGlobalRepository.UnitOfWork.SaveChangesAsync();

            return _configuracionGlobalMapper.Map<ConfiguracionGlobalResponse>(result);
        }
    }
}
