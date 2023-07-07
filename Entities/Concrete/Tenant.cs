using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Tenant:BaseEntity,IEntity
    {
        public string TenantName { get; set; }
        public string RegisterDate { get; set; }
        public bool IsActive { get; set; }
        //[ForeignKey("Stock")]
        //public long StockId { get; set; }
        //public Stock Stock { get; set; }
        //public long ShippingsId { get; set; }
        public ICollection<Shippings> Shippings { get; set; }

        public long UserId { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }
}
