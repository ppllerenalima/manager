using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly IMapper _personaMapper;
        private readonly IPersonaRepository _personaRepository;
        private readonly ILogger<PersonaService> _logger;


        public PersonaService(IPersonaRepository personaRepository, IMapper personaMapper)
        {
            _personaRepository = personaRepository;
            _personaMapper = personaMapper;
        }

        public PersonaService(IPersonaRepository personaRepository, IMapper personaMapper, ILogger<PersonaService> logger)
        {
            _personaRepository = personaRepository;
            _personaMapper = personaMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonaResponse>> GetPersonasAsync()
        {
            var result = await _personaRepository.GetAsync();
            return result
                .Select(x => _personaMapper.Map<PersonaResponse>(x));
        }

        public async Task<PersonaResponse> GetPersonaAsync(GetPersonaRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _personaRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _personaMapper.Map<PersonaResponse>(entity);
        }

        public async Task<PersonaResponse> AddPersonaAsync(AddPersonaRequest request)
        {
            var persona = _personaMapper.Map<Persona>(request);

            var result = _personaRepository.AddAsync(persona);
            await _personaRepository.UnitOfWork.SaveChangesAsync();

            return _personaMapper.Map<PersonaResponse>(result.Result);
        }

        public async Task<PersonaResponse> EditPersonaAsync(EditPersonaRequest request)
        {
            var existingRecord = await _personaRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _personaMapper.Map<Persona>(request);
            var result = _personaRepository.UpdateAsync(entity);

            await _personaRepository.UnitOfWork.SaveChangesAsync();
            return _personaMapper.Map<PersonaResponse>(result.Result);
        }

        public async Task<PersonaResponse> DeletePersonaAsync(DeletePersonaRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _personaRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _personaRepository.UpdateAsync(result);
            await _personaRepository.UnitOfWork.SaveChangesAsync();

            return _personaMapper.Map<PersonaResponse>(result);
        }
    }
}
