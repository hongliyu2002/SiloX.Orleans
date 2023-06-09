﻿using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Reminders.AdoNet.Contributors;

internal sealed class ConfigureAdoNetRemindersOptionsContributor : ConfigureOptionsContributorBase<AdoNetRemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders:AdoNet";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, AdoNetRemindersOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        context.Log("Configure(AdoNetRemindersOptions)",
                    services =>
                    {
                        services.Configure<AdoNetRemindersOptions>(reminders =>
                                                                   {
                                                                       reminders.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                   });
                    });
    }
}
