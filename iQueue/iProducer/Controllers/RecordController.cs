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
    public class RecordController : ControllerBase
    {
        [HttpPost]
        public IActionResult Save([FromBody] QueueData data)
        {
            lock (ProducerSaveDataProcess.QueueDatas) 
            {
                ProducerSaveDataProcess.QueueDatas.Add(data);
            }

            return NoContent();
        }
    }
}