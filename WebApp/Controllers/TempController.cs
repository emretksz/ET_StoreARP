using Application.Interface;
using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DataAccess.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Core.Utilities.Results;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using static Entities.Enums.Enums;
using System.Globalization;
using System.Web.Providers.Entities;

namespace WebApp.Controllers
{

    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class TempController : Controller
    {
        readonly IProductServices _productServices;
        readonly IStockServices _stockServices;
        readonly IMagazaStock _magazaRepository;
        readonly IColorServices _colorServices;
        readonly ITempServices _tempServices;
        readonly IShippingDetailsServices _shippingDetailsServices;
        readonly IShippingServices _shippingServices;
        readonly IOrderDateServices _orderDateServices;
        readonly IOrderServices _orderServices;
        readonly IMapper _mapper;
        private readonly IWebHostEnvironment _evn;
        private readonly ITenantServices _tenantServices;

        public TempController(IProductServices productServices, IStockServices stockServices, IMapper mapper, IColorServices colorServices, ITempServices tempServices, IShippingDetailsServices shippingDetailsServices, IOrderDateServices orderDateServices, IOrderServices orderServices, IShippingServices shippingServices, IMagazaStock magazaRepository, IWebHostEnvironment evn, ITenantServices tenantServices)
        {
            _mapper = mapper;
            _productServices = productServices;
            _stockServices = stockServices;
            _colorServices = colorServices;
            _tempServices = tempServices;
            _shippingDetailsServices = shippingDetailsServices;
            _orderDateServices = orderDateServices;
            _orderServices = orderServices;
            _shippingServices = shippingServices;
            _magazaRepository = magazaRepository;
            _evn = evn;
            _tenantServices = tenantServices;
        }
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> TempsIndex(int page=1)
        {
            return View();
        }
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> TempsIndexReturnJson()
        {
            var result = await _tempServices.TempIndex();
            var models = JsonConvert.SerializeObject(result);
            return Json(models);
        }
        /* Depocu aydınlara tıkladı onun siparişlerini görüyor. */
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> TenantShipping(long tenantId)
        {
            var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
      
            ViewBag.TenantId = tenantId;    
            return View();
        }

        public async Task<IActionResult> TenantShippingJson(long tenantId)
        {
            var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            var result = await _tempServices.TenantShippingList(tenantId);
            var qq = JsonConvert.SerializeObject(result);
            return Json(qq);
        }

   
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> PrepareOrders(long shippingId, int? page = 1)
        {

           var magazaRol= User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value.ToString();
            List<OrderShippingDetailsDto> result = new List<OrderShippingDetailsDto>();
            List<Product> models = new List<Product>();
            if (magazaRol.ToLower()=="magaza")
            {
                result = await _tempServices.OrderShippingProductsMagaza(shippingId);
            }
            else
            {
                models = await _tempServices.GetPrepareShipping(shippingId);

            }


       
            ViewBag.PageCount = models.Count;
            ViewBag.ShippingId = shippingId;
            //return View(result.ToPagedList(page != null ? (page == 1 ? 1 : page.Value) : 1, 6));            
            return View(models.ToPagedList(page != null ? (page == 1 ? 1 : page.Value) : 1, 6));
        }

        public async Task<IActionResult> ColorPartical(long shippingId, long productId, int? page = 1)
        {

            var magazaRol = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value.ToString();
            List<OrderShippingDetailsDto> result = new List<OrderShippingDetailsDto>();
            List<ColorDtoForShipping> list = new List<ColorDtoForShipping>();

            if (magazaRol.ToLower() == "magaza")
            {
                result = await _tempServices.OrderShippingProductsMagaza(shippingId);
            }
            else
            {
                list = await _tempServices.GetPrepareColorList(shippingId,productId);
            }
       

            ViewBag.ShippingId = shippingId;
            ViewBag.ProductId = productId;

            
                return PartialView(list.ToPagedList(page.Value, 12));
    
        }
        [Authorize(Roles = "Admin" + "," + "Depo" )]
        public async Task<IActionResult> Update(long id)
        {
            var result = await _tempServices.GetById(id);
            return View(result.Data);
        }
        [HttpPost]
        public IActionResult Update(Temp temp)
        {
            return View();
        }

        [HttpPost]
        public async Task<bool> AddShippingDetails(long ShippingId, long productId, String bodyy, bool refresh)
        {
            var model = JsonConvert.DeserializeObject<List<ColorDto>>(bodyy);
           
            foreach (var item in model)
            {

                Temp tmp = new Temp
                {
                    ShippigId = ShippingId,
                    ProductId = productId,
                    IsComplated = true,
                    IsFinished = false,
                    RegisterDate = DateTime.Now,
          
                };
                item.ColorName = item.ColorName.Replace(",", "");
                item.Count = item.Count.Replace(",", "");
                if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                {

                    tmp.ColorId = Convert.ToInt64(item.ColorName);
                    tmp.Count = Convert.ToInt64(item.Count);

                    var check = await _tempServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.ShippigId == ShippingId&&!a.IsFinished);
                    if (check.Data.Count > 0)
                    {
                      /*Temp'de eşit olan sp,p,color'a ekleme yapıldı   */
                        var query = check.Data.FirstOrDefault();
                        query.Count = query.Count + Convert.ToInt64(item.Count);
                        query.IsComplated = true;
                        await _tempServices.UpdateAll(query);
                       /*               ************               */

                        var shipping = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ShippingId && a.ProductId == productId && a.ColorId == Convert.ToInt32(item.ColorName));
                       /* Sipariş detayı bulundu*/

                        if (shipping.Data.Count > 0)
                        {
                            /* sipariş detayından gelen miktar düşüldü */
                            var querys = shipping.Data.FirstOrDefault();
                            if (querys != null)
                            {
                                if (querys.ShippingCount >0)
                                {
                                querys.ShippingCount = querys.ShippingCount - Convert.ToInt32(item.Count);
                                    if (querys.ShippingCount == 0|| querys.ShippingCount<0)
                                    {
                                        querys.ShippingCount = 0;
                                    }
                                await _shippingDetailsServices.UpdateAll(querys);
                                }
                  
                            }
                            var shippings = await _shippingServices.GetById(ShippingId);

                            /**Ana oluşturulan siparişte de miktar düşüldü */
                            if (Convert.ToInt64(shippings.Data.ShippingCount) > 0)
                            {
                            shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) - Convert.ToInt64(item.Count)).ToString();
                                if (Convert.ToInt64(shippings.Data.ShippingCount) < 0)
                                {
                                    shippings.Data.ShippingCount = "0";
                                }
                                await _shippingServices.UpdateAll(shippings.Data);
                            }

                            bool magazaMi = false;
                            var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
                            if (magaza.ToLower() == "magaza")
                            {
                                magazaMi = true;
                            }

                                 var years = DateTime.Now.Year;


                                var qe = await _stockServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.TekliMi == null && a.MagazaMi == null&&a.StockYear==years.ToString());
                                if (qe != null)
                                {
                                    var s = qe.Data.FirstOrDefault();
                                    s.StockCount = (s.StockCount - Convert.ToInt64(item.Count));
                                    if (s.StockCount < 0)
                                    {
                                        s.StockCount = 0;
                                    }
                                    await _stockServices.UpdateAll(s);
                                }





                        }
                    }
                    else
                    {
                        // Temp'de kayıt yoksa kayıt açıldı
                        await _tempServices.Add(tmp);

                        // Sipariş detayına bakıldı
                        var shipping = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ShippingId && a.ProductId == productId && a.ColorId == Convert.ToInt32(item.ColorName));
                        if (shipping.Data.Count > 0)
                        {
                            var querys = shipping.Data.FirstOrDefault();
                            if (querys != null)
                            {
                                if (querys.ShippingCount > 0)
                                {
                                    querys.ShippingCount = querys.ShippingCount - Convert.ToInt32(item.Count);
                                    if (querys.ShippingCount<0)
                                    {
                                        querys.ShippingCount = 0;
                                    }
                                    await _shippingDetailsServices.UpdateAll(querys);
                                }
                            }

                            //siparişlere bakıldı
                            var shippings = await _shippingServices.GetById(ShippingId);
                            if (Convert.ToInt64(shippings.Data.ShippingCount) > 0)
                            {
                                shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) - Convert.ToInt64(item.Count)).ToString();
                                if (Convert.ToInt64(shippings.Data.ShippingCount) <0)
                                {
                                     shippings.Data.ShippingCount ="0";
                                }
                                await _shippingServices.UpdateAll(shippings.Data);
                            }

                            bool magazaMi = false;
                            var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
                            if (magaza.ToLower() == "magaza")
                            {
                                magazaMi = true;
                            }
                           
                            if (magazaMi)
                            {
                                //magaza stoğundan düşüldü
                                var qe2 = await _magazaRepository.GetAllAsync(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName));
                                if (qe2 != null)
                                {
                                    var s = qe2.FirstOrDefault();
                                    s.StockCount = (s.StockCount - Convert.ToInt64(item.Count));
                                    if (s.StockCount < 0)
                                    {
                                        s.StockCount = 0;
                                    }
                                     _magazaRepository.UpdateAll(s);
                                }
                            }
                            else
                            {
                                //magaza stoğundan düşüldü

                                var years = DateTime.Now.Year;

                                var qe = await _stockServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.TekliMi == null && a.MagazaMi == null&&a.StockYear==years.ToString());
                                if (qe != null)
                                {
                                    var s = qe.Data.FirstOrDefault();
                                    s.StockCount = (s.StockCount - Convert.ToInt64(item.Count));
                                    if (s.StockCount < 0)
                                    {
                                        s.StockCount = 0;
                                    }
                                    await _stockServices.UpdateAll(s);
                                }
                            }
                         
                        }
                    }
                }
            }
            return true;


        }


        public async Task<bool> AddShippingDetails2(long ShippingId, long productId, String bodyy, bool refresh)
        {
            var model = JsonConvert.DeserializeObject<List<ColorDto>>(bodyy);
            var tenantId = await _shippingServices.GetById(ShippingId);
            foreach (var item in model)
            {

                Temp tmp = new Temp
                {
                    ShippigId = ShippingId,
                    ProductId = productId,
                    IsComplated = true,
                    IsFinished = false,
                    RegisterDate = DateTime.Now,
                    TenantId = tenantId.Data.TenantId,
                    RenkBarcode = item.Barcode != null ? item.Barcode :""

                };
              
                item.ColorName = item.ColorName.Replace(",", "");
                item.Count = item.Count.Replace(",", "");
                if (item.ColorName != "" && item.Count != "" && item.ColorName != "," && item.Count != "," && item.Count != "on" && item.ColorName != "on")
                {
                    tmp.ColorId = Convert.ToInt64(item.ColorName);
                    tmp.Count = Convert.ToInt64(item.Count);
                    var check = await _tempServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.ShippigId == ShippingId && !a.IsFinished);
                    if (check.Data.Count > 0)
                    {
                        var query = check.Data.FirstOrDefault();
                        query.Count = query.Count + Convert.ToInt64(item.Count);
                        query.IsComplated = true;
                        await _tempServices.UpdateAll(query);

                        ///sp details
                        var shipping = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ShippingId && a.ProductId == productId /*&& a.ColorId == Convert.ToInt32(item.ColorName)*/);

                        if (shipping.Data.Count > 0)
                        {
                            /* sipariş detayından gelen miktar düşüldü */
                            var querys = shipping.Data.FirstOrDefault();
                            if (querys != null)
                            {
                                if (querys.Amount > 0)
                                {
                                    querys.Amount = querys.Amount - Convert.ToInt32(item.Count);
                                    if (querys.Amount == 0 || querys.Amount < 0)
                                    {
                                        querys.Amount = 0;
                                    }
                                    await _shippingDetailsServices.UpdateAll(querys);
                                }

                            }
                            var shippings = await _shippingServices.GetById(ShippingId);

                            /**Ana oluşturulan siparişte de miktar düşüldü */
                            if (Convert.ToInt64(shippings.Data.ShippingCount) > 0)
                            {
                                shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) - Convert.ToInt64(item.Count)).ToString();
                                if (Convert.ToInt64(shippings.Data.ShippingCount) < 0)
                                {
                                    shippings.Data.ShippingCount = "0";
                                }
                                await _shippingServices.UpdateAll(shippings.Data);
                            }

                            // Fabrika stoğudan düşükdü

                            var years = DateTime.Now.Year;


                            var qe = await _stockServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.TekliMi == null&&a.StockYear==years.ToString());
                            if (qe != null)
                            {
                                var s = qe.Data.FirstOrDefault();
                                s.StockCount = (s.StockCount - Convert.ToInt64(item.Count));
                                if (s.StockCount < 0)
                                {
                                    s.StockCount = 0;
                                }
                                await _stockServices.UpdateAll(s);
                            }

                        }
                    
                    }
                    else
                    {
                        // Temp'de kayıt yoksa kayıt açıldı
                        await _tempServices.Add(tmp);

                        // Sipariş detayına bakıldı
                       var shipping = await _shippingDetailsServices.GetAll(a => a.ShippinbgId == ShippingId && a.ProductId == productId /*&& a.ColorId == Convert.ToInt32(item.ColorName)*/);
                        if (shipping.Data.Count > 0)
                        {
                            var querys = shipping.Data.FirstOrDefault();
                            if (querys != null)
                            {
                                if (querys.Amount > 0)
                                {
                                    querys.Amount = querys.Amount - Convert.ToInt32(item.Count);
                                    if (querys.Amount < 0)
                                    {
                                        querys.Amount = 0;
                                    }
                                    await _shippingDetailsServices.UpdateAll(querys);
                                }
                            }

                            //siparişlere bakıldı
                            var shippings = await _shippingServices.GetById(ShippingId);
                            if (Convert.ToInt64(shippings.Data.ShippingCount) > 0)
                            {
                                shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) - Convert.ToInt64(item.Count)).ToString();
                                if (Convert.ToInt64(shippings.Data.ShippingCount) < 0)
                                {
                                    shippings.Data.ShippingCount = "0";
                                }
                                await _shippingServices.UpdateAll(shippings.Data);
                            }

                            //magaza stoğundan düşüldü
                            var years = DateTime.Now.Year;

                            var qe = await _stockServices.GetAll(a => a.ProductId == productId && a.ColorId == Convert.ToInt64(item.ColorName) && a.TekliMi == null && a.StockYear == years.ToString());
                            if (qe != null)
                            {
                                var s = qe.Data.FirstOrDefault();
                                s.StockCount = (s.StockCount - Convert.ToInt64(item.Count));
                                if (s.StockCount < 0)
                                {
                                    s.StockCount = 0;
                                }
                                await _stockServices.UpdateAll(s);
                            }

                        }
                    }
                }

                //return true;
            }
            return true;
        }
        public async Task<IActionResult> IsComplatedShipping(long shippingId)
       {
            var shipping = await _tempServices.GetParticalShipping(shippingId);
            return PartialView("IsComplatedShipping", shipping);
        }

        public async Task<IActionResult> Delete(long productId, long colorId, long shippingId, long count,long tempId)
        {
            try
            {
                var rest2 = await _tempServices.GetById(tempId);
                shippingId = rest2.Data.ShippigId;

                if (rest2.Data.Count>0)
                {
                    rest2.Data.Count = rest2.Data.Count - count;
                    rest2.Data.IsComplated = false;
                    await _tempServices.Delete(rest2.Data);
                    /*****************/
                    var shippings = await _shippingServices.GetById(rest2.Data.ShippigId);
                    shippings.Data.ShippingCount = (Convert.ToInt64(shippings.Data.ShippingCount) + Convert.ToInt64(count)).ToString();
                    await _shippingServices.UpdateAll(shippings.Data);

                    var query2 = await _shippingDetailsServices.GetShippingDetailsForShippingId(rest2.Data.ShippigId,rest2.Data.ProductId/*,rest2.Data.ColorId*/);
                   // var deger = await _shippingDetailsServices.GetAll(a=>a.ShippinbgId==rest2.Data.ShippigId&&a.ProductId==rest2.Data.ProductId&&a.ColorId==rest2.Data.ColorId);
                    if (query2.Amount>=0)
                    {
                        query2.Amount = query2.Amount + count;
                        await _shippingDetailsServices.UpdateAll(query2);
                    }
                    bool magazaMi = false;
                    var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
                 
                    var years = DateTime.Now.Year;

                    var qes = await _stockServices.GetAll(a => a.ProductId == rest2.Data.ProductId && a.ColorId == rest2.Data.ColorId && a.TekliMi == null&&a.StockYear== years.ToString()/*&& a.MagazaMi == null*/);
                        if (qes.Data.Count > 0)
                        {
                            var lastReslt = qes.Data.FirstOrDefault();
                            if (lastReslt != null)
                            {
                                lastReslt.StockCount = (lastReslt.StockCount + Convert.ToInt64(count));
                                await _stockServices.UpdateAll(lastReslt);
                            }

                        }


                }

                return RedirectToAction("PrepareOrders", new { shippingId= shippingId });
            }
            catch (Exception)
            {

                return RedirectToAction("PrepareOrders", new { shippingId = shippingId });
            }
        }
        //FİYATLANDIRMAAA
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        [HttpPost]
        public async Task<bool> ShippingFinishDeleteItem(string bodys)
        {
            List<ComplatedShippingExcel> temps = new List<ComplatedShippingExcel>();
            var body = "";
            var model = JsonConvert.DeserializeObject<List<Temp>>(bodys);
            long shippingId = 0;
           
            var result = await _tempServices.GetAll();
  
            foreach (var item in model)
            {
                var query = result.Data.FirstOrDefault(a => a.ProductId == item.ProductId && a.ColorId == item.ColorId && a.ShippigId == item.ShippigId && !a.IsFinished);
                if (query!=null)
                { 
                shippingId = item.ShippigId;
                    //query.Count=query.Count!=0?query.Count-Convert.ToInt64(item.Count):0;
                    query.IsFinished = true;
                    query.IsDeleted = false;
                    await _tempServices.UpdateAll(query);
                    var res = await _tempServices.ComplatedShippingForExcel(item.ColorId,item.ProductId);
                    temps.Add(new() { Adet = item.Count, Product = res.Product, Renk = res.Renk });
              
                }
            }
         
            var returnList = await _shippingServices.GetById(shippingId);
            if (returnList.Data.ShippingCount=="0")
            {
                returnList.Data.IsComplated = true;
               await _shippingServices.UpdateAll(returnList.Data);
            }
            await OnaylananSiparisler(temps);
            ///todoo

            ///var result = await  HazirlananSiparislerFatura(model);
            return true;
          

        }

        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<bool> OnaylananSiparisler(List<ComplatedShippingExcel> model)
        {
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Siparisler_" + ".xlsx");
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
                {
                    document.AddWorkbookPart();
                    document.WorkbookPart.Workbook = new Workbook();
                    SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultColumnWidth = 12.75D, DefaultRowHeight = 5D, };
                    Worksheet worksheet = new Worksheet();
                    worksheet.Append(sheetFormatProperties);

                    #region stylesPart
                    var stylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = new Stylesheet();

                    // Font list
                    // Create a bold font
                    stylesPart.Stylesheet.Fonts = new Fonts();
                    Font bold_font = new Font();         // Bold font
                    Bold bold = new Bold();
                    bold_font.Append(bold);

                    // Add fonts to list
                    stylesPart.Stylesheet.Fonts.AppendChild(new Font());
                    stylesPart.Stylesheet.Fonts.AppendChild(bold_font); // Bold gets fontid = 1
                    stylesPart.Stylesheet.Fonts.Count = 2;

                    // Create fills list
                    stylesPart.Stylesheet.Fills = new Fills();

                    var formatRed = new PatternFill() { PatternType = PatternValues.Solid };
                    formatRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF6600") }; // red fill
                    formatRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

                    // Append fills to list
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = formatRed }); // Red gets fillid = 2

                    stylesPart.Stylesheet.Fills.Count = 3;

                    // Create border list
                    stylesPart.Stylesheet.Borders = new Borders();
                    // Create thin borders for passed/failed tests and default cells
                    LeftBorder leftThin = new LeftBorder() { Style = BorderStyleValues.Thin };
                    RightBorder rightThin = new RightBorder() { Style = BorderStyleValues.Thin };
                    TopBorder topThin = new TopBorder() { Style = BorderStyleValues.Thin };
                    BottomBorder bottomThin = new BottomBorder() { Style = BorderStyleValues.Thin };

                    Border borderThin = new Border();
                    borderThin.Append(leftThin);
                    borderThin.Append(rightThin);
                    borderThin.Append(topThin);
                    borderThin.Append(bottomThin);

                    // Create thick borders for headings
                    LeftBorder leftThick = new LeftBorder() { Style = BorderStyleValues.Thick };
                    RightBorder rightThick = new RightBorder() { Style = BorderStyleValues.Thick };
                    TopBorder topThick = new TopBorder() { Style = BorderStyleValues.Thick };
                    BottomBorder bottomThick = new BottomBorder() { Style = BorderStyleValues.Thick };

                    Border borderThick = new Border();
                    borderThick.Append(leftThick);
                    borderThick.Append(rightThick);
                    borderThick.Append(topThick);
                    borderThick.Append(bottomThick);

                    // Add borders to list
                    stylesPart.Stylesheet.Borders.AppendChild(new Border());
                    stylesPart.Stylesheet.Borders.AppendChild(borderThin);
                    stylesPart.Stylesheet.Borders.AppendChild(borderThick);
                    stylesPart.Stylesheet.Borders.Count = 3;


                    // Create blank cell format list
                    stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
                    stylesPart.Stylesheet.CellStyleFormats.Count = 1;
                    stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

                    // Create cell format list
                    stylesPart.Stylesheet.CellFormats = new CellFormats();
                    // empty one for index 0, seems to be required
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());

                    // Styleindex = 1, Normal
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    // Styleindex = 2, Bold Header
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();
                    var count = model.Count + 4;
                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    for (int i = 0; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i + 1), Max = Convert.ToUInt32(i + 2), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Siparisler" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    //row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                    //     rowIdex, "Sipariş Adresi" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Barcode" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Model Kodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Model Adı" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Yaş" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Cinsiyet" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Renk" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Miktar" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                            rowIdex, "Tarih" ?? string.Empty, 1));
                    #endregion
                    string tenantName = "";
                    string siparisAdresi = "";
                    string registerdate = DateTime.Now.ToString();
                    var counter = model.Count + 1;
                    long toplam = 0;
                    #region Add Data
                    for (int i = 0; i < counter; i++)
                    {
                    
                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);

                        if (i==(counter-1))
                        {
                            var cell = CreateTextCell(ColumnLetter(0),
                                rowIdex, "", 0);
                            row.AppendChild(cell);

                            var cell3 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell3);
                            var cell5 = CreateTextCell(ColumnLetter(2),
                                  rowIdex, "", 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, "", 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, "", 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(5),
                                  rowIdex, "Onaylanan Toplam Ürün Miktarı", 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                    rowIdex, toplam.ToString(), 0);
                            row.AppendChild(cell7);
                            var cell9 = CreateTextCell(ColumnLetter(7),
                            rowIdex, "", 0);
                            row.AppendChild(cell9);

                }
                        else
                        {
                            var cell = CreateTextCell(ColumnLetter(0),
                                    rowIdex, model[i].Product.Barcode, 0);
                            row.AppendChild(cell);

                            var cell3 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, model[i].Product.ModelCode, 0);
                            row.AppendChild(cell3);
                            var cell5 = CreateTextCell(ColumnLetter(2),
                                  rowIdex, model[i].Product.ModelName, 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, model[i].Product.ProductAges.Name, 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, ((Gender)(int)model[i].Product.Gender).ToString(), 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(5),
                                  rowIdex, model[i].Renk, 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                    rowIdex, model[i].Adet.ToString(), 0);
                            row.AppendChild(cell7);
                            var cell9 = CreateTextCell(ColumnLetter(7),
                            rowIdex, registerdate.ToString(), 0);
                            row.AppendChild(cell9);
                            toplam += model[i].Adet;

                        }



                    }
                    #endregion
                    document.WorkbookPart.Workbook.Save();
                    document.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }




        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> ShippingOrdersListForExcel()
        {
            var magaza = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            if (magaza.ToLower() == "depo")
            {
                ViewBag.depoMu = true;
               // RedirectToAction("TempIndex");
            }
            else
            {
                ViewBag.depoMu = null;
            }
            //bool depoMu = false;
            //if (ViewBag.depoMu != null)
            //{
            //    depoMu = true;
            //}
            return View();
        }

        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<IActionResult> ShippingOrdersReturnJson()
        {
            //var result = await _shippingServices.GetAll(a=>a.Id!=null);
            var result = await _tempServices.GetShippingsAsyncForGrid();
            var qq = JsonConvert.SerializeObject(await result.OrderByDescending(a=>a.Id).ToListAsync());
            return Json(qq);
        
        }


        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<bool> GetShippingOrderListForExcel(long Id)
        {
            var result = await _tempServices.GetShippingList(Id);
            //var model = JsonConvert.DeserializeObject<List<Temp>>(body);
            //var result2 = await HazirlananSiparislerFatura(model);
           await HazirlananSiparislerFatura(result);
            return true;


        }
        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<bool> GetFisAll(long shippingId)
        {
            var result = await _tempServices.GetFis(shippingId);
            await Fis(result);
            return true;
        }

        public async Task<bool> Fis(List<GetShippingOrderList> model)
        {
            try
            {

                var depoMu = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value.ToString();
                bool depo = depoMu.ToLower() == "depo" ? true : false;
             

                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fis_" + ".xlsx");
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
                {
                    document.AddWorkbookPart();
                    document.WorkbookPart.Workbook = new Workbook();
                    SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultColumnWidth = 12.75D, DefaultRowHeight = 5D, };
                    Worksheet worksheet = new Worksheet();
                    worksheet.Append(sheetFormatProperties);

                    #region stylesPart
                    var stylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = new Stylesheet();

                    // Font list
                    // Create a bold font
                    stylesPart.Stylesheet.Fonts = new Fonts();
                    Font bold_font = new Font();         // Bold font
                    Bold bold = new Bold();
                    bold_font.Append(bold);

                    // Add fonts to list
                    stylesPart.Stylesheet.Fonts.AppendChild(new Font());
                    stylesPart.Stylesheet.Fonts.AppendChild(bold_font); // Bold gets fontid = 1
                    stylesPart.Stylesheet.Fonts.Count = 2;

                    // Create fills list
                    stylesPart.Stylesheet.Fills = new Fills();

                    var formatRed = new PatternFill() { PatternType = PatternValues.Solid };
                    formatRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF6600") }; // red fill
                    formatRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

                    // Append fills to list
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = formatRed }); // Red gets fillid = 2

                    stylesPart.Stylesheet.Fills.Count = 3;

                    // Create border list
                    stylesPart.Stylesheet.Borders = new Borders();
                    // Create thin borders for passed/failed tests and default cells
                    LeftBorder leftThin = new LeftBorder() { Style = BorderStyleValues.Thin };
                    RightBorder rightThin = new RightBorder() { Style = BorderStyleValues.Thin };
                    TopBorder topThin = new TopBorder() { Style = BorderStyleValues.Thin };
                    BottomBorder bottomThin = new BottomBorder() { Style = BorderStyleValues.Thin };

                    Border borderThin = new Border();
                    borderThin.Append(leftThin);
                    borderThin.Append(rightThin);
                    borderThin.Append(topThin);
                    borderThin.Append(bottomThin);

                    // Create thick borders for headings
                    LeftBorder leftThick = new LeftBorder() { Style = BorderStyleValues.Thick };
                    RightBorder rightThick = new RightBorder() { Style = BorderStyleValues.Thick };
                    TopBorder topThick = new TopBorder() { Style = BorderStyleValues.Thick };
                    BottomBorder bottomThick = new BottomBorder() { Style = BorderStyleValues.Thick };

                    Border borderThick = new Border();
                    borderThick.Append(leftThick);
                    borderThick.Append(rightThick);
                    borderThick.Append(topThick);
                    borderThick.Append(bottomThick);

                    // Add borders to list
                    stylesPart.Stylesheet.Borders.AppendChild(new Border());
                    stylesPart.Stylesheet.Borders.AppendChild(borderThin);
                    stylesPart.Stylesheet.Borders.AppendChild(borderThick);
                    stylesPart.Stylesheet.Borders.Count = 3;


                    // Create blank cell format list
                    stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
                    stylesPart.Stylesheet.CellStyleFormats.Count = 1;
                    stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

                    // Create cell format list
                    stylesPart.Stylesheet.CellFormats = new CellFormats();
                    // empty one for index 0, seems to be required
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());

                    // Styleindex = 1, Normal
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    // Styleindex = 2, Bold Header
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    long count = model.Count + 1;
                    for (int i = 0; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i + 1), Max = Convert.ToUInt32(i + 2), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Siparisler" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Firma" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Model Kodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Model Adı" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Yaş" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Cinsiyet" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Renk" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Miktar" ?? string.Empty, 1));              
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Adet Fiyatı" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Toplam Tutar" ?? string.Empty, 1));
                    #endregion
                    long miktar = 0;
                    decimal tutar = 0;
                    #region Add Data
                    for (int i = 0; i < count; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                        if (i==(count-1))
                        {
                            var cell10 = CreateTextCell(ColumnLetter(0),
                           rowIdex, "" ,0);
                            row.AppendChild(cell10);
                            var cell5 = CreateTextCell(ColumnLetter(1),
                                      rowIdex, "", 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, "", 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(3),
                                    rowIdex,"", 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(4),
                                  rowIdex, "", 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(5),
                                    rowIdex,"", 0);
                            row.AppendChild(cell7);
                            var cell9 = CreateTextCell(ColumnLetter(6),
                            rowIdex,"Toplam Adet: "+ miktar.ToString(), 2);
                            row.AppendChild(cell9);         
                            var cell15 = CreateTextCell(ColumnLetter(7),
                            rowIdex,"", 0);
                            row.AppendChild(cell15);
                            if (depo)
                            {
                                var cell17 = CreateTextCell(ColumnLetter(8),
                                 rowIdex, "", 0);
                                row.AppendChild(cell17);
                            }
                            else
                            {
                                var cell17 = CreateTextCell(ColumnLetter(8),
                                rowIdex, "Toplam Fiyat: " + tutar.ToString("n"), 2);
                                row.AppendChild(cell17);
                            }
                     
                           

                        }
                        else
                        {
                            var cell10 = CreateTextCell(ColumnLetter(0),
                           rowIdex, model[i].TenantName, 0);
                            row.AppendChild(cell10);
                            var cell5 = CreateTextCell(ColumnLetter(1),
                                      rowIdex, model[i].ModelCode, 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, model[i].ModelName, 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, model[i].Age.ToString(), 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(4),
                                  rowIdex, model[i].Gender.ToString(), 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(5),
                                    rowIdex, model[i].ColorName, 0);
                            row.AppendChild(cell7);
                            var cell9 = CreateTextCell(ColumnLetter(6),
                            rowIdex, model[i].Amount.ToString(), 0);
                            row.AppendChild(cell9);
                            if (depo)
                            {
                                var cell15 = CreateTextCell(ColumnLetter(7),
                                rowIdex, "", 0);
                                row.AppendChild(cell15);    
                                var cell177 = CreateTextCell(ColumnLetter(8),
                                rowIdex, "", 0);
                                row.AppendChild(cell177);
                            }
                            else
                            {
                                var zaman = model[i].Price!=0 ? model[i].Price / model[i].Amount:0;
                                var cell18 = CreateTextCell(ColumnLetter(7),
                             rowIdex, zaman!=0?zaman.ToString():"", 0);
                                row.AppendChild(cell18);    
                                var cell15 = CreateTextCell(ColumnLetter(8),
                             rowIdex, model[i].Price != 0 ? model[i].Price.ToString("n") : "", 0);
                                row.AppendChild(cell15);
                            }
                       
                            miktar += model[i].Amount;
                            tutar += decimal.Parse(model[i].Price.ToString(), CultureInfo.CurrentCulture);

                        }




                    }
                    #endregion
                    document.WorkbookPart.Workbook.Save();
                    document.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public FileResult DowloandFis()
        {
            string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");
            string specificFolder = Path.Combine(folder, "ExcelTemplates");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            var path = Path.Combine(specificFolder, "Fis_" + ".xlsx");
            if (Directory.Exists(specificFolder))
            {
                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel", "Fis_" + ".xlsx");
            }
            else
                return null;


        }


        [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
        public async Task<bool> HazirlananSiparislerFatura(List<GetShippingOrderList> model)
        {
            try
            {

                var depoMu = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value.ToString();
                bool depo = depoMu.ToLower() == "depo" ? true : false;
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Siparisler_" + ".xlsx");
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
                {
                    document.AddWorkbookPart();
                    document.WorkbookPart.Workbook = new Workbook();
                    SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultColumnWidth = 12.75D, DefaultRowHeight = 5D, };
                    Worksheet worksheet = new Worksheet();
                    worksheet.Append(sheetFormatProperties);

                    #region stylesPart
                    var stylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = new Stylesheet();

                    // Font list
                    // Create a bold font
                    stylesPart.Stylesheet.Fonts = new Fonts();
                    Font bold_font = new Font();         // Bold font
                    Bold bold = new Bold();
                    bold_font.Append(bold);

                    // Add fonts to list
                    stylesPart.Stylesheet.Fonts.AppendChild(new Font());
                    stylesPart.Stylesheet.Fonts.AppendChild(bold_font); // Bold gets fontid = 1
                    stylesPart.Stylesheet.Fonts.Count = 2;

                    // Create fills list
                    stylesPart.Stylesheet.Fills = new Fills();

                    var formatRed = new PatternFill() { PatternType = PatternValues.Solid };
                    formatRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF6600") }; // red fill
                    formatRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

                    // Append fills to list
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = formatRed }); // Red gets fillid = 2

                    stylesPart.Stylesheet.Fills.Count = 3;

                    // Create border list
                    stylesPart.Stylesheet.Borders = new Borders();
                    // Create thin borders for passed/failed tests and default cells
                    LeftBorder leftThin = new LeftBorder() { Style = BorderStyleValues.Thin };
                    RightBorder rightThin = new RightBorder() { Style = BorderStyleValues.Thin };
                    TopBorder topThin = new TopBorder() { Style = BorderStyleValues.Thin };
                    BottomBorder bottomThin = new BottomBorder() { Style = BorderStyleValues.Thin };

                    Border borderThin = new Border();
                    borderThin.Append(leftThin);
                    borderThin.Append(rightThin);
                    borderThin.Append(topThin);
                    borderThin.Append(bottomThin);

                    // Create thick borders for headings
                    LeftBorder leftThick = new LeftBorder() { Style = BorderStyleValues.Thick };
                    RightBorder rightThick = new RightBorder() { Style = BorderStyleValues.Thick };
                    TopBorder topThick = new TopBorder() { Style = BorderStyleValues.Thick };
                    BottomBorder bottomThick = new BottomBorder() { Style = BorderStyleValues.Thick };

                    Border borderThick = new Border();
                    borderThick.Append(leftThick);
                    borderThick.Append(rightThick);
                    borderThick.Append(topThick);
                    borderThick.Append(bottomThick);

                    // Add borders to list
                    stylesPart.Stylesheet.Borders.AppendChild(new Border());
                    stylesPart.Stylesheet.Borders.AppendChild(borderThin);
                    stylesPart.Stylesheet.Borders.AppendChild(borderThick);
                    stylesPart.Stylesheet.Borders.Count = 3;


                    // Create blank cell format list
                    stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
                    stylesPart.Stylesheet.CellStyleFormats.Count = 1;
                    stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

                    // Create cell format list
                    stylesPart.Stylesheet.CellFormats = new CellFormats();
                    // empty one for index 0, seems to be required
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());

                    // Styleindex = 1, Normal
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    // Styleindex = 2, Bold Header
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    long count = model.Count +1 ;
                    for (int i = 0; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i + 1), Max = Convert.ToUInt32(i + 2), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Siparisler" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Firma" ?? string.Empty, 1));
                    //row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                    //     rowIdex, "Sipariş Adresi" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Barcode" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Model Kodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Model Adı" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Yaş" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Cinsiyet" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Renk" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Miktar" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                    rowIdex, "Adet fiyatı" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Toplam" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                            rowIdex, "Tarih" ?? string.Empty, 1));
                    #endregion
                    string tenantName = "";
                    string siparisAdresi = "";
                    string registerdate = DateTime.Now.Date.ToString();
                    long miktar = 0;
                    long shippingIdTemp = 0;
                    long productIdTemp = 0;
                    decimal? sonHesapla = 0;
                    decimal tutar = 0;
                    #region Add Data
                    for (int i = 0; i < count; i++)
                    {

                        //decimal values = decimal.Parse(item.price, CultureInfo.InvariantCulture);
                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);
                        if (i==(count-1))
                        {
                                var cell = CreateTextCell(ColumnLetter(0),
                                 rowIdex, "", 0);
                                row.AppendChild(cell);
                            var cell3 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell3);
                            var cell5 = CreateTextCell(ColumnLetter(2),
                                  rowIdex, "", 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, "", 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(4),
                                    rowIdex,"", 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(5),
                                  rowIdex,"", 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                    rowIdex,"", 0);
                            row.AppendChild(cell7);

                          //  var cell15 = CreateTextCell(ColumnLetter(7),
                          //rowIdex, "", 0);
                          //  row.AppendChild(cell15);

                            var cell9 = CreateTextCell(ColumnLetter(7),
                            rowIdex,"Toplam Miktar: "+ miktar.ToString(), 0);
                            row.AppendChild(cell9);
                            if (depo)
                            {
                                var cell10 = CreateTextCell(ColumnLetter(9), rowIdex, "", 0);
                                row.AppendChild(cell10);
                            }
                            else
                            {
                                var cell10 = CreateTextCell(ColumnLetter(9),
                   rowIdex, "Toplam Tutar: " + decimal.Parse(tutar.ToString("n"), CultureInfo.CurrentCulture).ToString("n")/* tutar.ToString("N")*/, 0);
                                row.AppendChild(cell10);
                            }
                   
                                var cell12 = CreateTextCell(ColumnLetter(10),
                                 rowIdex, "", 0);
                                row.AppendChild(cell12);


                        }
                        else
                        {
                            if (i == 0)
                            {
                                var cell = CreateTextCell(ColumnLetter(0),
                                        rowIdex, model[i].TenantName, 0);
                                row.AppendChild(cell);
                            }
                            else
                            {
                                var cell = CreateTextCell(ColumnLetter(0),
                                 rowIdex, "", 0);
                                row.AppendChild(cell);
                            }

                            var cell3 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, model[i].RenkBarcode, 0);
                            row.AppendChild(cell3);
                            var cell5 = CreateTextCell(ColumnLetter(2),
                                  rowIdex, model[i].ModelCode, 0);
                            row.AppendChild(cell5);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, model[i].ModelName, 0);
                            row.AppendChild(cell4);

                            var cell6 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, model[i].Age.ToString(), 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(5),
                                  rowIdex, model[i].Gender.ToString(), 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                    rowIdex, model[i].ColorName, 0);
                            row.AppendChild(cell7);
                            var cell9 = CreateTextCell(ColumnLetter(7),
                            rowIdex, model[i].Amount.ToString(), 0);
                            row.AppendChild(cell9);
                          
                            if (shippingIdTemp==0)
                            {
                                shippingIdTemp = model[i].ShippingId;
                                productIdTemp = model[i].ProductId;
                                var hesapla = await _shippingDetailsServices.QueryableId(shippingIdTemp, productIdTemp);
                                var sonuc = hesapla.Price / hesapla.ShippingCount;
                                var cell14 = CreateTextCell(ColumnLetter(8),
                                 rowIdex, sonuc.ToString(), 0);
                                row.AppendChild(cell14);
                                sonHesapla = sonuc;
                            }
                            else
                            {
                                //BİR KERE Mİ YAZILACAK YOKSA HEPSİNDE Mİ YAZSIN ONUN KONTROLÜ LAZIM
                                if (shippingIdTemp == model[i].ShippingId&&productIdTemp != model[i].ProductId)
                                {
                                    //shippingIdTemp = model[i].ShippingId;
                                    productIdTemp = model[i].ProductId;
                                    var hesapla = await _shippingDetailsServices.QueryableId(shippingIdTemp, productIdTemp);
                                    var sonuc = hesapla.Price / hesapla.ShippingCount;
                                    var cell14 = CreateTextCell(ColumnLetter(8),
                                    rowIdex, sonuc.ToString(), 0);
                                    row.AppendChild(cell14);
                                    sonHesapla = sonuc;
                                }
                                else if (shippingIdTemp != model[i].ShippingId)
                                {
                                    shippingIdTemp = model[i].ShippingId;
                                    productIdTemp = model[i].ProductId;
                                    var hesapla = await _shippingDetailsServices.QueryableId(shippingIdTemp, productIdTemp);
                                    var sonuc = hesapla.Price / hesapla.ShippingCount;
                                    var cell14 = CreateTextCell(ColumnLetter(8),
                                    rowIdex, sonuc.ToString(), 0);
                                    row.AppendChild(cell14);
                                    sonHesapla = sonuc;
                                }
                                else
                                {
                                    var cell14 = CreateTextCell(ColumnLetter(8), rowIdex, sonHesapla.HasValue? sonHesapla.Value.ToString():"", 0);
                                    row.AppendChild(cell14);
                                }
                            }

            

                            if (depo)
                            {
                                var cell10 = CreateTextCell(ColumnLetter(9),
                          rowIdex,"", 0);
                                row.AppendChild(cell10);
                            }
                            else
                            {
                                var cell10 = CreateTextCell(ColumnLetter(9),
                          rowIdex, model[i].Price != 0 ? decimal.Parse(model[i].Price.ToString("n"), CultureInfo.CurrentCulture).ToString("n") : "", 0);
                                row.AppendChild(cell10);
                            }
                          
                            if (i == 0)
                            {
                                var cell11 = CreateTextCell(ColumnLetter(10),
                                    rowIdex, DateTime.Now.ToString(), 0);
                                row.AppendChild(cell11);
                            }
                            else
                            {
                                var cell12 = CreateTextCell(ColumnLetter(10),
                                 rowIdex, "", 0);
                                row.AppendChild(cell12);
                            }
                            miktar += model[i].Amount;
                            tutar += decimal.Parse(model[i].Price.ToString(), CultureInfo.CurrentCulture);

                        }


                    }
                    #endregion
                    document.WorkbookPart.Workbook.Save();
                    document.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
         }

        public FileResult Dowloand()
        {
            string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");
            string specificFolder = Path.Combine(folder, "ExcelTemplates");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            var path = Path.Combine(specificFolder, "Siparisler_" + ".xlsx");
            if (Directory.Exists(specificFolder))
            {
                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel", "Siparisler_" + ".xlsx");
            }
            else
                return null;


        }

        [Authorize(Roles = "Admin" + "," + "Magaza"+","+"Depo")]
        public async Task<IActionResult> ShippingFinishFirstView(int page =1)
        {
            var role = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            if (role.ToLower()=="depo")
            {
                return RedirectToAction("TempsIndex", "Temp");
            }
            if (role.ToLower() == "magaza")
            {
                var magazaSp = await _tempServices.GetTenantShippingOrderPriceMagaza();
                ViewBag.PageCount = magazaSp.Count;
                var PagedList2 = magazaSp.ToPagedList(page, 5);
                return View("ShippingFinishFirstView", PagedList2);
                //var result = await _tempServices.GetTenantShippingOrderPrice();
            }
            var result = await _tempServices.GetTenantShippingOrderPrice();
            ViewBag.PageCount = result.Count;
            var PagedList= result.ToPagedList(page,5);
            return View("ShippingFinishFirstView", PagedList);

        }

        [Authorize(Roles = "Admin" + "," + "Depo")]
        public async Task<IActionResult> ShippingFinish(long tenantId, long shippingId)
        {
            var role = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            if (role.ToLower() == "magaza")
            {
                var result2 = await _tempServices.GetTenantShippingOrderPriceMagaza(tenantId, shippingId);
                ViewBag.TenantId = tenantId;
                return View("ShippingFinish", result2);
            }
            var result = await _tempServices.GetTenantShippingOrderPrice(tenantId,shippingId);
            ViewBag.TenantId = tenantId;
            return View("ShippingFinish", result);

        }

        [HttpPost]
        public async Task<bool> CloseOrder(String body, string tenantId=null)
        {
            try
            {
                Order order = new Order();

                var model = JsonConvert.DeserializeObject<List<Order>>(body);
                var orderId = await _orderDateServices.Add(new() { FaturaAdi = DateTime.Now.ToShortDateString(), FaturaOlusturmaZamani = DateTime.Now });
                foreach (var item in model)
                {
                    if (item.ProductId==0||item.ProductCount=="" || item.ColorId == 0 || item.ShippingId == 0)
                    {
                        continue;
                    }
                    await _orderServices.Add(new()
                    {
                        ShippingId = item.ShippingId,
                        ColorId = item.ColorId,
                        ProductId = item.ProductId,
                        IsActive = true,
                        RegisterDate = DateTime.Now,
                        TenantId = Convert.ToInt64(tenantId),
                        TotalPrice = item.TotalPrice,
                        //UnitePrice = item.UnitePrice,
                        OrderDateId = orderId,
                        IsFinised = true,
                        ProductCount = item.ProductCount,
                        //Price=item.Price

                    });

                
                        var tempResult = await _tempServices.GetAll(a => a.ShippigId == item.ShippingId && a.IsComplated && a.IsFinished && a.ColorId == item.ColorId && a.ProductId == item.ProductId/*&&!a.IsFinished*/);
                        if (tempResult.Data.Count > 0)
                        {
                            foreach (var item2 in tempResult.Data)
                            {
                                item2.IsComplated = true;
                                item2.IsFinished = true;
                                item2.IsDeleted = true;
                                 await _tempServices.UpdateAll(item2);
                            }
                        }
                   

                }
              

                    return true;
            }
            catch (Exception EX)
            {
                var xxx = EX;

                return false;
            }

        }
        public FileResult DepoIcinFatura()
        {

            return null;
        }





        private Cell CreateTextCell(string header, UInt32 index, string text, UInt32Value styleIndex)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = header + index,
                StyleIndex = styleIndex
            };

            var istring = new InlineString();
            var t = new Text { Text = text };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }
        private string ColumnLetter(int intCol)
        {
            var intFirstLetter = ((intCol) / 676) + 64;
            var intSecondLetter = ((intCol % 676) / 26) + 64;
            var intThirdLetter = (intCol % 26) + 65;

            var firstLetter = (intFirstLetter > 64)
                ? (char)intFirstLetter : ' ';
            var secondLetter = (intSecondLetter > 64)
                ? (char)intSecondLetter : ' ';
            var thirdLetter = (char)intThirdLetter;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }
    }
}
