using iModel.Queues;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace iQueue.ByteSerializer.Serializers
{
    public class IQueueSerializer
    {
        public QueueData PrepareQueueObject<T>(string channelName, T data, string? queueId = null) where T : class, new()
        {
            if (string.IsNullOrEmpty(queueId))
                queueId = Guid.NewGuid().ToString();

            QueueData queueData = new QueueData
            {
                Id = queueId,
                ChannelName = channelName,
                Data = MergeData(data, queueId)
            };

            return queueData;
        }

        private byte[] MergeData<T>(T data, string queueId) where T : class, new()
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var jsonDataLenght = jsonData.Length;
            byte queueIdLenght = (byte)queueId.Length;

            byte[] byteData = new byte[1 + queueIdLenght + jsonDataLenght];
            byteData[0] = queueIdLenght; //Information of the QueueId lenght

            var byteDataQueueIdStartIndex = 1;
            var queueIdByte = Encoding.UTF8.GetBytes(queueId);
            Array.Copy(queueIdByte, 0, byteData, byteDataQueueIdStartIndex, queueIdLenght);

            var byteDataOriginalDataStartIndex = 1 + queueIdLenght;
            var originalByteData = Encoding.UTF8.GetBytes(jsonData);
            Array.Copy(originalByteData, 0, byteData, byteDataOriginalDataStartIndex, jsonDataLenght);
            return byteData;
        }

        public QueueData UnMergeData(ReadOnlyMemory<byte> data)
        {
            var byteArrayData = data.Span.ToArray();
            var idLenght = byteArrayData[0];
            var id = Encoding.UTF8.GetString(data.Slice(1, idLenght).Span);

            QueueData queueData = new QueueData
            {
                Id = id,
                Data = byteArrayData,
            };

            return queueData;
        }
    }
}
