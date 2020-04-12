using System.Threading.Tasks;
using EasyCiteLib.Repository.EasyCite;

namespace EasyCiteLib.Interface.Helpers
{
    public interface IGetCurrentUserProcessor
    {
        Task<User> GetUserAsync();
        Task<int> GetUserIdAsync();
    }
}