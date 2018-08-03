using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers
{
    internal static class HttpContextExtensions
    {
        internal static readonly string ContextItemsExpirationModelOptions = "HttpCacheHeadersMiddleware-ExpirationModelOptions";
        internal static readonly string ContextItemsValidationModelOptions = "HttpCacheHeadersMiddleware-ValidationModelOptions";

        public static ExpirationModelOptions ExpirationModelOptionsOrDefault(this HttpContext httpContext, ExpirationModelOptions @default) =>
            httpContext.Items.ContainsKey(ContextItemsExpirationModelOptions)
                ? (ExpirationModelOptions)httpContext.Items[ContextItemsExpirationModelOptions]
                : @default;

        public static ValidationModelOptions ValidationModelOptionsOrDefault(this HttpContext httpContext, ValidationModelOptions @default) =>
            httpContext.Items.ContainsKey(ContextItemsValidationModelOptions)
                ? (ValidationModelOptions)httpContext.Items[ContextItemsValidationModelOptions]
                : @default;
    }
}