using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.Enums;

namespace Entities.Dtos
{
    public class CreateProductDto
    {
        public long ProductId { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string ModelColor { get; set; }
        public int Age { get; set; }
        public int Gender { get; set ; }
        public string ModelImageUrl { get; set; }
        public string ModelCount { get; set; }
        public string IsActive { get; set; }
        public string RegisterDate { get; set; }
        public string Barcode { get; set; }
        public long ProductAgesId { get; set; }
        public List<ColorDto> ColorDto { get; set; }
        public Stock Stock { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        public ICollection<Shippings> Shippings { get; set; }
    }
    public class ColorDto
    {
        public string ColorName { get; set; }
        public string Count { get; set; }
        public string Barcode { get; set; }
    }
}
