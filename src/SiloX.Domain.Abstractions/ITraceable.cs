namespace SiloX.Domain.Abstractions;

/// <summary>
///     Interface for traceable objects that contains the trace ID, operation timestamp and operator information.
/// </summary>
public interface ITraceable
{
    /// <summary>
    ///     Gets the trace ID associated with this traceable object.
    /// </summary>
    Guid TraceId { get; }
    
    /// <summary>
    ///     Gets the timestamp when the operation is performed.
    /// </summary>
    /// 
    DateTimeOffset OperatedAt { get; }

    /// <summary>
    ///     Gets the operator information who performs the operation.
    /// </summary>
    string OperatedBy { get; }
}
