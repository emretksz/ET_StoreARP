using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ShippingConfirmListDto : IDto
    {

        public long ShippingId { get; set; }
        public long DetailsId { get; set; }
        public long ProductId { get; set; }
        public string ProductModel { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public long Amount { get; set; }
        public decimal Price { get; set; }
    }
}
