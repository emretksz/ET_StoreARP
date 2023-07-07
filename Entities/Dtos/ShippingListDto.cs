using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class ShippingListDto
    {
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Temp Temp { get; set; }
        //sipariş depoya gidecek depodan geldiğinde  başarılı olursa 
        public long ProductId { get; set; }
        public ICollection<Product> Product { get; set; }
        //Depocuya sipariş gidecek. 
        //Siparişi onaylayıp giaytlandırmaya göndereceğinde IsComplated true olarka dönecek
        //Dönen değeregöre fiyatlandırma sayfası açılacak
        public bool IsComplated { get; set; }
        public ShippingStasus ShippingStasus { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }

        //Yasemin Fiyatlandırma Yapacak!!
        public string Price { get; set; }
        public string ShippingCount { get; set; }
        public long ColorId { get; set; }
        

    }
}
