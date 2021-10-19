using System;
using System.Collections.Generic;
using System.Text;

namespace Marvin.Cache.Headers
{

    /// <summary>
    /// Mark your action with this attribute to ensure the HttpCache middleware fully ignores it, for example: to avoid 
    /// ETags being generated for large file output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCacheIgnoreAttribute : Attribute
    { 
    }
}
