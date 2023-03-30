namespace Vending.Domain.Abstractions;

public static class Constants
{
    public const string LogConsistencyName = "EventStore-EventSourcing-1";
    public const string GrainStorageName = "EventStore-Grain-Storage-1";
    public const string StreamProviderName = "EventStore-Streaming-1";
    
    public const string SnacksNamespace = "Snacks";
    public const string SnacksBroadcastNamespace = "Snacks.Broadcast";
    public const string SnackMachinesNamespace = "SnackMachines";
    public const string SnackMachinesBroadcastNamespace = "SnackMachines.Broadcast";
    public const string PurchasesNamespace = "Purchases";
    public const string PurchasesBroadcastNamespace = "Purchases.Broadcast";
}
