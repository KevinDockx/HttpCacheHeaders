namespace Marvin.Cache.Headers.DistributedStore.Redis.Options
{
    public class RedisDistributedCacheKeyRetrieverOptions
    {
        /// <summary>
        /// The Redis database you wish keys to be retrieved from.
        /// </summary>
        public int Database { get; set; }
        
        /// <summary>
        /// Indicates whether only replicas should be used when choosing the servers to iterate looking for keys in the selected database.
        /// </summary>
        public bool OnlyUseReplicas { get; set;  }
    }
}