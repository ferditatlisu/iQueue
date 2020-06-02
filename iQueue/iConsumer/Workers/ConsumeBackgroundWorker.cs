using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using StackExchange.Redis;
using RabbitMQ.Client;
using iUtility.Channels;
using iModel.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using iConsumer.Consumers;
using iUtility.Logs;

namespace iConsumer.Workers
{
    public class ConsumeBackgroundWorker : BackgroundService
    {
        private readonly Lazy<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ConsumeBackgroundWorker> _logger;

        public ConsumeBackgroundWorker(ILogger<ConsumeBackgroundWorker> logger, Lazy<IConnection> rabbitMq, Lazy<IDatabase> redis, IHttpClientFactory httpClientFactory)
        {
            _lazyRabbitMq = rabbitMq;
            _lazyRedis = redis;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var channels = await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Get();
                    if (channels?.Count == 0)
                    {
                        //SlackLog.SendMessage("No channels");
                        continue;
                    }

                    if (!(channels is null) && channels.Count > 0)
                    {
                        var cacheBackgroundChannelHelper = new CacheBackgroundChannelHelper(_lazyRedis.Value);
                        var backgroundChannels = await cacheBackgroundChannelHelper.Get();
                        new CacheChannelCompare(_lazyRedis.Value).Execute(ref channels, ref backgroundChannels);
                        Parallel.ForEach(backgroundChannels, backgroundChannel =>
                        {
                            var needExecute = backgroundChannel.ExecutedDate.AddSeconds(backgroundChannel.ExecuteEverySecond) < DateTime.UtcNow;
                            if (needExecute)
                            {
                                backgroundChannel.ExecutedDate = DateTime.UtcNow;
                                cacheBackgroundChannelHelper.Update(backgroundChannel).Wait();
                                new ConsumeProcess(_lazyRabbitMq, _lazyRedis, _httpClientFactory, _logger).Execute(backgroundChannel).Wait();
                            }
                        });
                    }

                    _logger.LogInformation("ConsumerWorker running at: {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Background worker of the Consumer get error", e);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
