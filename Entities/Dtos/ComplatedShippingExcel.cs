using Core.Entities;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ComplatedShippingExcel:IDto
    {
   
        public Product  Product { get; set; }
        public string Renk { get; set; }
        public long Adet { get; set; }

    }
}
