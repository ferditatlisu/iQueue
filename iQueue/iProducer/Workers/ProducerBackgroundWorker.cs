using iModel.Customs;
using iProducer.Processes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace iProducer.Workers
{
    public class ProducerBackgroundWorker : BackgroundService
    {
        private readonly LazyQueue<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProducerBackgroundWorker> _logger;

        public ProducerBackgroundWorker(ILogger<ProducerBackgroundWorker> logger, LazyQueue<IConnection> rabbitMq, Lazy<IDatabase> redis, IHttpClientFactory httpClientFactory)
        {
            _lazyRabbitMq = rabbitMq;
            _lazyRedis = redis;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await new ProducerSaveDataProcess(_lazyRabbitMq, _lazyRedis, _logger).Execute();
            _logger.LogInformation("ProducerWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //Tekrar calistirmali!!!
            return base.StopAsync(cancellationToken);
        }
    }
}
