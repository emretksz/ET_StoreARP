using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class IndexShippingForJson
    {
     
        public long Id { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string Adres { get; set; }
        public string SiparisAdi { get; set; }
        public string SiparisTutari { get; set; }
        public bool IsComplated { get; set; }
        public string ShippingStasus { get; set; }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        public long? ProductId { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ShippingCount { get; set; }
        public bool DepoTamam { get; set; }
        public string TenantName { get; set; }
    }
}
