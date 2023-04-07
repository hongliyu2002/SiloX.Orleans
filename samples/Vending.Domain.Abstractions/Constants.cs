namespace Vending.Domain.Abstractions;

public static class Constants
{
    public const string LogConsistencyName = "EventStoreLogConsistency1";
    public const string GrainStorageName = "EventStoreStore1";
    public const string StreamProviderName = "EventStoreStream1";
    
    public const string SnacksNamespace = "Snacks";
    public const string SnacksBroadcastNamespace = "Snacks.Broadcast";
    public const string MachinesNamespace = "Machines";
    public const string MachinesBroadcastNamespace = "Machines.Broadcast";
    public const string PurchasesNamespace = "Purchases";
    public const string PurchasesBroadcastNamespace = "Purchases.Broadcast";
}
