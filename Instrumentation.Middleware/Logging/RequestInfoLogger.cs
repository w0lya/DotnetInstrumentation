using System;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;


namespace Instrumentation.Middleware.Logging
{
    public class RequestInfoLogger : ILogger
    {
        private readonly string _endpoint;

        public RequestInfoLogger(string endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task Log(string pageName, string requestUrl, double responseTimeForCompleteRequest, long responseLength, DateTime timeStamp)
        {
            var json = JsonConvert.SerializeObject(
                new {
                    RequestUrl = requestUrl,
                    TimeStamp = timeStamp,
                    ResponseBodySize = responseLength, 
                    ResponseTime = responseTimeForCompleteRequest                    
                });

            // posting event to 'monitoring' endpoint
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(_endpoint,
                      new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }
    }
}
