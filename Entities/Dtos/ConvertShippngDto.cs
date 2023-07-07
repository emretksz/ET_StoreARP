using Core.Entities;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
  
    public class ConvertShippngDto:IDto
    {
        public string shippinbgId { get; set; }
        //public string ShippingCount { get; set; }
        public string productId { get; set; }
        public string amount { get; set; }
        public string price { get; set; }
    }
}
