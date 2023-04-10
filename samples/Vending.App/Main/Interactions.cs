using System;
using System.Collections.Generic;
using Orleans.FluentResults;
using ReactiveUI;

namespace Vending.App;

/// <summary>
///     Provides interactions for the application.
/// </summary>
public static class Interactions
{
    /// <summary>
    ///     Interaction for an exception.
    /// </summary>
    public static Interaction<Exception, ErrorRecoveryOption> Exception { get; } = new();

    /// <summary>
    ///     Interaction for errors.
    /// </summary>
    public static Interaction<IEnumerable<IError>, ErrorRecoveryOption> Errors { get; } = new();

}