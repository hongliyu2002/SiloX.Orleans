using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Reminders.Redis.Contributors;

internal sealed class ConfigureRedisRemindersOptionsContributor : ConfigureOptionsContributorBase<RedisRemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders:Redis";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, RedisRemindersOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(RedisRemindersOptions)",
                    services =>
                    {
                        services.Configure<RedisRemindersOptions>(reminders =>
                                                                  {
                                                                      reminders.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                  });
                    });
    }
}
