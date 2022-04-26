using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Common.Interfaces
{
    public interface IHttpClientService: IDisposable
    {
        Task<HttpResponseMessage> PostAsync(string requestUrl);
    }
}
