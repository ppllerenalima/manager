namespace Manager.Domain.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepository _repo;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepository clienteRepository, IMapper clienteMapper, ILogger<ClienteService> logger)
        {
            _repo = clienteRepository;
            _mapper = clienteMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClienteResponse>> GetClientesAsync(string search)
        {
            var result = await _repo.GetAsync(search);
            return result
                .Select(x => _mapper.Map<ClienteResponse>(x));
        }

        public async Task<ClienteResponse> GetClienteAsync(GetClienteRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _repo.GetAsync(request.Id);
            return _mapper.Map<ClienteResponse>(entity);
        }

        public async Task<ClienteResponse> AddClienteAsync(AddClienteRequest request)
        {
            var cliente = _mapper.Map<Cliente>(request);

            var result = _repo.Add(cliente);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<ClienteResponse>(result);
        }

        public async Task<ClienteResponse> EditClienteAsync(EditClienteRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _mapper.Map<Cliente>(request);
            var result = _repo.Update(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map<ClienteResponse>(result);
        }

        public async Task<ClienteResponse> DeleteClienteAsync(DeleteClienteRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _repo.GetAsync(request.Id);
            result.IsInactive = true;

            _repo.Update(result);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<ClienteResponse>(result);
        }

        //public async Task<SunatAuthResponse> ObtenerTokenAsync(Guid clienteId)
        //{
        //    var cliente = await _repo.GetAsync(clienteId);
        //    if (cliente == null)
        //        throw new Exception("Cliente no encontrado");

        //    var authRequest = _mapper.ToSunatAuthRequest(cliente);
        //    return await _sireComprasService.AccessTokenAsync(authRequest);
        //}

        //public async Task<string> AceptarPropuestaAsync(string token, string periodo)
        //{
        //    return await _sireComprasService.AceptarPropuestaAsync(token, periodo);
        //}
    }
}