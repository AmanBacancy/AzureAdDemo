using AzureAdDemo.Data;
using AzureAdDemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace AzureAdDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureAdDemoDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(ILogger<HomeController> logger, AzureAdDemoDbContext context, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _context = context;
            _accessor = accessor;
        }

        //[Authorize(Policy = "Admin", Roles = "Admin,SuperAdmin")]
        [Authorize(Policy = "Admin")]
        //[Authorize(Policy = "SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Username == loginModel.Username && x.Password == loginModel.Password && x.IsActive);
                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();
                    var roles = await _context.UserRoleMapping.Where(x => x.UserId == user.Id && x.IsActive).Include(i => i.RoleNavigation).Select(s => s.RoleNavigation).ToListAsync();

                    // Sign in the user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        // Add additional claims as needed
                    };

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Set to true if you want to persist the cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Set the expiration time as needed
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                    _accessor.HttpContext.Response.Cookies.Append("loginID", user.Id.ToString());
                    _accessor.HttpContext.Response.Cookies.Append("token", token);
                    _accessor.HttpContext.Response.Cookies.Append("roleIds", string.Join(',', roles.Select(s => s.Id)));

                    return RedirectToAction("Index");
                }
                return View();
            }
            return View();
        }

        [HttpPost]
        public new IActionResult SignOut()
        {
            if (User.Identity.IsAuthenticated && (User.Identity.AuthenticationType == OpenIdConnectDefaults.AuthenticationScheme ||
         User.Identity.AuthenticationType == "Authentication.Federation"))
            {
                // Redirect to Azure AD sign-out
                var callbackUrl = Url.Action("Login", "Home", values: null, protocol: Request.Scheme);
                return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            }
            else
            {
                // Delete cookies
                Response.Cookies.Delete("loginID");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("roleIds");
                // Sign out locally
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // You can perform additional actions for local sign-out if needed

                // Redirect to your desired page after local sign-out
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult AzureADLogin()
        {
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}