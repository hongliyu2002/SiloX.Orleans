using JetBrains.Annotations;
using Orleans.Configuration;

namespace SiloX.Orleans.Transactions;

/// <summary>
///     Options for the transactional system based on Microsoft Orleans.
/// </summary>
[PublicAPI]
public sealed class TransactionsOptions
{
    /// <summary>
    ///     Max time a group can occupy the lock.
    /// </summary>
    public TimeSpan LockTimeout { get; set; } = TransactionalStateOptions.DefaultLockTimeout;

    /// <summary>
    ///     Max time the transaction manager (TM) will wait for prepare phase to complete.
    /// </summary>
    public TimeSpan PrepareTimeout { get; set; } = TransactionalStateOptions.DefaultPrepareTimeout;

    /// <summary>
    ///     Max time a transaction will wait for the lock to become available.
    /// </summary>
    public TimeSpan LockAcquireTimeout { get; set; } = TransactionalStateOptions.DefaultLockAcquireTimeout;

    /// <summary>
    ///     Frequency for the transaction manager (TM) to ping remote transactions.
    /// </summary>
    public TimeSpan RemoteTransactionPingFrequency { get; set; } = TransactionalStateOptions.DefaultRemoteTransactionPingFrequency;

    /// <summary>
    ///     Delay before retrying confirmation for a transaction.
    /// </summary>
    public TimeSpan ConfirmationRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Max size of the lock group.
    /// </summary>
    public int MaxLockGroupSize { get; set; } = TransactionalStateOptions.DefaultMaxLockGroupSize;

}
