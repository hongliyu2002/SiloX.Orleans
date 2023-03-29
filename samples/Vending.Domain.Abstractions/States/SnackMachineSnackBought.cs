namespace Vending.Domain.Abstractions.States;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackBought(Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, DateTimeOffset BoughtAt, string BoughtBy)
{
    public override string ToString()
    {
        return $"SnackMachineSnackBought with MachineId:'{MachineId}' Position:'{Position}' SnackId:'{SnackId}' BoughtPrice:'{BoughtPrice}' BoughtAt:'{BoughtAt}' BoughtBy:'{BoughtBy}'";
    }
}
