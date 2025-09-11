using Manager.Domain.Requests.PerTributario;
using Manager.Domain.Responses.ComprobanteResponses;
using Manager.Domain.Responses.PerTributarioResponses;

namespace Manager.Domain.Mappers
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<Cliente, ClienteResponse>()
                .ForMember(dest => dest.grupo, opt => opt.MapFrom(src => src.Grupo.Descripcion))
                .ReverseMap();
            CreateMap<AddClienteRequest, Cliente>().ReverseMap();
            CreateMap<EditClienteRequest, Cliente>().ReverseMap();

            CreateMap<ComprobanteResponse, Comprobante>().ReverseMap();

            CreateMap<CuentaBaseSolResponse, CuentaBaseSOL>().ReverseMap();
            CreateMap<AddCuentaBaseSolRequest, CuentaBaseSOL>().ReverseMap();
            CreateMap<EditCuentaBaseSolRequest, CuentaBaseSOL>().ReverseMap();

            CreateMap<GrupoResponse, Grupo>().ReverseMap();
            CreateMap<AddGrupoRequest, Grupo>().ReverseMap();
            CreateMap<EditGrupoRequest, Grupo>().ReverseMap();

            CreateMap<PerTributarioResponse, PerTributario>().ReverseMap();
            CreateMap<AddPerTributarioRequest, PerTributario>().ReverseMap();
            CreateMap<EditPerTributarioRequest, PerTributario>().ReverseMap();

            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.ApePaterno, opt => opt.MapFrom(src => src.Persona.ApePaterno))
                .ForMember(dest => dest.ApeMaterno, opt => opt.MapFrom(src => src.Persona.ApeMaterno))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Persona.Nombre))
                .ForMember(dest => dest.NombreCompleto,
                opt => opt.MapFrom(src => $"{src.Persona.ApePaterno} {src.Persona.ApeMaterno}, {src.Persona.Nombre}"))
                .ForMember(dest => dest.IsInactive, opt => opt.MapFrom(src => src.Persona.IsInactive))

                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault())) // 👈 toma el primer rol

                .ReverseMap();
            CreateMap<EditUserRequest, User>().ReverseMap();

            CreateMap<Role, RoleResponse>().ReverseMap();
            CreateMap<EditRoleRequest, Role>().ReverseMap();

            CreateMap<PersonaResponse, Persona>().ReverseMap();
            CreateMap<AddPersonaRequest, Persona>().ReverseMap();
            CreateMap<EditPersonaRequest, Persona>().ReverseMap();
            CreateMap<EditUserRequest, Persona>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PersonaId))
                .ReverseMap();


            CreateMap<TicketResponse, Ticket>().ReverseMap();
            CreateMap<AddTicketRequest, Ticket>().ReverseMap();
            CreateMap<EditTicketRequest, Ticket>().ReverseMap();

            CreateMap<TokenBaseResponse, TokenBase>().ReverseMap();
            CreateMap<AddTokenBaseRequest, TokenBase>().ReverseMap();
            CreateMap<EditTokenBaseRequest, TokenBase>().ReverseMap();

            CreateMap<TokenResponse, Token>().ReverseMap();
            CreateMap<AddTokenRequest, Token>().ReverseMap();
            CreateMap<EditTokenRequest, Token>().ReverseMap();
        }
    }
}