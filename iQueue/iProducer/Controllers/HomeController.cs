using System;
using System.Threading.Tasks;
using iModel.Customs;
using iModel.Queues;
using iProducer.Processes;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace iProducer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Hello()
            => "Hello I am iProcuder";

        [HttpPost]
        public async Task<string> Post([FromBody] QueueData data)
        {
            //TODO:Test it and remove lock!
            lock (ProducerSaveDataProcess.QueueDatas) 
            {
                ProducerSaveDataProcess.QueueDatas.Add(data);
            }

            return "Done";
        }
    }
}