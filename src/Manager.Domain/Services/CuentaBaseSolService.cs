namespace Manager.Domain.Services
{
    public class CuentaBaseSolService : ICuentaBaseSolService
    {
        private readonly IMapper _cuentaBaseSolMapper;
        private readonly ICuentaBaseSolRepository _cuentaBaseSolRepository;
        private readonly ILogger<CuentaBaseSolService> _logger;


        public CuentaBaseSolService(ICuentaBaseSolRepository cuentaBaseSolRepository, IMapper cuentaBaseSolMapper)
        {
            _cuentaBaseSolRepository = cuentaBaseSolRepository;
            _cuentaBaseSolMapper = cuentaBaseSolMapper;
        }

        public CuentaBaseSolService(ICuentaBaseSolRepository cuentaBaseSolRepository, IMapper cuentaBaseSolMapper, ILogger<CuentaBaseSolService> logger)
        {
            _cuentaBaseSolRepository = cuentaBaseSolRepository;
            _cuentaBaseSolMapper = cuentaBaseSolMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CuentaBaseSolResponse>> GetCuentaBaseSolsAsync()
        {
            var result = await _cuentaBaseSolRepository.GetAsync();
            return result
                .Select(x => _cuentaBaseSolMapper.Map<CuentaBaseSolResponse>(x));
        }

        public async Task<CuentaBaseSolResponse> GetCuentaBaseSolAsync(GetCuentaBaseSolRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _cuentaBaseSolRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _cuentaBaseSolMapper.Map<CuentaBaseSolResponse>(entity);
        }

        public async Task<CuentaBaseSolResponse> AddCuentaBaseSolAsync(AddCuentaBaseSolRequest request)
        {
            var cuentaBaseSol = _cuentaBaseSolMapper.Map<CuentaBaseSOL>(request);

            var result = _cuentaBaseSolRepository.AddAsync(cuentaBaseSol);
            await _cuentaBaseSolRepository.UnitOfWork.SaveChangesAsync();

            return _cuentaBaseSolMapper.Map<CuentaBaseSolResponse>(result.Result);
        }

        public async Task<CuentaBaseSolResponse> EditCuentaBaseSolAsync(EditCuentaBaseSolRequest request)
        {
            var existingRecord = await _cuentaBaseSolRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _cuentaBaseSolMapper.Map<CuentaBaseSOL>(request);
            var result = _cuentaBaseSolRepository.UpdateAsync(entity);

            await _cuentaBaseSolRepository.UnitOfWork.SaveChangesAsync();
            return _cuentaBaseSolMapper.Map<CuentaBaseSolResponse>(result.Result);
        }

        public async Task<CuentaBaseSolResponse> DeleteCuentaBaseSolAsync(DeleteCuentaBaseSolRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _cuentaBaseSolRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _cuentaBaseSolRepository.UpdateAsync(result);
            await _cuentaBaseSolRepository.UnitOfWork.SaveChangesAsync();

            return _cuentaBaseSolMapper.Map<CuentaBaseSolResponse>(result);
        }
    }
}
