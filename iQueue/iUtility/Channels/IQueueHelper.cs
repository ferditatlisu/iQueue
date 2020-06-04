using iModel.Channels;
using iUtility.Keys;
using System;
using System.Collections.Generic;
using System.Text;

namespace iUtility.Channels
{
    public class IQueueHelper
    {
        public static string GetChannelNameForProducer(IQueueChannel channel)
        {
            string queueName = channel.ChannelName;
            if (channel.IsSchedule)
                queueName = string.Format(CustomKey.SCHEDULE_QUEUE_NAME_FORMAT, channel.ChannelName);

            return queueName;
        }

        public static string GetChannelNameForConsumer(IQueueChannel channel)
            => channel.ChannelName;

        public static string GetExchangeNameForDeadLetter(IQueueChannel channel)
            => string.Format(CustomKey.SCHEDULE_QUEUE_EXCHANGE_NAME_FORMAT, channel.ChannelName);
    }
}
