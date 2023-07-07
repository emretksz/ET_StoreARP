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
    public interface ITenantServices
    {

        Task<IDataResults<List<Tenant>>> GetAll(Expression<Func<Tenant, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<Tenant>> GetById(long tenantId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<IResult> Add(Tenant tenant);
        Task<IResult> Update(Tenant tenant);
        Task<IResult> UpdateAll(Tenant tenant);
        Task<IResult> Delete(Tenant tenant);
        Task<List<GetShippingOrderList>> FabrikaErkek(DateTime date);
        Task<List<GetShippingOrderList>> FabrikaKiz(DateTime date);
        Task<List<StockListDto>> StockList(DateTime date, string tenantName, bool gender);
        Task<List<StockListDto>> KizRenkveUrun(DateTime date, long tenantId);
        Task<List<StockListDto>> ErkekRenkveUrun(DateTime date, long tenantId);
        Task<List<GetShippingOrderList>> Kiz(DateTime date, string tenantName);
        Task<List<GetShippingOrderList>> Erkek(DateTime date, string tenantName);
    }
}
