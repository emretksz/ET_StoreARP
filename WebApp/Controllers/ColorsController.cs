using Application.Interface;
using AutoMapper;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    //[Area("Colors")]
    [Authorize(Roles = "Admin"+","+"Depo")]
    public class ColorsController : Controller
    {
        readonly IProductServices _productServices;
        readonly IStockServices _stockServices;
        readonly IColorServices _colorServices;
        readonly IMapper _mapper;

        public ColorsController(IProductServices productServices, IStockServices stockServices, IMapper mapper, IColorServices colorServices)
        {
            _mapper = mapper;
            _productServices = productServices;
            _stockServices = stockServices;
            _colorServices = colorServices;
        }
        public async Task<IActionResult> ColorIndex()
        {
            var result = await _colorServices.GetAll();
            return View(result.Data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            var colors = await _colorServices.Add(color);
            return RedirectToAction("ColorIndex");
        }
        public async Task<IActionResult> Update(long id)
        {
            var colors = await _colorServices.GetById(id);
            return View(colors.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Color color)
        {
            var update = _colorServices.UpdateAll(color);
            return RedirectToAction("ColorIndex");
        }

        public async Task<IActionResult> Delete(long id)
        {
            var colors = await _colorServices.GetById(id);
            var delete = await _colorServices.Delete(colors.Data);
            return RedirectToAction("ColorIndex");
        }
    }
}
