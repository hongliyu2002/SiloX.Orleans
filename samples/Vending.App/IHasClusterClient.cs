using Orleans;

namespace Vending.App;

public interface IHasClusterClient
{
    IClusterClient? ClusterClient { get; set; }
}