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
    public class RavenStorageService : IStorageService
    {
        private readonly IDocumentStore _ravenDb;
        private IAsyncDocumentSession _connection;

        public RavenStorageService(IDocumentStore ravenDb)
        {
            _ravenDb = ravenDb;
        }

        public IStorageConnection CreateConnection()
        {
            return new RavenStorageConnection(_ravenDb, _ravenDb.OpenAsyncSession(ServiceKey.RAVEN_DATABASE_NAME));
        } 
    }
}
