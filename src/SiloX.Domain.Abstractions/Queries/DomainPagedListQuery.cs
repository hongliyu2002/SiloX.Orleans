﻿using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     An abstract class that represents a query for a domain list with paging feature.
///     It contains start position, maximum number of items to return, sorting criteria and traceability information.
/// </summary>
/// <param name="SkipCount">An integer representing the number of items to skip from the beginning of the list.</param>
/// <param name="MaxResultCount">An integer representing the maximum number of items to return.</param>
/// <param name="OrderBy">TAn immutable list of key-value pairs representing the sorting criteria.</param>
/// <param name="TraceId">The unique identifier for the trace.</param>
/// <param name="OperatedAt">The date and time when the operation was performed.</param>
/// <param name="OperatedBy">The name of the operator who performed the operation.</param>
[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record DomainPagedListQuery
    (int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(OrderBy, TraceId, OperatedAt, OperatedBy);
