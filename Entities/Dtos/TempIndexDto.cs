using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class TempIndexDto
    {
        public string TenantName { get; set; }
        public string RegisterDate { get; set; }
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public long TotalCount { get; set; }
        public long ShippingCount { get; set; }
        //public long ShippingKizCount { get; set; }
        //public long ShippingErkekCount { get; set; }
        public long TenantId { get; set; }
        public long ProductId { get; set; }
        public string SiparisAdi { get; set; }
        //public Gender Gender { get; set; }
   
        public List<Shippings> Shippings { get; set; }
   
    }
}
