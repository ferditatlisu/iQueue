using iModel.Channels;
using iUtility.Keys;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
