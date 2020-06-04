using iModel.Channels;
using iModel.Queues;
using iQueue.ByteSerializer.Serializers;
using iUtility.Channels;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{
    public class ConsumeGetDataProcess : IDisposable
    {
        private readonly Lazy<IConnection> _lazyRabbitMq;
        private readonly ILogger _logger;

        private readonly IModel _rabbitMqModel;

        public ConsumeGetDataProcess(Lazy<IConnection> lazyRabbitMq, ILogger logger)
        {
            _lazyRabbitMq = lazyRabbitMq;
            _logger = logger;
            _rabbitMqModel = _lazyRabbitMq.Value.CreateModel();
        }

        public async Task<QueueData> Execute(BackgroundQueueChannel channelData)
        {
            //TODO: 
            var queueData = _rabbitMqModel.BasicGet(IQueueHelper.GetChannelNameForConsumer(channelData), true);
            if (queueData is null)
                return null;

            IQueueSerializer serializer = new IQueueSerializer();
            var data = serializer.UnMergeData(queueData.Body);
            data.ChannelName = channelData.ChannelName;

            return data;
        }

        public void Dispose()
        {
            _rabbitMqModel.Dispose();
        }
    }
}
