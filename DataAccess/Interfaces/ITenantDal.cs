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
    public interface  ITenantDal: IRepository<Tenant>
    {
        Task<List<TempIndexDto>> TempIndex();
        Task<List<TempIndexDto>> TenantShippingList(long tenantId);
        Task<List<TempIndexDto>> TempMagazaIndex();
        Task<List<OrderShippingDetailsDto>> OrderShippingProducts(long shippinId);
        Task<List<OrderShippingDetailsDto>> OrderShippingProductsMagaza(long shippinId);
        Task<List<ColorDtoForShipping>> GetPrepareColorList(long shippingId, long productId);
        Task<List<GetShippingOrderList>> FabrikaErkek(DateTime date);
        Task<List<GetShippingOrderList>> FabrikaKiz(DateTime date);
        Task<List<StockListDto>> StockList(DateTime date, string tenantName, bool gender);
        Task<List<StockListDto>> KizRenkveUrun(DateTime date, long tenantId);
        Task<List<StockListDto>> ErkekRenkveUrun(DateTime date, long tenantId);
        Task<List<GetShippingOrderList>> Kiz(DateTime date, string tenantName);
        Task<List<GetShippingOrderList>> Erkek(DateTime date, string tenantName);
        }
}
