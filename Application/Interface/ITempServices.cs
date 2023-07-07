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
    public  interface ITempServices
    {
        Task<IDataResults<List<Temp>>> GetAll(Expression<Func<Temp, bool>> filter = null);
        Task<IDataResults<Temp>> GetById(long tempId);
        Task<IResult> Add(Temp temp);
        Task<IResult> Update(Temp temp);
        Task<IResult> UpdateAll(Temp temp);
        Task<IResult> Delete(Temp temp);
        Task<List<TempIndexDto>> TempIndex();
        Task<List<TempIndexDto>> TenantShippingList(long tenantId);
        Task<List<TempIndexDto>> TempMagazaIndex();
        Task<List<OrderShippingDetailsDto>> OrderShippingProducts(long shippinId);
        Task<List<OrderShippingDetailsDto>> OrderShippingProductsMagaza(long shippinId);
        Task<List<ShippingProduct>> GetParticalShipping(long shippingId);
        Task<List<ShippingProduct>> GetOrderShippingResult(long shippingId);
        Task<List<ShippingProduct>> GetTenantShippingOrderPrice(long? tenantId = null, long? shippingId = null);
        Task<List<ShippingProduct>> GetTenantShippingOrderPriceMagaza(long? tenantId = null, long? shippingId = null);
        Task<List<Product>> GetPrepareShipping(long shippingId);
        Task<List<ColorDtoForShipping>> GetPrepareColorList(long shippingId, long productId);
        Task<List<GetShippingOrderList>> GetShippingList(long shippingId);
        Task<ComplatedShippingExcel> ComplatedShippingForExcel(long colorId, long productId);
        Task<List<GetShippingOrderList>> GetFis(long shippingId);
        Task<List<Shippings>> GetShippingsAsyncForGrid();
        Task<List<ShippingProduct>> UpdateFinishShippingListView(long spId);
        Task<bool> UpdateFinishShippingPost(List<ShippingProduct> list);

        //Task<List<ShippingProduct>> GetTenantShippingOrderPrice(long? tenantId = null, long? shippingId = null);


    }
}
