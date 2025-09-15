using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _roleMapper;
        private readonly IRoleRepository _roleRepository;

        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, IMapper roleMapper, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _roleMapper = roleMapper;
            _logger = logger;
        }

        public async Task<RoleResponse> GetRoleAsync(GetRoleRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleRepository.GetAsync(request.Id, cancellationToken);
            return _roleMapper.Map<RoleResponse>(result);
        }

        public async Task<IEnumerable<RoleResponse>> GetRoleAsync(CancellationToken cancellationToken)
        {
            var result = await _roleRepository.GetAsync(cancellationToken);

            return result
                .Select(x => _roleMapper.Map<RoleResponse>(x));
        }

        public async Task<RoleResponse> AddRoleAsync(AddRoleRequest request, CancellationToken cancellationToken)
        {
            // 1️⃣ Mapear el request a Role (entidad)
            var role = _roleMapper.Map<Role>(request);

            // 2️⃣ Guardar el role en el repositorio
            var createdRole = await _roleRepository.AddAsync(role, cancellationToken);

            // 3️⃣ Mapear Role -> RoleResponse
            return _roleMapper.Map<RoleResponse>(createdRole);
        }

        public async Task<RoleResponse> EditRoleAsync(EditRoleRequest request, CancellationToken cancellationToken)
        {
            // 1️⃣ Buscar si existe
            var existingRecord = await _roleRepository.GetAsync(request.Id, cancellationToken);

            if (existingRecord == null)
                throw new KeyNotFoundException($"Role with Id {request.Id} was not found.");

            // 2️⃣ Mapear los cambios al role existente
            _roleMapper.Map(request, existingRecord);

            // 3️⃣ Guardar cambios
            var updatedRole = await _roleRepository.UpdateAsync(existingRecord, cancellationToken);

            // 4️⃣ Retornar DTO de respuesta
            return _roleMapper.Map<RoleResponse>(updatedRole);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _roleRepository.DeleteAsync(id);

            return result; // true si se eliminó correctamente, false si no
        }
    }
}