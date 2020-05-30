using iModel.Channels;
using iModel.Customs;
using iModel.Keys;
using iModel.Queues;
using iUtility.Channels;
using iUtility.Proxies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{
    public class ConsumeProcess
    {
        private readonly Lazy<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;


        public ConsumeProcess(Lazy<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Execute(BackgroundQueueChannel channelData)
        {
            var isHealthly = await new ConsumeHealthCheckProcess(_lazyRedis, _httpClientFactory, _logger).Execute(channelData);
            if (!isHealthly)
                return;

            var consumeGetDataProcess = new ConsumeGetDataProcess<QueueData>(_lazyRabbitMq, _logger);

            for (int i = 0; i < channelData.FetchCount; i++)
            {
                var data = await consumeGetDataProcess.Execute(channelData);
                if (data is null)
                    break;

                await new ConsumeSendDataProcess<QueueData>(channelData.ConsumeUrl, _httpClientFactory, _logger).Execute(data);
            }
        }
    }
}
