using iUtility.Proxies.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace iUtility.Proxies.Producers
{
    public class AddQueueProxy<T> : BaseProxy<T> where T: class
    {
        public AddQueueProxy(HttpClient httpClient, ILogger logger) : base(httpClient, "http://localhost:44334", logger)
        {
            _ActionName = "/api/home";
        }
    }
}
