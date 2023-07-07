using Application.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin"+","+"Magaza")]
    public class MagazaController : Controller
    {


        readonly IProductServices _productServices;
        readonly IColorServices _colorServices;
        readonly IStockServices _stockServices;
        readonly ITeklilerServices _teklilerServices;
        readonly IMapper _mapper;

        public MagazaController(IProductServices productServices, IStockServices stockServices, IMapper mapper, IColorServices colorServices, ITeklilerServices teklilerServices)
        {
            _mapper = mapper;
            _productServices = productServices;
            _stockServices = stockServices;
            _colorServices = colorServices;
            _teklilerServices = teklilerServices;
        }
       
        public async Task<IActionResult> MagazaIndex()
        {
            var query =HttpContext.User.Claims.ToList();
            var query2 = HttpContext.User;
            var q = User.Claims.ToList();
            //var userId = int.Parse((User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)).Value);
            HttpContext.Session.SetString("Magaza", "Magaza");
            return View();
          
        }
        //public IActionResult SiparisOlustur()
        //{
        //    return View();
        //}
        public async Task<IActionResult> MagazaStock()
        {
            var result = await _productServices.GetAll();
            return View(result.Data);
        }
        public async Task<IActionResult> ProductAndStockMagaza(long id)
        {
           var result =  await _productServices.OnlyMagaza(id);
            return View(result);
        }
        public IActionResult Update()
        {
            return View();
        }

    }
}
