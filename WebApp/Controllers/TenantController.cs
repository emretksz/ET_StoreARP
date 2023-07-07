using Application.Interface;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin" + "," + "Depo")]
    public class TenantController : Controller
    {
        private readonly ITenantServices _tenantServices;

        public TenantController(ITenantServices tenantServices)
        {
            _tenantServices = tenantServices;
        }

        public async Task<IActionResult> TenantIndex()
        {
            var result = await _tenantServices.GetAll();
            return View(result.Data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tenant tenant)
        {
            var tenants = await _tenantServices.Add(tenant);
            return RedirectToAction("TenantIndex");
        }
        public async Task<IActionResult> Update(long id)
        {
            var tenant = await _tenantServices.GetById(id);
            return View(tenant.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Tenant tenant)
        {
            var update = _tenantServices.UpdateAll(tenant);
            return RedirectToAction("TenantIndex");
        }
        public async Task<IActionResult> Delete(long id)
        {
            var tenants = await _tenantServices.GetById(id);
            var delete = await _tenantServices.Delete(tenants.Data);
            return RedirectToAction("TenantIndex");
        }

    }
}
