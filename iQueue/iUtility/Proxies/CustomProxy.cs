using iUtility.Proxies.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace iUtility.Proxies
{
    public class CustomProxy<T> : BaseProxy<T> where T : class
    {
        public CustomProxy(HttpClient httpClient, ILogger logger, string url) : base(httpClient, null, logger)
        {
            _ActionName = url;
        }
    }
}
