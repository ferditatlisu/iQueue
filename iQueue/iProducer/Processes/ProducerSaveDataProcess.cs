using iModel.Channels;
using iModel.Customs;
using iModel.Keys;
using iModel.Queues;
using iUtility.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        public static List<QueueData> QueueDatas { get; set; }

        private readonly ILogger _logger;
        private readonly LazyQueue<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;


        static ProducerSaveDataProcess()
        {
            QueueDatas = new List<QueueData>();
        }

        public ProducerSaveDataProcess(LazyQueue<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis, ILogger logger)
        {
            _logger = logger;
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
        }

        public async Task Execute()
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
                    var batchList = _lazyRabbitMq.Value.CreateModel().CreateBasicPublishBatch();
                    itemByChannel.Value.ForEach(x =>
                    {
                        var jsonData = JsonConvert.SerializeObject(x.Data);
                        var body = Encoding.UTF8.GetBytes(jsonData);
                        batchList.Add(CustomKey.QUEUE_EXCHANGE_KEY, itemByChannel.Key, false, null, body);
                    });

                    batchList.Publish();
                }
            }
        }
    }
}

