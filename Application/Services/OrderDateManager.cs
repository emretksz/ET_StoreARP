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
    public class OrderDateManager : IOrderDateServices
    {
        IOrderDateDal _orderRepository;

        public OrderDateManager(IOrderDateDal orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<long> Add(OrderDate order)
        {
            try
            {
            return await  _orderRepository.CreateAsyncReturnId(order);
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public async Task<bool> Delete(OrderDate order)
        {
            try
            {
               _orderRepository.UpdateAll(order);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<IDataResults<List<OrderDate>>> GetAll(Expression<Func<OrderDate, bool>> filter = null)
        {
            if (filter!=null)
            {
            return new SuccessDataResult<List<OrderDate>>( await _orderRepository.GetAllAsync(filter));
            }
            return new SuccessDataResult<List<OrderDate>>(await _orderRepository.GetAllAsync());
        }

        public Task<List<OrderPriceAndProduct>> GetFabrikaFaturalari(long? orderDate = null)
        {
        return _orderRepository.GetFabrikaFaturalari(orderDate);
        }

        public Task<List<OrderPriceAndProduct>> GetMagazaFaturalari(long? orderDate = null)
        {
          return _orderRepository.GetMagazaFaturalari(orderDate);
        }

        public async Task<bool> Update(OrderDate order)
        {
            try
            {
                _orderRepository.UpdateAll(order);
                return true;
            }
            catch (Exception)
            {

                return false; ;
            }
        }
    }
}
