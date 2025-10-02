using FluentValidation.AspNetCore;
using Manager.Domain.Services;
using Manager.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Manager.Domain.Extensions
{
    public static class DependenciesRegistration
    {
        //public static IServiceCollection AddMappers(this IServiceCollection services)
        //{
        //    services
        //        .AddSingleton<ITicketMapper, TicketMapper>()
        //        .AddSingleton<ITokenMapper, TokenMapper>()
        //        .AddSingleton<IClienteMapper, ClienteMapper>();

        //    return services;
        //}

        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            // Registra AutoMapper buscando todos los perfiles en la asamblea donde está ManagerProfile
            services
                .AddAutoMapper(typeof(ManagerProfile).Assembly);

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IClienteService, ClienteService>()
                .AddScoped<IComprobanteService, ComprobanteService>()
                .AddScoped<IConfiguracionGlobalService, ConfiguracionGlobalService>()
                .AddScoped<ICuentaBaseSolService, CuentaBaseSolService>()
                .AddScoped<IGrupoService, GrupoService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IPersonaService, PersonaService>()
                .AddScoped<IPerTributarioService, PerTributarioService>()
                .AddScoped<ITicketService, TicketService>()
                .AddScoped<ITokenBaseService, TokenBaseService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IXmlInvoiceParserService, XmlInvoiceParserService>();


            return services;
        }

        public static IMvcBuilder AddValidation(this IMvcBuilder builder)
        {
            builder
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            return builder;
        }
    }
}