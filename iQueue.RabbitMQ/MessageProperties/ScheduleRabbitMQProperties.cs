using iModel.Channels;
using iModel.Queues;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace iQueue.RabbitMQ.MessageProperties
{
    public class ScheduleRabbitMQProperties : BaseRabbitMQProperties
    {
        public ScheduleRabbitMQProperties(IModel rabbitMq, QueueChannel queueChannel, QueueData message) : base(rabbitMq, queueChannel, message)
        {
        }

        public override IBasicProperties GetProperties()
        {
            var properties = _RabbitMQ.CreateBasicProperties();
            properties.Expiration = _Message.ScheduleTime;
            return properties;
        }
    }
}
