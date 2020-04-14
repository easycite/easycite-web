using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EasyCiteLib.Interface.Account;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ICreateUserProcessor _createUserProcessor;
        private readonly IGenericDataContextAsync<User> _userDataContext;

        public AccountController(
            ICreateUserProcessor createUserProcessor,
            IGenericDataContextAsync<User> userDataContext)
        {
            _createUserProcessor = createUserProcessor;
            _userDataContext = userDataContext;
        }
        [AllowAnonymous]
        public async Task Login(string returnUrl = "/")
        {
            var url = Url.Action(Url.Action("LoginCallback", "Account", new { returnUrl = returnUrl }));
            await HttpContext.ChallengeAsync(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        public async Task Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userDataContext.DataSet.FirstOrDefaultAsync());
        }
    }
}