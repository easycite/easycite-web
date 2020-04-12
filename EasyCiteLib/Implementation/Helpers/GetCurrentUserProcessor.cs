using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Repository.EasyCite;
using EasyCiteLib.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System;
using System.Security;

namespace EasyCiteLib.Implementation.Helpers
{
    public class GetCurrentUserProcessor : IGetCurrentUserProcessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericDataContextAsync<User> _userDataContext;

        public GetCurrentUserProcessor(
            IHttpContextAccessor httpContextAccessor,
            IGenericDataContextAsync<User> userDataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userDataContext = userDataContext;
        }
        public async Task<User> GetUserAsync()
        {   
            var userIdValue = _httpContextAccessor.HttpContext.User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            int userId;
            bool parsed = int.TryParse(userIdValue, out userId);

            var user = await _userDataContext.DataSet.FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        /// <exception cref="FormatException">Thrown when the userId claim is not an int</exception>
        /// <exception cref="SecurityException">Thrown when the userId is not found in the database</exception>
        public async Task<int> GetUserIdAsync()
        {
            var userIdValue = _httpContextAccessor.HttpContext.User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            int userId;
            bool parsed = int.TryParse(userIdValue, out userId);
            if(parsed == false)
                throw new FormatException($"'{userIdValue}' could not be parsed to an int");

            if(await _userDataContext.DataSet.AnyAsync(u => u.Id != userId))
                throw new SecurityException("That userId could not be found");

            return userId;
        }
    }
}