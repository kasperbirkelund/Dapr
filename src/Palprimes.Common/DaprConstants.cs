namespace Palprimes.Common
{
    public static class DaprConstants
    {        
        public const string RedisPubSub = "pubsub";
        public const string KafkaPubSub = "kafka-pubsub";
        public const string RedisStateStore = "statestore";

        public static class PubSubTopics
        {
            public const string ReceiveNumber = "receivenumber";
            public const string Results = "results";
        }
    }
}