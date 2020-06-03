using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Channels
{
    public class QueueChannel : IQueueChannel
    {
        public string ChannelName { get; set; }
        public string ConsumeUrl { get; set; }
        public int ExecuteEverySecond { get; set; }
        public int FetchCount { get; set; }
        public string HealthCheckUrl { get; set; }
        public int FailureCount { get; set; }
        public bool IsSchedule { get; set; }
    }
}
