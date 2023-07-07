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
    public class Order:BaseEntity,IEntity
    {

        [ForeignKey("shippings")]
        public long ShippingId { get; set; }
        public ICollection<Shippings>Shippings { get; set; }
        public long  ProductId { get; set; }
        public List<Product> Products { get; set; }
        public long ColorId { get; set; }
        public List<Color> Color { get; set; }
        //public string UnitePrice { get; set; }
        //public string Price { get; set; }
        public string TotalPrice { get; set; }
        public string TotalCount { get; set; }
        public Orders OrdersStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }

        /********/
        public long TenantId { get; set; }
        public long OrderDateId { get; set; }
        //public List<OrderDate> OrderDate { get; set; }
        public bool IsFinised { get; set; }
        public string ProductCount { get; set; }

        /**********/
    
    }
}
