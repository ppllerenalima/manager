namespace Manager.Domain.Services.Interfaces
{
    public interface IClienteService
    {
        Task<(IEnumerable<ClienteResponse> Items, int Total)> GetClientesAsync(Guid? grupoId, Guid? userId,  string? search, int pageIndex, int pageSize);
        Task<ClienteResponse> GetClienteAsync(GetClienteRequest request);
        Task<ClienteResponse> AddClienteAsync(AddClienteRequest request);
        Task<ClienteResponse> EditClienteAsync(EditClienteRequest request);
        Task<ClienteResponse> DeleteClienteAsync(DeleteClienteRequest request);
    }
}