using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class ShippingDetails : BaseEntity, IEntity
    {
        public long ShippinbgId { get; set; }
        public Shippings Shippings { get; set; }
        public long? ColorId { get; set; }
        public Color Color { get; set; }
        public long? ShippingCount { get; set; }
        public long ProductId { get; set; }
        public Product Products { get; set; }
        public bool SiparislereEklendi { get; set; }
        public bool SiparisTamamlandi { get; set; }
        public long Amount { get; set; }
        public decimal Price { get; set; }
       // public long TempShippingCount { get; set; }

    }
}

