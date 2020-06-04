using iModel.Channels;
using iUtility.Keys;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iUtility.Channels
{
    public class CacheChannelHelper<T> where T: IQueueChannel
    {
        private readonly IDatabase _redis;
        protected string _CacheKey;

        public CacheChannelHelper(IDatabase redis)
        {
            _redis = redis;
            _CacheKey = CacheKey.CHANNEL_CACHE_HASH_KEY;
        }

        public async Task Remove(string channelName)
        {
            await _redis.HashDeleteAsync(_CacheKey, channelName);
        }

        public async Task RemoveAll()
        {
            await _redis.KeyDeleteAsync(_CacheKey);
        }

        public async Task Create(T channelData)
        { 
            var jsonChannel = JsonConvert.SerializeObject(channelData);
            await _redis.HashSetAsync(_CacheKey, channelData.ChannelName, jsonChannel);
        }

        public async Task Update(T channelData)
        {
            var jsonChannel = JsonConvert.SerializeObject(channelData);
            await _redis.HashSetAsync(_CacheKey, channelData.ChannelName, jsonChannel);
        }

        public async Task<bool> Exist(string channelName)
            => await _redis.HashExistsAsync(_CacheKey, channelName);

        public async Task<T> Get(string channelName)
        {
            T channelData = default;
            var channel = await _redis.HashGetAsync(_CacheKey, channelName);
            if (channel.HasValue)
                channelData = JsonConvert.DeserializeObject<T>(channel);

            return channelData;
        }

        public virtual async Task<List<T>> GetAll()
        {
            List<T> channels = new List<T>();
            var cacheChannels = await _redis.HashGetAllAsync(_CacheKey);
            if (cacheChannels != null)
            {
                foreach (var channelEntry in cacheChannels)
                {
                    channels.Add(JsonConvert.DeserializeObject<T>(channelEntry.Value));
                }
            }

            return channels;
        }
    }
}
