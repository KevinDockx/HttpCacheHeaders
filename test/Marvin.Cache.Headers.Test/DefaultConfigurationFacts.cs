﻿// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Test.TestStartups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using EntityTagHeaderValue = System.Net.Http.Headers.EntityTagHeaderValue;

namespace Marvin.Cache.Headers.Test;

public class DefaultConfigurationFacts
{
    private readonly IWebHostBuilder _hostBuilder = new WebHostBuilder()
        .UseStartup<DefaultStartup>();

    private readonly TestServer _server;

    public DefaultConfigurationFacts()
    {
        _server = new TestServer(_hostBuilder);
    }

    [Fact]
    public async Task Adds_Default_Validation_And_ExpirationHeaders()
    {
        using (var client = _server.CreateClient())
        {
            var response = await client.GetAsync("/");

            Assert.True(response.IsSuccessStatusCode);

            Assert.Collection(response.Headers,
                pair => Assert.True(pair.Key == HeaderNames.CacheControl && pair.Value.First() == "public, max-age=60"),
                pair => Assert.True(pair.Key == HeaderNames.ETag),
                pair =>
                {
                    Assert.True(pair.Key == HeaderNames.Vary);
                    Assert.Collection(response.Headers.Vary,
                        vary => Assert.Equal("Accept", vary),
                        vary => Assert.Equal("Accept-Language", vary),
                        vary => Assert.Equal("Accept-Encoding", vary));
                });
        }
    }

    [Fact]
    public async Task Returns_Same_Etag_For_Same_Request()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);

            Assert.Equal(
                response1.Headers.GetValues(HeaderNames.ETag).First(),
                response2.Headers.GetValues(HeaderNames.ETag).First());
        }
    }

    [Fact]
    public async Task Returns_Different_Etag_For_Different_Request()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/foo");
            var response2 = await client.GetAsync("/bar");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);

            Assert.NotEqual(
                response1.Headers.GetValues(HeaderNames.ETag).First(),
                response2.Headers.GetValues(HeaderNames.ETag).First());
        }
    }

    [Theory]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(1500)]
    public async Task Return_304_When_Request_Is_Cached(int delayInMs)
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var lastmodified = response1.Content.Headers.LastModified;
            var etag = response1.Headers.GetValues(HeaderNames.ETag).First();

            await Task.Delay(delayInMs);
            client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag, false));
            client.DefaultRequestHeaders.IfModifiedSince = lastmodified.Value.AddMilliseconds(delayInMs);
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == HttpStatusCode.NotModified);

            Assert.Equal(
                response1.Headers.GetValues(HeaderNames.ETag).First(),
                response2.Headers.GetValues(HeaderNames.ETag).First());
        }
    }

    [Fact]
    public async Task Returns_304_NotModified_When_Request_Has_Matching_Etag_And_No_IfModifiedSince()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var etag = response1.Headers.GetValues(HeaderNames.ETag).First();

            client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag, false));
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == HttpStatusCode.NotModified);
        }
    }

    [Fact]
    public async Task Returns_304_NotModified_When_Request_HasValid_IfModifiedSince_AndNo_Etag()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var lastmodified = response1.Content.Headers.LastModified;

            client.DefaultRequestHeaders.IfModifiedSince = lastmodified;
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == HttpStatusCode.NotModified);
        }
    }

    [Fact]
    public async Task IfModifiedSince_OnRequest_IsIgnored_When_RequestContains_Valid_IfNoneMatch_Header()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var lastmodified = response1.Content.Headers.LastModified;
            var etag = response1.Headers.GetValues(HeaderNames.ETag).First();

            var expiredLastModified = lastmodified.Value.AddMilliseconds(-600000);
            client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag, false));
            client.DefaultRequestHeaders.IfModifiedSince = expiredLastModified;
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == HttpStatusCode.NotModified);
        }
    }

    [Fact]
    public async Task Returns_200_Ok_When_Request_HasValid_IfModifiedSince_But_Invalid_Etag()
    {
        using (var client = _server.CreateClient())
        {
            var response1 = await client.GetAsync("/");
            var lastmodified = response1.Content.Headers.LastModified;

            client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue("\"invalid-etag\"", false));
            client.DefaultRequestHeaders.IfModifiedSince = lastmodified;
            var response2 = await client.GetAsync("/");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == HttpStatusCode.OK);
        }
    }
}