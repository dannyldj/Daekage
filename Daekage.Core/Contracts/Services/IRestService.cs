using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Daekage.Core.Contracts.Services
{
    public interface IRestService
    {
        Task<T> RestRequest<T>(Method method, string route, object body);

        Task<bool> NotReturnRestRequest(Method method, string route, object body);
    }
}
