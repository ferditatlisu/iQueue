using iModel.Channels;
using iUtility.Channels;
using iUtility.Keys;
using iUtility.Proxies;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace iConsumer.Consumers
{

    public class ConsumeHealthCheckProcess
    {
        private readonly Lazy<IDatabase> _lazyRedis;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;


        public ConsumeHealthCheckProcess(Lazy<IDatabase> lazyRedis, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _lazyRedis = lazyRedis;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> Execute(BackgroundQueueChannel channelData)
        {
            var healthCheckProxy = new CustomProxy<string>(_httpClientFactory.CreateClient(), _logger, channelData.HealthCheckUrl);
            var result = await healthCheckProxy.GetAsync();
            var response = await result.GetResponse();
            var isHealthly = response == CustomKey.HEALTH_CHECK_RESPONSE_KEY;
            if (!isHealthly)
            {
                if (++channelData.FailerHealthCheckCounter >= channelData.FailureCount)
                {
                    var cacheBackgroundChannelHelper = new CacheBackgroundChannelHelper(_lazyRedis.Value);
                    await cacheBackgroundChannelHelper.Update(channelData);
                    //TODO: Warning!!
                }
            }

            return isHealthly;
        }
    }
}
