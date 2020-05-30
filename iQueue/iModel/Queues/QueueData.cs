using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Queues
{
    public class QueueData
    {
        public string Data { get; set; }
        public string ChannelName { get; set; }
        public int FailedCount { get; set; }
    }
}
