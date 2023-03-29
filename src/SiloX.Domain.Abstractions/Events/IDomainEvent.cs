﻿namespace SiloX.Domain.Abstractions;

/// <summary>
///     An interface represents a domain event.
/// </summary>
public interface IDomainEvent : ITraceable
{
    /// <summary>
    ///     The unique identifier of the original object who raises this event.
    /// </summary>
    string Id { get; init; }

    /// <summary>
    ///     The version of the domain event.
    /// </summary>
    int Version { get; init; }
}
