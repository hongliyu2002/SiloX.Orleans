using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Queries;

/// <summary>
///     A query for a list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineListQuery(DateTimeOffsetRange? CreatedAtRange, string? CreatedBy, DateTimeOffsetRange? LastModifiedAtRange, string? LastModifiedBy, DateTimeOffsetRange? DeletedAtRange, string? DeletedBy, bool? IsDeleted,
                                           IImmutableList<KeyValuePair<string, bool>> Sortings, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
