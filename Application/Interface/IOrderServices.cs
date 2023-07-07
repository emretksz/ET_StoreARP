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
    public interface IOrderServices
    {
   
        public Task<IDataResults<List<Order>>> GetAll(Expression<Func<Order, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        public Task<IDataResults<Order>> GetById(long orderId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        public Task<IResult> Add(Order order);
        public Task<IResult> Update(Order order);
        public Task<IResult> UpdateAll(Order order);
        public Task<IResult> Delete(Order order);
        public Task<List<OrderShippingDto>> OrderTenantShippingDto(long tenantId);
        public Task<List<OrderShippingDto>> SP_TenantShippingOrderZamanaGore(long tenantId, DateTime start, DateTime end);
         public Task<List<YearMonth>> SP_GenelKazanc();
      public  Task<List<OrderShippingDto>> GetOrderTenantForExcel(long tenanId, DateTime date, bool fabrikaMi = false, string gender = null);
       public Task<List<OrderShippingDto>> GetTenantKizErkek(DateTime date, string tenantName, string gender = null);
        public Task<List<OrderShippingDto>> GetTenantKizErkekToplamUrunleri(DateTime date, long tenantId, string gender = null);
    }
}
