using iModel.Channels;
using iModel.Queues;
using iUtility.Storages;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class PrepareScheduleQueueBatchItem : BasePrepareQueueBatchItem
    {
        public PrepareScheduleQueueBatchItem(Lazy<IConnection> rabbitMqConnection, Lazy<IDatabase> redis, Lazy<IQueueStorage> _storageService, QueueChannel queueChannel, List<QueueData> queueData) : base(rabbitMqConnection, redis, _storageService, queueChannel, queueData)
        {
            _Properties = _RabbitMqModel.CreateBasicProperties();
        }

        public override void AddProperties(QueueData data)
        {
            _Properties.Expiration = data.ScheduleTime;
        }
    }
}
