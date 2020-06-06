using iUtility.Keys;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace iUtility.Proxies.Base
{
    public class BaseProxy<T> : IBaseProxy<T>, IDisposable
         where T : class
    {
        #region Field

        protected string _ActionName;

        protected readonly HttpClient _HttpClient;

        protected HttpResponseMessage _HttpResponseMessage;
        protected HttpContent _HttpContent;
        protected readonly ILogger _Logger;
        #endregion

        #region Property

        protected string BaseUrl { get; set; }

        #endregion

        #region Events

        public Action<HttpResponseMessage> OnSuccess { get; set; }
        public Action<HttpResponseMessage> OnError { get; set; }

        #endregion

        #region Public  Method

        public BaseProxy(HttpClient httpClient, string baseUrl, ILogger logger)
        {
            _HttpClient = httpClient;
            _Logger = logger;
            if(baseUrl != null)
                _HttpClient.BaseAddress = new Uri(baseUrl);
        }

        public virtual IBaseProxy<T> GetParameters(Dictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(CustomKey.QUESTION_MARK_KEY);
            foreach (var param in parameters)
            {
                builder.Append($"{param.Key}{CustomKey.EQUALS_SIGN_KEY}{param.Value}");
                builder.Append(CustomKey.AMPERSAND_KEY);
            }

            builder.Remove(builder.Length - 1, 1);
            _ActionName += builder.ToString();
            return this;
        }

        public virtual async Task<IBaseProxy<T>> GetAsync()
        {
            _HttpResponseMessage = await _HttpClient.GetAsync(_ActionName);
            if (_HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                OnSuccess?.Invoke(_HttpResponseMessage);
            else
                OnError?.Invoke(_HttpResponseMessage);

            return this;
        }

        public virtual async Task<IBaseProxy<T>> PutAsync()
        {
            _HttpResponseMessage = await _HttpClient.PutAsync(_ActionName, _HttpContent);
            if (_HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                OnSuccess?.Invoke(_HttpResponseMessage);
            else
            {
                OnError?.Invoke(_HttpResponseMessage);
                var apiResponse = await GetResponse();
                _Logger?.LogError(LogKey.PROXY_ERROR_MESSAGE, apiResponse);
            }

            return this;
        }

        public virtual IBaseProxy<T> BodyParameter<K>(K data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            _HttpContent = new StringContent(jsonData, Encoding.UTF8, CustomKey.JSON_CONTENT_TYPE);
            return this;
        }

        public virtual async Task<IBaseProxy<T>> PostAsync()
        {
            _HttpResponseMessage = await _HttpClient.PostAsync(_ActionName, _HttpContent);
            if (200 <= (int)_HttpResponseMessage.StatusCode && (int)_HttpResponseMessage.StatusCode < 300)
                OnSuccess?.Invoke(_HttpResponseMessage);
            else
                OnError?.Invoke(_HttpResponseMessage);

            return this;
        }

        public async Task<T> GetResponse()
        {
            var jsonResponse = await _HttpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                T responseData = JsonConvert.DeserializeObject<T>(jsonResponse);
                return responseData;
            }
            catch (Exception e)
            {
                return jsonResponse as T;
                //throw e;
            }
        }

        public virtual void Dispose()
        {
            OnSuccess = null;
            OnError = null;

            _HttpContent?.Dispose();
        }

        #endregion

        #region Private-Helper

        #endregion
    }
}
