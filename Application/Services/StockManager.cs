using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class StockManager : IStockServices
    {
        private readonly IStockDal _stockRepository;
        public StockManager(IStockDal stockRepository=null)
        {
            _stockRepository = stockRepository;
        }

        public async Task<IResult> Add(Stock stock)
        {
            if (stock != null)
            {
                await _stockRepository.CreateAsync(stock);
                return new SuccessResult();
            }
            return new ErrorResult();
        }

        public  async Task<IResult> Delete(Stock stock)
        {
            _stockRepository.Remove(stock);
            return new SuccessResult();
        }

        public async Task<IDataResults<List<Stock>>> GetAll(Expression<Func<Stock, bool>> filter = null)
        {
            return new SuccessDataResult<List<Stock>>(await _stockRepository.GetAllAsync(filter), "Ürünler Listelendi");
        }

        public async Task<IDataResults<Stock>> GetById(long stockId)
        {
            try
            {
                if (stockId != 0)
                {
                    return new SuccessDataResult<Stock>(await _stockRepository.GetByFilterAsync(a => a.Id == stockId));
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<IResult> Update(Stock stock)
        {
            try
            {
                if (stock != null)
                {
                   
                    var result = await _stockRepository.GetByFilterAsync(a => a.ProductId == stock.ProductId&&a.ColorId==stock.ColorId);
                    _stockRepository.Update(stock, result);
        
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public  async Task<IResult> UpdateAll(Stock stock)
        {
            try
            {
                if (stock != null)
                {
                    //var result = await _stockRepository.GetByFilterAsync(a => a.ProductId == stock.ProductId && a.ColorId == stock.ColorId);
                    _stockRepository.UpdateAll(stock);

                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }
    }
}
