using Application.Interface;
using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Entities.Enums.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
//using PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static System.Net.WebRequestMethods;
//using PagedList.Core.Mvc;
using X.PagedList.Mvc.Core;
using X.PagedList.Mvc;
using X.PagedList;
using Core.Entities;
using Core.Utilities.Results;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Controllers
{
    //[Area("Product")]
    [Authorize(Roles = "Admin" + "," + "Depo")]
    public class ProductController : Controller
    {

        readonly IProductServices _productServices;
        readonly IProductDal _productRepository;
        readonly IColorServices _colorServices;
        readonly IStockServices _stockServices;
        readonly ITeklilerServices _teklilerServices;
        private readonly IWebHostEnvironment _evn;
        private readonly IMagazaStock _stockRepository;
        readonly IMapper _mapper;
        readonly IProductAgeDal _ageRepository;
        readonly ITenantDal _tenantRepository;

        public ProductController(IProductServices productServices, IStockServices stockServices, IMapper mapper, IColorServices colorServices, ITeklilerServices teklilerServices, IWebHostEnvironment evn, IMagazaStock stockRepository, IProductAgeDal ageRepository, ITenantDal tenantRepository)
        {
            _mapper = mapper;
            _productServices = productServices;
            _stockServices = stockServices;
            _colorServices = colorServices;
            _teklilerServices = teklilerServices;
            _evn = evn;
            _stockRepository = stockRepository;
            _ageRepository = ageRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<IActionResult> ProductIndex(int page =1, SearchParticalDto p=null)
        {
            //   var model = await _productServices.GetAll();
            //= new SearchParticalDto();
            ViewBag.search = false;
            var model = await _productServices.GetFilterProduct(p, page, 12);
            List<Product> result = new List<Product>();
            return View(await model.ToPagedListAsync(page,12)/*Data.ToPagedList(page,12)*/);
        }
        public async Task<IActionResult> ProductPartical(int page = 1, SearchParticalDto p = null)
        {
            var model = await _productServices.GetFilterProduct(p, page,12);
            IPagedList<Product> productItems;
            if (p.Search)
            {
                productItems = await model.ToPagedListAsync(1,1000);
                ViewBag.search = true;
            }
            else
            {
                 productItems = model.ToPagedList(page,5);
                ViewBag.search = false; 
            }
            ViewBag.pageCount = productItems.Count;
            return PartialView("ProductParticalView", productItems);
        }

        public async Task<IActionResult> ProductAdd(CreateProductDto createProductDto, IFormFile file)
        {
            if (file != null)
            {
                var FileDic = "images";
                string FilePath = Path.Combine(_evn.WebRootPath, FileDic);
                if (!Directory.Exists(FilePath))

                    Directory.CreateDirectory(FilePath);
                Guid g = Guid.NewGuid();
                var fileName = g;
                var filePath = Path.Combine(FilePath, fileName.ToString() + ".png");

                createProductDto.ModelImageUrl = "/images/" + fileName + ".png";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                }
            }

            var productId = await _productServices.Add(_mapper.Map<Product>(createProductDto));
    



            return RedirectToAction("AddColorForProduct",new { productId = productId });
        }
        public async Task<IActionResult> AddColorForProduct(long productId)
        {
            ProductColorDto dto = new ProductColorDto();
            // var obje = await _productServices.GetById(productId);
            dto.SingleProduct = await _productServices.GetProductIdByAge(productId);
            dto.ProductColor = ((await _colorServices.GetAll()).Data);
            ViewBag.ProductId = productId;
            var colorList = await _colorServices.GetAll();
            ViewBag.ColorCount = colorList.Data.Count;
            return View(dto);
        }

        SelectList GetEnumList()
        {
            var list = from Gender e in Enum.GetValues(typeof(Gender))
                       select new { EnumId = (int)e, Gender = e.ToString() };
            return new SelectList(list, "EnumId", "Gender", 1);

        }
        public async Task<IActionResult> ProductColorList(int page =1)
        {
            var result = await _colorServices.GetAll();
            ViewBag.ColorCount = result.Data.Count;
            var colors = result.Data.ToPagedList(page,12);
            return PartialView("ProductColorList", colors);
        }
        public async Task<IActionResult> UpdateProductStockColor(int page = 1)
        {
            var result = await _colorServices.GetAll();
            var colorList = await _colorServices.GetAll();
            ViewBag.ColorCountForStock = result.Data.Count;
            var colors = colorList.Data.ToPagedList(page, 12);
            return PartialView("UpdateProductStockColor", colors);
        }

        public async Task<IActionResult> UpdateProduct(long id)
        {
            var product = await _productServices.GetByProductQuery(id);
            ViewBag.productAge = new SelectList(((await _ageRepository.GetAllAsync())), "Id", "Name");
            ViewBag.Gender = GetEnumListGender();
            return View("UpdateProduct", product);
        }
        SelectList GetEnumListGender()
        {
            var list = from Gender e in Enum.GetValues(typeof(Gender))
                       select new { EnumId = (int)e, Gender = e.ToString() };
            return new SelectList(list, "EnumId", "Gender", 1);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(CreateProductDto createProductDto, IFormFile file)
        {
            var old = await _productServices.GetById(createProductDto.ProductId);
            if (file != null)
            {
                var FileDic = "images";
                string FilePath = Path.Combine(_evn.WebRootPath, FileDic);
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);
                Guid g = Guid.NewGuid();
                var fileName = g;
                var split = file.FileName.Split('.');

                var filePath = Path.Combine(FilePath, split[0]+fileName + ".png");

                createProductDto.ModelImageUrl = "/images/" + split[0] + fileName + ".png";
                if (old.Data.ModelImageUrl!=null)
                {
                    var filePathdelte = Path.Combine(FilePath, old.Data.ModelImageUrl);
                    if (System.IO.Directory.Exists(filePathdelte))
                    {
                        System.IO.Directory.Delete(filePathdelte);
                    }
                }

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                }
            }
            else
            {
                createProductDto.ModelImageUrl = old.Data.ModelImageUrl;
            }
            var product = _mapper.Map<Product>(createProductDto);
            product.Id = createProductDto.ProductId;
            ProductAges age = new ProductAges();
            age.Id = createProductDto.ProductAgesId;
            age.Name = "";
            product.ProductAges = age;

            var result = await _productServices.UpdateAll(product);
            return RedirectToAction("ProductIndex"/*,(await _productServices.GetAll()).Data*/);
        }
        [HttpPost]
        public async Task<IActionResult> AddColorForProduct(long productId,  String body, bool? update = null,string date=null)
        {
            var model = JsonConvert.DeserializeObject<List<ColorDto>>(body);
            var modelim = model.Where(a => a.ColorName != "" && a.Count != ",");
            //DateTime newDate = new DateTime(Convert.ToInt32(date), 1, 1);
            foreach (var item in model)
            {

                item.ColorName = item.ColorName.Replace(",", "");
                item.Count = item.Count.Replace(",", "");
                if (update != null && update == true)
                {
                    if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                    {

                        /// magaza toğuna eklerken burada kontroller hatalı düzeltilecek
                        /// 
                      
                        /// STOCK YEAR GELİYOR
                        
                       var stockIds = (await _stockServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName))).Data.FirstOrDefault(a => a.ColorId == Convert.ToInt64(item.ColorName) && a.ProductId == productId&&a.StockYear==date);
                        if (stockIds==null/* &&!magazaStogunaEkle*/)
                        {

                            await _stockServices.Add(new()
                            {
                                ProductId = productId,
                                ColorId = Convert.ToInt64(item.ColorName),
                                StockCount = Convert.ToInt64(item.Count),
                                RegisterDate = DateTime.UtcNow.ToString(),
                                StockStatus = StockStatus.UrunEklendi,
                                MagazaMi =/*magazaStogunaEkle!=false?true:*/ null,
                                StockTemp = Convert.ToInt64(item.Count),
                                StockYear = date,
                                RenkBarcode=item.Barcode
                                //TenantId
                            }) ;
                        
                        
                          
                        }
                        else
                        {
                           

                            string barcodum = "";
                            if (item.Barcode!=null&&item.Barcode!="")
                            {
                                barcodum= item.Barcode;
                            }

                            long stockGuncelle = 0;
                            if (stockIds != null)
                            {
                                stockGuncelle = stockIds.StockCount - Convert.ToInt64(item.Count);
                            }
                            if (stockGuncelle!=0)
                            {
                                stockGuncelle = Convert.ToInt64(stockGuncelle.ToString().Replace("-", ""));
                            }
                            if (stockIds.StockCount >Convert.ToInt64(item.Count))
                            {
                              stockGuncelle= stockIds.StockCount - Convert.ToInt64(item.Count);
                              stockGuncelle= Convert.ToInt64(stockGuncelle.ToString().Replace("-", ""));
                                var reusltCount = stockIds.StockTemp - stockGuncelle;
                                await _stockServices.UpdateAll(new()
                                {
                                    Id = stockIds.Id,
                                    ProductId = productId,
                                    ColorId = Convert.ToInt64(item.ColorName),
                                    StockTemp = Convert.ToInt64(reusltCount.ToString().Replace("-", ""))/*Convert.ToInt64(item.Count)*/, /*stockGuncelle != 0 ? (stockIds.StockTemp + stockGuncelle) : Convert.ToInt64((stockIds.StockCount - Convert.ToInt64(item.Count)).ToString().Replace("-", "")) + stockIds.StockTemp,*/
                                    StockCount = Convert.ToInt64(item.Count) != 0 ? Convert.ToInt64(item.Count) : stockIds.StockCount,
                                    RegisterDate = DateTime.UtcNow.ToString(),
                                    StockStatus = StockStatus.UrunEklendi,
                                    MagazaMi = null,
                                    StockYear = date,
                                    RenkBarcode = barcodum != "" ? barcodum : stockIds.RenkBarcode
                                    //TenantId
                                });
                            }
                            else
                            {
                                await _stockServices.UpdateAll(new()
                                {
                                    Id = stockIds.Id,
                                    ProductId = productId,
                                    ColorId = Convert.ToInt64(item.ColorName),
                                    StockTemp = stockGuncelle != 0 ? (stockIds.StockTemp + stockGuncelle) : Convert.ToInt64((stockIds.StockCount - Convert.ToInt64(item.Count)).ToString().Replace("-", "")) + stockIds.StockTemp,
                                    StockCount = Convert.ToInt64(item.Count) != 0 ? Convert.ToInt64(item.Count) : stockIds.StockCount,
                                    RegisterDate = DateTime.UtcNow.ToString(),
                                    StockStatus = StockStatus.UrunEklendi,
                                    MagazaMi = null,
                                    StockYear = date,
                                    RenkBarcode = barcodum != "" ? barcodum : stockIds.RenkBarcode
                                    //TenantId
                                });
                            }

                         
                        }
                      
                    }

                }
                else
                {



                    /// STOCK YEAR GELİYOR
                    if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                    {
                        var stockCheck =await _stockServices.GetAll(a=>a.ProductId==productId&&a.ColorId== Convert.ToInt64(item.ColorName)&&a.tekliId==null&&a.StockYear==date);
                        var resq = stockCheck.Data.FirstOrDefault();
                        string barcodum = "";
                        if (item.Barcode!=null&&item.Barcode!="")
                        {
                            barcodum= item.Barcode;

                        }
                        await _stockServices.Add(new()
                        {
                            ProductId = productId,
                            ColorId = Convert.ToInt64(item.ColorName),
                            StockCount = Convert.ToInt64(item.Count),
                            RegisterDate = DateTime.UtcNow.ToString(),
                            StockStatus = StockStatus.UrunEklendi,
                            MagazaMi = /*magazaStogunaEkle != false ? true :*/ null,
                            StockTemp = resq != null ? (resq.StockTemp + Convert.ToInt64(item.Count)) : Convert.ToInt64(item.Count),
                            StockYear = date,
                            RenkBarcode= barcodum!=""?item.Barcode:resq!=null?resq.RenkBarcode:""
                            //TenantId
                        });
                    }
                }

            }
            return RedirectToAction("ProductIndex");
        }
        public async Task<IActionResult> GetAllProductAndStock(long id)
        {
            //var result = await _productServices.GetProductStock(id);
           ViewBag.ProductId = id;
            return View();
        }
        public async Task<IActionResult> GetAllProductAndStockJson(long id)
        {
            var result = await _productServices.GetProductStock(id);
            var models = JsonConvert.SerializeObject(result);
            return Json(models);
        }
        public async Task<IActionResult> SearchPartical(long id)
        {

           ViewBag.AgeList = await _ageRepository.GetAllAsync();
           ViewBag.Tenant = await _tenantRepository.GetAllAsync();
            return PartialView("SearchPartical");
        }
        public async Task<IActionResult> SearchParticalL(SearchParticalDto p)
        {
            var model = await _productServices.GetAll();
            return PartialView("SearchPartical");
        }
        
        public async Task<IActionResult> Delete(long id)
        {
            var product =await _productServices.GetById(id);
            var result = await _productServices.Delete(product.Data);
            return RedirectToAction("ProductIndex");
        }
        public async Task<IActionResult> ProductStockAdd(ProductStockAdd productStockAdd)
        {

            return View("ProductIndex", (await _productServices.GetAll()).Data);
        }
        public async Task<IActionResult> UpdateProductStock(long productId)
        {
            
            ProductColorDto dto = new ProductColorDto();
            var result = await _productServices.GetProductStockUpdate(productId);
            dto.Products = result;
            dto.ProductColor = (await _colorServices.GetAll()).Data;
            ViewBag.ProductId = productId;
            ViewBag.ColorCount = result.Count;
            return View(dto);
        }
        public async Task<IActionResult> Tekliler(int page=1)
        {
            var result = await _teklilerServices.GetAll();
            List<ShippingProduct> temp = new List<ShippingProduct>();
            foreach (var item in result.Data)
            {
                ShippingProduct tp = new ShippingProduct();
                var p1 = await _productServices.GetById(item.ProductId);
                var c1 = await _colorServices.GetById(item.ColorId);
                tp.ProductName = p1.Data.ModelName;
                tp.ColorName = c1.Data.ColorName;
                tp.Count = item.Count;
                tp.TekliId = item.Id;
                tp.ProductId = item.ProductId;
                tp.ModelCode = p1.Data.ModelCode;
                tp.Barcode = p1.Data.Barcode;
                temp.Add(tp);
            }
            ViewBag.ProductCount = result.Data.Count;
            return View(temp);
            //var result = await _teklilerServices.TekliListesi();
            //return View(result.ToPagedList(page,12));
        }
        public async Task<IActionResult> Deneme()
        {
          
            return View();
        }
        public async Task<IActionResult> DenemeJson()
        {
            var result = await _teklilerServices.GetAll();
            List<ShippingProduct> temp = new List<ShippingProduct>();
            foreach (var item in result.Data)
            {
                var resq = await _productServices.GetTekliAll(item.Id);
                ShippingProduct tp = new ShippingProduct();
                // var p1 = await _productServices.GetById(item.ProductId);
                //var q = await _productRepository.GetAllAsync();
                //var c1 = await _colorServices.GetById(item.ColorId);
                if (resq.ProductName!=null)
                {
                    tp.ProductName = resq.ProductName;
                    tp.ColorName = resq.ColorName;
                    tp.Count = resq.Count;
                    tp.TekliId = item.Id;
                    tp.ProductId = resq.ProductId;
                    tp.ModelCode = resq.ModelCode;
                    tp.Barcode = resq.Barcode;
                    tp.Age = resq.Age;
                    tp.Gender = (Gender)(int)resq.Gender;
                    temp.Add(tp);
                }
               
            }
            var models = JsonConvert.SerializeObject(temp);
            return Json(models);
        }
        public async Task<IActionResult> TekliEkle(int? page=1, int? colorPage=1)
        {

            PagedListShippingProduct dto = new PagedListShippingProduct();
            var produtList = await _productServices.TekliList();
           var dtos = produtList.ToPagedList(page.Value, 12);
          
            ViewBag.Count= produtList.Count;
            //   var colorList = await _colorServices.GetAll();
            //   dto.ProductColorr = colorList.Data;
            //var dto = colorList.Data.ToPagedList(colorPage.Value,5);
            return View("TekliEkle", dtos);
        } 
        public async Task<IActionResult> TekliColorPartical(int? page=1)
        {
            PagedListShippingProduct dto = new PagedListShippingProduct();
            var colorList = await _colorServices.GetAll();
            //(IPagedList)dto.ProductColor = colorList.Data.ToPagedList(page.Value,5);
            var q = colorList.Data.ToPagedList(page.Value,12);
            ViewBag.ColorCount= colorList.Data.Count;
            dto.pageCount = colorList.Data.Count;
            return PartialView("TekliColorPartical", q);
        }
        public async Task<IActionResult> Tekli(long productId,String body)
        {
            var model = JsonConvert.DeserializeObject<List<ColorDto>>(body);
            if (productId>0)
            {
                foreach (var item in model)
                {

                    item.ColorName = item.ColorName.Replace(",", "");
                        item.Count = item.Count.Replace(",", "");
                        if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                        {
                            var result = await _teklilerServices.Add(new Tekliler()
                            {
                                ProductId=productId,
                                ColorId=Convert.ToInt64(item.ColorName),
                                Count= Convert.ToInt64(item.Count),
                                IsActive=true,
                                RegisterDate=DateTime.Now
                            });
                                await _stockServices.Add(new()
                                {
                                    ProductId = productId,
                                    ColorId = Convert.ToInt64(item.ColorName),
                                    StockCount = Convert.ToInt64(item.Count),
                                    RegisterDate = DateTime.UtcNow.ToString(),
                                    StockStatus = StockStatus.UrunEklendi,
                                    TekliMi = true,
                                    tekliId= result,
                                    //TenantId
                                });
                        }
                }
             }
            //var query = await _teklilerServices.GetAll();
            return RedirectToAction("Deneme");

        }

        public async Task<IActionResult> DeleteTekli(long tekliId)
        {
            if (tekliId!=0)
            {
                var tekli = await _teklilerServices.GetById(tekliId);
                await _teklilerServices.Delete(tekli.Data);
               var tekliStok = await _stockServices.GetAll(a=>a.tekliId==tekliId);
                foreach (var item in tekliStok.Data)
                {
                await _stockServices.Delete(item);
                }
            }
            return RedirectToAction("Deneme");
        }
        public async Task<IActionResult> TekliDuzenle(long tekliId)
        {
            if (tekliId != 0)
            {
                var tekli = await _productServices.GetTekliDetay(tekliId);
                return View(tekli);
            }
            return RedirectToAction("Tekliler");
        }
        [HttpPost]
        public async Task<IActionResult> TekliDuzenle(ShippingProduct dto)
        {
            if (dto.Count != 0)
            {
                var tekli = await _teklilerServices.GetById(dto.TekliId);
                tekli.Data.Count = dto.Count;
                await _teklilerServices.UpdateAll(tekli.Data);

                //stock güncellemesi
                var stock =await _stockServices.GetAll(a=>a.tekliId==dto.TekliId);
                var result = stock.Data.FirstOrDefault();
                result.StockCount= dto.Count;
                await _stockServices.UpdateAll(result);
            }
            return RedirectToAction("Tekliler");
        }




        public async Task<IActionResult> AgeIndex()
        {
            var result = await _ageRepository.GetAllAsync();
            return View(result);
        }
        public async Task<IActionResult> AgeAdd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgeAdd(ProductAges productAges)
        {
            if (productAges.Name!=null)
            {
                await  _ageRepository.CreateAsync(productAges);
                return RedirectToAction("AgeIndex");
            }
            return View();
        }
        public async Task<IActionResult> AgeUpdate(long id)
        {
            var result = await _ageRepository.FindAsync(id);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> AgeUpdate(ProductAges productAges)
        {
            _ageRepository.UpdateAll(productAges);
            return RedirectToAction("AgeIndex");
        }
        public async Task<IActionResult> DeleteAge(long id)
        {
           var reulst = await _ageRepository.FindAsync(id);
            _ageRepository.Remove(reulst);
            return RedirectToAction("AgeIndex");
        }
        //public async Task<IActionResult> TekliEkle(String body)
        //{
        //    return View(dto);
        //}
        //public async Task<IActionResult> AddColor(Color color)
        //{
        //    await _colorServices.Add(color);
        //    return View("Color");
        //}
    }
}
