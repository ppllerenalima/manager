namespace Manager.Domain.Services
{
    public class GrupoService : IGrupoService
    {
        private readonly IMapper _grupoMapper;
        private readonly IGrupoRepository _grupoRepository;
        private readonly ILogger<GrupoService> _logger;

        public GrupoService(IGrupoRepository grupoRepository, IMapper grupoMapper)
        {
            _grupoRepository = grupoRepository;
            _grupoMapper = grupoMapper;
        }

        public GrupoService(IGrupoRepository grupoRepository, IMapper grupoMapper, ILogger<GrupoService> logger)
        {
            _grupoRepository = grupoRepository;
            _grupoMapper = grupoMapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<GrupoResponse> Items, int Total)> GetGruposAsync(
            string? search,
            int pageIndex,
            int pageSize)
        {
            // Trae el IQueryable ya filtrado
            var queryable = _grupoRepository.Get(
                x => string.IsNullOrEmpty(search) || x.Descripcion.Contains(search),
                x => x.Descripcion
            );

            var total = await queryable.CountAsync();

            // Paginación eficiente
            var items = await queryable
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = items.Select(x => _grupoMapper.Map<GrupoResponse>(x));

            return (mapped, total);
        }

        public async Task<GrupoResponse> GetGrupoAsync(GetGrupoRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _grupoRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _grupoMapper.Map<GrupoResponse>(entity);
        }

        public async Task<GrupoResponse> AddGrupoAsync(AddGrupoRequest request)
        {
            var grupo = _grupoMapper.Map<Grupo>(request);

            var result = _grupoRepository.AddAsync(grupo);
            await _grupoRepository.UnitOfWork.SaveChangesAsync();

            return _grupoMapper.Map<GrupoResponse>(result.Result);
        }

        public async Task<GrupoResponse> EditGrupoAsync(EditGrupoRequest request)
        {
            var existingRecord = await _grupoRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _grupoMapper.Map<Grupo>(request);
            var result = _grupoRepository.UpdateAsync(entity);

            await _grupoRepository.UnitOfWork.SaveChangesAsync();
            return _grupoMapper.Map<GrupoResponse>(result.Result);
        }

        public async Task<GrupoResponse> DeleteGrupoAsync(DeleteGrupoRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _grupoRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _grupoRepository.UpdateAsync(result);
            await _grupoRepository.UnitOfWork.SaveChangesAsync();

            return _grupoMapper.Map<GrupoResponse>(result);
        }
    }
}
