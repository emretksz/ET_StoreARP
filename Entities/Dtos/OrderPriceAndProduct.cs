using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class OrderPriceAndProduct
    {
        public long ShippingId { get; set; }
        public string OrderTarih { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ColorName { get; set; }
        public long Count { get; set; }
        public string BirimFiyati { get; set; }
        public string Price { get; set; }
        public string TotalPrice { get; set; }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        public string ModelCode { get; set; }
        public string TenantName { get; set; }
        public long OrderDateId { get; set; }
        public long OrderId { get; set; }
    }
}
