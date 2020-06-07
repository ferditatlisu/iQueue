using iModel.Channels;
using iModel.Queues;
using iModel.Utilities;
using iUtility.Services;
using iUtility.Storages;
using Microsoft.Extensions.Logging;
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
        private readonly Lazy<IQueueService> _queueService;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly Lazy<IStorageService> _storageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;


        public ConsumeProcess(Lazy<IQueueService> queueService, Lazy<IDatabase> lazyRedis, Lazy<IStorageService> storageService, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _queueService = queueService;
            _lazyRedis = lazyRedis;
            _storageService = storageService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Execute(BackgroundQueueChannel channelData)
        {
            var isHealthly = await new ConsumeHealthCheckProcess(_lazyRedis, _httpClientFactory, _logger).Execute(channelData);
            if (!isHealthly)
                return;

            using var consumeGetDataProcess = new ConsumeGetDataProcess(_queueService, _logger);
            //TODO: Circuit Breaker pattern
            List<QueueData> queueDataList = new List<QueueData>();
            for (int i = 0; i < channelData.FetchCount; i++)
            {
                var data = await consumeGetDataProcess.Execute(channelData);
                if (data is null)
                    break;

                queueDataList.Add(data);
            }

            if(queueDataList.Count > 0)
                await new ConsumeSendDataProcess<QueueData>(channelData.ConsumeUrl, _httpClientFactory, _logger).Execute(queueDataList);

            using var storageConnection = _storageService.Value.CreateConnection();
            await storageConnection.BulkInsertProducerEnterLog(queueDataList.Select(x=> x.QueueId), MessageStatus.ConsumerEntry);
        }
    }
}
