using Application.Interface;
using AutoMapper;
using Core.Entities;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
//using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using X.PagedList;
using static Entities.Enums.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class ShippingController : Controller
    {
        private readonly IShippingServices _shippingServices;
        private readonly IShippingDetailsServices _shippingDetailsServices;
        private readonly IColorServices _colorServices;
        private readonly ITempServices _tempServices;
        readonly IProductServices _productServices;
        readonly IStockServices _stockServices;
        readonly IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantServices _tenantServices;


        public ShippingController(IShippingServices shippingServices, IColorServices colorServices, IProductServices productServices, IStockServices stockServices, IShippingDetailsServices shippingDetailsServices, IMapper mapper, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, ITempServices tempServices)
        {
            _shippingServices = shippingServices;
            _colorServices = colorServices;
            _productServices = productServices;
            _stockServices = stockServices;
            _shippingDetailsServices = shippingDetailsServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _tenantServices = tenantServices;
            _tempServices = tempServices;
        }

      
        public async Task<IActionResult> ShippingIndex(int page = 1)
        {///mapping sorunu

            var magazaMi = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
     
            ViewBag.Yetki = true;
            if (magazaMi.ToLower() == "depo")
            {
                ViewBag.depoMu = true;
            }
            else
            {
                ViewBag.depoMu = null;
            }



            return View();

        }
        public async Task<IActionResult> ShippingIndexjson()
        {///mapping sorunu

            var res = await _shippingServices.IndexIcınGetir();
            var qq = JsonConvert.SerializeObject(res);
            return Json(qq);

        }

        public async Task<IActionResult> UpdateFinishShippingListView(long spId)
        {

            if (spId != 0)
            {
                var result = await _tempServices.UpdateFinishShippingListView(spId);

                if (result.Count>0)
                {
                return View("UpdateFinishShipping", result);

                }
            }
            return View("ShippingIndex");
            //return BadRequest();
        }
        public async Task<IActionResult> CheckShipping(long spId)
        {

            if (spId != 0)
            {
                var result = await _tempServices.UpdateFinishShippingListView(spId);

                if (result.Count > 0)
                {
                    return Json("ok");
                }
                else
                {
                    return Json("");
                }
            }
            return Json("");
            //return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFinishShippingPost([FromBody] List<ShippingProduct> shippingProduct)
        {
            if (shippingProduct.Count == 0)
            {
                return Json(false);
            }
            var result = await _tempServices.UpdateFinishShippingPost(shippingProduct.Where(item => item != null).ToList());
            return Json(result);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Enums = GetEnumList(1);
            ViewBag.Age = GetEnumList(2);
            ViewBag.Gender = GetEnumList(3);
            Shippings shippings = new Shippings();
            var result = await _tenantServices.GetAll();
            ViewBag.Tenants = result.Data;
            var productList = await _productServices.GetAll();
            ViewBag.Product = productList.Data.OrderBy(a => a.ModelCode).OrderBy(a => a.Age).ToList();
            return View(shippings);

        }



        SelectList GetEnumList(int type)
        {
            switch (type)
            {
                case 1:
                    var list = from ShippingStasus e in Enum.GetValues(typeof(ShippingStasus))
                               select new { EnumId = (int)e, Status = e.ToString() };
                    return new SelectList(list, "EnumId", "Status", 1);

                case 2:
                    var list2 = from Age e in Enum.GetValues(typeof(Age))
                                select new { EnumId = (int)e, Age = e.ToString() };
                    return new SelectList(list2, "EnumId", "Age", 1);
                case 3:
                    var list3 = from Gender e in Enum.GetValues(typeof(Gender))
                                select new { EnumId = (int)e, Gender = e.ToString() };
                    return new SelectList(list3, "EnumId", "Gender", 1);
                default:
                    return null;
            }


        }
        [HttpPost]
        public async Task<IActionResult> Create(Shippings shippings)
        {
            if (HttpContext.Session.GetString("shippingId") != "" || HttpContext.Session.GetString("shippingId") != null)
                HttpContext.Session.Clear();

            shippings.IsActive = true;
            shippings.TenantId = shippings.TenantId;
            var ids = await _shippingServices.Add(shippings);
            ViewBag.Ids = ids;
            HttpContext.Session.SetString("shippingId", ids.ToString());

            return View("NewShippingView");
        }


        public async Task<IActionResult> AddShippingDetail(long shippingId, int? page = 1)
        {


            PagedListShippingProduct dto = new PagedListShippingProduct();
            ViewBag.Ids = shippingId;
            var produtList = await _productServices.GetAll();
            var deneme = produtList.Data.ToPagedList(page.Value, 3);
            ViewBag.Product = produtList.Data.Count;
         

            return View(deneme);
        }
        public async Task<IActionResult> ShippingComplated()
        {

            PagedListShippingProduct dto = new PagedListShippingProduct();
            var shippingId = HttpContext.Session.GetString("shippingId");
            var ids = Convert.ToInt64(shippingId);
            ViewBag.Ids = ids;
            List<ShippingDetails> siparisListesi = new List<ShippingDetails>();

            if (ids != null && ids != 0)
            {
                var query = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ids);
                siparisListesi = query.Data;

            }
            List<ShippingProduct> shippingList = new List<ShippingProduct>();
            foreach (var item in siparisListesi)
            {
                ShippingProduct shipping = new ShippingProduct();
                shipping.ProductId = item.ProductId;
                if (item.ProductId != 0)
                {
                    var productName = await _productServices.GetById(item.ProductId);
                    shipping.ProductName = productName.Data.ModelName;
                }
                shipping.ShippingId = item.ShippinbgId;
                shipping.ColorId = item.ColorId.Value;

                if (item.ColorId != 0)
                {
                    var colorName = await _colorServices.GetById(item.ColorId.Value);
                    shipping.ColorName = colorName.Data.ColorName;
                }
                shipping.Count = item.ShippingCount.Value;
                shippingList.Add(shipping);
            }
            var spList = shippingList;

            return PartialView("ShippingComplated", spList);
        }

        public async Task<IActionResult> AddShippingDetailsColorPartical(int? color = 1)
        {
            PagedListShippingProduct dto = new PagedListShippingProduct();
            var colorList = await _colorServices.GetAll();
            var query = colorList.Data.ToPagedList(color.Value, 12);
            ViewBag.Count = query.Count;
            ViewBag.ColorCount = colorList.Data.Count + 2;
         
            return PartialView("AddShippingDetailsColorPartical", query);
        }


        [HttpPost]
        public async Task<bool> AddShippingDetails(long ShippingId, long productId, String body, bool refresh)
        {
            var model = JsonConvert.DeserializeObject<List<ColorDto>>(body);

            foreach (var item in model)
            {
                ShippingDetails shippingDetails = new()
                {
                    ShippinbgId = ShippingId,
                    ProductId = productId,
                };
                item.ColorName = item.ColorName.Replace(",", "");
                item.Count = item.Count.Replace(",", "");
                if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                {

                    shippingDetails.ColorId = Convert.ToInt64(item.ColorName);
                    shippingDetails.ShippingCount = Convert.ToInt64(item.Count);

                    var check = await _shippingDetailsServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.ShippinbgId == ShippingId);
                    if (check.Data.Count > 0)
                    {
                        var query = check.Data.FirstOrDefault();
                        query.ShippingCount = query.ShippingCount + Convert.ToInt64(item.Count);
                        await _shippingDetailsServices.UpdateAll(query);

                        var shippings = await _shippingServices.GetById(ShippingId);
                        shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) - Convert.ToInt64(item.Count)).ToString();
                        await _shippingServices.UpdateAll(shippings.Data);
                    }
                    else
                    {

                        await _shippingDetailsServices.Add(shippingDetails);
                    }
                }
            }
            return true;

        }

        [HttpPost]
        public async Task<bool> UpdateShippingDetails(ShippingDetails shippingDetail)
        {

            var result = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == shippingDetail.ShippinbgId && a.ProductId == shippingDetail.ProductId && a.ColorId == shippingDetail.ColorId);
            ShippingDetails shippingDetails = new()
            {
                Id = result.Data.FirstOrDefault().Id,
                ShippinbgId = shippingDetail.ShippinbgId,
                ProductId = shippingDetail.ProductId,
                ColorId = shippingDetail.ColorId,
                ShippingCount = shippingDetail.ShippingCount,
            };

            await _shippingDetailsServices.UpdateAll(shippingDetails);
            return true;
        }

        public async Task<bool> DeleteShippinDetails(ShippingDetails shippingDetail)
        {

            var result = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == shippingDetail.ShippinbgId && a.ProductId == shippingDetail.ProductId && a.ColorId == shippingDetail.ColorId);
            foreach (var item in result.Data)
            {
                await _shippingDetailsServices.Delete(item);
            }
            return true;
        }
        public async Task<IActionResult> DeleteShipping(ShippingDetails shippingDetail)
        {
            var result = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == shippingDetail.ShippinbgId);
            foreach (var item in result.Data)
            {
                await _shippingDetailsServices.Delete(item);

            }
            var tempresult = await _tempServices.GetAll(a => a.ShippigId == shippingDetail.ShippinbgId);
            foreach (var item in tempresult.Data)
            {
                await _tempServices.Delete(item);
            }
            var deleteShipping = await _shippingServices.GetById(shippingDetail.ShippinbgId);
            await _shippingServices.Delete(deleteShipping.Data);
            return RedirectToAction("ShippingIndex");
        }

        public async Task<IActionResult> ComplatedShippin(long ShippinbgId)
        {
            var result = await _shippingServices.GetById(ShippinbgId);
            var list = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ShippinbgId);
            long stockCount = 0;
            foreach (var item in list.Data)
            {
                stockCount += item.ShippingCount.Value;
            }
            result.Data.IsComplated = true;
            result.Data.ShippingCount = stockCount.ToString();
            await _shippingServices.UpdateAll(result.Data);
            return RedirectToAction("ShippingIndex");
        }


        public async Task<IActionResult> ShippingDetails(long ShippinbgId)
        {

            var result = await _shippingServices.IndexShipping(ShippinbgId);

            return View(result);
        }
        public async Task<IActionResult> ColorAndCount(long ShippinbgId, long productId)
        {

            var result = await _shippingServices.SiparisİicndekiRenkveMiktarlari(ShippinbgId, productId);

            return Json(result);
        }
        public async Task<IActionResult> ModalFadePartical(long ShippinbgId, long productId)
        {
            var result = await _shippingServices.SiparisİicndekiRenkveMiktarlari(ShippinbgId, productId);
            return PartialView("ModelFadeShipping", result);
        }
        public async Task<IActionResult> NewShippingViewPartical(SearchParticalDto p)
        {
            var model = await _productServices.GetFilterProduct(p, 1, 12);
            var models = await model.ToListAsync();
            return PartialView("NewShippingViewPartical", models);
        }   
        public async Task<IActionResult> UpdateAllShippingProductPartical(SearchParticalDto p)
        {
            var model = await _productServices.GetFilterProduct(p, 1, 12);
            var models = await model.ToListAsync();
            return PartialView("UpdateAllShippingProductPartical", models);
        }
        [HttpPost]
        public async Task<IActionResult> ShippingCofirmList2([FromBody] List<ConvertShippngDto> convertShippngDto)
        {

            try
            {
                long shippingId = 0;
                var query = convertShippngDto.FirstOrDefault(a => a.shippinbgId != null && a.shippinbgId != "");
                if (query != null)
                {
                    shippingId = Convert.ToInt64(query.shippinbgId);
                }
                List<ShippingConfirmListDto> models = new List<ShippingConfirmListDto>();

                foreach (var item in convertShippngDto)
                {
                    bool success = false;
                    if (item != null && item.price.ToString() != "" && item.amount.ToString() != "")
                    {
                       var checkSp=await _shippingDetailsServices.GetShippingDetailsForShippingId(Convert.ToInt64(item.shippinbgId), Convert.ToInt64(item.productId));
                        if (checkSp!=null)
                        {
                            decimal values2 = decimal.Parse(item.price, CultureInfo.InvariantCulture);
                            checkSp.Price = checkSp.Price + values2;
                            checkSp.Amount=checkSp.Amount+  Convert.ToInt64(item.amount);
                            checkSp.ShippingCount=checkSp.ShippingCount +  Convert.ToInt64(item.amount);
                            await _shippingDetailsServices.UpdateAll(checkSp);
                            success = true;
                        }
                        else
                        {
                            decimal values = decimal.Parse(item.price, CultureInfo.InvariantCulture);
                            ShippingDetails sp = new ShippingDetails();
                            sp.ProductId = Convert.ToInt64(item.productId);
                            sp.Price = values;//Convert.ToDecimal(item.price);
                            sp.Amount = Convert.ToInt64(item.amount);
                            sp.ShippinbgId = Convert.ToInt64(item.shippinbgId);
                            sp.ShippingCount = Convert.ToInt64(item.amount);

                            var add = await _shippingDetailsServices.Add(sp);
                            if (add.Success)
                            {
                                success = true;
                            }
                        }
                       
                        if (success)
                        {
                            decimal values = decimal.Parse(item.price, CultureInfo.InvariantCulture);
                            var update = await _shippingServices.GetById(Convert.ToInt64(item.shippinbgId));
                            update.Data.ShippingCount = (Convert.ToInt64(update.Data.ShippingCount) + Convert.ToInt64(item.amount)).ToString();

                            var prices = update.Data.SiparisTutari != null ? update.Data.SiparisTutari : "0";
                            var prices2 = decimal.Parse(prices, CultureInfo.CurrentCulture);
                            var resq = values + prices2;

                            update.Data.SiparisTutari = resq.ToString();
                            //update.Data.SiparisTutari = (Convert.ToDecimal(update.Data.SiparisTutari) + Convert.ToDecimal(item.price)).ToString();


                            await _shippingServices.UpdateAll(update.Data);
                        }
                        shippingId = Convert.ToInt64(item.shippinbgId);
                    }

                }
                models = await _shippingDetailsServices.GetShippingConfirmList(shippingId);
                return PartialView("ShippingConfirmList", models);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<IActionResult> ShippingCofirmListReturn( bool? reload = false, long spId = 0)
        {

            try
            {
           
                if (spId != 0)
                {
                    var models = await _shippingDetailsServices.GetShippingConfirmList(spId);
                    return PartialView("ShippingConfirmList", models);
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<bool> DeleteShippingDetails(long spDetailsId)
        {
            var model = await _shippingDetailsServices.GetById(spDetailsId);
            await _shippingDetailsServices.Delete(model.Data);

            /// detay silinice sipariş miktarından düş
            var update = await _shippingServices.GetById(Convert.ToInt64(model.Data.ShippinbgId));
            update.Data.ShippingCount = (Convert.ToInt64(update.Data.ShippingCount) - Convert.ToInt64(model.Data.Amount)).ToString();

            var prices = update.Data.SiparisTutari != null ? update.Data.SiparisTutari : "0";
            var prices2 = decimal.Parse(prices, CultureInfo.CurrentCulture);

            var priceData = decimal.Parse(model.Data.Price.ToString(), CultureInfo.CurrentCulture);
            var resq = prices2 - priceData;

            //update.Data.SiparisTutari = (Convert.ToDecimal(update.Data.SiparisTutari) - Convert.ToDecimal(model.Data.Price)).ToString();
            update.Data.SiparisTutari = resq.ToString();


            await _shippingServices.UpdateAll(update.Data);



            //update.Data.SiparisTutari = resq.ToString();


            return true;
        }


        public async Task<IActionResult> UpdateAllShipping(long shippingId)
        {
            var result = await _shippingServices.UpdateShippingAll(shippingId);
            ViewBag.SpId = shippingId;

            return View(result);
        }

        public async Task<IActionResult> UpdateShippingById(long shippingId, long productId, long count, decimal totalTutar)
        {
            ///SP DETAYI
            var shipping = await _shippingDetailsServices.GetShippingDetailsForShippingId(shippingId, productId);

            //ANA SİPARİŞ TABLOSU
            var oldShipping = await _shippingServices.GetById(shippingId);

            // ŞU ANKİ SİPARİŞ TUTARI
            var oldShippingTutar = Convert.ToDecimal(oldShipping.Data.SiparisTutari, CultureInfo.CurrentCulture);

            if (shipping.Price > totalTutar)
            {
                var fark = shipping.Price - totalTutar;
                oldShippingTutar = oldShippingTutar - fark;
                oldShipping.Data.SiparisTutari = oldShippingTutar.ToString("N");

                if (shipping.ShippingCount> count)
                {
                    var miktarFarkı = shipping.ShippingCount - count;
                    oldShipping.Data.ShippingCount = (Convert.ToInt64(oldShipping.Data.ShippingCount) - miktarFarkı).ToString();
                }
                else
                {
                    var miktarFarkı = count - shipping.ShippingCount;
                    oldShipping.Data.ShippingCount = (Convert.ToInt64(oldShipping.Data.ShippingCount) + miktarFarkı).ToString();
                }

                await _shippingServices.UpdateAll(oldShipping.Data);
            }
            else if (shipping.Price == totalTutar) { }
            else
            {
                var fark = totalTutar - shipping.Price;
                oldShippingTutar = oldShippingTutar + fark;
                oldShipping.Data.SiparisTutari = oldShippingTutar.ToString("N");

                if (shipping.ShippingCount > count)
                {
                    var miktarFarkı = shipping.ShippingCount - count;
                    oldShipping.Data.ShippingCount = (Convert.ToInt64(oldShipping.Data.ShippingCount) - miktarFarkı).ToString();
                }
                else
                {
                    var miktarFarkı = count- shipping.ShippingCount;
                    oldShipping.Data.ShippingCount = (Convert.ToInt64(oldShipping.Data.ShippingCount) + miktarFarkı).ToString();
                }

            
                await _shippingServices.UpdateAll(oldShipping.Data);
            }


            shipping.Amount = count;
            shipping.Price = totalTutar;
            shipping.ShippingCount = count;

            var result = await _shippingDetailsServices.UpdateAll(shipping);

            return RedirectToAction("UpdateAllShipping", new { shippingId = shippingId });
        }
        public async Task<IActionResult> DeleteShippingById(long shippingId, long productId)
        {
            ///SP DETAYI
            var shippingDetay = await _shippingDetailsServices.GetShippingDetailsForShippingId(shippingId, productId);

            //ANA SİPARİŞ TABLOSU
            var oldShipping = await _shippingServices.GetById(shippingId);
            oldShipping.Data.ShippingCount = (Convert.ToInt64(oldShipping.Data.ShippingCount) - shippingDetay.ShippingCount).ToString();
            // ŞU ANKİ SİPARİŞ TUTARI
            var oldShippingTutar = Convert.ToDecimal(oldShipping.Data.SiparisTutari, CultureInfo.CurrentCulture);
            oldShippingTutar = oldShippingTutar - shippingDetay.Price;
            oldShipping.Data.SiparisTutari = oldShippingTutar.ToString("N");
            await _shippingServices.UpdateAll(oldShipping.Data);

            var result = await _shippingDetailsServices.Delete(shippingDetay);

            return RedirectToAction("UpdateAllShipping", new { shippingId = shippingId });
        }


        public async Task<IActionResult> AddShippingUpdateView(long shippingId, long productId, long count, decimal totalTutar)
        {
           var checkSp= await _shippingDetailsServices.GetShippingDetailsForShippingId(shippingId, productId);
            if (checkSp!=null)
            {
                return Json("false");
            }
            if (count==0|| totalTutar==0)
            {
                return Json("0");
            }

            ShippingDetails shippingDetails = new ShippingDetails()
            {
                Amount = count,
                Price = totalTutar,
                ProductId = productId,
                ShippinbgId = shippingId,
                ShippingCount = count,
            };
            ///SP DETAYI

            //ANA SİPARİŞ TABLOSU
            var oldShipping = await _shippingServices.GetById(shippingId);
         
            oldShipping.Data.ShippingCount = ((Convert.ToInt64(oldShipping.Data.ShippingCount) + count)).ToString();
            // ŞU ANKİ SİPARİŞ TUTARI
            var oldShippingTutar = Convert.ToDecimal(oldShipping.Data.SiparisTutari, CultureInfo.CurrentCulture);
            oldShippingTutar = oldShippingTutar + totalTutar;
            oldShipping.Data.SiparisTutari = oldShippingTutar.ToString("N");
            await _shippingServices.UpdateAll(oldShipping.Data);  
            
            
            await _shippingDetailsServices.Add(shippingDetails);

            

            return RedirectToAction("UpdateAllShipping", new { shippingId = shippingId });
        }

        //  public IActionResult UpdateShippigVC(long shippingId,long productId,long colorId,long count ) => ViewComponent("ShippingViewComponent", new { shippingId, productId, colorId, count });

        //public IActionResult UpdateShippigVC(long shippingId, long productId, long colorId, long count)
        //{
        //    return ViewComponent("ShippingViewComponent", new { shippingId, productId, colorId, count });
        //}
    }
}
