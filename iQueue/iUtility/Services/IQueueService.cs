using iModel.Channels;
using iModel.Queues;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iUtility.Services
{
    public interface IQueueService
    {
        IQeueuConnection CreateConnection();
    }

    public interface IQeueuConnection : IDisposable
    {
        Task BulkInsertData(IEnumerable<QueueData> messages, QueueChannel queueChannel);
        Task<ReadOnlyMemory<byte>?> GetSingleData(string queueName);
    }
}
