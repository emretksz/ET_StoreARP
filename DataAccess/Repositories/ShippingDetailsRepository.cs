using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace DataAccess.Repositories
{
    public class ShippingDetailsRepository : Repository<ShippingDetails, ET_StoreARP>, IShippingDetails
    {

        public async Task<List<ShippingConfirmListDto>> GetShippingConfirmList(long shippingId)
        {

            using (ET_StoreARP db = new ET_StoreARP())
            {
                try
                {
                    var result = from a in db.ShippingDetails
                                        join b in db.Products.Include(a => a.ProductAges) on a.ProductId equals b.Id
                                        where a.ShippinbgId== shippingId
                                        select new ShippingConfirmListDto()
                                        {
                                            ShippingId = a.ShippinbgId,
                                            Age = b.ProductAges != null ? b.ProductAges.Name : "",
                                            ProductName = b.ModelName,
                                            Amount = a.Amount,
                                            Gender = ((Gender)(int)b.Gender).ToString(),
                                            Price = a.Price,
                                            ProductId = b.Id,
                                            ProductModel = b.ModelName,
                                            ProductCode=b.ModelCode,
                                            DetailsId=a.Id

                                        };
                    var reee = result.ToList();
                    return reee;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
      



        }
    }
}
