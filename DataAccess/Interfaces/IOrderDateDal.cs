using Core.DataAccess.EntityFramework.Repository;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IOrderDateDal:IRepository<OrderDate>
    {
        Task<List<OrderPriceAndProduct>> GetMagazaFaturalari(long? orderDate = null);
        Task<List<OrderPriceAndProduct>> GetFabrikaFaturalari(long? orderDate = null);
      
    }
}
