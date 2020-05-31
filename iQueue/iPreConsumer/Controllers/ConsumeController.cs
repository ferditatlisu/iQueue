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
        [HttpGet]
        public async Task<string> Get()
        {
            return "Done";
        }


        [HttpPost]
        public async Task Post([FromBody] QueueData request)
        {
            var yourobject = System.Text.Encoding.UTF8.GetString(request.Data);
            SlackLog.SendMessage($"I got : {yourobject}");
        }
    }
}