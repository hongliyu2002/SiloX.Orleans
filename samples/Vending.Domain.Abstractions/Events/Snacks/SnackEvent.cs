using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackEvent(Guid SnackId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainEvent(SnackId.ToString(), Version, TraceId, OperatedAt, OperatedBy)
{
    private readonly Guid _snackId = SnackId;
    public Guid SnackId
    {
        get => _snackId;
        init => _snackId = value;
    }
}
