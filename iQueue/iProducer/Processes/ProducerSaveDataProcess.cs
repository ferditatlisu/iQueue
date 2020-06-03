using iModel.Channels;
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
                var channelExist = await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Exist(itemByChannel.Key);
                if(channelExist)
                {
                    using var channel = _lazyRabbitMq.Value.CreateModel();
                    var batchList = channel.CreateBasicPublishBatch();
                    string queueName = $"{itemByChannel.Key}_Delay";

                    itemByChannel.Value.ForEach(x =>
                    {
                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.Expiration = "10000";
                        batchList.Add(CustomKey.QUEUE_DEFAULT_EXCHANGE_KEY, queueName, false, properties, x.Data);
                    });

                    batchList.Publish(); //TODO: Need research. All of them success or failed ? || Is that possible half of package published but others failed. 
                }
            }
        }
    }
}

