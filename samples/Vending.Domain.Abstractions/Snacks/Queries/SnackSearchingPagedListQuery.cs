﻿using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     Represents a query for retrieving a paged list of snacks with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackSearchingPagedListQuery
    (string? SearchCriteria,
     DateTimeOffsetRange? CreatedAtRange,
     string? CreatedBy,
     DateTimeOffsetRange? LastModifiedAtRange,
     string? LastModifiedBy,
     DateTimeOffsetRange? DeletedAtRange,
     string? DeletedBy,
     int? SkipCount,
     int? MaxResultCount,
     IImmutableList<KeyValuePair<string, bool>>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainSearchingPagedListQuery(SearchCriteria, SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
