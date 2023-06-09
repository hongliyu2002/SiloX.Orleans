﻿namespace Vending.Domain.Abstractions;

public static class Constants
{
    public const string LogConsistencyName = "EventStoreLogConsistency1";
    public const string GrainStorageName = "EventStoreStore1";
    public const string StreamProviderName = "InMemoryStream1";
    
    public const string SnacksNamespace = "Snacks";
    public const string SnacksBroadcastNamespace = "Snacks.Broadcast";
    public const string SnackInfosNamespace = "SnackInfos";
    public const string SnackInfosBroadcastNamespace = "SnackInfos.Broadcast";
    public const string SnackInfosSyncDifferencesReminderName = "SnackInfosSyncDifferencesReminder";
    public const string SnackInfosSyncAllReminderName = "SnackInfosSyncAllReminder";

    public const string MachinesNamespace = "Machines";
    public const string MachinesBroadcastNamespace = "Machines.Broadcast";
    public const string MachineInfosNamespace = "MachineInfos";
    public const string MachineInfosBroadcastNamespace = "MachineInfos.Broadcast";
    public const string MachineInfosSyncDifferencesReminderName = "MachineInfosSyncDifferencesReminder";
    public const string MachineInfosSyncAllReminderName = "MachineInfosSyncAllReminder";
    
    public const string PurchasesNamespace = "Purchases";
    public const string PurchasesBroadcastNamespace = "Purchases.Broadcast";
    public const string PurchaseInfosNamespace = "PurchaseInfos";
    public const string PurchaseInfosBroadcastNamespace = "PurchaseInfos.Broadcast";
    public const string PurchaseInfosSyncDifferencesReminderName = "PurchaseInfosSyncDifferencesReminder";
    public const string PurchaseInfosSyncAllReminderName = "PurchaseInfosSyncAllReminder";
}
