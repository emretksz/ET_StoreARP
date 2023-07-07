using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Concrete
{
  public  class Shippings:BaseEntity,IEntity
    {
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string Adres { get; set; }
        public string SiparisAdi { get; set; }
        public string SiparisTutari { get; set; }
        public bool IsComplated { get; set; }
        public ShippingStasus ShippingStasus { get; set; }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        public long? ProductId { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ShippingCount { get; set; }
        public bool DepoTamam { get; set; }

        //Yasemin Fiyatlandırma Yapacak!!
        //public string Price { get; set; }
        //public Temp Temp { get; set; }
        //  [ForeignKey("Temp")]
        //sipariş depoya gidecek depodan geldiğinde  başarılı olursa 
        //[ForeignKey("Product")]
        //public long ProductId { get; set; }
        //public ICollection<Product> Product { get; set; }
        //Depocuya sipariş gidecek. 
        //Siparişi onaylayıp giaytlandırmaya göndereceğinde IsComplated true olarka dönecek
        //Dönen değeregöre fiyatlandırma sayfası açılacak

        //public long ColorId { get; set; }
        //public bool ? MagazaMi { get; set; }
        //public ICollection<Color> Colors { get; set; }


    }
}
