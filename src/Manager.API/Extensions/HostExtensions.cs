using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Manager.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IHostEnvironment>();

            if (env.IsEnvironment("Development") || env.IsEnvironment("Integration"))
                return host;

            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(12)
                });

            var context = services.GetRequiredService<TContext>();
            retry.Execute(() => context.Database.Migrate());

            return host;
        }
    }
}
