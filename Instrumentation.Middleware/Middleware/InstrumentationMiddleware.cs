using System;

using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Instrumentation.Middleware.Logging;

namespace Instrumentation.Middleware
{
    public class InstrumentationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public InstrumentationMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();

                var requestUrl = (context.Request.IsHttps ? "https://" : "http://") + context.Request.HttpContext.Request.Host.Value + context.Request.Path;

                var requestTime = DateTime.UtcNow;
                
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    var response = context.Response;
                    response.Body = responseBody;
                    
                    await _next(context);

                    string responseBodyContent = null;
                    responseBodyContent = await ReadResponseBody(response);
                    
                    await responseBody.CopyToAsync(originalBodyStream);

                    stopWatch.Stop(); // processing time

                    await _logger.Log(
                        context.Request.Path,
                        requestUrl,
                        stopWatch.ElapsedMilliseconds,
                        responseBodyContent.Length * 2, // bytes
                        requestTime);
                }

            }
            catch (Exception ex)
            {
                await _next(context);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
           
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
           
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }       
    }

    // A nice way of exposing this through an extension method vs having to pass in the type explicitly.
    public static class InstrumentationMiddlewareExtensions
    {
        public static IApplicationBuilder UseInstrumentationMiddleware(this IApplicationBuilder builder, string reportingUrl)
        {            
            return builder.UseMiddleware<InstrumentationMiddleware>(new RequestInfoLogger(reportingUrl));
        }
    }
}

