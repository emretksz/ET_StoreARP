using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StockRepository:Repository<Stock,ET_StoreARP>,IStockDal
    {
        //public StockRepository(ET_StoreARP context):base(context)
        //{

        //}
    }
}
