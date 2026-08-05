namespace WebApp.Api.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }
}
