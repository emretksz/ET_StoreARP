using Application.Interface;
using AutoMapper;
using Core.Utilities.Results;
using DataAccess.Migrations;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
//using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities.Dtos;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
//using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using static Entities.Enums.Enums;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class OrderController : Controller
    {
        readonly IProductServices _productServices;
        readonly IStockServices _stockServices;
        readonly IColorServices _colorServices;
        readonly ITempServices _tempServices;
        readonly IShippingDetailsServices _shippingDetailsServices;
        readonly ITenantServices _tenantServices;
        readonly IOrderDateServices _orderDateServices;
        readonly IOrderServices _orderServices;
        private readonly IWebHostEnvironment _evn;

        public OrderController(IOrderServices orderServices, IOrderDateServices orderDateServices, ITenantServices tenantServices, IWebHostEnvironment evn)
        {
            _orderServices = orderServices;
            _orderDateServices = orderDateServices;
            _tenantServices = tenantServices;
            _evn = evn;
        }

        readonly IMapper _mapper;

        public async Task<IActionResult> OrderIndex(int page = 1)
        {
            var rol = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            List<OrderPriceAndProduct> result = new List<OrderPriceAndProduct>();
            if (rol.ToLower() == "magaza")
            {
                result = await _orderDateServices.GetMagazaFaturalari();
                var pagedList2 = result.ToPagedList(page, 10);
                ViewBag.PageCount = result.Count;
                return View(pagedList2);

            }
            result = await _orderDateServices.GetFabrikaFaturalari();
            var pagedList = result.ToPagedList(page, 10);
            ViewBag.PageCount = result.Count;
            return View(pagedList);
        }
        public async Task<IActionResult> TenantOrder()
        {
            // tenantları listeleyecek
            var result = await _tenantServices.GetAll();
            ViewBag.Tenant = result.Data;
            var rol = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            ViewBag.admin = true;
            if (rol.ToLower().Trim() == "depo")
            {
                ViewBag.admin = false;
            }
            return View(result.Data);
        }
        public async Task<IActionResult> TenantOrderTable()
        {
            // tenantları listeleyecek
            var result = await _tenantServices.GetAll();
            //ViewBag.Tenant = result.Data;
            var qq = JsonConvert.SerializeObject(await result.Data.ToListAsync());
            return Json(qq);
        }

        public async Task<bool> FiyatlandirilmisOrder(long dateId)
        {
            var rol = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role).Value;
            List<OrderPriceAndProduct> result = new List<OrderPriceAndProduct>();
            if (rol.ToLower() == "magaza")
            {
                result = await _orderDateServices.GetMagazaFaturalari(dateId);
            }
            else
            {
                result = await _orderDateServices.GetFabrikaFaturalari(dateId);
            }

            try
            {

                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fiyatli_Fatura" + ".xlsx");
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
                    for (int i = 0; i < result.Count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i + 1), Max = Convert.ToUInt32(i + 2), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fiyatli_Fatura" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Firma" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Barcode" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Model Kodu" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Model Adı" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Yaş" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Cinsiyet" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                         rowIdex, "Renk" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Miktar" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                       rowIdex, "Birim Fiyatı" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Fiyatlandırılmış Hali" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Toplam" ?? string.Empty, 2));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                            rowIdex, "Tarih" ?? string.Empty, 2));
                    #endregion

                    #region Add Data
                    for (int i = 0; i < result.Count + 1; i++)
                    {
                        var money = result[i].Price;
                        //priceTemps += decimal.Parse(money.ToString(), CultureInfo.InvariantCulture);
                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);

                        var cell = CreateTextCell(ColumnLetter(0),
                               rowIdex, result[i].TenantName, 0);
                        row.AppendChild(cell);
                        var cell2 = CreateTextCell(ColumnLetter(1),
                                rowIdex, result[i].Barcode, 0);
                        row.AppendChild(cell2);
                        var cell3 = CreateTextCell(ColumnLetter(2),
                                rowIdex, result[i].ModelCode, 0);
                        row.AppendChild(cell3);
                        var cell4 = CreateTextCell(ColumnLetter(3),
                                rowIdex, result[i].ProductName, 0);
                        row.AppendChild(cell4);
                        var cell5 = CreateTextCell(ColumnLetter(4),
                                rowIdex, result[i].Age.ToString(), 0);
                        row.AppendChild(cell5);
                        var cell6 = CreateTextCell(ColumnLetter(5),
                                rowIdex, result[i].Gender.ToString(), 0);
                        row.AppendChild(cell6);
                        var cell8 = CreateTextCell(ColumnLetter(6),
                              rowIdex, result[i].ColorName, 0);
                        row.AppendChild(cell8);
                        var cell7 = CreateTextCell(ColumnLetter(7),
                                rowIdex, result[i].Count.ToString(), 0);
                        row.AppendChild(cell7);
                        var cell9 = CreateTextCell(ColumnLetter(8),
                        rowIdex, result[i].BirimFiyati.ToString(), 0);
                        row.AppendChild(cell9);
                        var cell10 = CreateTextCell(ColumnLetter(9),
                    rowIdex, result[i].Price != null ? decimal.Parse((result[i].Price).ToString(), CultureInfo.CurrentCulture).ToString("n")/*result[i].Price.ToString() */: "", 0);
                        row.AppendChild(cell10);
                        if (result.Count - 1 == i)
                        {
                            var cell11 = CreateTextCell(ColumnLetter(10),
                            rowIdex, "Toplam: " + decimal.Parse((result[i].TotalPrice).ToString(), CultureInfo.CurrentCulture).ToString("n") /* result[i].TotalPrice.ToString()*/ + "₺", 0);
                            row.AppendChild(cell11);
                        }
                        else
                        {
                            var cell11 = CreateTextCell(ColumnLetter(10),
                                                   rowIdex, "", 0);
                            row.AppendChild(cell11);
                        }

                        if (result.Count - 1 == i)
                        {
                            var cell12 = CreateTextCell(ColumnLetter(11),
                         rowIdex, "Tarih: " + result[i].OrderTarih.ToString(), 0);
                            row.AppendChild(cell12);
                        }
                        else
                        {
                            var cell12 = CreateTextCell(ColumnLetter(11),
                                      rowIdex, "", 0);
                            row.AppendChild(cell12);
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
        public FileResult DowloandFiyatlandirma()
        {
            string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");
            string specificFolder = Path.Combine(folder, "ExcelTemplates");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            var path = Path.Combine(specificFolder, "Fiyatli_Fatura" + ".xlsx");
            if (Directory.Exists(specificFolder))
            {
                return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel", "Fiyatli_Fatura" + ".xlsx");
            }
            else
                return null;


        }

        public async Task<bool> TenantOrderDowloand(long tenantId, int allShip /*DateTime date*/, bool fabrikaMi = false, string gender = null)
        {       /// exel dökümü al
            // order sorgusu ve dönen shippingleri
            DateTime date = new DateTime(allShip, 10, 10);
            // DateTime date = new DateTime(year, month, day, hour, minute, 0);
            List<OrderShippingDto> result = new List<OrderShippingDto>();

            result = await _orderServices.GetOrderTenantForExcel(tenantId, date, fabrikaMi, gender);

            /**/
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
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
                    var count = result.Count + 4;
                    for (int i = 1; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

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
                     rowIdex, "Toplam" ?? string.Empty, 1));

                    #endregion
                    decimal priceTemps = 0;
                    long amountTemps = 0;
                    var resultCount = result.Count + 1;

                    string modelId = "";
                    #region Add Data
                    for (int i = 0; i < resultCount; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                        //    if (resultCount - 1==i)
                        //    {
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //      rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //    var cell4 = CreateTextCell(ColumnLetter(3),
                        //            rowIdex,"", 0);
                        //    row.AppendChild(cell4);
                        //    var cell5 = CreateTextCell(ColumnLetter(4),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell5);
                        //    var cell6 = CreateTextCell(ColumnLetter(5),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell6);
                        //    var cell8 = CreateTextCell(ColumnLetter(6),
                        //          rowIdex, "", 0);
                        //    row.AppendChild(cell8);
                        //    temps += result[i].Price;
                        //    var cell7 = CreateTextCell(ColumnLetter(7),
                        //         rowIdex,"Toplam Satış: "+ temps.ToString(), 0);
                        //        row.AppendChild(cell7);

                        //    var cell9 = CreateTextCell(ColumnLetter(8),
                        //     rowIdex, "", 0);
                        //    row.AppendChild(cell9);
                        //}
                        //else
                        //{
                        if (modelId == "")
                        {
                            modelId = result[i].ProductId.ToString();

                        }

                        if (i == (resultCount - 1))
                        {
                            //amountTemps += result[i].Amount;
                            //priceTemps += result[i].Price;
                            var cell = CreateTextCell(ColumnLetter(0),
                                                  rowIdex, "", 0);
                            row.AppendChild(cell);
                            var cell2 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell2);
                            var cell3 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, "", 0);
                            row.AppendChild(cell3);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, "", 0);
                            row.AppendChild(cell4);
                            var cell5 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, "", 0);
                            row.AppendChild(cell5);
                            var cell6 = CreateTextCell(ColumnLetter(5),
                                    rowIdex, "", 0);
                            row.AppendChild(cell6);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                  rowIdex, "", 0);
                            row.AppendChild(cell7);
                            var cell8 = CreateTextCell(ColumnLetter(7),
                                   rowIdex, "Toplam Adet : " + amountTemps.ToString(), 0);
                            row.AppendChild(cell8);
                            var cell9 = CreateTextCell(ColumnLetter(8),
                                  rowIdex, "Toplam Tutar : " + priceTemps.ToString("n"), 0);
                            row.AppendChild(cell9);

                        }
                        else
                        {
                            if (modelId != result[i].ProductId.ToString())
                            {
                                modelId = result[i].ProductId.ToString();
                                cellIdex = 0;
                                row = new Row { RowIndex = ++rowIdex };
                                sheetData.AppendChild(row);
                                var cellq = CreateTextCell(ColumnLetter(0),
                           rowIdex, result[i].TenantName, 0);
                                row.AppendChild(cellq);
                                var cell2q = CreateTextCell(ColumnLetter(1),
                                        rowIdex, result[i].RenkBarcode, 0);
                                row.AppendChild(cell2q);
                                var cell3q = CreateTextCell(ColumnLetter(2),
                                        rowIdex, result[i].ModelCode, 0);
                                row.AppendChild(cell3q);
                                var cell4q = CreateTextCell(ColumnLetter(3),
                                        rowIdex, result[i].ProductName, 0);
                                row.AppendChild(cell4q);
                                var cell5q = CreateTextCell(ColumnLetter(4),
                                        rowIdex, result[i].Age.ToString(), 0);
                                row.AppendChild(cell5q);
                                var cell6q = CreateTextCell(ColumnLetter(5),
                                        rowIdex, result[i].Gender.ToString(), 0);
                                row.AppendChild(cell6q);
                                var cell7q = CreateTextCell(ColumnLetter(6),
                                      rowIdex, result[i].Renk.ToString(), 0);
                                row.AppendChild(cell7q);
                                var cell8q = CreateTextCell(ColumnLetter(7),
                                       rowIdex, result[i].Amount.ToString(), 0);
                                row.AppendChild(cell8q);
                                var cell9q = CreateTextCell(ColumnLetter(8),
                                      rowIdex, result[i].Price.ToString() == "0" ? "" : result[i].Price.ToString(), 0);
                                row.AppendChild(cell9q);
                                amountTemps += result[i].Amount;
                                var moneyq = result[i].Price;
                                priceTemps += decimal.Parse(moneyq.ToString(), CultureInfo.CurrentCulture);
                            }
                            else
                            {

                                var cell = CreateTextCell(ColumnLetter(0),
                                rowIdex, result[i].TenantName, 0);
                                row.AppendChild(cell);
                                var cell2 = CreateTextCell(ColumnLetter(1),
                                        rowIdex, result[i].RenkBarcode, 0);
                                row.AppendChild(cell2);
                                var cell3 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, result[i].ModelCode, 0);
                                row.AppendChild(cell3);
                                var cell4 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, result[i].ProductName, 0);
                                row.AppendChild(cell4);
                                var cell5 = CreateTextCell(ColumnLetter(4),
                                        rowIdex, result[i].Age.ToString(), 0);
                                row.AppendChild(cell5);
                                var cell6 = CreateTextCell(ColumnLetter(5),
                                        rowIdex, result[i].Gender.ToString(), 0);
                                row.AppendChild(cell6);
                                var cell7 = CreateTextCell(ColumnLetter(6),
                                      rowIdex, result[i].Renk.ToString(), 0);
                                row.AppendChild(cell7);
                                var cell8 = CreateTextCell(ColumnLetter(7),
                                       rowIdex, result[i].Amount.ToString(), 0);
                                row.AppendChild(cell8);
                                var cell9 = CreateTextCell(ColumnLetter(8),
                                      rowIdex, result[i].Price.ToString() == "0" ? "" : result[i].Price.ToString(), 0);
                                row.AppendChild(cell9);
                                amountTemps += result[i].Amount;
                                var money = result[i].Price;
                                priceTemps += decimal.Parse(money.ToString(), CultureInfo.CurrentCulture);
                            }
                            //priceTemps += result[i].Price;

                        }

                    }
                    //}



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

        public async Task<bool> FabrikaErkekKiz(bool erkekMi, int year)
        {

            List<GetShippingOrderList> models = new List<GetShippingOrderList>();
            DateTime dates = new DateTime(year, 1, 1);
            string cinsiyet = "";
            if (erkekMi)
            {
                cinsiyet = "erkek";
                models = await _tenantServices.FabrikaErkek(dates);
            }
            else
            {
                cinsiyet = "kız";
                models = await _tenantServices.FabrikaKiz(dates);
            }

            /**/
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
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
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.MediumGray } }); // required, reserved by Excel
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
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    var count = (models.Count * 2);
                    for (int i = 1; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                             rowIdex, "ÜRÜN-MODEL AÇIKLAMASI" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                      rowIdex, "Renk Barkodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                          rowIdex, "RENK" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "MİKTAR" ?? string.Empty, 1));

                    #endregion
                    decimal priceTemps = 0;
                    long amountTemps = 0;
                    var resultCount = models.Count;
                    bool bosluk = false;
                    #region Add Data
                    string tempId = "";
                    bool eksi = false;
                    for (int i = 0; i < resultCount; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                        //    if (resultCount - 1==i)
                        //    {
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //      rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //    var cell4 = CreateTextCell(ColumnLetter(3),
                        //            rowIdex,"", 0);
                        //    row.AppendChild(cell4);
                        //    var cell5 = CreateTextCell(ColumnLetter(4),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell5);
                        //    var cell6 = CreateTextCell(ColumnLetter(5),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell6);
                        //    var cell8 = CreateTextCell(ColumnLetter(6),
                        //          rowIdex, "", 0);
                        //    row.AppendChild(cell8);
                        //    temps += result[i].Price;
                        //    var cell7 = CreateTextCell(ColumnLetter(7),
                        //         rowIdex,"Toplam Satış: "+ temps.ToString(), 0);
                        //        row.AppendChild(cell7);

                        //    var cell9 = CreateTextCell(ColumnLetter(8),
                        //     rowIdex, "", 0);
                        //    row.AppendChild(cell9);
                        //}
                        //else
                        //{

                        //if (i == (resultCount - 1))
                        //{
                        //    //amountTemps += result[i].Amount;
                        //    //priceTemps += result[i].Price;
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //                          rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //                  rowIdex, "Toplam Adet : " + amountTemps.ToString(), 0);
                        //    row.AppendChild(cell3);

                        //}
                        //else
                        //{
                        //}
                        //if (bosluk)
                        //{
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //    rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //}
                        //else
                        //{
                        //}
                        if (tempId == "")
                        {
                            tempId = models[i].ModelCode;
                        }

                        if (tempId != (models[i].ModelCode != null ? models[i].ModelCode : ""))
                        {
                            tempId = models[i].ModelCode;


                            var cell = CreateTextCell(ColumnLetter(0),
                            rowIdex, "", 0);
                            row.AppendChild(cell);
                            var cell2 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell2);
                            var cell144 = CreateTextCell(ColumnLetter(2),
                              rowIdex, "", 0);
                            row.AppendChild(cell144);
                            var cell3 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, amountTemps.ToString(), 2);
                            row.AppendChild(cell3);

                            cellIdex = 0;
                            row = new Row { RowIndex = ++rowIdex };
                            sheetData.AppendChild(row);


                            amountTemps = 0;
                            var modelCode = models[i].ModelCode != null ? models[i].ModelCode : "" + "-";
                            var modelName = models[i].ModelName != null ? models[i].ModelName : "" + "-";
                            var modelyas = models[i].Age != null ? models[i].Age : "";
                            var aciklama = modelCode + " " + modelName + " " + modelyas + " " + cinsiyet;
                            var cell5 = CreateTextCell(ColumnLetter(0),
                             rowIdex, aciklama, 0);
                            row.AppendChild(cell5);
                            var cell6 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, models[i].RenkBarcode ?? "", 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(2),
                             rowIdex, models[i].ColorName ?? "", 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, models[i].Amount.ToString() ?? "", 0);
                            row.AppendChild(cell7);
                            amountTemps += models[i].Amount != null ? models[i].Amount : 0;

                        }
                        else
                        {

                            if (i == (resultCount - 1))
                            {
                                var modelCode = models[i].ModelCode != null ? models[i].ModelCode : "" + "-";
                                var modelName = models[i].ModelName != null ? models[i].ModelName : "" + "-";
                                var modelyas = models[i].Age != null ? models[i].Age : "";

                                var aciklamas = modelCode + " " + modelName + " " + modelyas + " " + cinsiyet;
                                var cell10 = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklamas, 0);
                                row.AppendChild(cell10);
                                var cell166 = CreateTextCell(ColumnLetter(1),
                                   rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell166);
                                var cell11 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].ColorName ?? "", 0);
                                row.AppendChild(cell11);
                                var cell12 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Amount.ToString() ?? "", 0);
                                row.AppendChild(cell12);
                                amountTemps += models[i].Amount != null ? models[i].Amount : 0;
                                cellIdex = 0;
                                row = new Row { RowIndex = ++rowIdex };
                                sheetData.AppendChild(row);

                                var cell13 = CreateTextCell(ColumnLetter(0),
                         rowIdex, "", 0);
                                row.AppendChild(cell13);
                                var cell14 = CreateTextCell(ColumnLetter(1),
                                        rowIdex, "", 0);
                                row.AppendChild(cell14);
                                var cell18 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, "", 0);
                                row.AppendChild(cell18);
                                var cell15 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, amountTemps.ToString(), 2);
                                row.AppendChild(cell15);



                            }
                            else
                            {
                                var modelCode = models[i].ModelCode != null ? models[i].ModelCode : "" + "-";
                                var modelName = models[i].ModelName != null ? models[i].ModelName : "" + "-";
                                var modelyas = models[i].Age != null ? models[i].Age : "";

                                var aciklama = modelCode + " " + modelName + " " + modelyas + " " + cinsiyet; ;
                                var cell = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklama, 0);
                                row.AppendChild(cell);
                                var cell1231 = CreateTextCell(ColumnLetter(1),
                                      rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell1231);
                                var cell2 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].ColorName ?? "", 0);
                                row.AppendChild(cell2);
                                var cell3 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Amount.ToString() ?? "", 0);
                                row.AppendChild(cell3);
                                amountTemps += models[i].Amount != null ? models[i].Amount : 0;

                            }

                        }




                    }
                    //}



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


        public async Task<bool> KizErkek(int year, string tenantName, bool erkekMi)
        {


            List<GetShippingOrderList> models = new List<GetShippingOrderList>();
            DateTime dates = new DateTime(year, 1, 1);


            if (erkekMi)
            {
                models = await _tenantServices.Erkek(dates, tenantName);
            }
            else
            {
                models = await _tenantServices.Kiz(dates, tenantName);
            }

            /**/
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
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
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.MediumGray } }); // required, reserved by Excel
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
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    var count = (models.Count * 2);
                    for (int i = 1; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                             rowIdex, "ÜRÜN-MODEL AÇIKLAMASI" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                      rowIdex, "Renk Barkodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                          rowIdex, "RENK" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "MİKTAR" ?? string.Empty, 1));

                    #endregion
                    decimal priceTemps = 0;
                    long amountTemps = 0;
                    var resultCount = models.Count;
                    bool bosluk = false;
                    #region Add Data
                    string tempId = "";
                    bool eksi = false;
                    for (int i = 0; i < resultCount; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                        //    if (resultCount - 1==i)
                        //    {
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //      rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //    var cell4 = CreateTextCell(ColumnLetter(3),
                        //            rowIdex,"", 0);
                        //    row.AppendChild(cell4);
                        //    var cell5 = CreateTextCell(ColumnLetter(4),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell5);
                        //    var cell6 = CreateTextCell(ColumnLetter(5),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell6);
                        //    var cell8 = CreateTextCell(ColumnLetter(6),
                        //          rowIdex, "", 0);
                        //    row.AppendChild(cell8);
                        //    temps += result[i].Price;
                        //    var cell7 = CreateTextCell(ColumnLetter(7),
                        //         rowIdex,"Toplam Satış: "+ temps.ToString(), 0);
                        //        row.AppendChild(cell7);

                        //    var cell9 = CreateTextCell(ColumnLetter(8),
                        //     rowIdex, "", 0);
                        //    row.AppendChild(cell9);
                        //}
                        //else
                        //{

                        //if (i == (resultCount - 1))
                        //{
                        //    //amountTemps += result[i].Amount;
                        //    //priceTemps += result[i].Price;
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //                          rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //                  rowIdex, "Toplam Adet : " + amountTemps.ToString(), 0);
                        //    row.AppendChild(cell3);

                        //}
                        //else
                        //{
                        //}
                        //if (bosluk)
                        //{
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //    rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //}
                        //else
                        //{
                        //}
                        if (tempId == "")
                        {
                            tempId = models[i].ModelCode;
                        }

                        if (tempId != (models[i].ModelCode != null ? models[i].ModelCode : ""))
                        {
                            tempId = models[i].ModelCode;


                            var cell = CreateTextCell(ColumnLetter(0),
                            rowIdex, "", 0);
                            row.AppendChild(cell);
                            var cell2 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell2);
                            var cell144 = CreateTextCell(ColumnLetter(2),
                              rowIdex, "", 0);
                            row.AppendChild(cell144);
                            var cell3 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, amountTemps.ToString(), 2);
                            row.AppendChild(cell3);

                            cellIdex = 0;
                            row = new Row { RowIndex = ++rowIdex };
                            sheetData.AppendChild(row);


                            amountTemps = 0;
                            var aciklama = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Age ?? "";
                            var cell5 = CreateTextCell(ColumnLetter(0),
                             rowIdex, aciklama, 0);
                            row.AppendChild(cell5);
                            var cell6 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, models[i].RenkBarcode ?? "", 0);
                            row.AppendChild(cell6);
                            var cell8 = CreateTextCell(ColumnLetter(2),
                             rowIdex, models[i].ColorName ?? "", 0);
                            row.AppendChild(cell8);
                            var cell7 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, models[i].Amount.ToString() ?? "", 0);
                            row.AppendChild(cell7);
                            amountTemps += models[i].Amount != null ? models[i].Amount : 0;

                        }
                        else
                        {

                            if (i == (resultCount - 1))
                            {
                                var aciklamas = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Age ?? "";
                                var cell10 = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklamas, 0);
                                row.AppendChild(cell10);
                                var cell166 = CreateTextCell(ColumnLetter(1),
                                   rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell166);
                                var cell11 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].ColorName ?? "", 0);
                                row.AppendChild(cell11);
                                var cell12 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Amount.ToString() ?? "", 0);
                                row.AppendChild(cell12);
                                amountTemps += models[i].Amount != null ? models[i].Amount : 0;
                                cellIdex = 0;
                                row = new Row { RowIndex = ++rowIdex };
                                sheetData.AppendChild(row);

                                var cell13 = CreateTextCell(ColumnLetter(0),
                         rowIdex, "", 0);
                                row.AppendChild(cell13);
                                var cell14 = CreateTextCell(ColumnLetter(1),
                                        rowIdex, "", 0);
                                row.AppendChild(cell14);
                                var cell18 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, "", 0);
                                row.AppendChild(cell18);
                                var cell15 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, amountTemps.ToString(), 2);
                                row.AppendChild(cell15);



                            }
                            else
                            {
                                var aciklama = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Age ?? "";
                                var cell = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklama, 0);
                                row.AppendChild(cell);
                                var cell1231 = CreateTextCell(ColumnLetter(1),
                                      rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell1231);
                                var cell2 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].ColorName ?? "", 0);
                                row.AppendChild(cell2);
                                var cell3 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Amount.ToString() ?? "", 0);
                                row.AppendChild(cell3);
                                amountTemps += models[i].Amount != null ? models[i].Amount : 0;

                            }

                        }




                    }
                    //}



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

        #region fabrika kız erkek renk ve ürün 

        #region eski renk urun
        //public async Task<bool> RenkVeUrunList(long tenantId, int date, bool gender)
        //{

        //    List<StockListDto> models = new List<StockListDto>();
        //    DateTime dates = new DateTime(date, 1, 1);

        //    if (gender)
        //    {
        //        models = await _tenantServices.ErkekRenkveUrun(dates,tenantId);
        //    }
        //    else
        //    {
        //        models = await _tenantServices.KizRenkveUrun(dates,tenantId);
        //    }

        //    /**/
        //    try
        //    {
        //        string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

        //        string specificFolder = Path.Combine(folder, "ExcelTemplates");

        //        if (!Directory.Exists(specificFolder))
        //            Directory.CreateDirectory(specificFolder);

        //        var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
        //        using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
        //        {
        //            document.AddWorkbookPart();
        //            document.WorkbookPart.Workbook = new Workbook();
        //            SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultColumnWidth = 12.75D, DefaultRowHeight = 5D, };
        //            Worksheet worksheet = new Worksheet();
        //            worksheet.Append(sheetFormatProperties);

        //            #region stylesPart
        //            var stylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //            stylesPart.Stylesheet = new Stylesheet();

        //            // Font list
        //            // Create a bold font
        //            stylesPart.Stylesheet.Fonts = new Fonts();
        //            Font bold_font = new Font();         // Bold font
        //            Bold bold = new Bold();
        //            bold_font.Append(bold);

        //            // Add fonts to list
        //            stylesPart.Stylesheet.Fonts.AppendChild(new Font());
        //            stylesPart.Stylesheet.Fonts.AppendChild(bold_font); // Bold gets fontid = 1
        //            stylesPart.Stylesheet.Fonts.Count = 2;

        //            // Create fills list
        //            stylesPart.Stylesheet.Fills = new Fills();

        //            var formatRed = new PatternFill() { PatternType = PatternValues.Solid };
        //            formatRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF6600") }; // red fill
        //            formatRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

        //            // Append fills to list
        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.MediumGray } }); // required, reserved by Excel
        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = formatRed }); // Red gets fillid = 2

        //            stylesPart.Stylesheet.Fills.Count = 3;

        //            // Create border list
        //            stylesPart.Stylesheet.Borders = new Borders();
        //            // Create thin borders for passed/failed tests and default cells
        //            LeftBorder leftThin = new LeftBorder() { Style = BorderStyleValues.Thin };
        //            RightBorder rightThin = new RightBorder() { Style = BorderStyleValues.Thin };
        //            TopBorder topThin = new TopBorder() { Style = BorderStyleValues.Thin };
        //            BottomBorder bottomThin = new BottomBorder() { Style = BorderStyleValues.Thin };

        //            Border borderThin = new Border();
        //            borderThin.Append(leftThin);
        //            borderThin.Append(rightThin);
        //            borderThin.Append(topThin);
        //            borderThin.Append(bottomThin);

        //            // Create thick borders for headings
        //            LeftBorder leftThick = new LeftBorder() { Style = BorderStyleValues.Thick };
        //            RightBorder rightThick = new RightBorder() { Style = BorderStyleValues.Thick };
        //            TopBorder topThick = new TopBorder() { Style = BorderStyleValues.Thick };
        //            BottomBorder bottomThick = new BottomBorder() { Style = BorderStyleValues.Thick };

        //            Border borderThick = new Border();
        //            borderThick.Append(leftThick);
        //            borderThick.Append(rightThick);
        //            borderThick.Append(topThick);
        //            borderThick.Append(bottomThick);

        //            // Add borders to list
        //            stylesPart.Stylesheet.Borders.AppendChild(new Border());
        //            stylesPart.Stylesheet.Borders.AppendChild(borderThin);
        //            stylesPart.Stylesheet.Borders.AppendChild(borderThick);
        //            stylesPart.Stylesheet.Borders.Count = 3;


        //            // Create blank cell format list
        //            stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
        //            stylesPart.Stylesheet.CellStyleFormats.Count = 1;
        //            stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

        //            // Create cell format list
        //            stylesPart.Stylesheet.CellFormats = new CellFormats();
        //            // empty one for index 0, seems to be required
        //            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());

        //            // Styleindex = 1, Normal
        //            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 0, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

        //            // Styleindex = 2, Bold Header
        //            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

        //            stylesPart.Stylesheet.CellFormats.Count = 3;
        //            stylesPart.Stylesheet.Save();

        //            #endregion

        //            var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
        //            worksheetPart.Worksheet = worksheet;

        //            #region Column
        //            Columns columns = new Columns();

        //            //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
        //            var count = (models.Count * 2);
        //            for (int i = 1; i < count; i++)
        //            {
        //                columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
        //            }

        //            worksheetPart.Worksheet.Append(columns);
        //            #endregion

        //            var sheetData = new SheetData();
        //            worksheet.AppendChild(sheetData);

        //            Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

        //            Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

        //            sheets.Append(sheet);

        //            #region Headers

        //            // Add header
        //            UInt32 rowIdex = 0;
        //            var row = new Row { RowIndex = ++rowIdex };
        //            sheetData.AppendChild(row);
        //            var cellIdex = 0;

        //            row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
        //                     rowIdex, "ÜRÜN-MODEL AÇIKLAMASI" ?? string.Empty, 1));
        //            row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
        //              rowIdex, "Renk Barkodu" ?? string.Empty, 1));
        //            row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
        //                  rowIdex, "RENK" ?? string.Empty, 1));
        //            row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
        //                rowIdex, "MİKTAR" ?? string.Empty, 1));

        //            #endregion
        //            decimal priceTemps = 0;
        //            long amountTemps = 0;
        //            var resultCount = models.Count;
        //            bool bosluk = false;
        //            #region Add Data
        //            string tempId = "";
        //            bool eksi = false;
        //            for (int i = 0; i < resultCount; i++)
        //            {

        //                string formula = "";
        //                cellIdex = 0;
        //                row = new Row { RowIndex = ++rowIdex };
        //                sheetData.AppendChild(row);


        //                if (tempId == "")
        //                {
        //                    tempId = models[i].ModelCode;
        //                }

        //                if (tempId != (models[i].ModelCode != null ? models[i].ModelCode : ""))
        //                {
        //                    tempId = models[i].ModelCode;


        //                    //var cell = CreateTextCell(ColumnLetter(0),
        //                    //rowIdex, "", 0);
        //                    //row.AppendChild(cell);
        //                    //var cell2 = CreateTextCell(ColumnLetter(1),
        //                    //        rowIdex, "", 0);
        //                    //row.AppendChild(cell2);
        //                    //var cell144 = CreateTextCell(ColumnLetter(2),
        //                    //  rowIdex, "", 0);
        //                    //row.AppendChild(cell144);
        //                    //var cell3 = CreateTextCell(ColumnLetter(3),
        //                    //        rowIdex, amountTemps.ToString(), 2);
        //                    //row.AppendChild(cell3);

        //                    cellIdex = 0;
        //                    row = new Row { RowIndex = ++rowIdex };
        //                    sheetData.AppendChild(row);


        //                    amountTemps = 0;
        //                    var aciklamas = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "" + " " + models[i].Gender ?? "";
        //                    var cell5 = CreateTextCell(ColumnLetter(0),
        //                     rowIdex, aciklamas, 0);
        //                    row.AppendChild(cell5);
        //                    var cell6 = CreateTextCell(ColumnLetter(1),
        //                            rowIdex, models[i].RenkBarcode ?? "", 0);
        //                    row.AppendChild(cell6);
        //                    var cell8 = CreateTextCell(ColumnLetter(2),
        //                     rowIdex, models[i].Renk ?? "", 0);
        //                    row.AppendChild(cell8);
        //                    var cell7 = CreateTextCell(ColumnLetter(3),
        //                            rowIdex, models[i].Counts.ToString() ?? "", 0);
        //                    row.AppendChild(cell7);
        //                  //  amountTemps += models[i].Amount != null ? models[i].Amount : 0;

        //                }
        //                else
        //                {

        //                    if (i == (resultCount - 1))
        //                    {
        //                        var aciklamas = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "" + " " + models[i].Gender ?? "";
        //                        var cell10 = CreateTextCell(ColumnLetter(0),
        //                         rowIdex, aciklamas, 0);
        //                        row.AppendChild(cell10);
        //                        var cell166 = CreateTextCell(ColumnLetter(1),
        //                           rowIdex, models[i].RenkBarcode ?? "", 0);
        //                        row.AppendChild(cell166);
        //                        var cell11 = CreateTextCell(ColumnLetter(2),
        //                                rowIdex, models[i].Renk ?? "", 0);
        //                        row.AppendChild(cell11);
        //                        var cell12 = CreateTextCell(ColumnLetter(3),
        //                                rowIdex, models[i].Counts.ToString() ?? "", 0);
        //                        row.AppendChild(cell12);
        //                       // amountTemps += models[i].Amount != null ? models[i].Amount : 0;
        //                        cellIdex = 0;
        //                        row = new Row { RowIndex = ++rowIdex };
        //                        sheetData.AppendChild(row);

        //                 //       var cell13 = CreateTextCell(ColumnLetter(0),
        //                 //rowIdex, "", 0);
        //                 //       row.AppendChild(cell13);
        //                 //       var cell14 = CreateTextCell(ColumnLetter(1),
        //                 //               rowIdex, "", 0);
        //                 //       row.AppendChild(cell14);
        //                 //       var cell18 = CreateTextCell(ColumnLetter(2),
        //                 //               rowIdex, "", 0);
        //                 //       row.AppendChild(cell18);
        //                 //       var cell15 = CreateTextCell(ColumnLetter(3),
        //                 //               rowIdex, amountTemps.ToString(), 2);
        //                 //       row.AppendChild(cell15);



        //                    }
        //                    else
        //                    {
        //                        var aciklama = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "" + " " + models[i].Gender ?? "";
        //                        var cell = CreateTextCell(ColumnLetter(0),
        //                         rowIdex, aciklama, 0);
        //                        row.AppendChild(cell);
        //                        var cell1231 = CreateTextCell(ColumnLetter(1),
        //                              rowIdex, models[i].RenkBarcode ?? "", 0);
        //                        row.AppendChild(cell1231);
        //                        var cell2 = CreateTextCell(ColumnLetter(2),
        //                                rowIdex, models[i].Renk ?? "", 0);
        //                        row.AppendChild(cell2);
        //                        var cell3 = CreateTextCell(ColumnLetter(3),
        //                                rowIdex, models[i].Counts.ToString() ?? "", 0);
        //                        row.AppendChild(cell3);
        //                       // amountTemps += models[i].Amount != null ? models[i].Amount : 0;

        //                    }

        //                }




        //            }
        //            //}



        //            #endregion
        //            document.WorkbookPart.Workbook.Save();
        //            document.Close();
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        #endregion
        public async Task<bool> RenkVeUrunList(long tenantId, int date, bool gender)
        {

            List<OrderShippingDto> models = new List<OrderShippingDto>();
            DateTime dates = new DateTime(date, 1, 1);

            if (gender)
            {
                models = await _orderServices.GetTenantKizErkekToplamUrunleri(dates, tenantId, "1");
            }
            else
            {
                models = await _orderServices.GetTenantKizErkekToplamUrunleri(dates, tenantId, "2");
            }

            /**/
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
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
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.MediumGray } }); // required, reserved by Excel
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
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    var count = (models.Count * 2);
                    for (int i = 1; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
              rowIdex, "Ürün Kodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                             rowIdex, "ÜRÜN-MODEL AÇIKLAMASI" ?? string.Empty, 1));          
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                             rowIdex, "Yaş" ?? string.Empty, 1));
                    //row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                    //  rowIdex, "Renk Barkodu" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Adet" ?? string.Empty, 1));     
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Fiyat" ?? string.Empty, 1));  
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "Tutar" ?? string.Empty, 1));

                    #endregion
           
                    for (int i = 0; i < models.Count; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);

                        //cellIdex = 0;
                        //row = new Row { RowIndex = ++rowIdex };
                        //sheetData.AppendChild(row);
                        var modelCode = models[i].ModelCode == null ? "" : models[i].ModelCode;
                        var productName = models[i].ProductName == null ? "" : models[i].ProductName;
                        var barcode = models[i].Barcode == null ? "" : models[i].Barcode;
                        var genders = models[i].Gender == null ? "" : (models[i].Gender).ToString();

                        var aciklamas =  productName+" - "+ barcode+" - "+ genders;
                         
                        var cell5 = CreateTextCell(ColumnLetter(0),
                         rowIdex, modelCode, 0);
                        row.AppendChild(cell5);
                        var cell7 = CreateTextCell(ColumnLetter(1),
                                rowIdex, aciklamas, 0);
                        row.AppendChild(cell7);                        
                        var cell121 = CreateTextCell(ColumnLetter(2),
                                rowIdex, models[i].Age.ToLower(), 0);
                        row.AppendChild(cell121);
                        var cell123 = CreateTextCell(ColumnLetter(3),
                            rowIdex, models[i].Amount.ToString(), 0) ;
                        row.AppendChild(cell123);
                        var cell9 = CreateTextCell(ColumnLetter(4),
                   rowIdex, models[i].Amount!=0?(models[i].Price!=0? models[i].Price/ models[i].Amount:0).ToString():"0" ?? "", 0);
                        row.AppendChild(cell9);
                        var cell8 = CreateTextCell(ColumnLetter(5),
                                rowIdex, models[i].Price.ToString() ?? "", 0);
                        row.AppendChild(cell8);
                  
                    }


                    #endregion
                    document.WorkbookPart.Workbook.Save();
                    document.Close();
                }
            }
            catch (Exception ex)
            {

            }

                return true;
        }
         


        public async Task<bool> YilSonuStok(string datetimes, string tenantName, bool gender)
        {
            DateTime dateTime = new DateTime(Convert.ToInt32(datetimes), 1, 1);

            var models = await _tenantServices.StockList(dateTime, tenantName, gender);
            /**/
            try
            {
                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                string specificFolder = Path.Combine(folder, "ExcelTemplates");

                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);

                var path = Path.Combine(specificFolder, "Kalan_Stok_" + ".xlsx");
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
                    stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.MediumGray } }); // required, reserved by Excel
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
                    stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 2, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });

                    stylesPart.Stylesheet.CellFormats.Count = 3;
                    stylesPart.Stylesheet.Save();

                    #endregion

                    var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = worksheet;

                    #region Column
                    Columns columns = new Columns();

                    //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                    var count = (models.Count * 2);
                    for (int i = 1; i < count; i++)
                    {
                        columns.Append(new Column() { Min = Convert.ToUInt32(i), Max = Convert.ToUInt32(i + 1), Width = 50, CustomWidth = true });
                    }

                    worksheetPart.Worksheet.Append(columns);
                    #endregion

                    var sheetData = new SheetData();
                    worksheet.AppendChild(sheetData);

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

                    sheets.Append(sheet);

                    #region Headers

                    // Add header
                    UInt32 rowIdex = 0;
                    var row = new Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);
                    var cellIdex = 0;

                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                             rowIdex, "ÜRÜN-MODEL AÇIKLAMASI" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                       rowIdex, "BARCODE" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                          rowIdex, "RENK" ?? string.Empty, 1));
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
                        rowIdex, "MİKTAR" ?? string.Empty, 1));

                    #endregion
                    decimal priceTemps = 0;
                    long amountTemps = 0;
                    var resultCount = models.Count;
                    bool bosluk = false;
                    #region Add Data
                    string tempId = "";
                    bool eksi = false;
                    for (int i = 0; i < resultCount; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                 
                        if (tempId == "")
                        {
                            tempId = models[i].ModelCode;
                        }

                        if (tempId != (models[i].ModelCode != null ? models[i].ModelCode : ""))
                        {
                            tempId = models[i].ModelCode;


                            //var cell = CreateTextCell(ColumnLetter(0),
                            //rowIdex, "", 0);
                            //row.AppendChild(cell);
                            //var cell2 = CreateTextCell(ColumnLetter(1),
                            //        rowIdex, "", 0);
                            //row.AppendChild(cell2);
                            //var cell3 = CreateTextCell(ColumnLetter(2),
                            //        rowIdex, amountTemps.ToString(), 2);
                            //row.AppendChild(cell3);

                            cellIdex = 0;
                            row = new Row { RowIndex = ++rowIdex };
                            sheetData.AppendChild(row);



                            //var aciklama = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "";
                            var aciklama = models[i].ModelCode + " - " + models[i].ModelName + " " + models[i].Name+"  "+models[i].Gender;
                            var cell5 = CreateTextCell(ColumnLetter(0),
                             rowIdex, aciklama, 0);
                            row.AppendChild(cell5);
                            var cell112 = CreateTextCell(ColumnLetter(1),
                                rowIdex, models[i].RenkBarcode ?? "", 0);
                            row.AppendChild(cell112);
                            var cell6 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, models[i].Renk ?? "", 0);
                            row.AppendChild(cell6);
                            var cell7 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, models[i].Miktar.ToString() ?? "", 0);
                            row.AppendChild(cell7);
                            amountTemps += models[i].Miktar != null ? models[i].Miktar : 0;
                            amountTemps = 0;
                        }
                        else
                        {

                            if (i == (resultCount - 1))
                            {
                                //  var aciklamas = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "";
                                var aciklama = models[i].ModelCode + " - " + models[i].ModelName + " " + models[i].Name + "  " + models[i].Gender;
                                var cell10 = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklama, 0);
                                row.AppendChild(cell10);
                                var cell114 = CreateTextCell(ColumnLetter(1),
                                  rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell114);
                                var cell11 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].Renk ?? "", 0);
                                row.AppendChild(cell11);
                                var cell12 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Miktar.ToString() ?? "", 0);
                                row.AppendChild(cell12);
                                amountTemps += models[i].Miktar != null ? models[i].Miktar : 0;
                                cellIdex = 0;
                                row = new Row { RowIndex = ++rowIdex };
                                sheetData.AppendChild(row);

                         //       var cell13 = CreateTextCell(ColumnLetter(0),
                         //rowIdex, "", 0);
                         //       row.AppendChild(cell13);
                         //       var cell14 = CreateTextCell(ColumnLetter(1),
                         //               rowIdex, "", 0);
                         //       row.AppendChild(cell14);
                         //       var cell15 = CreateTextCell(ColumnLetter(2),
                         //               rowIdex, amountTemps.ToString(), 2);
                         //       row.AppendChild(cell15);



                            }
                            else
                            {
                               // var aciklama = models[i].ModelCode ?? "" + " - " + models[i].ModelName ?? "" + " " + models[i].Name ?? "";
                                var aciklama = models[i].ModelCode + " - " + models[i].ModelName + " " + models[i].Name + "  " + models[i].Gender ;
                                var cell = CreateTextCell(ColumnLetter(0),
                                 rowIdex, aciklama, 0);
                                row.AppendChild(cell);
                                var cell24 = CreateTextCell(ColumnLetter(1),
                                     rowIdex, models[i].RenkBarcode ?? "", 0);
                                row.AppendChild(cell24);
                                var cell2 = CreateTextCell(ColumnLetter(2),
                                        rowIdex, models[i].Renk ?? "", 0);
                                row.AppendChild(cell2);
                                var cell3 = CreateTextCell(ColumnLetter(3),
                                        rowIdex, models[i].Miktar.ToString() ?? "", 0);
                                row.AppendChild(cell3);
                                amountTemps += models[i].Miktar != null ? models[i].Miktar : 0;

                            }

                        }




                    }
                    //}



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
                var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
                if (Directory.Exists(specificFolder))
                {
                    return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel", "Fatura_" + ".xlsx");
                }
                else
                    return null;


            }
         public FileResult StockDowloand()
            {

                string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");
                string specificFolder = Path.Combine(folder, "ExcelTemplates");
                if (!Directory.Exists(specificFolder))
                    Directory.CreateDirectory(specificFolder);
                var path = Path.Combine(specificFolder, "Kalan_Stok_" + ".xlsx");
                if (Directory.Exists(specificFolder))
                {
                    return File(new FileStream(path, FileMode.Open), "application/vnd.ms-excel", "Kalan_Stok_" + ".xlsx");
                }
                else
                    return null;


            }
            //public async Task<IActionResult> OrderByDatetime()
            //{
            //    sıralanacak yer
            //    return View();
            //}

            public async Task<bool> OrderByDatetime(long tenantId, string startDate, string endDate)
            {
            //tenant order'a dateimeklencekm sırala derse burada exlcl dökümü alacak
          
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);
                var result = await _orderServices.SP_TenantShippingOrderZamanaGore(tenantId, start, end);

                try
                {
                    string folder = _evn.WebRootPath;   //Server.MapPath("~/App_Data");

                    string specificFolder = Path.Combine(folder, "ExcelTemplates");

                    if (!Directory.Exists(specificFolder))
                        Directory.CreateDirectory(specificFolder);

                    var path = Path.Combine(specificFolder, "Fatura_" + ".xlsx");
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
                       var countResult= result.Count + 1;
                        //columns.Append(new Column() { Min = 1, Max = 1, Width = 10, CustomWidth = true });
                        for (int i = 0; i < countResult; i++)
                        {
                            columns.Append(new Column() { Min = Convert.ToUInt32(i + 1), Max = Convert.ToUInt32(i + 2), Width = 50, CustomWidth = true });
                        }

                        worksheetPart.Worksheet.Append(columns);
                        #endregion

                        var sheetData = new SheetData();
                        worksheet.AppendChild(sheetData);

                        Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                        Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Fatura" };

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
                     rowIdex, "Toplam" ?? string.Empty, 1));

                    #endregion
                    decimal temps = 0;
                    var resultCount = result.Count + 1;
                    decimal priceTemp = 0;
                    long amountTemp = 0;

                    #region Add Data
                    for (int i = 0; i < resultCount; i++)
                    {

                        string formula = "";
                        cellIdex = 0;
                        row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);


                        //    if (resultCount - 1==i)
                        //    {
                        //    var cell = CreateTextCell(ColumnLetter(0),
                        //      rowIdex, "", 0);
                        //    row.AppendChild(cell);
                        //    var cell2 = CreateTextCell(ColumnLetter(1),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell2);
                        //    var cell3 = CreateTextCell(ColumnLetter(2),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell3);
                        //    var cell4 = CreateTextCell(ColumnLetter(3),
                        //            rowIdex,"", 0);
                        //    row.AppendChild(cell4);
                        //    var cell5 = CreateTextCell(ColumnLetter(4),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell5);
                        //    var cell6 = CreateTextCell(ColumnLetter(5),
                        //            rowIdex, "", 0);
                        //    row.AppendChild(cell6);
                        //    var cell8 = CreateTextCell(ColumnLetter(6),
                        //          rowIdex, "", 0);
                        //    row.AppendChild(cell8);
                        //    temps += result[i].Price;
                        //    var cell7 = CreateTextCell(ColumnLetter(7),
                        //         rowIdex,"Toplam Satış: "+ temps.ToString(), 0);
                        //        row.AppendChild(cell7);

                        //    var cell9 = CreateTextCell(ColumnLetter(8),
                        //     rowIdex, "", 0);
                        //    row.AppendChild(cell9);
                        //}
                        //else
                        //{

                        if (i == (resultCount - 1))
                        {
                            //priceTemp += Convert.ToDecimal(result[i].Price);
                            //amountTemp += Convert.ToInt64(result[i].Amount);
                            var cell = CreateTextCell(ColumnLetter(0),
                           rowIdex,"", 0);
                            row.AppendChild(cell);
                            var cell2 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, "", 0);
                            row.AppendChild(cell2);
                            var cell3 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, "", 0);
                            row.AppendChild(cell3);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, "", 0);
                            row.AppendChild(cell4);
                            var cell5 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, "", 0);
                            row.AppendChild(cell5);
                            var cell6 = CreateTextCell(ColumnLetter(5),
                                    rowIdex, "", 0);
                            row.AppendChild(cell6);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                  rowIdex, "", 0);
                            row.AppendChild(cell7);
                            var cell8 = CreateTextCell(ColumnLetter(7),
                                   rowIdex, "Toplam Adet: "+amountTemp.ToString(), 0);
                            row.AppendChild(cell8);
                            var cell9 = CreateTextCell(ColumnLetter(8),
                                  rowIdex, "Toplam Tutar: "+priceTemp.ToString("n") , 0);
                            row.AppendChild(cell9);
                        }
                        else
                        {
                            var cell = CreateTextCell(ColumnLetter(0),
                             rowIdex, result[i].TenantName, 0);
                            row.AppendChild(cell);
                            var cell2 = CreateTextCell(ColumnLetter(1),
                                    rowIdex, result[i].RenkBarcode, 0);
                            row.AppendChild(cell2);
                            var cell3 = CreateTextCell(ColumnLetter(2),
                                    rowIdex, result[i].ModelCode, 0);
                            row.AppendChild(cell3);
                            var cell4 = CreateTextCell(ColumnLetter(3),
                                    rowIdex, result[i].ProductName, 0);
                            row.AppendChild(cell4);
                            var cell5 = CreateTextCell(ColumnLetter(4),
                                    rowIdex, result[i].Age.ToString(), 0);
                            row.AppendChild(cell5);
                            var cell6 = CreateTextCell(ColumnLetter(5),
                                    rowIdex, result[i].Gender.ToString(), 0);
                            row.AppendChild(cell6);
                            var cell7 = CreateTextCell(ColumnLetter(6),
                                  rowIdex, result[i].Renk.ToString(), 0);
                            row.AppendChild(cell7);
                            var cell8 = CreateTextCell(ColumnLetter(7),
                                   rowIdex, result[i].Amount.ToString(), 0);
                            row.AppendChild(cell8);
                            var cell9 = CreateTextCell(ColumnLetter(8),
                                  rowIdex, result[i].Price.ToString() != "0" ? result[i].Price.ToString("n") : "", 0);
                            row.AppendChild(cell9);

                            var money = result[i].Price;
                            priceTemp += decimal.Parse(money.ToString(), CultureInfo.CurrentCulture); 
                            amountTemp += Convert.ToInt64(result[i].Amount);
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
