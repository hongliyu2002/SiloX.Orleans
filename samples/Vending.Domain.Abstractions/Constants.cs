namespace Vending.Domain.Abstractions;

public static class Constants
{
    public const string LogConsistencyName1 = "EventStore-EventSourcing-1";
    public const string LogConsistencyName2 = "EventStore-EventSourcing-2";
    public const string GrainStorageName1 = "EventStore-Grain-Storage-1";
    public const string GrainStorageName2 = "EventStore-Grain-Storage-2";
    public const string StreamProviderName1 = "EventStore-Streaming-1";
    public const string StreamProviderName2 = "EventStore-Streaming-2";
    public const string SnacksNamespace = "Snacks";
    public const string SnackMachinesNamespace = "SnackMachines";
    public const string SnackMachineSnackPurchasesNamespace = "SnackMachineSnackPurchases";
}
