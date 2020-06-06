using iModel.Utilities;
using iUtility.Storages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iProducer.Processes
{
    public class ProducerProcessCompletedProcess
    {
        public static readonly List<string> ProcessCompletedIds;

        private readonly ILogger _logger;
        private readonly Lazy<IQueueStorage> _lazyStorageService;

        static ProducerProcessCompletedProcess()
        {
            ProcessCompletedIds = new List<string>();
        }

        public ProducerProcessCompletedProcess(Lazy<IQueueStorage> lazyStorageService, ILogger logger)
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
            await _lazyStorageService.Value.BulkInsertProducerEnterLog(ProcessCompletedIds, MessageStatus.CustomerApprove);
        }
    }
}
