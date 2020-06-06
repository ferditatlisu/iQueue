using iModel.Queues;
using iModel.Storages;
using iModel.Utilities;
using iUtility.Keys;
using iUtility.Storages;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iQueue.RavenStorage
{
    public class RavenStorage : IQueueStorage
    {
        private readonly IDocumentStore _ravenDb;
        private IAsyncDocumentSession _connection;

        public RavenStorage(IDocumentStore ravenDb)
        {
            _ravenDb = ravenDb;
        }

        public async Task OpenConnection()
        {
            _connection = _ravenDb.OpenAsyncSession(ServiceKey.RAVEN_DATABASE_NAME);
        }

        public async Task AddData(QueueData queueData)
        {
            await _connection.StoreAsync(queueData, queueData.QueueId);
            await _connection.SaveChangesAsync();
        }

        public async Task AddLog(QueueData queueData, MessageStatus status)
        {
            var log = new QueueMessageLogData
            {
                QueueId = queueData.QueueId,
                MessageStatus = status,
                CreatedDate = DateTime.UtcNow
            };

            await _connection.StoreAsync(log);
            await _connection.SaveChangesAsync();
        }

        public async Task BulkInsertData(IEnumerable<QueueData> messages)
        {
            using var bulk = _ravenDb.BulkInsert();
            foreach (var message in messages)
            {
                await bulk.StoreAsync(message, message.QueueId);
            }
        }

        public async Task BulkInsertProducerEnterLog(IEnumerable<string> ids, MessageStatus messageStatus)
        {
            using var bulk = _ravenDb.BulkInsert();
            foreach (var id in ids)
            {
                var log = new QueueMessageLogData { QueueId = id, MessageStatus = messageStatus, CreatedDate = DateTime.UtcNow };
                await bulk.StoreAsync(log, Guid.NewGuid().ToString());
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
