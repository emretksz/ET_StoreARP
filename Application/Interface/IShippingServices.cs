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
    public interface IShippingServices
    {
        Task<IDataResults<List<Shippings>>> GetAll(Expression<Func<Shippings, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<Shippings>> GetById(long shippingsId);
        Task<List<ShippingListDto>> GetShippingDetails();
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<long> Add(Shippings shippings);
        Task<IResult> Update(Shippings shippings);
        Task<IResult> UpdateAll(Shippings shippings);
        Task<IResult> Delete(Shippings shippings);
      Task<List<ShippingDetails>> IndexShipping(long id);
        Task<List<IndexShippingForJson>> IndexIcınGetir();
        Task<List<Shippings>> IndexIcınGetirMagaza();
    Task<List<ShippingProduct>> SiparisİicndekiRenkveMiktarlari(long shippingId, long productId);
        Task<List<ShippingDetails>> UpdateShippingAll(long shippingId);
    }
}
