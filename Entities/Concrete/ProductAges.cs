using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class ProductAges:BaseEntity,IEntity
    {
        public string Name { get; set; }
    }
}
