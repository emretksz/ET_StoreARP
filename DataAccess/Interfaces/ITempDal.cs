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
    public interface  ITempDal: IRepository<Temp>
    {
        Task<List<ShippingProduct>> GetParticalShipping(long shippingId);
        Task<List<ShippingProduct>> GetOrderShippingResult(long shippingId);
        Task<List<ShippingProduct>> GetTenantShippingOrderPrice(long? tenantId = null, long? shippingId = null);
        Task<List<ShippingProduct>> GetTenantShippingOrderPriceMagaza(long? tenantId = null, long? shippingId = null);
        Task<List<Product>> GetPrepareShipping(long shippingId);
        Task<List<GetShippingOrderList>> GetShippingList(long shippingId);
        Task<ComplatedShippingExcel> ComplatedShippingForExcel(long colorId,long productId);
        Task<List<GetShippingOrderList>> GetFis(long shippingId);
      Task<List<Shippings>> GetShippingsAsyncForGrid();
        Task<List<ShippingProduct>> UpdateFinishShippingListView(long spId);
    }
}
