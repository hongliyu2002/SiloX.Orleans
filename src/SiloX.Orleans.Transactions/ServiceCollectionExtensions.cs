using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace SiloX.Orleans.Transactions;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansTransactions(this IServiceCollection services, TransactionsOptions options)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.Configure<TransactionalStateOptions>(transactions =>
                                                                                                    {
                                                                                                        transactions.LockTimeout = options.LockTimeout;
                                                                                                        transactions.PrepareTimeout = options.PrepareTimeout;
                                                                                                        transactions.LockAcquireTimeout = options.LockAcquireTimeout;
                                                                                                        transactions.RemoteTransactionPingFrequency = options.RemoteTransactionPingFrequency;
                                                                                                        transactions.ConfirmationRetryDelay = options.ConfirmationRetryDelay;
                                                                                                        transactions.MaxLockGroupSize = options.MaxLockGroupSize;
                                                                                                    });
                                                 clientBuilder.UseTransactions();
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.Configure<TransactionalStateOptions>(transactions =>
                                                                                        {
                                                                                            transactions.LockTimeout = options.LockTimeout;
                                                                                            transactions.PrepareTimeout = options.PrepareTimeout;
                                                                                            transactions.LockAcquireTimeout = options.LockAcquireTimeout;
                                                                                            transactions.RemoteTransactionPingFrequency = options.RemoteTransactionPingFrequency;
                                                                                            transactions.ConfirmationRetryDelay = options.ConfirmationRetryDelay;
                                                                                            transactions.MaxLockGroupSize = options.MaxLockGroupSize;
                                                                                        });
                                       siloBuilder.UseTransactions();
                                   });
    }
}
