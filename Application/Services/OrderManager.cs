using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
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
    public class OrderManager : IOrderServices
    {
        private readonly IOrderDal _orderRepository;
        public OrderManager(IOrderDal orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IResult> Add(Order order)
        {
            if (order != null)
            {
                await _orderRepository.CreateAsync(order);
                return new SuccessResult();
            }
            return new ErrorResult();
        }

        public async Task<IResult> Delete(Order order)
        {
            _orderRepository.Remove(order);
            return new SuccessResult();
        }

        public async Task<IDataResults<List<Order>>> GetAll(Expression<Func<Order, bool>> filter = null)
        {
            if (filter!=null)
            {
            return new SuccessDataResult<List<Order>>(await _orderRepository .GetAllAsync(filter), "Ürünler Listelendi");
            }
            return new SuccessDataResult<List<Order>>(await _orderRepository .GetAllAsync(), "Ürünler Listelendi");
        }

        public async Task<IDataResults<Order>> GetById(long orderId)
        {
            try
            {
                if (orderId != 0)
                {
                    return new SuccessDataResult<Order>(await _orderRepository.GetByFilterAsync(a => a.Id == orderId));
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public Task<List<OrderShippingDto>> GetOrderTenantForExcel(long tenanId, DateTime date, bool fabrikaMi = false, string gender = null)
        {
            return _orderRepository.GetOrderTenantForExcel(tenanId,date,fabrikaMi,gender);
        }

        public async Task<List<OrderShippingDto>> GetTenantKizErkek(DateTime date, string tenantName, string gender = null)
        {

            return await _orderRepository.GetTenantKizErkek(date, tenantName, gender);

        }  
        public async Task<List<OrderShippingDto>> GetTenantKizErkekToplamUrunleri(DateTime date, long tenantId, string gender = null)
        {
            return await _orderRepository.GetTenantKizErkekToplamUrunleri(date, tenantId, gender);
        }

        public Task<List<OrderShippingDto>> OrderTenantShippingDto(long tenantId)
        {
            return _orderRepository.OrderTenantShippingDto(tenantId);
        }

        public Task<List<YearMonth>> SP_GenelKazanc()
        {
            return  _orderRepository.SP_GenelKazanc();
        }

        public Task<List<OrderShippingDto>> SP_TenantShippingOrderZamanaGore(long tenantId, DateTime start, DateTime end)
        {
            return _orderRepository.SP_TenantShippingOrderZamanaGore(tenantId,start,end);
        }

        public async Task<IResult> Update(Order order)
        {
            try
            {
                if (order != null)
                {

                    var result = await _orderRepository.GetByFilterAsync(a => a.Id==order.Id);
                    _orderRepository.Update(order, result);

                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public async Task<IResult> UpdateAll(Order order)
        {
            try
            {
                if (order != null)
                {
                    //var result = await _stockRepository.GetByFilterAsync(a => a.ProductId == stock.ProductId && a.ColorId == stock.ColorId);
                    _orderRepository.UpdateAll(order);

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
