using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace DataAccess.Repositories
{
    public class TempRepository : Repository<Temp, ET_StoreARP>, ITempDal
    {
        //public TempRepository(ET_StoreARP context):base(context)
        //{

        //}
    
        private string connectionString = new ConnectionString().ConnectionStringPath();
        public async Task<List<ShippingProduct>> GetParticalShipping(long shippingId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = await (from a in context.Temps
                                    join c in context.Products on a.ProductId equals c.Id
                                    join b in context.Colors on a.ColorId equals b.Id
                                    where a.Count > 0 && a.ShippigId == shippingId && !a.IsFinished
                                    select new ShippingProduct
                                    {
                                        ColorId = b.Id,
                                        ColorName = b.ColorName,
                                        Count = a.Count,
                                        ProductName = c.ModelName,
                                        ProductId = c.Id,
                                        ShippingId = shippingId,
                                        TempId = a.Id,
                                        RenkBarcode=a.RenkBarcode
                                     
                                       

                                    }).ToListAsync();
                return result;
            }
            return null;
        }
        public async Task<List<ShippingProduct>> GetOrderShippingResult(long shippingId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var result = await (from a in context.Temps
                                    join b in context.Colors on a.ColorId equals b.Id
                                    join c in context.Products on a.ProductId equals c.Id
                                    where a.ProductId == c.Id && b.Id == a.ColorId && a.ShippigId == shippingId && a.IsComplated && !a.IsFinished && !a.IsDeleted
                                    select new ShippingProduct
                                    {
                                        ColorId = b.Id,
                                        ColorName = b.ColorName,
                                        Count = a.Count,
                                        ProductName = c.ModelName,
                                        ProductId = c.Id,
                                        ShippingId = shippingId,
                                        Age = ((Age)(int)c.Age).ToString(),
                                        Gender = c.Gender,
                                        ModelCode = c.ModelCode,
                                        TempId = a.Id
                                    }).ToListAsync();
                return result;
            }
            return null;
        }


        public async Task<List<ShippingProduct>> GetTenantShippingOrderPrice(long? tenantId = null, long? shippingId = null)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                List<ShippingProduct> result = new List<ShippingProduct>();

                if (tenantId != null)
                {

                    result = await (from t in context.Temps
                                    join s in context.Shippings on t.ShippigId equals s.Id
                                    join tenant in context.Tenants on s.TenantId equals tenant.Id
                                    join c in context.Colors on t.ColorId equals c.Id
                                    join p in context.Products on t.ProductId equals p.Id
                                    where t.ShippigId == shippingId.Value && !t.IsDeleted && t.IsFinished
                                    select new ShippingProduct
                                    {
                                        ColorId = c.Id,
                                        ColorName = c.ColorName,
                                        Count = t.Count,
                                        ProductName = p.ModelName,
                                        ProductId = t.ProductId,
                                        ShippingId = t.ShippigId,
                                        Age = ((Age)(int)p.Age).ToString(),
                                        Gender = p.Gender,
                                        ModelCode = p.ModelCode,
                                        TenantName = tenant.TenantName,
                                        TenantId = tenant.Id,
                                        TempId = t.Id
                                    }).AsNoTracking().ToListAsync();


                }
                else
                {
                    result = await (from a in context.Shippings
                                    join b in context.Tenants on a.TenantId equals b.Id
                                    join c in context.Temps on a.Id equals c.ShippigId
                                    join p in context.Products on c.ProductId equals p.Id
                                    where /*c.IsComplated && c.IsFinished &&*/ !c.IsDeleted/*&&a.MagazaMi ==null*/
                                    select new ShippingProduct
                                    {
                                        ColorId = c.ColorId,
                                        ColorName = b.TenantName,
                                        Count = c.Count,
                                        ProductName = p.ModelName,
                                        ProductId = c.ProductId,
                                        ShippingId = c.ShippigId,
                                        Age = ((Age)(int)p.Age).ToString(),
                                        Gender = p.Gender,
                                        ModelCode = p.ModelCode,
                                        TenantName = b.TenantName,
                                        TenantId = b.Id,
                                        TempId = c.Id
                                    }).AsNoTracking().ToListAsync();
                }


                List<ShippingProduct> sp = new List<ShippingProduct>();

                List<long> tenantTmp = new List<long>();
                List<long> tenantShippingId = new List<long>();
                foreach (var item in result)
                {
                    ShippingProduct pr = new ShippingProduct();
                    if (sp.Count == 0)
                    {
                        pr.ProductId = item.ProductId;
                        pr.ProductName = item.ProductName;
                        pr.ColorName = item.ColorName;
                        pr.Gender = item.Gender;
                        pr.Age = item.Age;
                        pr.Count = item.Count;
                        pr.ModelCode = item.ModelCode;
                        pr.ShippingId = item.ShippingId;
                        pr.TenantName = item.TenantName;
                        pr.TenantId = item.TenantId;
                        pr.TempId = item.TempId;
                        pr.ShippingLists = new List<ShippingList>()
                       {
                           new ShippingList
                           {
                               Age = (((Age)Convert.ToInt32(item.Age))),
                               ColorId = item.ColorId,
                               ColorName=item.ColorName,
                               Count=item.Count,
                               Gender=item.Gender,ModelCode=item.ModelCode,ProductId=item.ProductId,ProductName=item.ProductName
                           }
                       };
                        sp.Add(pr);
                        tenantTmp.Add(item.TenantId);
                        tenantShippingId.Add(item.ShippingId);
                    }
                    else
                    {
                        if (tenantTmp.Contains(item.TenantId) && tenantShippingId.Contains(item.ShippingId))
                        {
                            var result2 = sp.FirstOrDefault(a => a.TenantId == item.TenantId && a.ShippingId == item.ShippingId);
                            result2.ShippingLists.Add(new ShippingList
                            {
                                Age = (((Age)Convert.ToInt32(item.Age))),
                                ColorId = item.ColorId,
                                ColorName = item.ColorName,
                                Count = item.Count,
                                Gender = item.Gender,
                                ModelCode = item.ModelCode,
                                ProductId = item.ProductId,
                                ProductName = item.ProductName
                            });
                        }
                        else
                        {
                            pr.ProductId = item.ProductId;
                            pr.ProductName = item.ProductName;
                            pr.ColorName = item.ColorName;
                            pr.Gender = item.Gender;
                            pr.Age = item.Age;
                            pr.Count = item.Count;
                            pr.ModelCode = item.ModelCode;
                            pr.ShippingId = item.ShippingId;
                            pr.TenantName = item.TenantName;
                            pr.TenantId = item.TenantId;
                            pr.TempId = item.TempId;
                            pr.ShippingLists = new List<ShippingList>()
                       {
                           new ShippingList
                           {
                               Age = (((Age)Convert.ToInt32(item.Age))),
                               ColorId = item.ColorId,
                               ColorName=item.ColorName,
                               Count=item.Count,
                               Gender=item.Gender,ModelCode=item.ModelCode,ProductId=item.ProductId,ProductName=item.ProductName
                           }
                       };
                            sp.Add(pr);
                            tenantTmp.Add(item.TenantId);
                            tenantShippingId.Add(item.ShippingId);
                        }
                    }

                }


                return sp;
            }
            return null;
        }

        public async Task<List<ShippingProduct>> GetTenantShippingOrderPriceMagaza(long? tenantId = null, long? shippingId = null)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {

                List<ShippingProduct> result = new List<ShippingProduct>();

                if (tenantId != null)
                {

                    result = await (from t in context.Temps
                                    join s in context.Shippings on t.ShippigId equals s.Id
                                    join tenant in context.Tenants on s.TenantId equals tenant.Id 
                                    join c in context.Colors on t.ColorId equals c.Id
                                    join p in context.Products on t.ProductId equals p.Id
                                    where t.ShippigId == shippingId.Value && !t.IsDeleted && t.IsFinished/*&& s.MagazaMi.Value*/
                                   
                                    select new ShippingProduct
                                    {
                                        ColorId = c.Id,
                                        ColorName = c.ColorName,
                                        Count = t.Count,
                                        ProductName = p.ModelName,
                                        ProductId = t.ProductId,
                                        ShippingId = t.ShippigId,
                                        Age = ((Gender)(int)p.Age).ToString(),
                                        Gender = p.Gender,
                                        ModelCode = p.ModelCode,
                                        TenantName = tenant.TenantName,
                                        TenantId = tenant.Id,
                                        TempId = t.Id
                                    }).AsNoTracking().ToListAsync();
                }
                else
                {
                    result = await (from a in context.Shippings
                                    join b in context.Tenants on a.TenantId equals b.Id
                                    join c in context.Temps on a.Id equals c.ShippigId
                                    join p in context.Products on c.ProductId equals p.Id
                                    where /*c.IsComplated && c.IsFinished &&*/ !c.IsDeleted/*&&a.MagazaMi.Value*/
                                    select new ShippingProduct
                                    {
                                        ColorId = c.ColorId,
                                        ColorName = b.TenantName,
                                        Count = c.Count,
                                        ProductName = p.ModelName,
                                        ProductId = c.ProductId,
                                        ShippingId = c.ShippigId,
                                        Age = ((Age)(int)p.Age).ToString(),
                                        Gender = p.Gender,
                                        ModelCode = p.ModelCode,
                                        TenantName = b.TenantName,
                                        TenantId = b.Id,
                                        TempId = c.Id
                                    }).AsNoTracking().ToListAsync();
                }


                List<ShippingProduct> sp = new List<ShippingProduct>();

                List<long> tenantTmp = new List<long>();
                List<long> tenantShippingId = new List<long>();
                foreach (var item in result)
                {
                    ShippingProduct pr = new ShippingProduct();
                    if (sp.Count == 0)
                    {
                        pr.ProductId = item.ProductId;
                        pr.ProductName = item.ProductName;
                        pr.ColorName = item.ColorName;
                        pr.Gender = item.Gender;
                        pr.Age = item.Age;
                        pr.Count = item.Count;
                        pr.ModelCode = item.ModelCode;
                        pr.ShippingId = item.ShippingId;
                        pr.TenantName = item.TenantName;
                        pr.TenantId = item.TenantId;
                        pr.TempId = item.TempId;
                        pr.ShippingLists = new List<ShippingList>()
                       {
                           new ShippingList
                           {
                               Age = (((Age)Convert.ToInt32(item.Age))),
                               ColorId = item.ColorId,
                               ColorName=item.ColorName,
                               Count=item.Count,
                               Gender=item.Gender,ModelCode=item.ModelCode,ProductId=item.ProductId,ProductName=item.ProductName
                           }
                       };
                        sp.Add(pr);
                        tenantTmp.Add(item.TenantId);
                        tenantShippingId.Add(item.ShippingId);
                    }
                    else
                    {
                        if (tenantTmp.Contains(item.TenantId) && tenantShippingId.Contains(item.ShippingId))
                        {
                            var result2 = sp.FirstOrDefault(a => a.TenantId == item.TenantId && a.ShippingId == item.ShippingId);
                            result2.ShippingLists.Add(new ShippingList
                            {
                                Age = (((Age)Convert.ToInt32(item.Age))),
                                ColorId = item.ColorId,
                                ColorName = item.ColorName,
                                Count = item.Count,
                                Gender = item.Gender,
                                ModelCode = item.ModelCode,
                                ProductId = item.ProductId,
                                ProductName = item.ProductName
                            });
                        }
                        else
                        {
                            pr.ProductId = item.ProductId;
                            pr.ProductName = item.ProductName;
                            pr.ColorName = item.ColorName;
                            pr.Gender = item.Gender;
                            pr.Age = item.Age;
                            pr.Count = item.Count;
                            pr.ModelCode = item.ModelCode;
                            pr.ShippingId = item.ShippingId;
                            pr.TenantName = item.TenantName;
                            pr.TenantId = item.TenantId;
                            pr.TempId = item.TempId;
                            pr.ShippingLists = new List<ShippingList>()
                       {
                           new ShippingList
                           {
                               Age = (((Age) Convert.ToInt32(item.Age))),
                               ColorId = item.ColorId,
                               ColorName=item.ColorName,
                               Count=item.Count,
                               Gender=item.Gender,ModelCode=item.ModelCode,ProductId=item.ProductId,ProductName=item.ProductName
                           }
                       };
                            sp.Add(pr);
                            tenantTmp.Add(item.TenantId);
                            tenantShippingId.Add(item.ShippingId);
                        }
                    }

                }


                return sp;
            }
            return null;
        }



        public async Task<List<Product>> GetPrepareShipping(long shippingId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = await(from sp in context.ShippingDetails
                             join p in context.Products on sp.ProductId equals p.Id
                             join pAges in context.ProductAges on p.ProductAges.Id equals pAges.Id
                             where sp.ShippinbgId== shippingId
                             select new Product()
                             {
                                  Id=sp.ProductId,
                                  ModelCode=p.ModelCode,
                                  ModelName=p.ModelName,
                                  ModelImageUrl=p.ModelImageUrl,
                                  Barcode=p.Barcode,
                                 ProductAges=pAges,
                                 ModelCount= sp.Amount.ToString(),
                                 Gender=p.Gender
                                 
                             }).ToListAsync();

                return result;
            }
        }

        public async Task<List<GetShippingOrderList>> GetShippingList(long shippingId)
        { 
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_GetShippingOrderList]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@ShippingId", shippingId);
                sqlCmd.Parameters.Add(param);
                sqlDr = sqlCmd.ExecuteReader();
                /////////
               // List<string>  = new List<string>();
               List<GetShippingOrderList> list= new List<GetShippingOrderList>();
                while (sqlDr.Read())
                {
                    var tmp = new GetShippingOrderList();
                    if (list.Count==0)
                    {
                        tmp.ShippingId = Convert.ToInt64(sqlDr["ShippigId"]);
                        tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                        tmp.ModelName = sqlDr["ModelName"].ToString() != "" ? sqlDr["ModelName"].ToString() : "";
                        tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : "";
                        tmp.Amount = sqlDr["Amount"].ToString() != "" ? Convert.ToInt64(sqlDr["Amount"]) : 0;
                        tmp.Age = sqlDr["Age"].ToString() != "" ? sqlDr["Age"].ToString() : "";
                        tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : "";
                        //todo
                       // tmp.Price = sqlDr["Price"].ToString() != "" ? Convert.ToInt64(sqlDr["Price"]) : 0;
                        tmp.Price = sqlDr["Price"].ToString() != "" ? decimal.Parse(sqlDr["Price"].ToString(), CultureInfo.CurrentCulture) : 0;
                        tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)(Convert.ToInt32(sqlDr["Gender"])) : 0;
                        tmp.TenantName = sqlDr["TenantName"].ToString() != "" ? sqlDr["TenantName"].ToString() : "";
                        tmp.ColorName = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : "";
                        tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";
                        // tmp.Gender = ((Gender)(int)sqlDr["gender"]);
                        list.Add(tmp);
                    }
                    else
                    {
                        var colorNameIsValid= sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : "";
                        var query = list.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]) && a.ColorName == colorNameIsValid);
                        if (query!=null)
                        {
                            long amountCheck = sqlDr["Amount"].ToString() != "" ? Convert.ToInt64(sqlDr["Amount"]) : 0;
                            query.Amount=( query.Amount+ amountCheck);
                            //long priceCheck = sqlDr["Price"].ToString() != "" ? Convert.ToInt64(sqlDr["Price"]) : 0;
                            //query.Price = (query.Price + priceCheck);
                        }
                        else
                        {
                             query = list.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));
                            if (query!=null)
                            {
                                tmp.ShippingId = Convert.ToInt64(sqlDr["ShippigId"]);
                                tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                                tmp.ModelName = sqlDr["ModelName"].ToString() != "" ? sqlDr["ModelName"].ToString() : "";
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : "";
                                tmp.Amount = sqlDr["Amount"].ToString() != "" ? Convert.ToInt64(sqlDr["Amount"]) : 0;
                                tmp.Age = sqlDr["Age"].ToString() != "" ? sqlDr["Age"].ToString() : "";
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : "";
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)(Convert.ToInt32(sqlDr["Gender"])) : 0;
                                tmp.TenantName = sqlDr["TenantName"].ToString() != "" ? sqlDr["TenantName"].ToString() : "";
                                tmp.ColorName = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : "";
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";

                                list.Add(tmp);
                            }
                            else
                            {
                           
                                tmp.ShippingId = Convert.ToInt64(sqlDr["ShippigId"]);
                                tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                                tmp.ModelName = sqlDr["ModelName"].ToString() != "" ? sqlDr["ModelName"].ToString() : "";
                                tmp.ModelCode = sqlDr["ModelCode"].ToString() != "" ? sqlDr["ModelCode"].ToString() : "";
                                tmp.Amount = sqlDr["Amount"].ToString() != "" ? Convert.ToInt64(sqlDr["Amount"]) : 0;
                                tmp.Age = sqlDr["Age"].ToString() != "" ? sqlDr["Age"].ToString() : "";
                                tmp.Barcode = sqlDr["Barcode"].ToString() != "" ? sqlDr["Barcode"].ToString() : "";
                                tmp.Price = sqlDr["Price"].ToString() != "" ?  decimal.Parse(sqlDr["Price"].ToString(), CultureInfo.CurrentCulture): 0;
                                tmp.Gender = sqlDr["Gender"].ToString() != "" ? (Gender)(int)(Convert.ToInt32(sqlDr["Gender"])) : 0;
                                tmp.TenantName = sqlDr["TenantName"].ToString() != "" ? sqlDr["TenantName"].ToString() : "";
                                tmp.ColorName = sqlDr["ColorName"].ToString() != "" ? sqlDr["ColorName"].ToString() : "";
                                tmp.RenkBarcode = sqlDr["RenkBarcode"].ToString() != "" ? sqlDr["RenkBarcode"].ToString() : "";

                                list.Add(tmp);
                            }
                         
                        }
                    }
                

                }

                return list.OrderByDescending(a=>a.ProductId).ThenByDescending(a=>a.Age).ToList();
            }

        }
        public async Task<List<GetShippingOrderList>> GetFis(long shippingId)
        {
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
                try
                {
                    {
                        SqlDataReader sqlDr = null;
                        sqlConn.Open();
                  


                        SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_Fis]", sqlConn);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ShippingId", SqlDbType.BigInt).Value = shippingId;
                        sqlDr = sqlCmd.ExecuteReader();


                        /////////
                        // List<string>  = new List<string>();
                        List<GetShippingOrderList> list = new List<GetShippingOrderList>();
                        while (sqlDr.Read())
                        {
                            var tmp = new GetShippingOrderList();
                          
                                var pricecheck = sqlDr["fiyat"].ToString() != "" ? sqlDr["fiyat"].ToString() : "0";
                                tmp.ModelName = sqlDr["ad"].ToString() != "" ? sqlDr["ad"].ToString() : "";
                                tmp.ModelCode = sqlDr["kod"].ToString() != "" ? sqlDr["kod"].ToString() : "";
                                tmp.Age = sqlDr["yas"].ToString() != "" ? sqlDr["yas"].ToString() : "";
                                tmp.Price = decimal.Parse(pricecheck, CultureInfo.CurrentCulture);
                                tmp.Gender = sqlDr["cinsiyet"].ToString() != "" ? (Gender)(int)(Convert.ToInt32(sqlDr["cinsiyet"])) : 0; 
                                tmp.Amount = sqlDr["miktar"].ToString() != "" ? Convert.ToInt64(sqlDr["miktar"]) : 0;
                                tmp.TenantName = sqlDr["TenantName"].ToString() != "" ? sqlDr["TenantName"].ToString() : "";
                                list.Add(tmp);
                            

                        }


                        return list;
                    }
                }

                catch (Exception ex)
                {

                    return null;
                }
            


        }

        public async Task<ComplatedShippingExcel> ComplatedShippingForExcel(long colorId, long productId)
        {
            using (ET_StoreARP context = new ET_StoreARP())
            {
                var result = from p in context.Products.Include(a => a.ProductAges)
                             join c in context.Colors on colorId equals c.Id
                             where p.Id == productId && c.Id == colorId
                             select new ComplatedShippingExcel()
                             {
                                 Product = p,
                                 Renk = c.ColorName
                             };

                return await result.FirstOrDefaultAsync();
            }
        }


        public async Task<List<Shippings>> GetShippingsAsyncForGrid()
        {

            try
            {
                using (ET_StoreARP db = new ET_StoreARP())
                {
                    var result = await (from s in db.Shippings
                                        join t in db.Tenants on s.TenantId equals t.Id
                                        select new Shippings()
                                        {
                                            Id = s.Id,
                                            SiparisAdi = s.SiparisAdi,
                                            SiparisTutari = s.SiparisTutari,
                                            RegisterDate = s.RegisterDate,
                                            Tenant = t
                                        }).ToListAsync();
                    return result;

                }
            }
            catch (Exception ex )
            {
                List<Shippings> nullSp = new List<Shippings>();

                return nullSp;
            }
        }

        public async Task<List<ShippingProduct>> UpdateFinishShippingListView(long spId)
        {

            using (ET_StoreARP db=new ET_StoreARP())
            {
                var result = await (from t in db.Temps
                             join s in db.Shippings on t.ShippigId equals s.Id
                             join p in db.Products on t.ProductId equals p.Id
                             join c in db.Colors on t.ColorId equals c.Id
                             join st in db.Stocks on new { t.ColorId,t.ProductId } equals new { st.ColorId,st.ProductId }
                             where t.ShippigId == spId && st.ProductId==t.ProductId&& st.ColorId==t.ColorId
                             select new ShippingProduct()
                             {
                                 TempId=t.Id,
                                 ProductName = p.ModelName,
                                 Barcode = p.Barcode,
                                 RenkBarcode= db.Stocks.FirstOrDefault(a => a.ProductId == p.Id&&a.ColorId==c.Id).RenkBarcode,
                                 ColorName = c.ColorName,
                                 ShippingId = s.Id,
                                 ColorId=t.ColorId,
                                 ProductId=t.ProductId,
                                 Count = t.Count,
                                 TotalCount = db.ShippingDetails.FirstOrDefault(a => a.ShippinbgId == s.Id).ShippingCount.Value,
                                 StockCount=st.StockCount,
                                 ModelCode=p.ModelCode
                             }).ToListAsync();
                return result;
            }


        }
    }
}
