﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ExternalUserService.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq.Protected;
using Moq;

namespace ExternalUserService.Tests
{
    public class ReqresClientTests
    {
        [Fact]
        public async Task GetUserByIdAsync_ReturnsValidUser()
        {
            var responseContent = @"{ ""data"": { ""id"": 1, ""email"": ""test@test.com"", ""first_name"": ""John"", ""last_name"": ""Doe"" } }";
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var client = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("https://reqres.in/api/") };
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ReqresClient(client, cache, NullLogger<ReqresClient>.Instance);

            var result = await service.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }
    }
}
