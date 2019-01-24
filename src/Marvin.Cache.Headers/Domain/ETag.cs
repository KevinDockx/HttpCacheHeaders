// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

namespace Marvin.Cache.Headers
{
    public class ETag
    {
        public ETagType ETagType { get; }
        public string Value { get; }

        public ETag(ETagType eTagType, string value)
        {
            ETagType = eTagType;
            Value = value;
        }

        // Create an ETag object from the given value and determine if the ETag
        // is string of weak. If the strength of the ETag cannot be determined,
        // the default Etag type is applied.
        public ETag(string value, ETagType defaultETagType = ETagType.Weak)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                ETagType = ETagType.Weak;
            }
            else if (value.StartsWith("W\"") && value.EndsWith("\"")) 
            {
                Value = value.Substring(2, value.Length - 3);
                ETagType = ETagType.Weak;
            }
            else if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                Value = value.Substring(1, value.Length - 2);
                ETagType = ETagType.Strong;
            }
            else
            {
                Value = value;
                ETagType = defaultETagType;
            }
        }

        public override string ToString()
        {
            switch (ETagType)
            {
                case ETagType.Strong:
                    return  $"\"{Value}\"";

                case ETagType.Weak:
                    return  $"W\"{Value}\"";

                default:
                    return $"\"{Value}\"";
            }
        }
    }
}
