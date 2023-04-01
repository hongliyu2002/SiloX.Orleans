using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     A query for a list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineManagerListQuery
    (DateTimeOffsetRange? CreatedAtRange,
     string? CreatedBy,
     DateTimeOffsetRange? LastModifiedAtRange,
     string? LastModifiedBy,
     DateTimeOffsetRange? DeletedAtRange,
     string? DeletedBy,
     bool? IsDeleted,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
