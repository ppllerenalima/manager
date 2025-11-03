namespace Manager.Domain.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IMapper _clienteMapper;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepository clienteRepository, IMapper clienteMapper, ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository;
            _clienteMapper = clienteMapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<ClienteResponse> Items, int Total)> GetClientesAsync(
            Guid? grupoId,
            Guid? userId,
            string? search,
            int pageIndex,
            int pageSize)
        {
            // Trae el IQueryable ya filtrado
            var queryable = _clienteRepository.Get(
                x => string.IsNullOrEmpty(search) || x.Razonsocial.Contains(search) || x.Ruc.Contains(search),
                x => x.Razonsocial
            );

            if (grupoId is not null) queryable = queryable.Where(z => z.GrupoId == grupoId);

            if (userId is not null) queryable = queryable.Where(z => z.UserId == userId);

            var total = await queryable.CountAsync();

            // Paginación eficiente
            var items = await queryable
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = items.Select(x => _clienteMapper.Map<ClienteResponse>(x));

            return (mapped, total);
        }

        public async Task<ClienteResponse> GetClienteAsync(GetClienteRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _clienteRepository.GetAsync(request.Id);
            return _clienteMapper.Map<ClienteResponse>(entity);
        }

        public async Task<ClienteResponse> AddClienteAsync(AddClienteRequest request)
        {
            var cliente = _clienteMapper.Map<Cliente>(request);

            var result = _clienteRepository.AddAsync(cliente);
            await _clienteRepository.UnitOfWork.SaveChangesAsync();

            return _clienteMapper.Map<ClienteResponse>(result.Result);
        }

        public async Task<ClienteResponse> EditClienteAsync(EditClienteRequest request)
        {
            var existingRecord = await _clienteRepository.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _clienteMapper.Map<Cliente>(request);
            var result = _clienteRepository.UpdateAsync(entity);

            await _clienteRepository.UnitOfWork.SaveChangesAsync();
            return _clienteMapper.Map<ClienteResponse>(result.Result);
        }

        public async Task<BaseResponse> DarPermisoAsync(Guid id)
        {
            var response = new BaseResponse();

            try
            {
                var existingRecord = await _clienteRepository.GetAsync(id);

                if (existingRecord == null)
                {
                    response.Success = false;
                    response.Message = $"No se encontró el cliente con ID {id}.";
                    response.StatusCode = 404;
                    return response;
                }

                // 🔹 Cambiar el estado del permiso
                existingRecord.tienePermiso = true;

                await _clienteRepository.UpdateAsync(existingRecord);
                await _clienteRepository.UnitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Message = "Permiso activado correctamente.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al actualizar el permiso: {ex.Message}";
                response.ErrorCode = "EXCEPTION";
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ClienteResponse> DeleteClienteAsync(DeleteClienteRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _clienteRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _clienteRepository.UpdateAsync(result);
            await _clienteRepository.UnitOfWork.SaveChangesAsync();

            return _clienteMapper.Map<ClienteResponse>(result);
        }
    }
}