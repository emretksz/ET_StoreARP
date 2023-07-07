using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAppUserServices
    {
        Task<IDataResults<List<AppUser>>> GetAll(Expression<Func<AppUser, bool>> filter = null);
        //IDataResults<List<UserDetailsDto>> GetUserDetails(int id);
        Task<IDataResults<AppUser>> GetById(long userId);
        //Task< IDataResults<List<Product>>> GetByCategorId(int id);
        Task<IResult> Add(AppUser user);
        Task<IResult> Update(AppUser user);
        Task<IResult> UpdateAll(AppUser user);
        Task<IResult> Delete(AppUser user);
        Task<AppUser> CheckUserAsync(UserSingInModel user);
        Task<string> GetRolesByUserIdAsync(long id);
        Task<UserAndRoleDto> GetUserAndRole(long userId);
        Task<List<UserAndRoleDto>> GetUsersAndRoles();
    }
}
