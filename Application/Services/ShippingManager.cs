using Application.Interface;
using AutoMapper;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class ShippingManager : IShippingServices
    {

        private readonly IProductDal _productRepository;
        private readonly StockManager _stockManager = new StockManager(new StockRepository());
        private readonly ColorManager _colorManager = new ColorManager(new ColorRepository());
       // private readonly TempManager _tempManager = new TempManager(new TempRepository());
    
        private readonly IMapper _mapper;
        private readonly IShippingDal _shippigRepository;

        public ShippingManager(IProductDal productRepository, IMapper mapper, IShippingDal shippigRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _shippigRepository = shippigRepository;
        }
        public  async Task<long> Add(Shippings shippings)
        {
           var ids= await _shippigRepository.CreateAsyncReturnId(shippings);
            
           
            return ids;
        }

        public  async Task<IResult> Delete(Shippings shippings)
        {
            try
            {
             
                _shippigRepository.Remove(shippings);
                return new SuccessResult();
            }
            catch (Exception)
            {
                return new ErrorResult(); ;
            }
        }

        public async Task<IDataResults<List<Shippings>>> GetAll(Expression<Func<Shippings, bool>> filter = null)
        {
            var result =  await _shippigRepository.GetAllAsync(filter);
            return new SuccessDataResult<List<Shippings>>(result);
        }

        public async Task<IDataResults<Shippings>> GetById(long shippingsId)
        {

            var result = await _shippigRepository.GetByFilterAsync(a=>a.Id== shippingsId);
            return new SuccessDataResult<Shippings>(result);
        }

        public async Task<List<ShippingListDto>> GetShippingDetails()
        {
            return await _shippigRepository.GetAllShippigDto();
        }

        public async Task<List<IndexShippingForJson>> IndexIcınGetir()
        {
            return  await _shippigRepository.IndexIcınGetir();
        } 
        public async Task<List<Shippings>> IndexIcınGetirMagaza()
        {
            return  await _shippigRepository.IndexIcınGetirMagaza();
        }

        public Task<List<ShippingDetails>> IndexShipping(long id)
        {
            return _shippigRepository.IndexShipping(id);
        }

        public async Task<IResult> Update(Shippings shippings)
        {

            var result = await _shippigRepository.GetByFilterAsync(a => a.Id == shippings.Id);
            _shippigRepository.Update(shippings, result);
            return new SuccessResult();
        }

        public async Task<IResult> UpdateAll(Shippings shippings)
        {
            _shippigRepository.UpdateAll(shippings);
            return new SuccessResult(); 
        }

        public async Task<List<ShippingProduct>> SiparisİicndekiRenkveMiktarlari(long shippingId, long productId)
        {
            return await _shippigRepository.SiparisİicndekiRenkveMiktarlari(shippingId, productId);
        }
        public async Task<List<ShippingDetails>> UpdateShippingAll(long shippingId)
        {
            return await _shippigRepository.UpdateShippingAll(shippingId);
        }
    }
}
