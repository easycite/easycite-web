using System.Threading.Tasks;
using EasyCiteLib.Interface.Account;
using EasyCiteLib.Models.Account;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;

namespace EasyCiteLib.Implementation.Account
{
    public class CreateUserProcessor : ICreateUserProcessor
    {
        private readonly IGenericDataContextAsync<User> _userContext;

        public CreateUserProcessor(IGenericDataContextAsync<User> userContext)
        {
            _userContext = userContext;
        }
        public Task CreateIfNotExistsAsync(UserSaveData saveData)
        {
            _userContext.DataSet.Add(new User
            {
                GoogleIdentifier = saveData.ProviderKey,
                Firstname = saveData.Firstname,
                Lastname = saveData.Lastname,
                Email = saveData.Email
            });

            return _userContext.SaveChangesAsync();
        }
    }
}