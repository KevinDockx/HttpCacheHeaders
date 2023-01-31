using System.Collections.Generic;
using System.Linq;

// Used list on https://en.wikipedia.org/wiki/List_of_HTTP_status_codes

namespace Marvin.Cache.Headers.Utils
{
    /// <summary>
    /// Contains predefined list of status codes.
    /// </summary>
    public static class HttpStatusCodes
    {
        /// <summary>
        /// Contains all status codes for client errors in the 4xx range.
        /// </summary>
        public static readonly IEnumerable<int> ClientErrors = new[] { 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 428, 429, 430, 431, 440, 449, 450, 451, 498, 499 };

        /// <summary>
        /// Contains all status codes for server errors in the 5xx range.
        /// </summary>
        public static readonly IEnumerable<int> ServerErrors = new[] { 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 529, 530, 598, 599 };

        /// <summary>
        /// Contains all error status codes in the 4xx and 5xx range.
        /// </summary>
        public static readonly IEnumerable<int> AllErrors = ClientErrors.Concat(ServerErrors);
    }
}