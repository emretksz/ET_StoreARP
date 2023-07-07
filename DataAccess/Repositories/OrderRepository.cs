using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccess.Repositories
{
    public class OrderRepository : Repository<Order, ET_StoreARP>, IOrderDal
    {

   private string connectionString = new ConnectionString().ConnectionStringPath();
        public async Task<List<OrderShippingDto>> OrderTenantShippingDto(long tenantId)
        {
     
            List<OrderShippingDto> SPPP = new List<OrderShippingDto>();
   
         
            // }
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = (from t in context.Temps
                              join s in context.ShippingDetails on t.ShippigId equals s.ShippinbgId
                              join p in context.Products on t.ProductId equals p.Id
                              join a in context.ProductAges on p.ProductAges.Id equals a.Id
                              join c in context.Colors on t.ColorId equals c.Id
                              join tenant in context.Tenants on t.TenantId equals tenant.Id
                              where t.TenantId == tenantId
                              select
                              new OrderShippingDto()
                              {
                                  ModelCode = p.ModelCode,
                                  Age = p.ProductAges != null ? p.ProductAges.Name : "",
                                  Barcode = p.Barcode,
                                  Gender = p.Gender,
                                  //Price = s.Amount,
                                  Amount = t.Count,
                                  TenantName=tenant.TenantName

                              });

                /// her product ıd için renkler ve ürübnler toplanmal ve toplam adet 

                var qqq = await result.ToListAsync();

            }


            return null;
        }


        public async Task<List<OrderShippingDto>> SP_TenantShippingOrderZamanaGore(long tenantId,DateTime start,DateTime end)
        {

            try
            {
                List<OrderShippingDto> SPPP = new List<OrderShippingDto>();
            
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
         
                {
                SqlDataReader sqlDr = null;

                    sqlConn.Open();

                    SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_TenantDateFilter]", sqlConn);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    //SqlParameter param = new SqlParameter("@TenantId", tenantId);
                    sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;
                    sqlCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = start;
                    sqlCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                    sqlDr = sqlCmd.ExecuteReader();

                    List<string> productId = new List<string>();
                    while (sqlDr.Read())
                    {
                        var tmp = new OrderShippingDto();
                        if (SPPP.Count == 0)
                        {
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ProductName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                            tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                            tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                            tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                            tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                            tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                            tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                            tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                            tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                            tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                            SPPP.Add(tmp);
                        }
                        else
                        {
                     
                            var colorsId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            var check = SPPP.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]) && a.ColorId == colorsId && a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]));
                            var checkSp = SPPP.FirstOrDefault(a => a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]) && a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));

                            if (check != null)
                            {
                                var amountCheck = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                check.Amount = check.Amount + amountCheck;
                            }
                            else if (checkSp != null)
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                SPPP.Add(tmp);
                            }
                            else
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                                tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                SPPP.Add(tmp);
                            }
                        }
                       


                    }


                    return SPPP.OrderByDescending(a=>a.Age).ToList();
                }

          
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<List<YearMonth>> SP_GenelKazanc()
        {

            List<YearMonth> SPPP = new List<YearMonth>();

            string date = DateTime.Now.Date.Year.ToString();
           using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_GetDayIndexMoney]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                //SqlParameter param = new SqlParameter("@TenantId", tenantId);
                sqlCmd.Parameters.Add("@Date", SqlDbType.VarChar).Value = date;
                //sqlCmd.Parameters.Add(param);
                sqlDr = sqlCmd.ExecuteReader();

                List<string> productId = new List<string>();
                while (sqlDr.Read())
                {

                    var tmp = new YearMonth();


                    switch (sqlDr["Ay"].ToString())
                    {
                        case "1":
                            var ocak = SPPP.FirstOrDefault(a => a.Ay == "1");
                            if (ocak != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                ocak.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "1";

                                var todecimal = sqlDr["Miktar"].ToString()!=""? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture):0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                                
                            }
                            break;
                        case "2":
                            var subat = SPPP.FirstOrDefault(a => a.Ay == "2");
                            if (subat != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                subat.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                    tmp.Ay = "2";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "3":
                            var mart = SPPP.FirstOrDefault(a => a.Ay == "3");
                            if (mart != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                mart.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "3";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "4":
                            var nisan = SPPP.FirstOrDefault(a => a.Ay == "4");
                            if (nisan != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                nisan.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "4";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "5":
                            var mayis = SPPP.FirstOrDefault(a => a.Ay == "5");
                            if (mayis != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                mayis.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "5";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "6":
                            var haziran = SPPP.FirstOrDefault(a => a.Ay == "6");
                            if (haziran != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                haziran.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "6";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "7":
                            var temmuz = SPPP.FirstOrDefault(a => a.Ay == "7");
                            if (temmuz != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                temmuz.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "7";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "8":
                            var agustos = SPPP.FirstOrDefault(a => a.Ay == "8");
                            if (agustos != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                agustos.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "8";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "9":
                            var eylul = SPPP.FirstOrDefault(a => a.Ay == "9");
                            if (eylul != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                eylul.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "9";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "10":
                            var ekim = SPPP.FirstOrDefault(a => a.Ay == "10");
                            if (ekim != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                ekim.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "10";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "11":
                            var kasim = SPPP.FirstOrDefault(a => a.Ay == "11");
                            if (kasim != null)
                            {
                                //var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                var todecimal = Convert.ToDecimal(sqlDr["Miktar"].ToString());
                                kasim.Miktar += Convert.ToDecimal(todecimal.ToString("N")); ;
                            }
                            else
                            {
                                tmp.Ay = "11";
                                //var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                var todecimal =Convert.ToDecimal(sqlDr["Miktar"].ToString());
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;
                        case "12":
                            var aralik = SPPP.FirstOrDefault(a => a.Ay == "12");
                            if (aralik != null)
                            {
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                aralik.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                            }
                            else
                            {
                                tmp.Ay = "12";
                                var todecimal = sqlDr["Miktar"].ToString() != "" ? decimal.Parse(sqlDr["Miktar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Miktar += Convert.ToDecimal(todecimal.ToString("N"));
                                SPPP.Add(tmp);
                            }
                            break;

                        default:
                            break;
                    }
                }
                return SPPP;
            }
           
        }


       public async Task<List<OrderShippingDto>> GetOrderTenantForExcel(long tenanId, DateTime date, bool fabrikaMi = false,string gender=null)
        {
           
            List<OrderShippingDto> SPPP = new List<OrderShippingDto>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    SqlDataReader sqlDr = null;
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    if (!fabrikaMi)
                    {
                        if (gender == "1")
                        {
                            sqlCmd = new SqlCommand("[dbo].[SP_GetTenantGenderKız]", sqlConn);
                            sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenanId;
                            sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;

                        }
                        else if (gender == "2")
                        {
                            sqlCmd = new SqlCommand("[dbo].[SP_GetTenantGenderErkek]", sqlConn);
                            sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenanId;
                            sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                        }
                    }
                    else
                    {
                        sqlCmd = new SqlCommand("[dbo].[SP_GetFabrikaAllDokuman]", sqlConn);
                        sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;


                    }

                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlDr = sqlCmd.ExecuteReader();


                    List<string> productId = new List<string>();
                    while (sqlDr.Read())
                    {
                        var tmp = new OrderShippingDto();
                        if (SPPP.Count == 0)
                        {
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ProductName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                            tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                            //tmp.Price = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0;
                            tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                            tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                            tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                            tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                            tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                            tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                            tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                            tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                            SPPP.Add(tmp);
                        }
                        else
                        {
                            var colorsId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            var check = SPPP.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]) && a.ColorId == colorsId && a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]));
                            var checkSp = SPPP.FirstOrDefault(a => a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]) && a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));
                            if (check != null)
                            {
                                var amountCheck = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                check.Amount = check.Amount + amountCheck;
              
                            }
                            else if (checkSp != null)
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                SPPP.Add(tmp);
                            }
                            else
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                               // tmp.Price = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0;  
                                tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                SPPP.Add(tmp);
                            }
                        }

                    }
                    List<OrderShippingDto> listem = new List<OrderShippingDto>();
                    using (ET_StoreARP db = new ET_StoreARP())
                    {
                        foreach (var item in db.Products)
                        {
                            var productList = SPPP.Where(a => a.ProductId == item.Id);
                            if (productList.Count()>0)
                            {
                                foreach (var item2 in productList)
                                {
                                    if (listem.Count==0)
                                    {
                                    listem.Add(item2);
                                    }
                                    else
                                    {
                                        var check = listem.FirstOrDefault(a=>a.ProductId==item2.ProductId&&a.ColorId==item2.ColorId);
                                        if (check!=null)
                                        {
                                            check.Amount = check.Amount + item2.Amount;
                                            check.Price = check.Price + item2.Price;
                                        }
                                        else
                                        {
                                            listem.Add(item2);
                                        }
                                    }
                                }
                            }
                           

                        }
                    }
                    return listem.OrderByDescending(a=>a.Age).ToList();
                }
            }
            catch (Exception ex )
            {

                return null;
            }

        }
        //public async Task<List<OrderShippingDto>> GetOrderTenantForExcel(long tenanId)
        //{



        //}


        public async Task<List<OrderShippingDto>> GetTenantKizErkek(DateTime date,string tenantName, string gender =null)
        {
            List<OrderShippingDto> SPPP = new List<OrderShippingDto>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    SqlDataReader sqlDr = null;
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
             
                        if (gender == "1")
                        {
                            // sqlCmd = new SqlCommand("[dbo].[SP_GetFabrikaAllDokuman]", sqlConn);
                            sqlCmd = new SqlCommand("[dbo].[SP_TenantKiz]", sqlConn);
                            sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                            sqlCmd.Parameters.Add("@TenantName", SqlDbType.VarChar).Value = tenantName;
                       
                        }
                        else if (gender == "2")
                        {
                            sqlCmd = new SqlCommand("[dbo].[SP_TenantErkek]", sqlConn);
                        sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                        sqlCmd.Parameters.Add("@TenantName", SqlDbType.VarChar).Value = tenantName;
                         }
                 

                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    //SqlParameter param = new SqlParameter("@TenantId", tenanId);
                    //

                    //qlParameter param = new SqlParameter("@TenantId", tenantId);

                    // sqlCmd.Parameters.Add(param);
                    sqlDr = sqlCmd.ExecuteReader();


                    List<string> productId = new List<string>();
                    while (sqlDr.Read())
                    {
                        var tmp = new OrderShippingDto();
                        if (SPPP.Count == 0)
                        {
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ProductName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                            tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                            //tmp.Price = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0;
                            tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                            tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                            tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                            tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                            tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                            tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                            tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                            tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                            SPPP.Add(tmp);
                        }
                        else
                        {
                            var colorsId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                            var check = SPPP.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]) && a.ColorId == colorsId && a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]));
                            var checkSp = SPPP.FirstOrDefault(a => a.SpId == Convert.ToInt64(sqlDr["ShippinbgId"]) && a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));
                            if (check != null)
                            {
                                var amountCheck = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                check.Amount = check.Amount + amountCheck;
                                //var tutar = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0;
                                //    check.Price = check.Price + tutar;
                            }
                            else if (checkSp != null)
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                                // tmp.Price = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0; yopkk
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                SPPP.Add(tmp);
                            }
                            else
                            {
                                tmp.TenantName = sqlDr["TenantName"].ToString();
                                tmp.ProductName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                                // tmp.Price = sqlDr["tutar"].ToString() != "" ? Convert.ToDecimal(sqlDr["tutar"]) : 0;  
                                tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                                tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                                tmp.Renk = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : null;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                                tmp.ColorId = sqlDr["ColorId"].ToString() != "" ? Convert.ToInt64(sqlDr["ColorId"]) : 0;
                                tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                                tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                                SPPP.Add(tmp);
                            }
                        }

                    }
                    List<OrderShippingDto> listem = new List<OrderShippingDto>();
                    using (ET_StoreARP db = new ET_StoreARP())
                    {
                        foreach (var item in db.Products)
                        {
                            var productList = SPPP.Where(a => a.ProductId == item.Id);
                            if (productList.Count() > 0)
                            {
                                foreach (var item2 in productList)
                                {
                                    if (listem.Count == 0)
                                    {
                                        listem.Add(item2);
                                    }
                                    else
                                    {
                                        var check = listem.FirstOrDefault(a => a.ProductId == item2.ProductId && a.ColorId == item2.ColorId);
                                        if (check != null)
                                        {
                                            check.Amount = check.Amount + item2.Amount;
                                            check.Price = check.Price + item2.Price;
                                        }
                                        else
                                        {
                                            listem.Add(item2);
                                        }
                                    }
                                }
                            }


                        }
                    }
                    return listem.OrderByDescending(a=>a.Age).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }       
        
        public async Task<List<OrderShippingDto>> GetTenantKizErkekToplamUrunleri(DateTime date,long tenantId, string gender =null)
        {
       
            List<OrderShippingDto> SPPP = new List<OrderShippingDto>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
              
                {
                    SqlDataReader sqlDr = null;
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                        
                        if (gender == "1")
                        {
                            // sqlCmd = new SqlCommand("[dbo].[SP_GetFabrikaAllDokuman]", sqlConn);
                            sqlCmd = new SqlCommand("[dbo].[SP_FirmaKizToplamSayisiVeTutari]", sqlConn);
                            sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                            sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;
                       
                        }
                        else if (gender == "2")
                        {
                            sqlCmd = new SqlCommand("[dbo].[SP_FirmaErkekToplamSayisiVeTutari]", sqlConn);
                        sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                        sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;
                         }
                 

                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlDr = sqlCmd.ExecuteReader();


                    List<string> productId = new List<string>();
                    while (sqlDr.Read())
                    {
                        var tmp = new OrderShippingDto();
                 
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ProductName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : null;
                            tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : null;
                            tmp.Price = sqlDr["tutar"].ToString() != "" ? decimal.Parse(sqlDr["tutar"].ToString(), CultureInfo.CurrentCulture) : 0;
                            tmp.Amount = sqlDr["siparisMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["siparisMiktari"]) : 0;
                            tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : null;
                            tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)sqlDr["Gender"] : 0;
                            tmp.ProductId = sqlDr["ProductId"].ToString() != "" ? Convert.ToInt64(sqlDr["ProductId"]) : 0;
                            tmp.SpId = sqlDr["ShippinbgId"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippinbgId"]) : 0;

                        SPPP.Add(tmp);
                       
                    }
                    return SPPP.OrderByDescending(a=>a.Age)/*.ThenBy(a=>a.Age)*/.ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }

    }
}
