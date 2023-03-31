using JetBrains.Annotations;
using OrleansDashboard;

namespace Vending.Hosting;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class HostingOptions
{
    /// <summary>
    ///     The orleans dashboard options.
    /// </summary>
    public DashboardOptions Dashboard { get; set; } = new();
}
