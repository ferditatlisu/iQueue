using iModel.Channels;
using iModel.Queues;
using iUtility.Channels;
using iUtility.Keys;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class BasePrepareQueueBatchItem : IDisposable
    {
        protected readonly IModel _RabbitMqModel;
        protected readonly IDatabase _Redis;
        private readonly QueueChannel _queueChannel;
        private readonly List<QueueData> _messages;

        protected IBasicProperties _Properties = null;

        public BasePrepareQueueBatchItem(Lazy<IConnection> _rabbitMqConnection, Lazy<IDatabase> _redis, QueueChannel queueChannel, List<QueueData> queueData)
        {
            _RabbitMqModel = _rabbitMqConnection.Value.CreateModel();
            _queueChannel = queueChannel;
            _messages = queueData;
            _Redis = _redis.Value;
        }

        public virtual async Task SendBatchPackage()
        { 
            var batchList = _RabbitMqModel.CreateBasicPublishBatch();
            string queueName = IQueueHelper.GetChannelNameForProducer(_queueChannel);
            HashEntry[] hashEntries = new HashEntry[_messages.Count];

            for (int i = 0; i < _messages.Count; i++)
            {
                var message = _messages[i];
                AddProperties(message);
                hashEntries[i] = new HashEntry(message.Id, message.Data);
                batchList.Add(CustomKey.QUEUE_DEFAULT_EXCHANGE_KEY, queueName, false, _Properties, message.Data);
            }

            await _Redis.HashSetAsync(queueName, hashEntries);
            batchList.Publish(); //TODO: Need research. All of them success or failed ? || Is that possible half of package published but others failed. 
        }

        public virtual void AddProperties(QueueData data)
        { 
        
        }

        public void Dispose()
        {
            _RabbitMqModel.Dispose();
        }
    }
}
