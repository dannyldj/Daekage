using System.Threading.Tasks;

namespace Daekage.Contracts.Services
{
    public interface IOAuthService
    {
        Task GoogleAuth();

        Task UserinfoCall();
    }
}
