using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Channels
{
    public class BackgroundQueueChannel : QueueChannel
    {
        public DateTime ExecutedDate { get; set; }
        public int FailerHealthCheckCounter { get; set; }
    }
}
