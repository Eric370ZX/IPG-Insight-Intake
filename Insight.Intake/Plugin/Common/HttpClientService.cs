using Insight.Intake.Plugin.Common.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Common
{
    public class HttpClientService : IHttpClientService
    {
        private HttpClient _httpClient;
        
        public HttpClientService()
        {
            _httpClient = new HttpClient();
        }


        public async Task<HttpResponseMessage> PostAsync(string requestUrl)
        {
            return await _httpClient.PostAsync(requestUrl, null).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
