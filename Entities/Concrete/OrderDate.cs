using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class OrderDate:BaseEntity,IEntity
    {
        public string FaturaAdi { get; set; }
        public DateTime FaturaOlusturmaZamani { get; set; }
        //public Order Order { get; set; }
    }
}
