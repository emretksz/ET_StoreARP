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
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Entities.Enums.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccess.Repositories
{
    public class TenantRepository : Repository<Tenant, ET_StoreARP>, ITenantDal
    {
      
        private string connectionString = new ConnectionString().ConnectionStringPath();
        public async Task<List<TempIndexDto>> TempIndex()
        {


            List<TempIndexDto> SPPP = new List<TempIndexDto>();
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
            SqlDataReader sqlDr = null;

                sqlConn.Open();
               
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_TenantShippingCount]", sqlConn);

             
                sqlDr = sqlCmd.ExecuteReader();
                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                    var tmp = new TempIndexDto();
                    if (tenantIdTemp.Count == 0)
                    {
                        tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                        tmp.TenantId = Convert.ToInt64(sqlDr["TenantId"]);
                        tmp.TenantName = sqlDr["TenantName"].ToString();
                        tmp.ShippingCount = sqlDr["ShippingCount"].ToString()!=""?Convert.ToInt64(sqlDr["ShippingCount"]):0;
                        SPPP.Add(tmp);
                        tenantIdTemp.Add(sqlDr["TenantId"].ToString()); 
                    }
                    else
                    {
                        if (tenantIdTemp.Contains(sqlDr["TenantId"].ToString()))
                        {

                            var changeValue = SPPP.FirstOrDefault(a => a.TenantId == Convert.ToInt64(sqlDr["TenantId"]));
                            var gelen = sqlDr["ShippingCount"].ToString();
                            changeValue.ShippingCount+= sqlDr["ShippingCount"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippingCount"]) : 0;

                        }
                        else
                        {
                            tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                            tmp.TenantId = Convert.ToInt64(sqlDr["TenantId"]);
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ShippingCount = sqlDr["ShippingCount"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippingCount"]) : 0;
                            SPPP.Add(tmp);
                            tenantIdTemp.Add(sqlDr["TenantId"].ToString());
                        }
                    }

                }
           


                return SPPP.OrderBy(a=>a.TenantId).ToList();
            }

        }
        public async Task<List<TempIndexDto>> TenantShippingList(long tenantId)
        {

            List<TempIndexDto> SPPP = new List<TempIndexDto>();

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {

                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_TenantShippingList]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@TenantId", tenantId);
                sqlCmd.Parameters.Add(param);
                sqlDr = sqlCmd.ExecuteReader();
             
                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                        var tmp = new TempIndexDto();
                        tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                        tmp.TenantId = Convert.ToInt64(sqlDr["TenantId"]);
                        tmp.TenantName = sqlDr["TenantName"].ToString();
                        tmp.ShippingCount = sqlDr["ShippingCount"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippingCount"]) : 0;
                        tmp.SiparisAdi = sqlDr["SiparisAdi"].ToString() != "" ? sqlDr["SiparisAdi"].ToString() : "";
                        tmp.RegisterDate = sqlDr["RegisterDate"].ToString() != "" ? sqlDr["RegisterDate"].ToString() : "";
                        SPPP.Add(tmp);
                    
                }

                return SPPP/*.Where(a=>a.ShippingCount>0)*/.OrderBy(a => a.TenantId).ToList();
            }

        }
        public async Task<List<TempIndexDto>> GetShippingDetails(long shippingId)
        {

            List<TempIndexDto> SPPP = new List<TempIndexDto>();

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
             

                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_GetShippingDetails]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@ShippingId", shippingId);
                sqlCmd.Parameters.Add(param);
                sqlDr = sqlCmd.ExecuteReader();
                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                    var tmp = new TempIndexDto();
                    tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                    tmp.TenantId = Convert.ToInt64(sqlDr["TenantId"]);
                    tmp.TenantName = sqlDr["TenantName"].ToString();
                    tmp.ShippingCount = sqlDr["ShippingCount"].ToString() != "" ? Convert.ToInt64(sqlDr["ShippingCount"]) : 0;
                    tmp.SiparisAdi = sqlDr["SiparisAdi"].ToString() != "" ? sqlDr["SiparisAdi"].ToString() : "";
                    tmp.RegisterDate = sqlDr["RegisterDate"].ToString() != "" ? sqlDr["RegisterDate"].ToString() : "";
                    SPPP.Add(tmp);

                }

                return SPPP.OrderBy(a => a.TenantId).ToList();
            }

        }


        public async Task<List<TempIndexDto>> TempMagazaIndex()
        {
            List<TempIndexDto> SPPP = new List<TempIndexDto>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[Sp_TenantMagaza]", sqlConn);


                // Execute the command and get the data in a data reader.
                sqlDr = sqlCmd.ExecuteReader();

                // Iterate through the datareader and write the data to the console

                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                    var tmp = new TempIndexDto();
                    if (tenantIdTemp.Count == 0)
                    {
                        tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                        tmp.TenantName = sqlDr["TenantName"].ToString();
                        //tmp.TotalCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                        tmp.ShippingCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                        tmp.Shippings = new List<Shippings>
                    {
                        new Shippings
                        {
                            Id=Convert.ToInt64(sqlDr["shipping_dot_Id"]),
                                ShippingCount = sqlDr["ShippingCount"].ToString(),
                            SiparisAdi= sqlDr["SiparisAdi"].ToString()!=""?sqlDr["SiparisAdi"].ToString():"",
                        }
                    };
                        tmp.RegisterDate = sqlDr["RegisterDate"].ToString();
                        SPPP.Add(tmp);
                        tenantIdTemp.Add(tmp.Id.ToString());
                    }
                    else
                    {
                        if (tenantIdTemp.Contains(sqlDr["Id"].ToString()))
                        {
                            var r = SPPP.FirstOrDefault(a => a.Id == Convert.ToInt64(sqlDr["Id"]));
                            r.Shippings.Add(new Shippings
                            {
                                Id = Convert.ToInt64(sqlDr["shipping_dot_Id"]),
                                ShippingCount = sqlDr["ShippingCount"].ToString(),
                                SiparisAdi = sqlDr["SiparisAdi"].ToString() != "" ? sqlDr["SiparisAdi"].ToString() : "",
                            });
                            r.ShippingCount += /*(Convert.ToInt64(r.ShippingCount)+*/ Convert.ToInt64(sqlDr["ShippingCount"]);/*)).ToString();*/
                        }
                        else
                        {
                            tmp.Id = Convert.ToInt64(sqlDr["Id"]);
                            tmp.TenantName = sqlDr["TenantName"].ToString();
                            tmp.ShippingCount += Convert.ToInt64(sqlDr["ShippingCount"]);
                            tmp.Shippings = new List<Shippings>
                            {
                                new Shippings
                                {
                                    Id=Convert.ToInt64(sqlDr["shipping_dot_Id"]),
                                     ShippingCount = sqlDr["ShippingCount"].ToString(),
                                 SiparisAdi= sqlDr["SiparisAdi"].ToString()!=""?sqlDr["SiparisAdi"].ToString():"",
                                }
                            };
                            tmp.RegisterDate = sqlDr["RegisterDate"].ToString();
                            SPPP.Add(tmp);
                            tenantIdTemp.Add(tmp.Id.ToString());
                        }
                    }


                }


                return SPPP;
            }


        }


        public async Task<List<OrderShippingDetailsDto>> OrderShippingProducts(long shippinId)
        {
            List<OrderShippingDetailsDto> SPPP = new List<OrderShippingDetailsDto>();
            using (SqlConnection sqlConn= new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                // Create a command object with parameters for stored procedure
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_ShippinDetails]", sqlConn);
                //sqlCmd.Parameters.Add(new SqlParameter("@ShippingId", shippinId));
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@ShippingId", shippinId);
                sqlCmd.Parameters.Add(param);




                // Execute the command and get the data in a data reader.
                sqlDr = sqlCmd.ExecuteReader();

                // Iterate through the datareader and write the data to the console
                Console.WriteLine("\nTop 10 Customer Names:\n");
                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                    var tmp = new OrderShippingDetailsDto();
                    if (tenantIdTemp.Count == 0)
                    {
                        if (Convert.ToInt64(sqlDr["ShippingCount"]) > 0)
                        {
                            tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                            tmp.ShippingCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                            tmp.Color = new List<ColorDtoForShipping>()
                    {
                        new ColorDtoForShipping
                        {

                            ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                            ColorName = sqlDr["ColorName"].ToString(),
                            shippingColorCount =sqlDr["ShippingCount"].ToString(),
                            StockCount= (sqlDr["StockCount"].ToString()!=""?Convert.ToInt64(sqlDr["StockCount"]):0)
                        }
                    };
                            tmp.ModelName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString();
                            tmp.Gender = sqlDr["Gender"].ToString();
                            tmp.Age = sqlDr["Age"].ToString();

                            SPPP.Add(tmp);
                            tenantIdTemp.Add(sqlDr["ProductId"].ToString());
                        }

                    }
                    else
                    {
                        if (Convert.ToInt64(sqlDr["ShippingCount"]) > 0)
                        {


                            if (tenantIdTemp.Contains(sqlDr["ProductId"].ToString()))
                            {
                                var r = SPPP.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));
                                r.Color.Add(new ColorDtoForShipping
                                {

                                    ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                                    ColorName = sqlDr["ColorName"].ToString(),
                                    shippingColorCount = sqlDr["ShippingCount"].ToString(),
                                    StockCount = (sqlDr["StockCount"].ToString() != "" ? Convert.ToInt64(sqlDr["StockCount"]) : 0)
                                });
                              
                            }
                            else
                            {
                                tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                                tmp.ShippingCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                                tmp.Color = new List<ColorDtoForShipping>()
                        {
                            new ColorDtoForShipping
                            {
                                   ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                                ColorName = sqlDr["ColorName"].ToString(),
                                shippingColorCount = sqlDr["ShippingCount"].ToString(),
                                StockCount= (sqlDr["StockCount"].ToString()!=""?Convert.ToInt64(sqlDr["StockCount"]):0)
                            }
                        };
                                tmp.ModelName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString();
                                tmp.Gender = sqlDr["Gender"].ToString();
                                tmp.Age = sqlDr["Age"].ToString();
                                SPPP.Add(tmp);
                                tenantIdTemp.Add(sqlDr["ProductId"].ToString());
                            }
                        }
                    }


                }
                return SPPP.OrderByDescending(a => a.Age).ToList();
            }
         

        }


        public async Task<List<ColorDtoForShipping>> GetPrepareColorList(long shippingId,long productId)
        {
            string year = DateTime.Now.Year.ToString();
            using (ET_StoreARP context = new ET_StoreARP())
            {

                var result = await (from sp in context.ShippingDetails
                                    join p in context.Products on sp.ProductId equals p.Id
                                    join s in context.Stocks on sp.ProductId equals s.ProductId
                                    join c in context.Colors on s.ColorId equals c.Id
                                    where sp.ShippinbgId == shippingId && s.ProductId== productId&&  s.TekliMi==null&& s.tekliId==null&&s.StockYear==year/*s.ProductId == p.Id && s.ColorId == c.Id*/
                                    select new ColorDtoForShipping()
                                    {
                                        ColorId = c.Id,
                                        ColorName = c.ColorName,
                                        StockCount = s.StockCount,
                                        RenkBarcode= s.RenkBarcode!=null?s.RenkBarcode:"",
                                    }).ToListAsync();
                return result;
            }
        }
        public async Task<List<OrderShippingDetailsDto>> OrderShippingProductsMagaza(long shippinId)
        {



            List<OrderShippingDetailsDto> SPPP = new List<OrderShippingDetailsDto>();
          using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
            SqlDataReader sqlDr = null;
                sqlConn.Open();
                // Create a command object with parameters for stored procedure
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_MagazaSiparisDetayiInnerJoin]", sqlConn);
                //sqlCmd.Parameters.Add(new SqlParameter("@ShippingId", shippinId));
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@ShippingId", shippinId);
                sqlCmd.Parameters.Add(param);


                // Execute the command and get the data in a data reader.
                sqlDr = sqlCmd.ExecuteReader();

                // Iterate through the datareader and write the data to the console
                Console.WriteLine("\nTop 10 Customer Names:\n");
                List<string> tenantIdTemp = new List<string>();
                while (sqlDr.Read())
                {
                    ///***//
                    var qqq = sqlDr["ShippingCount"];
                    var qqqqq = sqlDr["StockCount"].ToString() == "" ? "0" : sqlDr["StockCount"].ToString();
                    /// Dbden null geldiliğinde kontrol etmek gerekiyor
                    //**//
                    var tmp = new OrderShippingDetailsDto();
                    if (tenantIdTemp.Count == 0)
                    {
                        if (Convert.ToInt64(sqlDr["ShippingCount"]) > 0)
                        {
                            tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                            tmp.ShippingCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                            tmp.Color = new List<ColorDtoForShipping>()
                    {
                        new ColorDtoForShipping
                        {
                            ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                            ColorName = sqlDr["ColorName"].ToString(),
                            shippingColorCount =sqlDr["ShippingCount"].ToString()!=""?sqlDr["ShippingCount"].ToString():"0",
                            StockCount=sqlDr["StockCount"].ToString()!=""?Convert.ToInt64(sqlDr["StockCount"]):0
                        }
                    };
                            tmp.ModelName = sqlDr["ModelName"].ToString();
                            tmp.ModelCode = sqlDr["ModelCode"].ToString();
                            tmp.Gender = sqlDr["Gender"].ToString();
                            tmp.Age = sqlDr["Age"].ToString();
                            tmp.ImageUrl = sqlDr["ModelImageUrl"].ToString() != "" ? sqlDr["ModelImageUrl"].ToString() : "";
                            SPPP.Add(tmp);
                            tenantIdTemp.Add(sqlDr["ProductId"].ToString());
                        }

                    }
                    else
                    {
                        if (Convert.ToInt64(sqlDr["ShippingCount"]) > 0)
                        {


                            if (tenantIdTemp.Contains(sqlDr["ProductId"].ToString()))
                            {
                                var r = SPPP.FirstOrDefault(a => a.ProductId == Convert.ToInt64(sqlDr["ProductId"]));
                                r.Color.Add(new ColorDtoForShipping
                                {
                                    ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                                    ColorName = sqlDr["ColorName"].ToString(),
                                    shippingColorCount = sqlDr["ShippingCount"].ToString() != "" ? sqlDr["ShippingCount"].ToString() : "0",
                                    StockCount = sqlDr["StockCount"].ToString() != "" ? Convert.ToInt64(sqlDr["StockCount"]) : 0
                                });
                                /*)).ToString();*/
                                //r.ShippingCount = /*(Convert.ToInt64(r.ShippingCount)+*/ Convert.ToInt64(sqlDr["ShippingCount"]);/*)).ToString();*/
                                //SPPP.Add(tmp);
                            }
                            else
                            {
                                tmp.ProductId = Convert.ToInt64(sqlDr["ProductId"]);
                                tmp.ShippingCount = Convert.ToInt64(sqlDr["ShippingCount"]);
                                tmp.Color = new List<ColorDtoForShipping>()
                        {
                            new ColorDtoForShipping
                            {
                                ColorId = Convert.ToInt64(sqlDr["ColorId"]),
                                ColorName = sqlDr["ColorName"].ToString(),
                                shippingColorCount = sqlDr["ShippingCount"].ToString() != "" ? sqlDr["ShippingCount"].ToString() : "0",
                                StockCount = sqlDr["StockCount"].ToString() != "" ? Convert.ToInt64(sqlDr["StockCount"]) : 0
                            }
                        };
                                tmp.ModelName = sqlDr["ModelName"].ToString();
                                tmp.ModelCode = sqlDr["ModelCode"].ToString();
                                tmp.Gender = sqlDr["Gender"].ToString();
                                tmp.Age = sqlDr["Age"].ToString();
                                tmp.ImageUrl = sqlDr["ModelImageUrl"].ToString() != "" ? sqlDr["ModelImageUrl"].ToString() : "";
                                SPPP.Add(tmp);
                                tenantIdTemp.Add(sqlDr["ProductId"].ToString());
                            }
                        }
                    }


                }
                return SPPP;
            }

          
        }




        public async Task<List<GetShippingOrderList>> FabrikaKiz(DateTime date)
        {

            List<GetShippingOrderList> SPPP = new List<GetShippingOrderList>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_FabrikaKiz]", sqlConn);
                sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlDr = sqlCmd.ExecuteReader();
                while (sqlDr.Read())
                {
                    GetShippingOrderList temp = new GetShippingOrderList();
                    temp.ModelCode = sqlDr["ModelCode"].ToString();
                    temp.ModelName = sqlDr["ModelName"].ToString();
                    temp.Age= sqlDr["Name"].ToString();
                    temp.ColorName= sqlDr["ColorName"].ToString();
                    temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                    temp.Amount = sqlDr["StockTemp"].ToString() != "" ? Convert.ToInt64(sqlDr["StockTemp"]) : 0;
                    SPPP.Add(temp);
                }
                return SPPP.OrderByDescending(a => a.Age).ToList();
            }
        }

        public async Task<List<GetShippingOrderList>> FabrikaErkek(DateTime date)
        {

            List<GetShippingOrderList> SPPP = new List<GetShippingOrderList>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_FabrikaErkek]", sqlConn);
                sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlDr = sqlCmd.ExecuteReader();

                while (sqlDr.Read())
                {
                    GetShippingOrderList temp = new GetShippingOrderList();
                    temp.ModelCode = sqlDr["ModelCode"].ToString();
                    temp.ModelName = sqlDr["ModelName"].ToString();
                    temp.Age = sqlDr["Name"].ToString();
                    temp.ColorName = sqlDr["ColorName"].ToString();
                    temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                    temp.Amount = sqlDr["StockTemp"].ToString() != "" ? Convert.ToInt64(sqlDr["StockTemp"]) : 0;
                    SPPP.Add(temp);

                }
                return SPPP.OrderByDescending(a => a.Age).ToList();
            }
        }
        ////////
        /// <summary>
        /// 
        /// 
        /// 
        /// 

        public async Task<List<GetShippingOrderList>> Kiz(DateTime date, string tenantName)
        {

            List<GetShippingOrderList> SPPP = new List<GetShippingOrderList>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    SqlDataReader sqlDr = null;
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_TenantKiz]", sqlConn);
                    sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                    sqlCmd.Parameters.Add("@TenantName", SqlDbType.VarChar).Value = tenantName;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlDr = sqlCmd.ExecuteReader();

                    while (sqlDr.Read())
                    {
                        GetShippingOrderList temp = new GetShippingOrderList();
                        temp.ModelCode = sqlDr["ModelCode"].ToString();
                        temp.ModelName = sqlDr["ModelName"].ToString();
                        temp.Age = sqlDr["Name"].ToString();
                        temp.ColorName = sqlDr["ColorName"].ToString();
                        temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                        temp.Amount = sqlDr["StockTemp"].ToString() != "" ? Convert.ToInt64(sqlDr["StockTemp"]) : 0;
                        SPPP.Add(temp);

                    }
                    return SPPP.OrderByDescending(a => a.Age).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<List<GetShippingOrderList>> Erkek(DateTime date, string tenantName)
        {

            List<GetShippingOrderList> SPPP = new List<GetShippingOrderList>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    SqlDataReader sqlDr = null;
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_TenantErkek]", sqlConn);
                    sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                    sqlCmd.Parameters.Add("@TenantName", SqlDbType.VarChar).Value = tenantName;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlDr = sqlCmd.ExecuteReader();

                    while (sqlDr.Read())
                    {
                        GetShippingOrderList temp = new GetShippingOrderList();
                        temp.ModelCode = sqlDr["ModelCode"].ToString();
                        temp.ModelName = sqlDr["ModelName"].ToString();
                        temp.Age = sqlDr["Name"].ToString();
                        temp.ColorName = sqlDr["ColorName"].ToString();
                        temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                        temp.Amount = sqlDr["StockTemp"].ToString() != "" ? Convert.ToInt64(sqlDr["StockTemp"]) : 0;
                        SPPP.Add(temp);

                    }
                    return SPPP.OrderByDescending(a => a.Age).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        /// 
        /// 

        public async Task<List<StockListDto>> StockList(DateTime date,string tenantName,bool gender)
        {
            
            List<StockListDto> SPPP = new List<StockListDto>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                if (gender)
                {
                    sqlCmd.CommandText = "[dbo].[SP_StockCount_Erkek]";
                    sqlCmd.Connection = sqlConn;
                }
                else
                {
                    sqlCmd.CommandText = "[dbo].[SP_StockCount_Kiz]";
                    sqlCmd.Connection = sqlConn;
                }
                sqlCmd.CommandType = CommandType.StoredProcedure;
               // SqlParameter param = new SqlParameter("@Date", date);
                sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                sqlCmd.Parameters.Add("@TenantName", SqlDbType.VarChar).Value = tenantName;
        

                sqlDr = sqlCmd.ExecuteReader();

                try
                {
                    while (sqlDr.Read())
                    {
                        StockListDto temp = new StockListDto();
                        temp.ModelCode = sqlDr["ModelCode"].ToString();
                        temp.ModelName = sqlDr["ModelName"].ToString();
                        temp.Name = sqlDr["Name"].ToString();
                        temp.Gender = ((Gender)(Convert.ToInt32( sqlDr["Gender"].ToString()))).ToString();
                        temp.Renk = sqlDr["ColorName"].ToString();
                        temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                        temp.Miktar = sqlDr["StockCount"].ToString() != "" ? Convert.ToInt64(sqlDr["StockCount"]) : 0;
                        SPPP.Add(temp);

                    }
                    return SPPP.OrderByDescending(a=>a.Name).ToList();
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }

        public async Task<List<StockListDto>> ErkekRenkveUrun(DateTime date,long tenantId)
        {

            List<StockListDto> SPPP = new List<StockListDto>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_FirmaErkekRenkveUrun]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                // SqlParameter param = new SqlParameter("@Date", date);
                sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;


                sqlDr = sqlCmd.ExecuteReader();

                try
                {
                    while (sqlDr.Read())
                    {
                        StockListDto temp = new StockListDto();
                        if (SPPP.Count==0)
                        {
                            temp.ModelCode = sqlDr["ModelCode"].ToString();
                            temp.ModelName = sqlDr["ModelName"].ToString();
                            temp.Name = sqlDr["Name"].ToString();
                            temp.Gender = ((Gender)(Convert.ToInt32(sqlDr["Gender"].ToString()))).ToString();
                            temp.Renk = sqlDr["ColorName"].ToString();
                            temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                            temp.Miktar = sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                            temp.Counts = sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0;
                            SPPP.Add(temp);
                        }
                        else
                        {
                            var check = SPPP.FirstOrDefault(a=>a.Renk == sqlDr["ColorName"].ToString()&& a.ModelCode== sqlDr["ModelCode"].ToString());
                            if (check != null)
                            {
                                check.Miktar = check.Miktar + sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                                check.Counts = check.Counts + (sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0);


                            }
                            else
                            {
                                temp.ModelCode = sqlDr["ModelCode"].ToString();
                                temp.ModelName = sqlDr["ModelName"].ToString();
                                temp.Name = sqlDr["Name"].ToString();
                                temp.Gender = ((Gender)(Convert.ToInt32(sqlDr["Gender"].ToString()))).ToString();
                                temp.Renk = sqlDr["ColorName"].ToString();
                                temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                                temp.Miktar = sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                                temp.Counts = sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0;

                                SPPP.Add(temp);

                            }
                                
                        }
                    

                    }
                    return SPPP.OrderByDescending(a => a.Name).ToList() ;
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }

        public async Task<List<StockListDto>> KizRenkveUrun(DateTime date,long tenantId)
        {

            List<StockListDto> SPPP = new List<StockListDto>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_FirmaKizRenkveUrun]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                // SqlParameter param = new SqlParameter("@Date", date);
                sqlCmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                sqlCmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;


                sqlDr = sqlCmd.ExecuteReader();

                try
                {
                    while (sqlDr.Read())
                    {
                        StockListDto temp = new StockListDto();
                        if (SPPP.Count == 0)
                        {
                            temp.ModelCode = sqlDr["ModelCode"].ToString();
                            temp.ModelName = sqlDr["ModelName"].ToString();
                            temp.Name = sqlDr["Name"].ToString();
                            temp.Gender = ((Gender)(Convert.ToInt32(sqlDr["Gender"].ToString()))).ToString();
                            temp.Renk = sqlDr["ColorName"].ToString();
                            temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                            temp.Miktar = sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                            temp.Counts = sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0;
                            SPPP.Add(temp);
                        }
                        else
                        {
                            var check = SPPP.FirstOrDefault(a => a.Renk == sqlDr["ColorName"].ToString() && a.ModelCode == sqlDr["ModelCode"].ToString());
                            if (check != null)
                            {
                                check.Miktar = check.Miktar + sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                                check.Counts = check.Counts + (sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0);


                            }
                            else
                            {
                                temp.ModelCode = sqlDr["ModelCode"].ToString();
                                temp.ModelName = sqlDr["ModelName"].ToString();
                                temp.Name = sqlDr["Name"].ToString();
                                temp.Gender = ((Gender)(Convert.ToInt32(sqlDr["Gender"].ToString()))).ToString();
                                temp.Renk = sqlDr["ColorName"].ToString();
                                temp.RenkBarcode = sqlDr["RenkBarcode"].ToString();
                                temp.Miktar = sqlDr["RenkMiktari"].ToString() != "" ? Convert.ToInt64(sqlDr["RenkMiktari"]) : 0;
                                temp.Counts = sqlDr["Counts"].ToString() != "" ? Convert.ToInt64(sqlDr["Counts"]) : 0;

                                SPPP.Add(temp);

                            }

                        }


                    }
                    return SPPP.OrderByDescending(a => a.Name).ToList();
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }
    }
}
