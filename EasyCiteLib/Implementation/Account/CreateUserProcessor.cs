using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Account;
using EasyCiteLib.Models.Account;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Account
{
    public class CreateUserProcessor : ICreateUserProcessor
    {
        private readonly IGenericDataContextAsync<User> _userContext;

        public CreateUserProcessor(IGenericDataContextAsync<User> userContext)
        {
            _userContext = userContext;
        }
        public async Task CreateIfNotExistsAsync(UserSaveData saveData)
        {
            if (await _userContext.DataSet.Where(u => u.GoogleIdentifier == saveData.ProviderKey).AnyAsync())
                return;

            _userContext.DataSet.Add(new User
            {
                GoogleIdentifier = saveData.ProviderKey,
                Firstname = saveData.Firstname,
                Lastname = saveData.Lastname,
                Email = saveData.Email
            });
            await _userContext.SaveChangesAsync();
        }
    }
}