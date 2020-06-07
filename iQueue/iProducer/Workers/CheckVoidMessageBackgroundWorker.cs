using iProducer.Processes;
using iUtility.Storages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace iProducer.Workers
{
    public class CheckVoidMessageBackgroundWorker : BackgroundService
    {
        private readonly Lazy<IStorageService> _storageService;
        private readonly Lazy<IDatabase> _redis;
        private readonly ILogger<CheckVoidMessageBackgroundWorker> _logger;

        public CheckVoidMessageBackgroundWorker(ILogger<CheckVoidMessageBackgroundWorker> logger, Lazy<IStorageService> storageService,
            Lazy<IDatabase> redis)
        {
            _storageService = storageService;
            _redis = redis;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await new ProducerGetVoidMessagesProcess(_redis, _storageService, _logger).Execute();
                    _logger.LogInformation("BackwardCheckMessageBackgroundWorker running at: {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(30000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogCritical("BackwardCheckMessageBackgroundWorker got error", e);
                    await Task.Delay(1000);
                }
            }
        }
    }
}
