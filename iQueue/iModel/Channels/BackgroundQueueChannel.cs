using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace iModel.Channels
{
    public class BackgroundQueueChannel : QueueChannel
    {
        public DateTime ExecutedDate { get; set; }
        public int FailerHealthCheckCounter { get; set; }

        public BackgroundQueueChannel()
        {

        }

        public BackgroundQueueChannel(QueueChannel channel)
        {
            ChannelName = channel.ChannelName;
            ConsumeUrl = channel.ConsumeUrl;
            FailureCount = channel.FailureCount;
            FetchCount = channel.FetchCount;
            HealthCheckUrl = channel.HealthCheckUrl;
            ExecuteEverySecond = channel.ExecuteEverySecond;
        }
    }
}
