using iProducer.Processes;
using iUtility.Storages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace iProducer.Workers
{
    public class ProcessCompletedBackgroundWorker : BackgroundService
    {
        private readonly Lazy<IStorageService> _storageService;
        private readonly ILogger<ProcessCompletedBackgroundWorker> _logger;

        public ProcessCompletedBackgroundWorker(ILogger<ProcessCompletedBackgroundWorker> logger, Lazy<IStorageService> storageService)
        {
            _storageService = storageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    new ProducerProcessCompletedProcess(_storageService, _logger).Execute();
                    _logger.LogInformation("ProcessCompletedBackgroundWorker running at: {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogCritical("ProcessCompletedBackgroundWorker got error", e);
                    await Task.Delay(1000);
                }
            }
        }
    }
}
