using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class OrderShippingDetailsDto
    {
        public long ProductId { get; set; }
        public long ShippingCount { get; set; }
        //public long ColorId { get; set; }
        //public string ColorName { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string ImageUrl { get; set; }
        public List<ColorDtoForShipping> Color { get; set; }
        public int pageCount { get; set; }

    }

    public  class    ColorDtoForShipping{
        public long ColorId { get; set; }
        public string ColorName { get; set; }
        public string shippingColorCount { get; set; }
        public long StockCount { get; set; }
        public string RenkBarcode { get; set; }
    }
}
