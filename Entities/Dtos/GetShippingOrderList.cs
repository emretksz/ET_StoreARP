using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class GetShippingOrderList :IDto
    {

        public long ShippingId { get; set; }
        public long ProductId { get; set; }
        public string ModelName { get; set; }
        public string ModelCode { get; set; }
        public string Barcode { get; set; }
        public string Age { get; set; }
        public long Amount { get; set; }
        public decimal Price { get; set; }
        public Gender Gender { get; set; }
        public string  TenantName { get; set; }
        public string  ColorName { get; set; }
        public string  Adres { get; set; }
        public DateTime  RegisterDate { get; set; }

        public ShippingStasus ShippingStasus { get; set; }
        public string SiparisAdi { get; set; }
        public string SiparisTutari { get; set; }
        public string RenkBarcode { get; set; }
    }
}
