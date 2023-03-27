using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Transactions.Contributors;

internal sealed class ConfigureTransactionsOptionsContributor : ConfigureOptionsContributorBase<TransactionsOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Transactions";
}
