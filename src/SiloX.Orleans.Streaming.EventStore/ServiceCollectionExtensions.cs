using EventStore.Client;
using Fluxera.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Streaming.EventStore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public const string QueueBalancerModeDoesNotSupport = "Queue balancer mode does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="eventStoreOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansEventStoreStreaming(this IServiceCollection services, StreamingOptions options, EventStoreStreamingOptions eventStoreOptions)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 foreach (var streams in eventStoreOptions.Streams)
                                                 {
                                                     if (eventStoreOptions.ConnectionStrings.TryGetValue(streams.ProviderName, out var connectionString))
                                                     {
                                                         clientBuilder.AddEventStoreStreams(streams.ProviderName,
                                                                                            configurator =>
                                                                                            {
                                                                                                configurator.ConfigureEventStore(builder =>
                                                                                                                                 {
                                                                                                                                     builder.Configure(store =>
                                                                                                                                                       {
                                                                                                                                                           store.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                                                                           if (streams.Username.IsNotNullOrEmpty() && streams.Password.IsNotNullOrEmpty())
                                                                                                                                                           {
                                                                                                                                                               store.Credentials = new UserCredentials(streams.Username!, streams.Password!);
                                                                                                                                                           }
                                                                                                                                                           store.Name = streams.Name.IsNullOrEmpty() ? streams.ProviderName : streams.Name;
                                                                                                                                                           store.Queues = streams.Queues;
                                                                                                                                                       });
                                                                                                                                 });
                                                                                                configurator.ConfigureStreamPubSub(streams.PubSubType);
                                                                                                configurator.ConfigureLifecycle(builder =>
                                                                                                                                {
                                                                                                                                    builder.Configure(lifecycle =>
                                                                                                                                                      {
                                                                                                                                                          lifecycle.InitStage = streams.InitStage;
                                                                                                                                                          lifecycle.StartStage = streams.StartStage;
                                                                                                                                                          lifecycle.StartupState = streams.StartupState;
                                                                                                                                                      });
                                                                                                                                });
                                                                                            });
                                                     }
                                                 }
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var streams in eventStoreOptions.Streams)
                                       {
                                           if (eventStoreOptions.ConnectionStrings.TryGetValue(streams.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreStreams(streams.ProviderName,
                                                                                configurator =>
                                                                                {
                                                                                    configurator.ConfigureEventStore(builder =>
                                                                                                                     {
                                                                                                                         builder.Configure(store =>
                                                                                                                                           {
                                                                                                                                               store.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                                                               if (streams.Username.IsNotNullOrEmpty() && streams.Password.IsNotNullOrEmpty())
                                                                                                                                               {
                                                                                                                                                   store.Credentials = new UserCredentials(streams.Username!, streams.Password!);
                                                                                                                                               }
                                                                                                                                               store.Name = streams.Name.IsNullOrEmpty() ? streams.ProviderName : streams.Name;
                                                                                                                                               store.Queues = streams.Queues;
                                                                                                                                           });
                                                                                                                     });
                                                                                    configurator.ConfigureReceiver(builder =>
                                                                                                                   {
                                                                                                                       builder.Configure(receiver =>
                                                                                                                                         {
                                                                                                                                             receiver.SubscriptionSettings = new PersistentSubscriptionSettings(streams.SubscriptionResolveLinkTos,
                                                                                                                                                                                                                null,
                                                                                                                                                                                                                streams.SubscriptionExtraStatistics,
                                                                                                                                                                                                                streams.SubscriptionMessageTimeout,
                                                                                                                                                                                                                streams.SubscriptionMaxRetryCount,
                                                                                                                                                                                                                streams.SubscriptionLiveBufferSize,
                                                                                                                                                                                                                streams.SubscriptionReadBatchSize,
                                                                                                                                                                                                                streams.SubscriptionHistoryBufferSize,
                                                                                                                                                                                                                streams.SubscriptionCheckPointAfter,
                                                                                                                                                                                                                streams.SubscriptionCheckPointLowerBound,
                                                                                                                                                                                                                streams.SubscriptionCheckPointUpperBound,
                                                                                                                                                                                                                streams.SubscriptionMaxSubscriberCount,
                                                                                                                                                                                                                streams.SubscriptionConsumerStrategyName);
                                                                                                                                             receiver.PrefetchCount = streams.PrefetchCount;
                                                                                                                                             receiver.StartFromNow = streams.StartFromNow;
                                                                                                                                         });
                                                                                                                   });
                                                                                    configurator.ConfigureCachePressuring(builder =>
                                                                                                                          {
                                                                                                                              builder.Configure(pressure =>
                                                                                                                                                {
                                                                                                                                                    pressure.SlowConsumingMonitorFlowControlThreshold = streams.SlowConsumingMonitorFlowControlThreshold;
                                                                                                                                                    pressure.SlowConsumingMonitorPressureWindowSize = streams.SlowConsumingMonitorPressureWindowSize;
                                                                                                                                                    pressure.AveragingCachePressureMonitorFlowControlThreshold = streams.AveragingCachePressureMonitorFlowControlThreshold;
                                                                                                                                                });
                                                                                                                          });
                                                                                    configurator.ConfigureStreamPubSub(streams.PubSubType);
                                                                                    configurator.ConfigureLifecycle(builder =>
                                                                                                                    {
                                                                                                                        builder.Configure(lifecycle =>
                                                                                                                                          {
                                                                                                                                              lifecycle.InitStage = streams.InitStage;
                                                                                                                                              lifecycle.StartStage = streams.StartStage;
                                                                                                                                              lifecycle.StartupState = streams.StartupState;
                                                                                                                                          });
                                                                                                                    });
                                                                                    configurator.ConfigureCacheEviction(builder =>
                                                                                                                        {
                                                                                                                            builder.Configure(eviction =>
                                                                                                                                              {
                                                                                                                                                  eviction.DataMinTimeInCache = streams.DataMinTimeInCache;
                                                                                                                                                  eviction.DataMaxAgeInCache = streams.DataMaxAgeInCache;
                                                                                                                                                  eviction.MetadataMinTimeInCache = streams.MetadataMinTimeInCache;
                                                                                                                                              });
                                                                                                                        });
                                                                                    configurator.ConfigureStatistics(builder =>
                                                                                                                     {
                                                                                                                         builder.Configure(statistic =>
                                                                                                                                           {
                                                                                                                                               statistic.StatisticMonitorWriteInterval = streams.StatisticMonitorWriteInterval;
                                                                                                                                           });
                                                                                                                     });
                                                                                    configurator.ConfigurePullingAgent(builder =>
                                                                                                                       {
                                                                                                                           builder.Configure(agent =>
                                                                                                                                             {
                                                                                                                                                 agent.BatchContainerBatchSize = streams.BatchContainerBatchSize;
                                                                                                                                                 agent.GetQueueMsgsTimerPeriod = streams.GetQueueMsgsTimerPeriod;
                                                                                                                                                 agent.InitQueueTimeout = streams.InitQueueTimeout;
                                                                                                                                                 agent.MaxEventDeliveryTime = streams.MaxEventDeliveryTime;
                                                                                                                                                 agent.StreamInactivityPeriod = streams.StreamInactivityPeriod;
                                                                                                                                             });
                                                                                                                       });
                                                                                    switch (streams.QueueBalancerMode)
                                                                                    {
                                                                                        case EventStoreQueueBalancerMode.ConsistentRing:
                                                                                            configurator.UseConsistentRingQueueBalancer();
                                                                                            break;
                                                                                        case EventStoreQueueBalancerMode.LeaseBased:
                                                                                            configurator.UseLeaseBasedQueueBalancer(builder =>
                                                                                                                                    {
                                                                                                                                        builder.Configure(balancer =>
                                                                                                                                                          {
                                                                                                                                                              balancer.LeaseLength = streams.LeaseLength;
                                                                                                                                                              balancer.LeaseRenewPeriod = streams.LeaseRenewPeriod;
                                                                                                                                                              balancer.LeaseAquisitionPeriod = streams.LeaseAquisitionPeriod;
                                                                                                                                                              balancer.LeaseCategory = streams.LeaseCategory;
                                                                                                                                                          });
                                                                                                                                    });
                                                                                            break;
                                                                                        case EventStoreQueueBalancerMode.DynamicClusterConfigDeployment:
                                                                                            configurator.UseDynamicClusterConfigDeploymentBalancer(streams.SiloMaturityPeriod);
                                                                                            break;
                                                                                        case EventStoreQueueBalancerMode.StaticClusterConfigDeployment:
                                                                                            configurator.UseStaticClusterConfigDeploymentBalancer(streams.SiloMaturityPeriod);
                                                                                            break;
                                                                                        default:
                                                                                            throw new ArgumentOutOfRangeException(nameof(streams.QueueBalancerMode), QueueBalancerModeDoesNotSupport);
                                                                                    }
                                                                                    if (eventStoreOptions.ConnectionStrings.TryGetValue(streams.CheckpointProviderName, out var checkpointerConnectionString))
                                                                                    {
                                                                                        configurator.UseEventStoreCheckpointer(builder =>
                                                                                                                               {
                                                                                                                                   builder.Configure(checkpointer =>
                                                                                                                                                     {
                                                                                                                                                         checkpointer.ClientSettings = EventStoreClientSettings.Create(checkpointerConnectionString);
                                                                                                                                                         if (streams.CheckpointUsername.IsNotNullOrEmpty() && streams.CheckpointPassword.IsNotNullOrEmpty())
                                                                                                                                                         {
                                                                                                                                                             checkpointer.Credentials = new UserCredentials(streams.CheckpointUsername!, streams.CheckpointPassword!);
                                                                                                                                                         }
                                                                                                                                                         checkpointer.PersistInterval = streams.CheckpointPersistInterval;
                                                                                                                                                     });
                                                                                                                               });
                                                                                    }
                                                                                });
                                           }
                                       }
                                   });
    }
}
