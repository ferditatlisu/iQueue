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
    public class ChannelController : ControllerBase
    {

        private readonly LazyQueue<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;

        public ChannelController(LazyQueue<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis)
        {
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] QueueChannel channelData)
        {
            _lazyRabbitMq.Value.CreateModel().QueueDeclare(channelData.ChannelName, true, false, false, null);
            await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Create(channelData);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetChannels()
        {
            List<string> channels = new List<string>();
            var chanels = await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Get();
            if (channels != null)
                foreach (var channel in chanels)
                {
                    channels.Add(channel.ChannelName);
                }

            return Ok(channels);
        }
    }
}