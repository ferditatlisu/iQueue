using iModel.Channels;
using iModel.Queues;
using iModel.Utilities;
using iUtility.Channels;
using iUtility.Keys;
using iUtility.Services;
using iUtility.Storages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class ProducerSaveDataProcess
    {
        public static int QeueuDataCounter;
        public static readonly List<QueueData> QueueDatas;

        private readonly ILogger _logger;
        private readonly Lazy<IQueueService> _lazyQueueService;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly Lazy<IStorageService> _lazyStorageService;
        static ProducerSaveDataProcess()
        {
            QueueDatas = new List<QueueData>();
        }

        public ProducerSaveDataProcess(Lazy<IQueueService> lazyQueueService, Lazy<IDatabase> lazyRedis, Lazy<IStorageService> lazyStorageService, ILogger logger)
        {
            _logger = logger;
            _lazyQueueService = lazyQueueService;
            _lazyRedis = lazyRedis;
            _lazyStorageService = lazyStorageService;
        }

        public void Execute()
        {
            lock (QueueDatas)
            {
                if (QueueDatas.Count > 0)
                { 
                    Do().Wait();
                    QueueDatas.Clear();
                }
            }
        }

        private async Task Do()
        {
            var groupedChannelItems =  QueueDatas.GroupBy(x => x.ChannelName, (key, value) => new { Key = key, Value = value.ToList() }).ToList();
            foreach (var itemByChannel in groupedChannelItems)
            {
                var cacheChannelHelper = new CacheChannelHelper<QueueChannel>(_lazyRedis.Value);
                var channelExist = await cacheChannelHelper.Exist(itemByChannel.Key);
                if(channelExist)
                {
                    var channelData = await cacheChannelHelper.Get(itemByChannel.Key);
                    using var queueConnection = _lazyQueueService.Value.CreateConnection();
                    await queueConnection.BulkInsertData(QueueDatas, channelData);
                    using var storageConnection = _lazyStorageService.Value.CreateConnection();

                    await storageConnection.BulkInsertData(QueueDatas);
                    await storageConnection.BulkInsertProducerEnterLog(QueueDatas.Select(x => x.QueueId), MessageStatus.ProcuderEntry);
                }
            }
        }
    }
}

