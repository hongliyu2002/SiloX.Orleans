using Orleans;

namespace Vending.App.Wpf;

public interface IOrleansObject
{
    IClusterClient? ClusterClient { get; set; }
}