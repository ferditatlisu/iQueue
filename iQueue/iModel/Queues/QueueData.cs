using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace iModel.Queues
{
    public class QueueData
    {
        public byte[] Data { get; set; }
        public string ChannelName { get; set; }
        public string ScheduleTime { get; set; }
        public int FailedCount { get; set; }
    }
}
