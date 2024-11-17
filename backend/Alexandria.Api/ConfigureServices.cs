using FluentValidation;

namespace Alexandria.Api;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
    }
}