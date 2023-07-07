using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class IndexDto
    {
        public List<UserAndRoleDto> UserAndRoleDto { get; set; }
        public List<YearMonth> YearMonth { get; set; }
    }
}
