using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using iModel.Queues;
using iUtility.Logs;
using iUtility.Proxies;
using iUtility.Proxies.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iPreConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumeController : ControllerBase
    {
        public static int Counter;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public ConsumeController(IHttpClientFactory httpClientFactory, ILogger<ConsumeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return "Done : " + Counter;
        }

        [HttpPost]
        public async Task Post([FromBody] List<QueueData> request)
        {
            Counter+= request.Count;

            request.RemoveAt(0);
            request.RemoveAt(1);

            var consumerProxy = new CustomProxy<string>(_httpClientFactory.CreateClient(), _logger, "http://iproducer/api/record/CompletedAsList");
            consumerProxy.BodyParameter(request.Select(x => x.QueueId).ToList());
            consumerProxy.PostAsync();
        }
    }
}