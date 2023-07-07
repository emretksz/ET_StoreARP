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

namespace Application.Services
{
    public class TeklilerManager : ITeklilerServices
    {
        ITekliler _teklilerDal;

        public TeklilerManager(ITekliler teklilerDal)
        {
            _teklilerDal = teklilerDal;
        }

        public async Task<long> Add(Tekliler tekliler)
        {
           var result = await _teklilerDal.CreateAsyncReturnId(tekliler);
            return result;
        }

        public async Task<IResult> Delete(Tekliler tekliler)
        {
            _teklilerDal.Remove(tekliler);
            return new SuccessResult();
        }


        public async Task<IDataResults<List<Tekliler>>> GetAll(Expression<Func<Tekliler, bool>> filter = null)
        {
            if (filter != null)
            {
                return new SuccessDataResult<List<Tekliler>>(await _teklilerDal.GetAllAsync(filter));
            }
            return new SuccessDataResult<List<Tekliler>>(await _teklilerDal.GetAllAsync());
        }

        public async Task<IDataResults<Tekliler>> GetById(long teklilerId)
        {
            return new SuccessDataResult<Tekliler>(await _teklilerDal.GetByFilterAsync(a => a.Id == teklilerId));
        }

        public async Task<IResult> Update(Tekliler tekliler)
        {
            _teklilerDal.Update(tekliler, await _teklilerDal.FindAsync(tekliler.Id));
            return new SuccessResult();
        }


        public async Task<IResult> UpdateAll(Tekliler tekliler)
        {
            _teklilerDal.UpdateAll(tekliler);
            return new SuccessResult();
        }

        public async Task<List<ShippingProduct>> TekliListesi()
        {
            return await _teklilerDal.TekliListesi();
        }
    }
}
