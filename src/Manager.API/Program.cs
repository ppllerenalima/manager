#region Configuración de Servicios

using Manager.Domain.Services.Interfaces;
using Manager.Infrastructure.FileAdapters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 🔐 CORS
var myAllowSpecificOrigins = "myAllowSpecificOrigins";

// 🔌 Cadena de conexión (forma estándar en .NET)
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// 🔧 Agregar servicios al contenedor (order lógico por tipo)
builder.Services
    // CONTEXTO y DB
    .AddManagerContext(connectionString)

    // REPOSITORIOS
    .AddScoped<IClienteRepository, ClienteRepository>()
    .AddScoped<IComprobanteRepository, ComprobanteRepository>()
    .AddScoped<IConfiguracionGlobalRepository, ConfiguracionGlobalRepository>()
    .AddScoped<ICuentaBaseSolRepository, CuentaBaseSolRepository>()
    .AddScoped<IGrupoRepository, GrupoRepository>()
    .AddScoped<IPersonaRepository, PersonaRepository>()
    .AddScoped<IPerTributarioRepository, PerTributarioRepository>()
    .AddScoped<IRoleRepository, RoleRepository>()
    .AddScoped<ITicketRepository, TicketRepository>()
    .AddScoped<ITokenBaseRepository, TokenBaseRepository>()
    .AddScoped<ITokenRepository, TokenRepository>()
    .AddScoped<IUserRepository, UserRepository>()

    .AddScoped<IZipFileParser, ZipFileParser>()
    .AddScoped<IZipReader, ZipReader>()

    // MAPEADORES Y LÓGICA DE NEGOCIO
    .AddMappers()
    .AddServices()
    .AddExternalServices();

// 🔹 Registramos el Seeder como Transient (se usa una vez al iniciar la app)
builder.Services.AddTransient<UserDataSeeder>();

// 🔹 HttpClient para SUNAT (se agrega fuera del chain principal)
builder.Services.AddHttpClient<ICpeService, CpeService>(client =>
{
    client.BaseAddress = new Uri("https://api-cpe.sunat.gob.pe/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

// 🔹 Controladores y JSON
builder.Services
    .AddControllers()
    .AddValidation()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

// 🔐 JWT u otro mecanismo
builder.Services.AddTokenAuthentication(builder.Configuration);

// ⚙️ Configuración de ModelState manual (Desactivar validación automática de ASP.NET Core)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        builder =>
        {
            builder
            .WithOrigins(
                    "http://localhost:4200",   // Angular en desarrollo
                    "http://192.168.1.254:4200",   // Angular publicado en IIS
                    "http://misire.com"        // dominio real si lo usas
                )
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// 🔍 Swagger
builder.Services.AddEndpointsApiExplorer();

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Manager.API",
        Description = "Documentación de la API Manager",
        Version = "v1"
    });

    // ✅ Incluir comentarios XML
    config.IncludeXmlComments(xmlPath);
});
#endregion

#region Pipeline HTTP

var app = builder.Build();

// 🗄️ Migraciones (Ejecutar migraciones de forma modular)
app.MigrateDatabase<ManagerContext>();

// 🌍 Pipeline HTTP (Configure the HTTP request pipeline.)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    // Habilita Swagger solo en desarrollo o staging
    app.UseSwagger();
    app.UseSwaggerUI();

    //app.UseSwaggerUI(c =>
    //{
    //    // Detecta dinámicamente la ruta base
    //    var swaggerJsonBasePath = string.IsNullOrEmpty(c.RoutePrefix) ? "." : "..";

    //    // Puedes agregar múltiples endpoints si manejas varias versiones
    //    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Manager.API v1");
    //    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v2/swagger.json", "Manager.API v2");

    //    // Opciones de UI
    //    c.RoutePrefix = string.Empty;          // Swagger en la raíz
    //    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Colapsa secciones
    //    c.DefaultModelsExpandDepth(-1);       // Oculta modelos por defecto
    //    c.DisplayRequestDuration();           // Muestra tiempos de respuesta
    //    c.EnableFilter();                     // Permite buscar operaciones
    //});
}

app.UseHttpsRedirection();
app.UseStaticFiles();                       // Static files antes de routing
app.UseRouting();                           // Routing primero

app.UseCors(myAllowSpecificOrigins);        // CORS después de routing, antes de auth

app.UseAuthentication();                    // Authenticar antes de autorizar
app.UseAuthorization();

app.UseMiddleware<ResponseTimeMiddlewareAsync>(); // Middleware personalizado

app.MapControllers();                       // Map controllers al final

await ApplyMigrationsAndSeedDataAsync(app);

app.Run();

#endregion

#region Método Semilla y Migraciones

static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ManagerContext>();

    try
    {
        var pending = await dbContext.Database.GetPendingMigrationsAsync();

        if (pending.Any())
        {
            logger.LogInformation("Aplicando migraciones pendientes...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migraciones aplicadas correctamente.");
        }

        var userDataSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
        await userDataSeeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error aplicando migraciones o seed data.");
        throw; // Opcional: relanzar para evitar levantar la app con BD inconsistente
    }
}
#endregion
