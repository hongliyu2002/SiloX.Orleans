﻿using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet.Contributors;

internal sealed class ConfigureAdoNetPersistenceOptionsContributor : ConfigureOptionsContributorBase<AdoNetPersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence:AdoNet";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, AdoNetPersistenceOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(AdoNetPersistenceOptions)",
                    services =>
                    {
                        services.Configure<AdoNetPersistenceOptions>(persistence =>
                                                                    {
                                                                        persistence.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                    });
                    });
    }
}