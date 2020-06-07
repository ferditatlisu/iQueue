using iModel.Channels;
using iModel.Queues;
using iQueue.RabbitMQ.MessageProperties;
using iUtility.Channels;
using iUtility.Keys;
using iUtility.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace iQueue.RabbitMQ
{
    public class RabbitMQConnection : IQeueuConnection
    {
        private readonly IModel _session;
        public RabbitMQConnection(IModel session)
        {
            _session = session;
        }

        public async Task BulkInsertData(IEnumerable<QueueData> messages, QueueChannel queueChannel)
        {
            var batchList = _session.CreateBasicPublishBatch();
            string queueName = IQueueHelper.GetChannelNameForProducer(queueChannel);
            foreach (var message in messages)
            {
                var properties = queueChannel.IsSchedule ?
                    new ScheduleRabbitMQProperties(_session, queueChannel, message).GetProperties() :
                    new BaseRabbitMQProperties(_session, queueChannel, message).GetProperties();

                batchList.Add(CustomKey.QUEUE_DEFAULT_EXCHANGE_KEY, queueName, false, properties, message.Data);
            }

            batchList.Publish();
        }

        public async Task<ReadOnlyMemory<byte>?> GetSingleData(string queueName)
        {
            var data = _session.BasicGet(queueName, true);
            return data?.Body;
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        
    }
}
