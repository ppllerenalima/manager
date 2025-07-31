using Manager.Domain.Entities;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Responses;

namespace Manager.Domain.Mappers
{
    public class ClienteMapper : IClienteMapper
    {
        public Cliente Map(AddClienteRequest request)
        {
            if (request == null) return null;

            var cliente = new Cliente
            {
                Ruc = request.Ruc,
                Razonsocial = request.Razonsocial,
                Numero = request.Numero,
                Direccion = request.Direccion,
                Image = request.Image,

                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Username = request.Username,
                Password = request.Password,
                Dt_registro = request.FechaRegistro,
                IsInactive = request.IsInactive,
            };

            return cliente;
        }

        public Cliente Map(EditClienteRequest request)
        {
            if (request == null) return null;

            var cliente = new Cliente
            {
                Id = request.Id,
                Ruc = request.Ruc,
                Razonsocial = request.Razonsocial,
                Numero = request.Numero,
                Direccion = request.Direccion,
                Image = request.Image,

                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Username = request.Username,
                Password = request.Password,
                Dt_registro = request.FechaRegistro,
                IsInactive = request.IsInactive,
            };

            return cliente;
        }

        public ClienteResponse Map(Cliente request)
        {
            if (request == null) return null;

            var response = new ClienteResponse
            {
                Id = request.Id,
                Ruc = request.Ruc,
                Razonsocial = request.Razonsocial,
                Numero = request.Numero,
                Direccion = request.Direccion,
                Image = request.Image,

                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Username = request.Username,
                Password = request.Password,
                FechaRegistro = request.Dt_registro,
                IsInactive = request.IsInactive,
            };

            return response;
        }

        public SunatAuthRequest ToSunatAuthRequest(Cliente entity)
        {
            return new SunatAuthRequest
            {
                ClientId = entity.ClientId,
                ClientSecret = entity.ClientSecret,
                Username = entity.Username,
                Password = entity.Password
            };
        }
    }
}