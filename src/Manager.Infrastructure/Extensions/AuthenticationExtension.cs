namespace Manager.Infrastructure.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("AuthenticationSettings");
            var settingsTyped = settings.Get<AuthenticationSettings>();

            if (string.IsNullOrWhiteSpace(settingsTyped?.Secret))
                throw new InvalidOperationException("Authentication secret is not configured.");

            var key = Encoding.UTF8.GetBytes(settingsTyped.Secret);

            services.Configure<AuthenticationSettings>(settings);

            //services.AddIdentity<User, IdentityRole>(polices =>
            //{
            //    polices.Password.RequireDigit = true;
            //    polices.Password.RequiredLength = 6;
            //    polices.User.RequireUniqueEmail = true;
            //})
            //    .AddEntityFrameworkStores<ManagerContext>()
            //    .AddDefaultTokenProviders();

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ManagerContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = settingsTyped.ValidateIssuer,
                    ValidateAudience = settingsTyped.ValidateAudience,
                    ValidIssuer = settingsTyped.Issuer,
                    ValidAudience = settingsTyped.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            return services;
        }

    }
}