using Manager.Domain.Requests.Cliente;
using Manager.Domain.Responses;

namespace Manager.Domain.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResponse>> GetClientesAsync(string search);
        Task<ClienteResponse> GetClienteAsync(GetClienteRequest request);
        Task<ClienteResponse> AddClienteAsync(AddClienteRequest request);
        Task<ClienteResponse> EditClienteAsync(EditClienteRequest request);
        Task<ClienteResponse> DeleteClienteAsync(DeleteClienteRequest request);

        //Task<SunatAuthResponse> ObtenerTokenAsync(Guid clienteId);
    }
}