using iUtility.Storages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class ProducerGetVoidMessagesProcess
    {
        private readonly Lazy<IStorageService> _lazyStorageService;
        private readonly Lazy<IDatabase> _lazyRedis;

        private readonly ILogger _logger;

        public ProducerGetVoidMessagesProcess(Lazy<IDatabase> lazyRedis, Lazy<IStorageService> lazyStorageService, ILogger logger)
        {
            _logger = logger;
            _lazyRedis = lazyRedis;
            _lazyStorageService = lazyStorageService;
        }

        public async Task Execute()
        { 
            using var storageConnection = _lazyStorageService.Value.CreateConnection();
            var ids = await storageConnection.GetVoidMessages();
            if (ids?.Count > 0)
            { 
                var queueItems = await storageConnection.GetMessages(ids);
                lock (ProducerSaveDataProcess.QueueDatas)
                {
                    ProducerSaveDataProcess.QueueDatas.AddRange(queueItems);
                }
            }
        }
    }
}
