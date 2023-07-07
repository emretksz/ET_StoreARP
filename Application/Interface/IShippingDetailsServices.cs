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
    public interface IShippingDetailsServices
    {
        Task<IDataResults<List<ShippingDetails>>> GetAll(Expression<Func<ShippingDetails, bool>> filter = null);
        Task<IDataResults<ShippingDetails>> GetById(long shippingDetailsId);
        Task<IResult> Add(ShippingDetails shippingDetails);
        Task<IResult> Update(ShippingDetails shippingDetails);
        Task<IResult> UpdateAll(ShippingDetails shippingDetails);
        Task<IResult> Delete(ShippingDetails shippingDetails);
        Task<ShippingDetails> QueryableId(long shippingId,long productId);
        Task<ShippingDetails> GetShippingDetailsForShippingId(long shippingId, long productId/*, long colorId*/);
        Task<List<ShippingConfirmListDto>> GetShippingConfirmList(long shippingId);
 
    }
}
