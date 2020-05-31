using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;


namespace iUtility.Logs
{
    public class SlackLog
    {
        private const string _LogFormatString = "{0}";
        private const string _LogFormatException = "Exception : {0} \n Inner : {1} \n Stack Trace : {2}";
        private const string _LogFormat = "{0} - Exception : {1} \n Inner : {2} \n Stack Trace : {3}";
        private const string _Url = "https://hooks.slack.com/services/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";
        private readonly static HttpClient _Client;
        static SlackLog()
        {
            _Client = new HttpClient();
        }

        #region Public - Methods

        public static async Task SendMessage(string message)
        {
            var errorMessage = string.Format(_LogFormatString, message);
            await PostMessage(errorMessage);
        }

        public static async Task SendMessage(Exception ex)
        {
            var errorMessage = string.Format(_LogFormatException, ex.Message, ex.InnerException, ex.StackTrace);
            await PostMessage(errorMessage);
        }

        public static async Task SendMessage(string message, Exception ex)
        {
            var errorMessage = string.Format(_LogFormat, message, ex.Message, ex.InnerException, ex.StackTrace);
            await PostMessage(errorMessage);
        }

        #endregion

        #region Private - Methods

        private static async Task PostMessage(string errorMessage)
        {
            var data = new SlackMessageData { Text = errorMessage };
            var stringData = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(stringData);
            await _Client.PostAsync(_Url, httpContent);
        }

        #endregion
    }

    #region Data-Class

    public class SlackMessageData
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    #endregion
}
