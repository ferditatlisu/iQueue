using iModel.Utilities;
using iUtility.Storages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class ProducerProcessCompletedProcess
    {
        public static readonly List<string> ProcessCompletedIds;

        private readonly ILogger _logger;
        private readonly Lazy<IStorageService> _lazyStorageService;

        static ProducerProcessCompletedProcess()
        {
            ProcessCompletedIds = new List<string>();
        }

        public ProducerProcessCompletedProcess(Lazy<IStorageService> lazyStorageService, ILogger logger)
        {
            _logger = logger;
            _lazyStorageService = lazyStorageService;
        }

        public void Execute()
        {
            lock (ProcessCompletedIds)
            {
                if (ProcessCompletedIds.Count > 0)
                { 
                    Do().Wait();
                    ProcessCompletedIds.Clear();
                }
            }
        }

        private async Task Do()
        {
            using var storageConnection = _lazyStorageService.Value.CreateConnection();
            await storageConnection.BulkInsertProducerEnterLog(ProcessCompletedIds, MessageStatus.CustomerApprove);
        }
    }
}
