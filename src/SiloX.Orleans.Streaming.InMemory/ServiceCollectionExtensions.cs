using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Streaming.InMemory;

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
    /// <param name="inMemoryOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryStreaming(this IServiceCollection services, StreamingOptions options, InMemoryStreamingOptions inMemoryOptions)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 foreach (var streams in inMemoryOptions.Streams)
                                                 {
                                                     clientBuilder.AddMemoryStreams(streams.ProviderName,
                                                                                    configurator =>
                                                                                    {
                                                                                        configurator.ConfigureStreamPubSub(streams.PubSubType);
                                                                                        configurator.ConfigurePartitioning(streams.NumQueues);
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
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var streams in inMemoryOptions.Streams)
                                       {
                                           siloBuilder.AddMemoryStreams(streams.ProviderName,
                                                                        configurator =>
                                                                        {
                                                                            configurator.ConfigureStreamPubSub(streams.PubSubType);
                                                                            configurator.ConfigurePartitioning(streams.NumQueues);
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
                                                                                case InMemoryQueueBalancerMode.ConsistentRing:
                                                                                    configurator.UseConsistentRingQueueBalancer();
                                                                                    break;
                                                                                case InMemoryQueueBalancerMode.LeaseBased:
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
                                                                                case InMemoryQueueBalancerMode.DynamicClusterConfigDeployment:
                                                                                    configurator.UseDynamicClusterConfigDeploymentBalancer(streams.SiloMaturityPeriod);
                                                                                    break;
                                                                                case InMemoryQueueBalancerMode.StaticClusterConfigDeployment:
                                                                                    configurator.UseStaticClusterConfigDeploymentBalancer(streams.SiloMaturityPeriod);
                                                                                    break;
                                                                                default:
                                                                                    throw new ArgumentOutOfRangeException(nameof(streams.QueueBalancerMode), QueueBalancerModeDoesNotSupport);
                                                                            }
                                                                        });
                                       }
                                   });
    }
}
