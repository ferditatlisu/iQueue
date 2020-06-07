using iModel.Channels;
using iModel.Queues;
using iQueue.ByteSerializer.Serializers;
using iUtility.Channels;
using iUtility.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{
    public class ConsumeGetDataProcess : IDisposable
    {
        private readonly Lazy<IQueueService> _queueService;
        private readonly ILogger _logger;

        private readonly IQeueuConnection _queueConnection;

        public ConsumeGetDataProcess(Lazy<IQueueService> queueService, ILogger logger)
        {
            _queueService = queueService;
            _logger = logger;
            _queueConnection = _queueService.Value.CreateConnection();
        }

        public async Task<QueueData> Execute(BackgroundQueueChannel channelData)
        {
            //TODO: 
            var queueData = await _queueConnection.GetSingleData(IQueueHelper.GetChannelNameForConsumer(channelData));
            if (queueData is null)
                return null;

            IQueueSerializer serializer = new IQueueSerializer();
            var data = serializer.UnMergeData(queueData.Value);
            data.ChannelName = channelData.ChannelName;

            return data;
        }

        public void Dispose()
        {
            _queueConnection.Dispose();
        }
    }
}
