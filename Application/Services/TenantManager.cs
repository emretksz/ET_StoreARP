using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Application.Services
{
    public class TenantManager : ITenantServices
    {
        ITenantDal _tenantDal;

        public TenantManager(ITenantDal tenantDal)
        {
            _tenantDal = tenantDal;
        }

        public async Task<IResult> Add(Tenant tenant)
        {
            await _tenantDal.CreateAsyncReturnId(tenant);
            return new SuccessResult("başarılı");
        }

        public async Task<IResult> Delete(Tenant tenant)
        {
             _tenantDal.Remove(tenant);
            return new SuccessResult("başarılı");
        }

        public async Task<List<GetShippingOrderList>> Erkek(DateTime date, string tenantName)
        {
            return await _tenantDal.Erkek(date,tenantName);
        }

        public async Task<List<StockListDto>> ErkekRenkveUrun(DateTime date, long tenantId)
        {
            return await _tenantDal.ErkekRenkveUrun(date,tenantId);
        }

        public async Task<List<GetShippingOrderList>> FabrikaErkek(DateTime date)
        {
            return await _tenantDal.FabrikaErkek(date);
        }

        public async Task<List<GetShippingOrderList>> FabrikaKiz(DateTime date)
        {
            return await _tenantDal.FabrikaKiz(date);
        }

        public async Task<IDataResults<List<Tenant>>> GetAll(Expression<Func<Tenant, bool>> filter = null)
        {
            return new SuccessDataResult<List<Tenant>>( await _tenantDal.GetAllAsync(),"Ürün getirildi");
        }

        public async Task<IDataResults<Tenant>> GetById(long tenantId)
        {
            return new SuccessDataResult<Tenant>(await _tenantDal.GetByFilterAsync(a => a.Id == tenantId));
        }

        public async Task<List<GetShippingOrderList>> Kiz(DateTime date, string tenantName)
        {
         return await _tenantDal.Kiz(date,tenantName);
        }

        public async Task<List<StockListDto>> KizRenkveUrun(DateTime date, long tenantId)
        {
            return await _tenantDal.KizRenkveUrun(date,tenantId);
        }

        public async Task<List<StockListDto>> StockList(DateTime date,string tenantName, bool gender)
        {
            return await _tenantDal.StockList(date, tenantName, gender);
        }

        public async Task<IResult> Update(Tenant tenant)
        {
            var result = await _tenantDal.GetByFilterAsync(a => a.Id == tenant.Id);
            _tenantDal.Update(tenant, result);
            return new SuccessResult();
        }

        public async Task<IResult> UpdateAll(Tenant tenant)
        {
            _tenantDal.UpdateAll(tenant);
            return new SuccessResult();
        }
    }
}
