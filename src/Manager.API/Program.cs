using Manager.Domain.Services;
using Manager.Infrastructure.ExternalServices.Cpe;

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
    .AddScoped<ITicketRepository, TicketRepository>()
    .AddScoped<ITokenRepository, TokenRepository>()
    .AddScoped<IClienteRepository, ClienteRepository>()
    .AddScoped<IUserRepository, UserRepository>()

    // MAPEADORES Y LÓGICA DE NEGOCIO
    .AddMappers()
    .AddServices()
    .AddExternalServices();

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
            builder.WithOrigins("http://localhost:7440") // puerto de Angular
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// 🔍 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🗄️ Migraciones (Ejecutar migraciones de forma modular)
app.MigrateDatabase<ManagerContext>();

// 🌍 Pipeline HTTP (Configure the HTTP request pipeline.)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();                       // Static files antes de routing
app.UseRouting();                           // Routing primero

app.UseCors(myAllowSpecificOrigins);        // CORS después de routing, antes de auth

app.UseAuthentication();                    // Authenticar antes de autorizar
app.UseAuthorization();

app.UseMiddleware<ResponseTimeMiddlewareAsync>(); // Middleware personalizado

app.MapControllers();                       // Map controllers al final

app.Run();