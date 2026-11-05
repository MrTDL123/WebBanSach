namespace WebApp.Api.Utilities
{
    public class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            await DbInitializer.SeedAsync(serviceProvider);
        }
    }
}
