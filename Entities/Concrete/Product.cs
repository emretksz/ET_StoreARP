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
   public class Product:BaseEntity,IEntity
    {
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string ModelColor { get; set; }
        //public int Age { get{ return (int)this.Age; } set { Ages = (Age)value; } }
        //public string Gender { get; set; }
        public string ModelImageUrl { get; set; }
        public string ModelCount { get; set; }
        public string IsActive { get; set; }
        public string RegisterDate   { get; set; }
        public string Barcode   { get; set; }
        public Stock Stock { get; set; }
        //public int Gender { get { return (int)this.Gender; } set { Genders = (Gender)value; } }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        //public Age Age { get; set; }
        public ICollection<Color> Colors { get; set; }
        public ICollection<Shippings> Shippings { get; set; }
        public long ProductAgesId { get; set; }

        [ForeignKey("ProductAgesId")]
        public ProductAges ProductAges { get; set; }

    }
}
