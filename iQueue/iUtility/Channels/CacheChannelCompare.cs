using iModel.Channels;
using iModel.Queues;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace iUtility.Channels
{
    public class CacheChannelCompare
    {
        private readonly IDatabase _redis;
        private readonly CacheBackgroundChannelHelper _cacheBackgroundChannelHelper;

        public CacheChannelCompare(IDatabase redis)
        {
            _redis = redis;
            _cacheBackgroundChannelHelper = new CacheBackgroundChannelHelper(_redis);
        }

        public void Execute(ref List<QueueChannel> channels, ref List<BackgroundQueueChannel> backgroundChannels)
        {
            // Phase 1 --------------------------------------------
            if (channels is null || channels.Count == 0)
            {
                ThereIsNoChannel(ref backgroundChannels);
                return;
            }
            // Phase 1 ----------------------------------------------

            // Phase 2 ----------------------------------------------

            foreach (var channel in channels)
            {
                var channelName = channel.ChannelName;
                var backgroundChannelItem = backgroundChannels.FirstOrDefault(x => x.ChannelName == channelName);
                if (backgroundChannelItem is null) 
                {
                    backgroundChannelItem = new BackgroundQueueChannel(channel);
                    _cacheBackgroundChannelHelper.Create(backgroundChannelItem).Wait();  //Add
                    backgroundChannels.Add(backgroundChannelItem);
                }
                else
                {
                    //Update if it's needed!
                    _cacheBackgroundChannelHelper.Update(backgroundChannelItem).Wait();  //Add
                }
            }

            // Phase 2 ----------------------------------------------

            // Phase 3 ----------------------------------------------

            foreach (var backgroundChannel in backgroundChannels)
            {
                var backgroundChannelName = backgroundChannel.ChannelName;
                var channelItem = channels.FirstOrDefault(x => x.ChannelName == backgroundChannelName);
                if (channelItem is null) 
                { 
                    _cacheBackgroundChannelHelper.Remove(backgroundChannelName).Wait();
                    backgroundChannels.RemoveAll(x=> x.ChannelName == backgroundChannelName);
                }
            }

            // Phase 3 ----------------------------------------------

        }

        private void ThereIsNoChannel(ref List<BackgroundQueueChannel> backgroundChannels)
        {
            backgroundChannels.Clear();
            _cacheBackgroundChannelHelper.RemoveAll().Wait();
        }
    }
}
