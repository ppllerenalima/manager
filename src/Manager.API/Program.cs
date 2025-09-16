#region Configuración de Servicios

using Manager.Domain.Services.Interfaces;
using Manager.Infrastructure.FileAdapters;

var builder = WebApplication.CreateBuilder(args);

// 🔐 CORS
var myAllowSpecificOrigins = "myAllowSpecificOrigins";

// 🔌 Cadena de conexión
var connectionString = builder.Configuration.GetSection("DataSource:ConnectionString").Value;

// 🔧 Agregar servicios al contenedor (order lógico por tipo)
builder.Services
    // CONTEXTO y DB
    .AddManagerContext(connectionString)

    // REPOSITORIOS
    .AddScoped<IClienteRepository, ClienteRepository>()
    .AddScoped<IComprobanteRepository, ComprobanteRepository>()
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
                    "http://localhost:8080",   // Angular publicado en IIS
                    "http://misire.com"        // dominio real si lo usas
                )
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// 🔍 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

    var dbContext = scope.ServiceProvider.GetRequiredService<ManagerContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    var userDataSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await userDataSeeder.SeedAsync();
}

#endregion
