using Application.Interface;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Core.Utilities.Results;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        //private readonly UserManager<AppUser> _userManager;
        //private readonly SignInManager<AppUser> _signInManager;

       private readonly IAppUserServices _appUserServices;
       private readonly IUserRolServices _userRolServices;

        public AccountController(IAppUserServices appUserServices, IUserRolServices userRolServices)
        {
            _appUserServices = appUserServices;
            _userRolServices = userRolServices;
        }
        //[Authorize(Roles = "Admin"+","+"Magaza")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserIndex()
        {
            var users = await _appUserServices.GetUsersAndRoles();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = (await _userRolServices.GetAll()).Data;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AppUser user)
        {
            // default tenant koydum silinecek
            user.TenantId = 1;
            var result = await _appUserServices.Add(user);
            if (result.Success)
            {
                return RedirectToAction("UserIndex");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(long id)
        {
            ViewBag.Roles = (await _userRolServices.GetAll()).Data;
            var users = await _appUserServices.GetUserAndRole(id);
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(AppUser user)
        {
            user.TenantId = 1;
            var result = await _appUserServices.UpdateAll(user);
            if (result.Success)
            {
                return RedirectToAction("UserIndex");
            }
            return View();

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var res = await _appUserServices.GetById(id);
                await _appUserServices.Delete(res.Data);
                return RedirectToAction("UserIndex");
            }
            catch (System.Exception)
            {
                return RedirectToAction("UserIndex");
            }
        }
        public IActionResult SignIn()
        {
            return View();
        }
        public string HtmlScriptRegex(string htmlText)
        {
            string textReplace = htmlText;
            string regexPattern = @"[<>£#$½\[\]\}\|\{]|javascript|script";
            if (Regex.IsMatch(textReplace, regexPattern, RegexOptions.IgnoreCase))
            {
                Regex specialCharsRegex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                htmlText = specialCharsRegex.Replace(textReplace, "");

            }
            return htmlText;
        }
        [HttpPost]
        public async Task<string> SignIn(UserSingInModel signIn)
        {
            //**/
            /* giriş yapan kulalncıyı yani deopcu mu magaza mı yojksa admin mi session da yada cailmde tutacakm
                ş
            imdilik session deniyorumö rollere geçene kadar
             
             */
            signIn.Password = HtmlScriptRegex(signIn.Password);
            signIn.Password=  signIn.Password.Replace("[", "").Replace("]", "").Replace("/", "").Replace("@", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "");
            string role = "";
            var result = await _appUserServices.CheckUserAsync(signIn);
            if (result!=null)
            {
                var roleResult = await _appUserServices.GetRolesByUserIdAsync(result.Id);
                // ilgili kullanıcının rollerini çekmemiz.
                var claims = new List<Claim>();

                if (roleResult!=null)
                {
                    //foreach (var role in roleResult)
                    //{
                    //}
                    role = roleResult.ToString();
                        claims.Add(new Claim(ClaimTypes.Role, roleResult.ToString()));
                }

                claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()));

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = /*dto.RememberMe*/ true,
                };
               
              await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return role;
            }
            ModelState.AddModelError("Kullanıcı adı veya şifre hatalı","hata");
            return "";
        }
       

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }
    }
}
