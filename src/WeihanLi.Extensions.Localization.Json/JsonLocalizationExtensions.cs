using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using WeihanLi.Common;

namespace WeihanLi.Extensions.Localization.Json;

public static class JsonLocalizationExtensions
{
    /// <summary>
    /// Adds services required for application localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
    {
        Guard.NotNull(services);

        services.AddOptions();
        services.AddLogging();
        services.PostConfigure<JsonLocalizationOptions>(options => { options.ResourcesPath ??= "Resources"; });
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        return services;
    }

    /// <summary>
    /// Adds services required for application localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="setupAction">
    /// An <see cref="Action{LocalizationOptions}"/> to configure the <see cref="JsonLocalizationOptions"/>.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
    {
        Guard.NotNull(services);
        Guard.NotNull(setupAction);

        services.Configure(setupAction);            
        AddJsonLocalization(services);

        return services;
    }
}
