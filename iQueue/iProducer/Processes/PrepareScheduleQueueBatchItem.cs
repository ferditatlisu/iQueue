using iModel.Channels;
using iModel.Queues;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class PrepareScheduleQueueBatchItem : BasePrepareQueueBatchItem
    {
        public PrepareScheduleQueueBatchItem(Lazy<IConnection> _rabbitMqConnection, QueueChannel queueChannel, List<QueueData> queueData) : base(_rabbitMqConnection, queueChannel, queueData)
        {
            _Properties = _RabbitMqModel.CreateBasicProperties();
        }

        public override void AddProperties(QueueData data)
        {
            _Properties.Expiration = data.ScheduleTime;
        }
    }
}
