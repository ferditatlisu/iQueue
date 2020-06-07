using iModel.Queues;
using iModel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iUtility.Storages
{
    public interface IStorageService
    {
        IStorageConnection CreateConnection();
    }

    public interface IStorageConnection : IDisposable
    {
        Task AddData(QueueData queueData);
        Task AddLog(QueueData queueData, MessageStatus status);
        Task BulkInsertData(IEnumerable<QueueData> messages);
        Task BulkInsertProducerEnterLog(IEnumerable<string> ids, MessageStatus messageStatus);
    }
}
