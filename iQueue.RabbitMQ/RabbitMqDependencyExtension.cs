using iUtility.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace iQueue.RabbitMQ
{
    public static class RabbitMqDependencyExtension
    {
        public static void AddSingletonRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<Lazy<IQueueService>>(x =>
                new Lazy<IQueueService>(() =>
                {
                    var factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672, UserName = "test", Password = "test", RequestedConnectionTimeout = TimeSpan.FromSeconds(10) };
                    var connection = factory.CreateConnection();
                    return new RabbitMQService(connection);
                }));
        }
    }
}
