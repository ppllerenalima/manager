using Manager.Domain.Entities;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Responses;

namespace Manager.Domain.Mappers
{
    public interface IClienteMapper
    {
        Cliente Map(AddClienteRequest request);
        Cliente Map(EditClienteRequest request);
        ClienteResponse Map(Cliente request);

        SunatAuthRequest ToSunatAuthRequest(Cliente entity);
    }
}