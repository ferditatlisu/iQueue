using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iModel.Channels;
using iModel.Customs;
using iUtility.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
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
            using var channel = _lazyRabbitMq.Value.CreateModel();
            channel.QueueDeclare(channelData.ChannelName, true, false, false, null);
            await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).Create(channelData);
            if (channelData.IsSchedule)
                await CreateScheduleQueue(channelData);
            
            return NoContent();
        }

        private async Task CreateScheduleQueue(QueueChannel channelData)
        {
            using var channel = _lazyRabbitMq.Value.CreateModel();
            Dictionary<string, object> arguments = null;

            var delayQueueName = IQueueHelper.GetChannelNameForProducer(channelData);
            string exchangeName = IQueueHelper.GetExchangeNameForDeadLetter(channelData);
            
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            arguments = new Dictionary<string, object> {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key",  delayQueueName},
                //{ "x-queue-mode", "lazy" }
            };
            
            channel.QueueDeclare(delayQueueName, true, false, false, arguments);
            channel.QueueBind(channelData.ChannelName, exchangeName, delayQueueName, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetChannels()
        {
            List<string> channels = new List<string>();
            var chanels = await new CacheChannelHelper<QueueChannel>(_lazyRedis.Value).GetAll();
            if (channels != null)
                foreach (var channel in chanels)
                {
                    channels.Add(channel.ChannelName);
                }

            return Ok(channels);
        }
    }
}