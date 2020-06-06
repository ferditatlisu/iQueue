using iUtility.Keys;
using iUtility.Logs;
using iUtility.Storages;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iQueue.RavenStorage.Extensions
{

    public static class RavenDependencyExtension
    {
        private readonly static string _url = "http://raven:8080";

        public static void AddRavenDB(this IServiceCollection services)
        {
            services.AddSingleton<Lazy<IQueueStorage>>(x =>
                new Lazy<IQueueStorage>(() =>
                {
                    var iDocumentStore = CreateConnection().Result;
                    return new RavenStorage(iDocumentStore);
                }));

            //services.AddScoped<IQueueStorage>(sp =>
            //new RavenStorage(sp.GetRequiredService<IDocumentStore>().OpenAsyncSession(ServiceKey.RAVEN_DATABASE_NAME)));
        }

        public static async Task ReConnection(this IDocumentStore store)
        {
            store = new DocumentStore
            {
                Urls = new[] { _url },
                Database = store.Database
            }.Initialize();
        }

        private static async Task<IDocumentStore> CreateConnection()
        {
            var store = new DocumentStore
            {
                Urls = new[] { _url },
                Database = ServiceKey.RAVEN_DATABASE_NAME
            }.Initialize();

            try
            {
                store.Maintenance.ForDatabase(store.Database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                try
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }
            }

            return store;
        }
    }
}
