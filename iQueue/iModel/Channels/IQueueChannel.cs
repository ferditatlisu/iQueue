using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Channels
{
    public interface IQueueChannel
    {
        string ChannelName { get; set; }
        string ConsumeUrl { get; set; }
        int FetchCount { get; set; }
        string HealthCheckUrl { get; set; }
        int FailureCount { get; set; }
        bool IsSchedule { get; set; }
    }
}
