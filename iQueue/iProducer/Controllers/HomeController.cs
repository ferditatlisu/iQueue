using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using iHealthCheck.Managers;
using iModel.Channels;
using iModel.Customs;
using iModel.Queues;
using iProducer.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace iProducer.Controllers
{
    [Route("api/[controller]/[action]")]
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

        [HttpGet]
        public async Task<string> Hello()
            => "Hello I am iProcuder";

        [HttpPost]
        public async Task<string> Post([FromBody] QueueData data)
        {
            lock(ProducerSaveDataProcess.QueueDatas)
            {
                ProducerSaveDataProcess.QueueDatas.Add(data);
            }

            return "Done";
        }
    }
}