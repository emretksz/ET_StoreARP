using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ShippingIndexDto
    {
        public long ShippingId { get; set; }
        public long ProductId { get; set; }
        public long ColorId { get; set; }
        public long TenantId { get; set; }
        public long StockId { get; set; }
        public List<Product> Product { get; set; }
        public List<Color> Color { get; set; }
        public List<Stock> Stock { get; set; }
        public string TenantName { get; set; }
        public DateTime Registerdate { get; set; }
     
    }
}
