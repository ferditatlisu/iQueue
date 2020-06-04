using System.Collections.Generic;
using System.Threading.Tasks;
using iModel.Queues;
using iUtility.Logs;
using Microsoft.AspNetCore.Mvc;

namespace iPreConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumeController : ControllerBase
    {
        public static int Counter;

        [HttpGet]
        public async Task<string> Get()
        {
            return "Done : " + Counter;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<QueueData> request)
        {
            Counter+= request.Count;
            return Ok();
        }
    }
}