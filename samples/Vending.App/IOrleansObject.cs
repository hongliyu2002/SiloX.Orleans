using Orleans;

namespace Vending.App;

public interface IOrleansObject
{
    IClusterClient? ClusterClient { get; set; }
}