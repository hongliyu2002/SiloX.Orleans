namespace Vending.App.Wpf.Services;

public interface IClusterClientReady
{
    TaskCompletionSource<IClusterClient> ClusterClient { get; }

    void SetClusterClient(IClusterClient clusterClient);
}

public sealed class ClusterClientReady : IClusterClientReady
{
    public TaskCompletionSource<IClusterClient> ClusterClient { get; } = new();

    public void SetClusterClient(IClusterClient clusterClient)
    {
        ClusterClient.SetResult(clusterClient);
    }
}