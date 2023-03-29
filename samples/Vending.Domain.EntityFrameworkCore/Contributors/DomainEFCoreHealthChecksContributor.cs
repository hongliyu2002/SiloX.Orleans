using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Vending.Domain.EntityFrameworkCore.Contributors;

internal sealed class DomainEFCoreHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var efCoreOptions = context.Services.GetOptions<DomainEFCoreOptions>();
        efCoreOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        if (efCoreOptions.ConnectionStrings.TryGetValue(efCoreOptions.ProviderName, out var connectionString))
        {
            builder.AddSqlServer(connectionString, "SELECT 1;", efCoreOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        }
    }
}
