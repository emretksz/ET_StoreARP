using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUserRolServices
    {
        Task<IDataResults<List<AppRole>>> GetAll(Expression<Func<AppRole, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<AppRole>> GetById(long roleId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<IResult> Add(AppRole role);
        Task<IResult> Update(AppRole role);
        Task<IResult> UpdateAll(AppRole role);
        Task<IResult> Delete(AppRole role);
    }
}
