using System.Threading.Tasks;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyCite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IGenericDataContextAsync<User> _userDataContext;

        public AccountController(IGenericDataContextAsync<User> userDataContext)
        {
            _userDataContext = userDataContext;
        }

        [AllowAnonymous]
        public async Task Login(string returnUrl = "/Projects")
        {
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