using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Concrete
{
    public class MagazaStock:BaseEntity,IEntity
    {
        public long ProductId { get; set; }
        public ICollection<Product> Product { get; set; }
        public long ColorId { get; set; }
        public ICollection<Color> Colors { get; set; }
        public StockStatus StockStatus { get; set; }
        public string RegisterDate { get; set; }
        public long StockCount { get; set; }
        //public long TenantId { get; set; }
        //public Tenant Tenant { get; set; }
        public Temp Temp { get; set; }

    }
}
