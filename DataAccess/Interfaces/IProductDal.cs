using Core.DataAccess.EntityFramework.Repository;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface  IProductDal: IRepository<Product>
    {
       Task <List<Product>> GetStockAndProduct();
       Task<List<Product>> GetProductStockUpdate(long id);
       Task<List<Product>> GetProductStock(long id);
        Task<List<Product>> OnlyMagaza(long id);
        Task<ShippingProduct> GetTekliDetay(long tekliId);
        Task<ShippingProduct> GetTekliAll(long tekliId);
        Task<Product> GetProductIdByAge(long productId);
    }
}
