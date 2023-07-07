using Core.Entities.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class AppUser:BaseEntity, IEntity
    {
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
        public AppRole AppRole { get; set; }
    }
}
