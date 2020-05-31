using iModel.Channels;
using iModel.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var queueData = _rabbitMqModel.BasicGet(channelData.ChannelName, true);
            if (queueData is null)
                return null;

            QueueData data = new QueueData
            {
                ChannelName = channelData.ChannelName,
                Data = queueData.Body.ToArray()
            };

            return data;
        }

        public void Dispose()
        {
            _rabbitMqModel.Dispose();
        }
    }
}
