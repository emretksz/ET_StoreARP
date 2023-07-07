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
    public interface  IOrderDal: IRepository<Order>
    {
        Task<List<OrderShippingDto>> OrderTenantShippingDto(long tenantId);
        Task<List<OrderShippingDto>> SP_TenantShippingOrderZamanaGore(long tenantId, DateTime start, DateTime end);
        Task<List<YearMonth>> SP_GenelKazanc();
        Task<List<OrderShippingDto>> GetOrderTenantForExcel(long tenanId, DateTime date, bool fabrikaMi = false, string gender = null);
        Task<List<OrderShippingDto>> GetTenantKizErkek(DateTime date, string tenantName, string gender = null);
        Task<List<OrderShippingDto>> GetTenantKizErkekToplamUrunleri(DateTime date, long tenantId, string gender = null);
    }
}
