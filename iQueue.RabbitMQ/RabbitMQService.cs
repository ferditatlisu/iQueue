using iUtility.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;



namespace iQueue.RabbitMQ
{
    public class RabbitMQService : IQueueService
    {
        private readonly IConnection _rabbitMQ;
        public RabbitMQService(IConnection rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public IQeueuConnection CreateConnection()
        {
            var iModel = _rabbitMQ.CreateModel();
            return new RabbitMQConnection(iModel);
        }
    }
}
