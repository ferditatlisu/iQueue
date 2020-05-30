using iModel.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{
    public class ConsumeGetDataProcess<T> where T: class
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

        public async Task<T> Execute(BackgroundQueueChannel channelData)
        {
            var queueData = _rabbitMqModel.BasicGet(channelData.ChannelName, true);
            if (queueData is null)
                return null;

            var queueDataJson = Encoding.UTF8.GetString(queueData.Body.Span);
            var data = JsonConvert.DeserializeObject<T>(queueDataJson);
            return data;
        }
    }
}
