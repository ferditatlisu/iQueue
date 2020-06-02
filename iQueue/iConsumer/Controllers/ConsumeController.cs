using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using iModel.Channels;
using iModel.Customs;
using iModel.Queues;
using iUtility.Proxies;
using iUtility.Proxies.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;


namespace iConsumer.Controllers
{
    [Route("api/[controller]/[action]")]

    [ApiController]
    public class ConsumeController : ControllerBase
    {
        private readonly LazyQueue<IConnection> _lazyRabbitMq;
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public ConsumeController(LazyQueue<IConnection> lazyRabbitMq, Lazy<IDatabase> lazyRedis, IHttpClientFactory httpClientFactory, ILogger<ConsumeController> logger)
        {
            _lazyRabbitMq = lazyRabbitMq;
            _lazyRedis = lazyRedis;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
    }
}