using Fluxera.Extensions.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Vending.Domain;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomain(this IServiceCollection services, DomainOptions options)
    {
        return services.AddGuidGenerator(generator => generator.DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd);
    }
}
