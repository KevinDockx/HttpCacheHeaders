// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Domain;
using Marvin.Cache.Headers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers
{
    public class DefaultStoreKeyGenerator : IStoreKeyGenerator
    {
        public Task<StoreKey> GenerateStoreKey(StoreKeyContext context)
        {
            // generate a key to store the entity tag with in the entity tag store
            List<string> requestHeaderValues;

            // get the request headers to take into account (VaryBy) & take
            // their values        
            if (context.VaryByAll)
            {
                requestHeaderValues = context.HttpRequest
                        .Headers
                        .SelectMany(h => h.Value)
                        .ToList();
            }
            else
            {
                requestHeaderValues = context.HttpRequest
                        .Headers
                        .Where(x => context.Vary.Any(h => 
                            h.Equals(x.Key, StringComparison.CurrentCultureIgnoreCase)))
                        .SelectMany(h => h.Value)
                        .ToList();
            }

            // get the resource path
            var resourcePath = context.HttpRequest.Path.ToString();

            // get the query string
            var queryString = context.HttpRequest.QueryString.ToString();

            // combine these
            return Task.FromResult(new StoreKey
            {
                { nameof(resourcePath), resourcePath },
                { nameof(queryString), queryString },
                { nameof(requestHeaderValues), string.Join("-", requestHeaderValues)}
            });
        }
    }
}
