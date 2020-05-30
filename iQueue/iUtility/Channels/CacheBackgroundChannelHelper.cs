using iModel.Channels;
using iModel.Keys;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace iUtility.Channels
{
    public class CacheBackgroundChannelHelper : CacheChannelHelper<BackgroundQueueChannel>
    {
        public CacheBackgroundChannelHelper(IDatabase redis) : base(redis)
        {
            _CacheKey = CacheKey.BACKGROUND_CHANNEL_CACHE_HASH_KEY;
        }
    }
}
