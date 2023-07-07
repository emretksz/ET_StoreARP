using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Color: BaseEntity,IEntity
    {
        public string ColorName { get; set; }
        public bool IsActive { get; set; }
       public ICollection<Product> Products { get; set; }
        public long ShippingsId { get; set; }
        public ICollection<Shippings> Shippings { get; set; }
        public Stock Stock { get; set; }
    }
}
