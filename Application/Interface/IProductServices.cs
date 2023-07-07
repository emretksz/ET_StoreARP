using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
   public interface IProductServices
    {
       Task<IDataResults<List<Product>>> GetAll(Expression<Func<Product, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
      Task<IDataResults<Product>> GetById(long productId);
      Task<Product> GetByProductQuery(long productId);
      Task<Product> GetProductIdByAge(long productId);
       //Task< IDataResults<List<Product>>> GetByCategorId(int id);
       Task<long> Add(Product product);
        Task<IResult> Update(Product product);
        Task<IResult> UpdateAll(Product product);
        Task<IResult> Delete(Product product);
        Task<List<Product>>GetStockAndProduct();
        Task<List<Product>> GetProductStockUpdate(long id);
        Task<List<Product>> GetProductStock(long id);
        Task<bool> AddStockCount(Stock stock);
         Task<List<Product>> OnlyMagaza(long id);
        Task<ShippingProduct> GetTekliDetay(long tekliId);
        Task<IQueryable<Product>> GetFilterProduct(SearchParticalDto p,int page,int size);
        Task<List<Product>> TekliList();
        Task<ShippingProduct> GetTekliAll(long tekliId);

    }
}
