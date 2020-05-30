using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iModel.Channels;
using iModel.Customs;
using iUtility.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace iQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly LazyQueue<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;

        public HomeController(LazyQueue<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis)
        {
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
        }

        [HttpPost]
        public async Task<string> CreateChannel([FromBody] QueueChannel channelData)
        {
            _lazyRabbitMq.Value.CreateModel().QueueDeclare(channelData.ChannelName, true, false, false, null);
            await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Create(channelData);
            return "Created";
        }
    }
}