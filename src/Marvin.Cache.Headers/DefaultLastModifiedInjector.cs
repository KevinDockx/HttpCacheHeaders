using Marvin.Cache.Headers.Domain;
using Marvin.Cache.Headers.Interfaces;
using System;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers;

public class DefaultLastModifiedInjector : ILastModifiedInjector
{
    public Task<DateTimeOffset> CalculateLastModified(ResourceContext context)
    {
        // the default implementation returns the current date without
        // milliseconds
        var now = DateTimeOffset.UtcNow;

        return Task.FromResult(new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Offset));
    }
}