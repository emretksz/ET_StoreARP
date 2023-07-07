using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using static Entities.Enums.Enums;

namespace Entities.Concrete
{
    public  class Stock:BaseEntity,IEntity
    {
     
        public long ProductId { get; set; }
        public ICollection<Product> Product { get; set; }
        public long ColorId { get; set; }
        public ICollection<Color> Colors { get; set; }
        public StockStatus StockStatus { get; set; }
        public string RegisterDate { get; set; }
        public long StockCount { get; set; }
        public bool? TekliMi { get; set; }
        public long? tekliId { get; set; }
        //public long TenantId { get; set; }
        //public Tenant Tenant { get; set; }
        public Temp Temp { get; set; }
        public long? MagazaId { get; set; }
        public bool? MagazaMi { get; set; }
        public long StockTemp { get; set; }
        public string StockYear { get; set; }
        public string RenkBarcode { get; set; }

    }
}
