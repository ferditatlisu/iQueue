﻿using iModel.Channels;
using iModel.Queues;
using iUtility.Channels;
using iUtility.Keys;
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
        private readonly Lazy<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;

        static ProducerSaveDataProcess()
        {
            QueueDatas = new List<QueueData>();
        }

        public ProducerSaveDataProcess(Lazy<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis, ILogger logger)
        {
            _logger = logger;
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
        }

        public void Execute()
        {
            lock (QueueDatas)
            {
                Do().Wait();
                QueueDatas.Clear();
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
                    using var prepareQueueItem = channelData.IsSchedule ? 
                        new PrepareScheduleQueueBatchItem(_lazyRabbitMq, channelData, itemByChannel.Value) : 
                        new BasePrepareQueueBatchItem(_lazyRabbitMq, channelData, itemByChannel.Value);

                    await prepareQueueItem.SendBatchPackage();
                }
            }
        }
    }
}

