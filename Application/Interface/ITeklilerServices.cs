using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ITeklilerServices
    {
        Task<IDataResults<List<Tekliler>>> GetAll(Expression<Func<Tekliler, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<Tekliler>> GetById(long tekliId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<long> Add(Tekliler tekliler);
        Task<IResult> Update(Tekliler tekliler);
        Task<IResult> UpdateAll(Tekliler tekliler);
        Task<IResult> Delete(Tekliler tekliler);
       Task<List<ShippingProduct>> TekliListesi();
    }
}
