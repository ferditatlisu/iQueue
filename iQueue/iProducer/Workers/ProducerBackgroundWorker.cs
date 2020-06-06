using iModel.Customs;
using iProducer.Processes;
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
        private readonly Lazy<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly Lazy<IQueueStorage> _storageService;

        private readonly ILogger<ProducerBackgroundWorker> _logger;

        public ProducerBackgroundWorker(ILogger<ProducerBackgroundWorker> logger, Lazy<IConnection> rabbitMq, Lazy<IDatabase> redis, Lazy<IQueueStorage> storageService)
        {
            _lazyRabbitMq = rabbitMq;
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
                    new ProducerSaveDataProcess(_lazyRabbitMq, _lazyRedis, _storageService, _logger).Execute();
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
