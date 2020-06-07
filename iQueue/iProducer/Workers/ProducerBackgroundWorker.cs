using iModel.Customs;
using iProducer.Processes;
using iUtility.Services;
using iUtility.Storages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace iProducer.Workers
{
    public class ProducerBackgroundWorker : BackgroundService
    {
        private readonly Lazy<IQueueService> _lazyQueueService;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly Lazy<IStorageService> _storageService;

        private readonly ILogger<ProducerBackgroundWorker> _logger;

        public ProducerBackgroundWorker(ILogger<ProducerBackgroundWorker> logger, Lazy<IQueueService> lazyQueueService, Lazy<IDatabase> redis, Lazy<IStorageService> storageService)
        {
            _lazyQueueService = lazyQueueService;
            _lazyRedis = redis;
            _storageService = storageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    new ProducerSaveDataProcess(_lazyQueueService, _lazyRedis, _storageService, _logger).Execute();
                    _logger.LogInformation("ProducerWorker running at: {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Background worker of the Producer get error", e);
                    await Task.Delay(1000);
                }
            }
        }
    }
}
