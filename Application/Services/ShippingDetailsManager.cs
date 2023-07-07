using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    internal class ShippingDetailsManager: IShippingDetailsServices
    {
        private readonly IShippingDetails _shippingRepository;

        public ShippingDetailsManager(IShippingDetails shippingRepository)
        {
            //_mapper = mapper;
          
            _shippingRepository = shippingRepository;
        }
        public async Task<IResult> Add(ShippingDetails shippingDetails)
        {
            try
            {
                if (shippingDetails != null)
                {
                    await _shippingRepository.CreateAsyncReturnId(shippingDetails);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IResult> Delete(ShippingDetails shippingDetails)
        {
            try
            {
                if (shippingDetails != null)
                {
                    _shippingRepository.Remove(shippingDetails);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IDataResults<List<ShippingDetails>>> GetAll(Expression<Func<ShippingDetails, bool>> filter = null)
        {
            return new SuccessDataResult<List<ShippingDetails>>(await _shippingRepository.GetAllAsync(filter), "Sipariş Eklendi");
        }

        public async Task<IDataResults<ShippingDetails>> GetById(long shippingDetailsId)
        {
            if (shippingDetailsId != 0)
            {
                return new SuccessDataResult<ShippingDetails>(await _shippingRepository.GetByFilterAsync(a => a.Id == shippingDetailsId));
            }
            return new ErrorDataResult<ShippingDetails>("Sipariş bulunamadı");
        }

        public async Task<List<ShippingConfirmListDto>> GetShippingConfirmList(long shippingId)
        {
           return await _shippingRepository.GetShippingConfirmList(shippingId);
        }

        public async Task<ShippingDetails> GetShippingDetailsForShippingId(long shippingId, long productId/*, long colorId*/)
        {
            return await _shippingRepository.GetByFilterAsync(a=>a.ShippinbgId==shippingId&&a.ProductId==productId/*&&a.ColorId== colorId*/);
        }

        public async Task<ShippingDetails> QueryableId(long shippingId,long productId)
        {
           var result =  _shippingRepository.GetQuery();
          var output = await result.Where(a => a.ShippinbgId == shippingId&&a.ProductId== productId).FirstOrDefaultAsync();
            return output;
        }

        public async Task<IResult> Update(ShippingDetails shippingDetails)
        {
            try
            {
                var shippingDetailspRepo = await _shippingRepository.GetByFilterAsync(a => a.Id == shippingDetails.Id);
                _shippingRepository.Update(shippingDetails, shippingDetailspRepo);
                return new SuccessResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IResult> UpdateAll(ShippingDetails shippingDetails)
        {
            try
            {
                _shippingRepository.UpdateAll(shippingDetails);
                return new SuccessResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }
      
    }
}
