using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserRoleManager : IUserRolServices
    {
        private readonly IAppRoleDal _roleDal;

        public UserRoleManager(IAppRoleDal roleDal)
        {
            _roleDal = roleDal;
        }

        public async Task<IResult> Add(AppRole role)
        {
            await _roleDal.CreateAsyncReturnId(role);
            return new SuccessResult();
        }

        public async Task<IResult> Delete(AppRole role)
        {
            _roleDal.Remove(role);
            return new SuccessResult();
        }


        public async Task<IDataResults<List<AppRole>>> GetAll(Expression<Func<AppRole, bool>> filter = null)
        {
            if (filter != null)
            {
                return new SuccessDataResult<List<AppRole>>(await _roleDal.GetAllAsync(filter));
            }
            return new SuccessDataResult<List<AppRole>>(await _roleDal.GetAllAsync());
        }

        public async Task<IDataResults<AppRole>> GetById(long roleId)
        {
            return new SuccessDataResult<AppRole>(await _roleDal.GetByFilterAsync(a => a.Id == roleId));
        }

        public async Task<IResult> Update(AppRole role)
        {
            _roleDal.Update(role, await _roleDal.FindAsync(role.Id));
            return new SuccessResult();
        }


        public async Task<IResult> UpdateAll(AppRole role)
        {
            _roleDal.UpdateAll(role);
            return new SuccessResult();
        }

   
    }
}
