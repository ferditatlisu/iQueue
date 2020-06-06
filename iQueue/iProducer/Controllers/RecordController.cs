using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iModel.Customs;
using iModel.Queues;
using iModel.Utilities;
using iProducer.Processes;
using iQueue.RavenStorage;
using iUtility.Storages;
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

        [HttpPost]
        public async Task<IActionResult> CompletedAsItem([FromBody] string id)
        {
            lock (ProducerProcessCompletedProcess.ProcessCompletedIds)
            {
                ProducerProcessCompletedProcess.ProcessCompletedIds.Add(id);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CompletedAsList([FromBody] List<string> ids)
        {
            lock (ProducerProcessCompletedProcess.ProcessCompletedIds)
            {
                ProducerProcessCompletedProcess.ProcessCompletedIds.AddRange(ids);
            }

            return NoContent();
        }
    }
}