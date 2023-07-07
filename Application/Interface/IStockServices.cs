using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
   public interface IStockServices
    {
        Task<IDataResults<List<Stock>>> GetAll(Expression<Func<Stock, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<Stock>> GetById(long stockId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<IResult> Add(Stock stock);
        Task<IResult> Update(Stock stock);
        Task<IResult> UpdateAll(Stock stock);
        Task<IResult> Delete(Stock stock);
    }
}
