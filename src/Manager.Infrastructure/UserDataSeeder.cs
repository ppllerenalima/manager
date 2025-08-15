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
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var personaRepository = service.GetRequiredService<IPersonaRepository>();

            const string adminRole = "Administrador";

            // 1️⃣ Crear rol si no existe
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

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
                    PersonaId = persona.Id // 🔹 Aquí ya existe el ID
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
    }
}
