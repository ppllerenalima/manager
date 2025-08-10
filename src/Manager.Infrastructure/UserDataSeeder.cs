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

            // 1️⃣ Crear rol si no existe
            const string adminRole = "Administrador";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // 2️⃣ Verificar si el usuario ya existe
            string userName = "42928945";
            var adminUser = await userManager.FindByNameAsync(userName);

            if (adminUser == null)
            {
                // 3️⃣ Crear el usuario
                adminUser = new User
                {
                    UserName = userName,   // Mejor sin espacios para username
                    Email = "pp.llerenalima@gmail.com",
                    EmailConfirmed = true
                };

                // 4️⃣ Crear el usuario con contraseña por defecto
                var result = await userManager.CreateAsync(adminUser, "Aa123*");

                if (result.Succeeded)
                {
                    // 5️⃣ Asignar rol SuperAdmin
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    // Log de errores si falla la creación
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"❌ Error creando usuario: {error.Description}");
                    }
                }
            }
        }


    }
}
