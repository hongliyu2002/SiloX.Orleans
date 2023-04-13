using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.AspNetCore.Components.Server.Contributors;

/// <summary>
///  </summary>
internal sealed class ConfigureBlazorOptionsContributor : ConfigureOptionsContributorBase<BlazorOptions>
{
    /// <inheritdoc />
    public override string SectionName => "AspNetCore:Blazor";
}