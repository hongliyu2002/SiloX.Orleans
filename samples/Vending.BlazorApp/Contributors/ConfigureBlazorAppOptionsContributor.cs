using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.BlazorApp.Contributors;

internal sealed class ConfigureBlazorAppOptionsContributor : ConfigureOptionsContributorBase<BlazorAppOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:BlazorApp";
}
