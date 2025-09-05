namespace Manager.Infrastructure
{
    public class UserDataSeeder
    {
        private readonly IServiceProvider service;
        private readonly ManagerContext context;

        public UserDataSeeder(IServiceProvider service, ManagerContext context)
        {
            this.service = service;
            this.context = context;

        }

        public async Task SeedAsync()
        {
            var userManager = service.GetRequiredService<UserManager<User>>();
            var roleManager = service.GetRequiredService<RoleManager<Role>>();
            var personaRepository = service.GetRequiredService<IPersonaRepository>();

            const string adminRole = "Administrador";

            // 1️⃣ Seedear roles primero
            await SeedRolesAsync(service);

            // 2️⃣ Verificar si el usuario ya existe
            string userName = "42928945";
            var adminUser = await userManager.FindByNameAsync(userName);

            if (adminUser == null)
            {
                // 3️⃣ Crear y guardar la persona primero
                var persona = new Persona
                {
                    ApePaterno = "Llerena",
                    ApeMaterno = "Lima",
                    Nombre = "Piero",
                    IsInactive = false
                };

                await personaRepository.AddAsync(persona);
                await personaRepository.UnitOfWork.SaveChangesAsync();

                // 4️⃣ Crear usuario con PersonaId
                adminUser = new User
                {
                    UserName = userName,
                    Email = "pp.llerenalima@gmail.com",
                    EmailConfirmed = true,
                    PersonaId = persona.Id
                };

                var result = await userManager.CreateAsync(adminUser, "Aa123*");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"❌ Error creando usuario: {error.Description}");
                    }
                }
            }
        }

        public async Task SeedRolesAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<Role>>();

            // Lista de roles iniciales
            var roles = new List<Role>
            {
                new Role { Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
                new Role { Name = "Usuario", NormalizedName = "USUARIO" },
                new Role { Name = "Supervisor", NormalizedName = "SUPERVISOR" }
    };

            foreach (var role in roles)
            {
                // Verifica por nombre, no por Id
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}
