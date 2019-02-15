using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marvin.Cache.Headers.Domain
{
    /// <summary>
    /// Context containing information on a specific resource
    /// </summary>
    public class ResourceContext
    {
        /// <summary>
        /// The current <see cref="HttpRequest"/>
        /// </summary>
        public HttpRequest HttpRequest { get; }

        /// <summary>
        /// The current <see cref="StoreKey"/> for the resource, if available
        /// </summary>
        public StoreKey StoreKey { get; }

        public ResourceContext(HttpRequest httpRequest,
            StoreKey storeKey)
        {
            HttpRequest = httpRequest;
            StoreKey = storeKey;
        }
    }
}
