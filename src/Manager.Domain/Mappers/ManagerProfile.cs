namespace Manager.Domain.Mappers
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<ClienteResponse, Cliente>().ReverseMap();
            CreateMap<AddClienteRequest, Cliente>().ReverseMap();
            CreateMap<EditClienteRequest, Cliente>().ReverseMap();

            CreateMap<CuentaBaseSolResponse, CuentaBaseSOL>().ReverseMap();
            CreateMap<AddCuentaBaseSolRequest, CuentaBaseSOL>().ReverseMap();
            CreateMap<EditCuentaBaseSolRequest, CuentaBaseSOL>().ReverseMap();

            CreateMap<GrupoResponse, Grupo>().ReverseMap();
            CreateMap<AddGrupoRequest, Grupo>().ReverseMap();
            CreateMap<EditGrupoRequest, Grupo>().ReverseMap();

            CreateMap<PersonaResponse, Persona>().ReverseMap();
            CreateMap<AddPersonaRequest, Persona>().ReverseMap();
            CreateMap<EditPersonaRequest, Persona>().ReverseMap();

            CreateMap<TicketResponse, Ticket>().ReverseMap();
            CreateMap<AddTicketRequest, Ticket>().ReverseMap();
            CreateMap<EditTicketRequest, Ticket>().ReverseMap();

            CreateMap<TokenResponse, Token>().ReverseMap();
            CreateMap<AddTokenRequest, Token>().ReverseMap();
            CreateMap<EditTokenRequest, Token>().ReverseMap();
        }
    }
}