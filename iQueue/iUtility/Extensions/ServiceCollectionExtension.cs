using iModel.Customs;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace iUtility.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddTransientRabbitMq(this IServiceCollection services)
        {
            services.AddScoped<LazyQueue<IConnection>>(x =>
               new LazyQueue<IConnection>(() =>
               {
                   var factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672, UserName = "test", Password = "test", RequestedConnectionTimeout = TimeSpan.FromSeconds(10) };
                   var connection = factory.CreateConnection();
                   return connection;
               }));
        }

        public static void AddSingletonRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<Lazy<IConnection>>(x =>
                  new Lazy<IConnection>(() =>
                  {
                      var factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672, UserName = "test", Password = "test", RequestedConnectionTimeout = TimeSpan.FromSeconds(10) };
                      var connection = factory.CreateConnection();
                      return connection;
                  }));
        }

        public static void AddTransientRedis(this IServiceCollection services)
        {
            services.AddTransient<Lazy<IDatabase>>(x =>
               new Lazy<IDatabase>(() =>
               {
                   ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
                   return redis.GetDatabase();
               }));
        }

        public static void AddSingletonRedis(this IServiceCollection services)
        {
            services.AddSingleton<Lazy<IDatabase>>(x =>
                new Lazy<IDatabase>(() =>
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
                    return redis.GetDatabase();
                }));
        }
    }
}
