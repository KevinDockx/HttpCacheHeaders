namespace Marvin.Cache.Headers
{
    using System.Collections.Generic;

    public class RequestKey : Dictionary<string, string>
    {
        public override string ToString() => string.Join("-", Values);
    }
}