using Manager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Manager.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddManagerContext(this IServiceCollection services, string connectionString)
        {
            return services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<ManagerContext>(opt =>
                {
                    opt.UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                        });
                });
        }
    }
}