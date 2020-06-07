using iModel.Channels;
using iModel.Queues;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace iQueue.RabbitMQ.MessageProperties
{
    public class BaseRabbitMQProperties
    {
        protected readonly IModel _RabbitMQ;
        protected readonly QueueChannel _QueueChannel;
        protected readonly QueueData _Message;

        public BaseRabbitMQProperties(IModel rabbitMq, QueueChannel queueChannel, QueueData message)
        {
            _RabbitMQ = rabbitMq;
            _QueueChannel = queueChannel;
            _Message = message;
        }

        public virtual IBasicProperties GetProperties()
        {
            return null;
        }
    }
}
