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
    public interface IOrderDateServices
    {
        Task<IDataResults<List<OrderDate>>> GetAll(Expression<Func<OrderDate, bool>> filter = null);
        public Task<long> Add(OrderDate order);
        public Task<bool> Delete(OrderDate order);
        public Task<bool> Update(OrderDate order);
        Task<List<OrderPriceAndProduct>> GetMagazaFaturalari(long? orderDate = null);
        Task<List<OrderPriceAndProduct>> GetFabrikaFaturalari(long? orderDate = null);
    }
}
