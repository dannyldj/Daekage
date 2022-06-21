using System.Threading.Tasks;
using Daekage.Core.Contracts.Services;
using Newtonsoft.Json;
using RestSharp;

namespace Daekage.Core.Services
{
    public class RestService : IRestService
    {
        private readonly RestClient _client = new RestClient("http://52.79.163.97:5000/");

        public async Task<T> RestRequest<T>(Method method, string route, object body)
        {
            var request = new RestRequest(route, method);
            _ = request.AddHeader("Content-Type", "application/json");

            if (body != null)
                _ = request.AddBody(JsonConvert.SerializeObject(body), "application/json");

            var data = await _client.ExecuteAsync(request);
            if (!data.IsSuccessful) return default;

            var result = JsonConvert.DeserializeObject<T>(data.Content);
            return result;
        }

        public async Task<bool> NotReturnRestRequest(Method method, string route, object body)
        {
            var request = new RestRequest(route, method);
            _ = request.AddHeader("Content-Type", "application/json");

            if (body != null)
                _ = request.AddBody(JsonConvert.SerializeObject(body), "application/json");

            var data = await _client.ExecuteAsync(request);
            return data.IsSuccessful;
        }
    }
}
