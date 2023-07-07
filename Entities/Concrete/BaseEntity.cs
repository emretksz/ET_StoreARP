using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
   public class BaseEntity:IBaseEntity
    {
        public long Id { get; set; }
    }
}
