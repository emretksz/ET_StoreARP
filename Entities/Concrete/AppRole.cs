using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class AppRole:BaseEntity, IEntity
    {
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public List<AppUser> AppUser { get; set; }
    }
}
