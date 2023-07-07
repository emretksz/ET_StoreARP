using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TeklilerRepository : Repository<Tekliler, ET_StoreARP>, ITekliler
    {
      
        private string connectionString = new ConnectionString().ConnectionStringPath();
   turn null;
        //}

        public async Task<List<ShippingProduct>> TekliListesi()
        {

            List<ShippingProduct> SPPP = new List<ShippingProduct>();
          using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlDataReader sqlDr = null;
   
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("[dbo].[SP_Tekliler]", sqlConn);

                sqlDr = sqlCmd.ExecuteReader();


                while (sqlDr.Read())
                {
                    var tmp = new ShippingProduct();
                    tmp.ProductName = sqlDr["ModelName"].ToString();
                    tmp.ModelCode = sqlDr["ModelCode"].ToString();
                    tmp.Barcode = sqlDr["Barcode"].ToString();
                    tmp.ColorName = sqlDr["ColorName"].ToString();
                    tmp.Count = Convert.ToInt64(sqlDr["StockCount"].ToString());
                    tmp.TekliId = Convert.ToInt64(sqlDr["Id"].ToString());
                }


                return SPPP;
            }

        }

    }
}
