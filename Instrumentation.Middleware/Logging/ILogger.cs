using System;
using System.Threading.Tasks;

namespace Instrumentation.Middleware.Logging
{
    public interface ILogger
    {
        Task Log(string pageName, string requestUrl, double responseTimeForCompleteRequest, long responseLength, DateTime timeStamp);
    }
}