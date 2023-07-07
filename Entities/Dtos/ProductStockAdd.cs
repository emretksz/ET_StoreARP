using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ProductStockAdd
    {
        public long ProductId { get; set; }
        public long StockCount { get; set; }
    }
}
