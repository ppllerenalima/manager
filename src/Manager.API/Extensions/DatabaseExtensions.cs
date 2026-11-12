using System.Reflection;

namespace Manager.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddManagerContext(this IServiceCollection services, string connectionString)
        {
            return services
                //.AddEntityFrameworkSqlServer()
                .AddDbContext<ManagerContext>(opt =>
                {
                    opt.UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            sqlOptions.UseCompatibilityLevel(120); // 👈 Compatibilidad para SQL Server 2014 y 2016+
                        });
                });
        }
    }
}