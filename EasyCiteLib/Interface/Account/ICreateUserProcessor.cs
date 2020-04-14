using System.Threading.Tasks;
using EasyCiteLib.Models.Account;

namespace EasyCiteLib.Interface.Account
{
    public interface ICreateUserProcessor
    {
        Task<int> CreateIfNotExistsAsync(UserSaveData saveData);
    }
}