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
        public async Task<int> CreateIfNotExistsAsync(UserSaveData saveData)
        {
            var user = await _userContext.DataSet.FirstOrDefaultAsync(u => u.GoogleIdentifier == saveData.ProviderKey); 
            if (user != null)
                return user.Id;

            user = new User
            {
                GoogleIdentifier = saveData.ProviderKey,
                Firstname = saveData.Firstname,
                Lastname = saveData.Lastname,
                Email = saveData.Email
            };
            _userContext.DataSet.Add(user);
            
            await _userContext.SaveChangesAsync();

            return user.Id;
        }
    }
}