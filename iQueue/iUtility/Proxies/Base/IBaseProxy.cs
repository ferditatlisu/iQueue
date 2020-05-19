using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iUtility.Proxies.Base
{
    public interface IBaseProxy<T> where T : class
    {
        #region Property

        Action<HttpResponseMessage> OnSuccess { get; set; }
        Action<HttpResponseMessage> OnError { get; set; }

        #endregion

        #region Public Method

        IBaseProxy<T> GetParameters(Dictionary<string, string> parameters);
        IBaseProxy<T> BodyParameter<K>(K data);
        Task<IBaseProxy<T>> GetAsync();
        Task<IBaseProxy<T>> PostAsync();
        Task<IBaseProxy<T>> PutAsync();
        Task<T> GetResponse();

        #endregion
    }
}
