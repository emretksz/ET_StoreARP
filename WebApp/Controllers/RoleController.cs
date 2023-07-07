using Application.Interface;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin" )]
    public class RoleController : Controller
    {
        IUserRolServices _userRolServices;

        public RoleController(IUserRolServices userRolServices)
        {
            _userRolServices = userRolServices;
        }

        public async Task<IActionResult> RoleIndex()
        {
            var result = await _userRolServices.GetAll();
            return View(result.Data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppRole role)
        {
            var roles = await _userRolServices.Add(role);
            return RedirectToAction("RoleIndex");
        }
        public async Task<IActionResult> Update(long id)
        {
            var tenant = await _userRolServices.GetById(id);
            return View(tenant.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AppRole role)
        {
            var update = _userRolServices.UpdateAll(role);
            return RedirectToAction("RoleIndex");
        }
     
        public async Task<IActionResult> Delete(long id)
        {
            var tenants = await _userRolServices.GetById(id);
            var delete = await _userRolServices.Delete(tenants.Data);
            return RedirectToAction("RoleIndex");
        }
    }
}
