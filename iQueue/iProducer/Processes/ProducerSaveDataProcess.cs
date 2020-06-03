using iModel.Channels;
using iModel.Customs;
using iModel.Keys;
using iModel.Queues;
using iUtility.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        batchList.Add("", queueName, false, properties, x.Data);
                    });

                    batchList.Publish(); //TODO: Need research. All of them success or failed ? || Is that possible half of package published but others failed. 
                }
            }
        }
    }
}

