// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

namespace Marvin.Cache.Headers
{
    using System.Collections.Generic;

    public class RequestKey : Dictionary<string, string>
    {
        public override string ToString() => string.Join("-", Values);
    }
}
