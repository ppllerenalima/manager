using AutoMapper;
using Manager.Domain.Entities;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Token;
using Manager.Domain.Responses;
using Manager.Domain.Responses.TokenResponses;

namespace Manager.Domain.Mappers
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<ClienteResponse, Cliente>().ReverseMap();
            CreateMap<AddClienteRequest, Cliente>().ReverseMap();
            CreateMap<EditClienteRequest, Cliente>().ReverseMap();

            CreateMap<TokenResponse, Token>().ReverseMap();
            CreateMap<AddTokenRequest, Token>().ReverseMap();
            CreateMap<EditTokenRequest, Token>().ReverseMap();
        }
    }
}