using iModel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Storages
{
    public class QueueMessageLogData
    {
        public string QueueId { get; set; }
        public MessageStatus MessageStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
