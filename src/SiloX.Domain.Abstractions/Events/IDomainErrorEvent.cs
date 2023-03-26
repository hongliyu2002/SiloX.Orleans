using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     An interface represents a domain error event.
/// </summary>
public interface IDomainErrorEvent : IDomainEvent
{
    /// <summary>
    ///     The error code.
    /// </summary>
    int Code { get; init; }

    /// <summary>
    ///     List of strings containing the reasons for the error.
    /// </summary>
    IImmutableList<string> Reasons { get; init; }
}
