// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

namespace Marvin.Cache.Headers;

public class ETag
{
    public ETagType ETagType { get; }
    public string Value { get; }

    public ETag(ETagType eTagType, string value)
    {
        ETagType = eTagType;
        Value = value;
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
