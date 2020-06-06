using iModel.Channels;
using iModel.Queues;
using iQueue.ByteSerializer.Serializers;
using iUtility.Logs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iPreProducer
{
    class Program
    {
        private static string _channelName = "Dos";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            while (true)
            {
                Console.WriteLine("Choose number :  \n0-> Create Channel \n1+ -> Count of request send to channel that created");
                var input = int.Parse(Console.ReadLine());

                switch (input)
                {
                    case 0:
                        await CreateChannel();
                        break;
                    default:
                        await SendRequest(input);
                        break;
                }
            }
            
        }

        private static async Task CreateChannel()
        {
            var data = new QueueChannel
            {
                ChannelName = _channelName,
                ConsumeUrl = "http://ipreconsumer/api/consume",
                FetchCount = 20,
                FailureCount = 0,
                ExecuteEverySecond = 1,
                HealthCheckUrl = "https://dpe-ru-deliveryclub-dev.azurewebsites.net/health/API",
                IsSchedule = false
            };

            var json = JsonConvert.SerializeObject(data);
            HttpClient client = new HttpClient();
            var _HttpContent = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:8000/api/channel", _HttpContent);
        }

        private static async Task SendRequest(int count = 1)
        {
            var sendingData = new QueueChannel { ChannelName = "xxxxx" };
            IQueueSerializer serializer = new IQueueSerializer();
            Parallel.For(0, count, (x) =>
            {
                var queueData = serializer.PrepareQueueObject(_channelName, sendingData);
                var httpContent = new StringContent(JsonConvert.SerializeObject(queueData), Encoding.UTF8, "application/json");
                using HttpClient client = new HttpClient();
                var response = client.PostAsync("http://localhost:8001/api/record/save", httpContent).Result;
                var error = response.StatusCode != System.Net.HttpStatusCode.NoContent;
                if (error)
                    SlackLog.SendMessage($"Error. ResponseCode : {response.StatusCode}");
            });
        }
    }
}
