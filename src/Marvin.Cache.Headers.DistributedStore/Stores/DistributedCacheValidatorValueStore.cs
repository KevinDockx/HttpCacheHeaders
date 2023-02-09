using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Marvin.Cache.Headers.DistributedStore.Stores
{
    public class DistributedCacheValidatorValueStore : IValidatorValueStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRetrieveDistributedCacheKeys _distributedCacheKeyRetriever;

        public DistributedCacheValidatorValueStore(IDistributedCache distributedCache, IRetrieveDistributedCacheKeys distributedCacheKeyRetriever)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _distributedCacheKeyRetriever = distributedCacheKeyRetriever ?? throw new ArgumentNullException(nameof(distributedCacheKeyRetriever));
        }

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = await _distributedCache.GetAsync(key.ToString(), CancellationToken.None);
            return result == null ? null : CreateValidatorValue(result);
        }

        private ValidatorValue CreateValidatorValue(byte[] validatorValueBytes)
        {
            var validatorValueUtf8String = Encoding.UTF8.GetString(validatorValueBytes);
            var validatorValueETagTypeString = validatorValueUtf8String[..validatorValueUtf8String.IndexOf(" ", StringComparison.InvariantCulture)];
            var validatorValueETagType = Enum.Parse<ETagType>(validatorValueETagTypeString);
            var validatorValueETagValueWithLastModifiedDate = validatorValueUtf8String[(validatorValueETagTypeString.Length+7)..];
            var lastModifiedIndex = validatorValueETagValueWithLastModifiedDate.LastIndexOf("LastModified=", StringComparison.InvariantCulture);
            var validatorValueETagValueWithQuotes = validatorValueETagValueWithLastModifiedDate.Substring(0, lastModifiedIndex-1);
            var validatorValueETagValue = validatorValueETagValueWithQuotes.Substring(1, validatorValueETagValueWithQuotes.Length - 2); //We can't use String.Replace here as we may have embedded quotes.
            var lastModifiedDateString = validatorValueETagValueWithLastModifiedDate.Substring(validatorValueETagValueWithLastModifiedDate.LastIndexOf("=", StringComparison.InvariantCulture)+1);
            DateTimeOffset parsedDateTime =DateTimeOffset.Parse(lastModifiedDateString, CultureInfo.InvariantCulture);
            return new ValidatorValue(new ETag(validatorValueETagType, validatorValueETagValue), parsedDateTime);
        }

        public async Task SetAsync(StoreKey key, ValidatorValue validatorValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(StoreKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase)
        {
            throw new NotImplementedException();
        }
    }
}