using iModel.Channels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace iPreProducer
{
    class Program
    {
        private static string _channelName = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            while (true)
            {
                Console.WriteLine("Choose number :  \n0-> Create Channel \n1+ -> Count of request send to channel that created");
                var input = int.Parse(Console.ReadLine());

                switch (input)
                {
                    case 0:
                        CreateChannel();
                        break;
                    default:
                        SendRequest(input);
                        break;
                }
            }
            
        }

        private static void CreateChannel()
        {
            _channelName = "Test";
            var data = new QueueChannel
            {
                ChannelName = _channelName,
                ConsumeUrl = "http://ipreconsumer/api/consume",
                FetchCount = 20,
                FailureCount = 0,
                ExecuteEverySecond = 2,
                HealthCheckUrl = "https://dpe-ru-deliveryclub-dev.azurewebsites.net/health/API"
            };

            var json = JsonConvert.SerializeObject(data);
            HttpClient client = new HttpClient();
            var _HttpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var aaaaaa = client.PostAsync("http://localhost:8000/api/home", _HttpContent);
        }

        private static void SendRequest(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                PreProducerRequest rquest = new PreProducerRequest
                {
                    ChannelName = _channelName,
                    Data = Encoding.UTF8.GetBytes("MyData")
                };

                HttpClient client = new HttpClient();
                var _HttpContent = new StringContent(JsonConvert.SerializeObject(rquest), Encoding.UTF8, "application/json");
                var aaaaaa = client.PostAsync("http://localhost:8001/api/home/post", _HttpContent).Result;
            }
        }
    }

    public class PreProducerRequest
    {
        public string ChannelName { get; set; }
        public byte[] Data { get; set; }
    }
}
