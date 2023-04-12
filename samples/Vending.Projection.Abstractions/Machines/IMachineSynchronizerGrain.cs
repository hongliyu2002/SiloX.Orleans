namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     The grain for machines synchronization.
/// </summary>
public interface IMachineSynchronizerGrain : IGrainWithStringKey, IRemindable
{
    /// <summary>
    ///     Asynchronously starts a reminder.
    /// </summary>
    /// <param name="reminderName">The name of the reminder.</param>
    /// <param name="dueTime">The amount of time to delay before the <see cref="T:Orleans.Runtime.IRemindable.ReceiveReminder" /> method is called for the first time.</param>
    /// <param name="period">The time interval between calls to the <see cref="T:Orleans.Runtime.IRemindable.ReceiveReminder" /> method. <see cref="F:System.TimeSpan.Zero" /> indicates that the reminder should be raised only once.</param>
    Task StartReminder(string reminderName, TimeSpan dueTime, TimeSpan period);

    /// <summary>
    ///     Asynchronously stops a reminder.
    /// </summary>
    /// <param name="reminderName">The name of the reminder.</param>
    Task StopReminder(string reminderName);

    /// <summary>
    ///     Asynchronously synchronizes a machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    Task SyncAsync(Guid machineId);

    /// <summary>
    ///     Asynchronously synchronizes all machines that do not exist.
    /// </summary>
    Task SyncDifferencesAsync();

    /// <summary>
    ///     Asynchronously synchronizes all machines.
    /// </summary>
    Task SyncAllAsync();
}