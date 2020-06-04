using iUtility.Proxies;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{
    public class ConsumeSendDataProcess<T> where T: class
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly string _targetUrl;

        public ConsumeSendDataProcess(string targetUrl, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _targetUrl = targetUrl;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Execute(List<T> data)
        {
            //TODO: Data kaybini onlemek icin dusunelim ve karsi taraf bizi yormadan! Guven -------o------Hiz
            var consumerProxy = new CustomProxy<string>(_httpClientFactory.CreateClient(), _logger, _targetUrl);
            consumerProxy.BodyParameter(data);
            consumerProxy.PostAsync();
        }
    }
}
