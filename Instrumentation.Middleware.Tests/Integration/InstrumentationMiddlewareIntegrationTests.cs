using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net.Http;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Instrumentation.Website;

namespace Instrumentation.Middleware.Tests.Integration
{
    public class InstrumentationMiddlewareIntegrationTests
    {
        [Fact]
        public async void Middleware_Does_Not_Change_Response_Async()
        {
            // Arrange
            var builder = new WebHostBuilder()                
                .Configure(app => { })
                .UseStartup<Startup>();

            var builderWithCustomMiddleware = new WebHostBuilder()
                .Configure(app => {
                    app.UseInstrumentationMiddleware(""); // endpoint doesn't matter here.
                })
                .UseStartup<Startup>();

            var server = new TestServer(builder);

            var serverWithMiddleware = new TestServer(builderWithCustomMiddleware);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/testpage");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            var requestCloneMessage = new HttpRequestMessage(new HttpMethod("GET"), "/testpage");
            var responseMessageWithMiddleware = await serverWithMiddleware.CreateClient().SendAsync(requestCloneMessage);

            var response = await responseMessage.Content.ReadAsStringAsync();
            var responsewithMiddleware = await responseMessageWithMiddleware.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response == responsewithMiddleware && responseMessage.StatusCode == responseMessageWithMiddleware.StatusCode);
        }

        [Fact]
        public async void Request_IsNotFailing_To_App_With_Middleware_With_Incorrect_Endpoint()
        {
            // Arrange 
            var builderWithCustomMiddleware = new WebHostBuilder()
                .Configure(app => {
                    app.UseInstrumentationMiddleware(""); // endpoint doesn't matter here.
                }
            );

            var server = new TestServer(builderWithCustomMiddleware);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/testpage");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);
            

            // Assert
            Assert.True(responseMessage.StatusCode != System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
