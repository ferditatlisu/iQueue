using iModel.Queues;
using iModel.Storages;
using iModel.Utilities;
using iUtility.Storages;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iQueue.RavenStorage
{
    public class RavenStorageConnection : IStorageConnection
    {
        private readonly IDocumentStore _ravenDb;
        private IAsyncDocumentSession _connection;

        public RavenStorageConnection(IDocumentStore ravenDb, IAsyncDocumentSession connection)
        {
            _ravenDb = ravenDb;
            _connection = connection;
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

        public async Task<List<string>> GetVoidMessages()
        {
            try
            {
                var minimumDate = DateTime.UtcNow.AddMinutes(-50);

                var lastData = await _connection
                    .Query<QueueMessageLogData>()
                    .Where(z => z.CreatedDate > minimumDate)
                    .ToListAsync();

                var notConsumedButSendedToConsumerMessages = lastData
                    .GroupBy(z => new { z.QueueId })
                    .Select(x => new
                    {
                        Id = x.Key.QueueId,
                        Count = x.Count(),
                    })
                    .Where(z => z.Count == 2)
                    .Select(z => z.Id)
                    .ToList();

                var willRemove = lastData
                    .Where(x => x.QueueId.In(notConsumedButSendedToConsumerMessages))
                    .Where(z => z.MessageStatus == MessageStatus.CustomerApprove)
                    .Select(x=> x.QueueId)
                    .ToList();

                if(willRemove?.Count > 0)
                    notConsumedButSendedToConsumerMessages.RemoveAll(x => willRemove.Contains(x));

                return notConsumedButSendedToConsumerMessages;
                //return notConsumedButSendedToConsumerMessages;
            }
            catch (Exception e)
            {

                throw;
            }
           
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public async Task<QueueData> GetMessage(string queueId)
        {
            var lastData = await _connection
                  .Query<QueueData>()
                  .Where(z => z.QueueId == queueId).FirstOrDefaultAsync();

            return lastData;
        }

        public async Task<List<QueueData>> GetMessages(List<string> queueIds)
        {
            var lastData = await _connection
                  .Query<QueueData>()
                  .Where(z => z.QueueId.In(queueIds)).ToListAsync();

            return lastData;
        }
    }
}
