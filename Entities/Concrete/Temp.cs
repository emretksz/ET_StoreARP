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
   public class Temp:BaseEntity,IEntity
    {
        //[ForeignKey("Shippings")]
        public long ShippigId { get; set; }
        public ICollection<Shippings> Shippings { get; set; }
        public Depocu DepocuDurumu { get; set; }
        public long Count { get; set; }
        public long ProductId { get; set; }
        public ICollection<Product> Products { get; set; }

        public long ColorId { get; set; }
        public ICollection<Color> Colors { get; set; }
        public DateTime RegisterDate { get; set; }
        //[ForeignKey("Stock")]
        //public string Price { get; set; }
        //public string BirimFiyati { get; set; }
        public bool IsComplated { get; set; }
        
        /* sipariş tamamlanınca */
        public bool IsDeleted { get; set; }
        public bool IsFinished { get; set; }

        public long StockId { get; set; }
        public ICollection<Stock> Stock { get; set; }
        ///buraya mağaza mı adında bir ayar ekliyorum eğer aktifse
        ///mağaza tenant IDsine kayıt olsun :)
        //public bool  MağazaMi { get; set; }

        public long OrderDateId { get; set; }
        public long TenantId { get; set; }
        public string RenkBarcode { get; set; }
    }
}
