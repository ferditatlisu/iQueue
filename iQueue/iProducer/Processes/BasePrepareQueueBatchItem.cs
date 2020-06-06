using iModel.Channels;
using iModel.Queues;
using iModel.Utilities;
using iUtility.Channels;
using iUtility.Keys;
using iUtility.Storages;
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
        protected readonly IQueueStorage _StorageService;
        private readonly QueueChannel _queueChannel;
        private readonly List<QueueData> _messages;

        protected IBasicProperties _Properties = null;

        public BasePrepareQueueBatchItem(Lazy<IConnection> _rabbitMqConnection, Lazy<IDatabase> _redis, Lazy<IQueueStorage> _storageService, QueueChannel queueChannel, List<QueueData> queueData)
        {
            _StorageService = _storageService.Value;
            _RabbitMqModel = _rabbitMqConnection.Value.CreateModel();
            _queueChannel = queueChannel;
            _messages = queueData;
            _Redis = _redis.Value;
        }

        public virtual async Task SendBatchPackage()
        { 
            var batchList = _RabbitMqModel.CreateBasicPublishBatch();
            string queueName = IQueueHelper.GetChannelNameForProducer(_queueChannel);
            for (int i = 0; i < _messages.Count; i++)
            {
                var message = _messages[i];
                AddProperties(message);
                batchList.Add(CustomKey.QUEUE_DEFAULT_EXCHANGE_KEY, queueName, false, _Properties, message.Data);
            }

            batchList.Publish();
            await _StorageService.BulkInsertData(_messages);
            await _StorageService.BulkInsertProducerEnterLog(_messages.Select(x=> x.QueueId), MessageStatus.ProcuderEntry);
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
