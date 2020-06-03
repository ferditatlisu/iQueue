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
        public async Task<IActionResult> Save([FromBody] QueueData data)
        {
            lock (ProducerSaveDataProcess.QueueDatas) 
            {
                ProducerSaveDataProcess.QueueDatas.Add(data);
                ProducerSaveDataProcess.QeueuDataCounter++;
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetCounter()
        {
            return Ok(ProducerSaveDataProcess.QeueuDataCounter);
        }
    }
}