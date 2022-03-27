using System.Collections.Generic;

namespace Palprimes.Common
{
    public static class DaprDomain
    {
        public const string RedisPubSub = "pubsub";
        public const string KafkaPubSub = "kafka-pubsub";
        public const string RedisStateStore = "statestore";

        public static class EventMetadata
        {
            private const string PartitionKey = "partitionKey";
            public static Dictionary<string, string> PartitionKeyMetadata(int key)
            {
                return new Dictionary<string, string> { { DaprDomain.EventMetadata.PartitionKey, key.ToString() } };
            }
        }

        public static class PubSubTopics
        {
            public const string ReceiveNumber = "receivenumber";
            public const string Results = "results";
        }
    }
}