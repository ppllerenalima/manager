using FluentValidation.AspNetCore;
using Manager.Domain.Mappers;
using Manager.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Manager.Domain.Extensions
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services
                .AddSingleton<ITicketMapper, TicketMapper>()
                .AddSingleton<ITokenMapper, TokenMapper>()
                .AddSingleton<IClienteMapper, ClienteMapper>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<ITicketService, TicketService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IClienteService, ClienteService>()
                .AddScoped<IUserService, UserService>();
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