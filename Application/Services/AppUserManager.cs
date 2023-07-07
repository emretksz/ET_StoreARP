using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AppUserManager : IAppUserServices
    {
        IAppUserDal _appUser;
        IAppRoleDal _appRole;

        public AppUserManager(IAppUserDal appUser, IAppRoleDal appRole)
        {
            _appUser = appUser;
            _appRole = appRole;
        }
        private readonly string privateKey = "uTWwq{#ZhTX&$T4mE]]O,.;C>e1;00S41cuPijS.d&dtk+";
        private readonly string oldkey = "";
        public string HtmlScriptRegex(string htmlText)
        {
            string textReplace = htmlText;
            string regexPattern = @"[<>£#$½\[\]\}\|\{]|javascript|script";
            if (Regex.IsMatch(textReplace, regexPattern, RegexOptions.IgnoreCase))
            {
                Regex specialCharsRegex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                htmlText = specialCharsRegex.Replace(textReplace, "");

            }
            return htmlText;
        }
        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{password}{salt}";
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        [Authorize(Roles = "Admin" + "," + "Magaza" )]
        public async Task<IResult> Add(AppUser user)
        {
            user.Password = HtmlScriptRegex(user.Password);
            string hashedPassword = HashPassword(user.Password, privateKey+oldkey);

            var hashPass = hashedPassword;
            user.Password = hashPass;
            await _appUser.CreateAsyncReturnId(user);

            return new SuccessResult();
        }
        public async Task<AppUser> CheckUserAsync(UserSingInModel user)
        {
            var result =await _appUser.GetByFilterAsync(a=>a.UserName==user.UserName);
            //var hashPass = user.Password + hashCode;
            string hashedPassword = HashPassword(user.Password, privateKey + oldkey);
            if (result!=null)
            {
                if (hashedPassword != result.Password)
                {
                    return null;
                }
                return result;
            }
            return null;
        }
        [Authorize(Roles = "Admin" + "," + "Magaza")]
        public async Task<IResult> Delete(AppUser user)
        {
             _appUser.Remove(user);
            return new SuccessResult();
        }
        [Authorize(Roles = "Admin" + "," + "Magaza")]
        public  async Task<IDataResults<List<AppUser>>> GetAll(Expression<Func<AppUser, bool>> filter = null)
        {
            if (filter!= null)
            {
                return new SuccessDataResult<List<AppUser>>(await _appUser.GetAllAsync(filter));
            }
            return new SuccessDataResult<List<AppUser>>(await _appUser.GetAllAsync());
        }
        [Authorize(Roles = "Admin" + "," + "Magaza")]
        public async Task<IDataResults<AppUser>> GetById(long userId)
        {
            return new SuccessDataResult<AppUser>( await _appUser.GetByFilterAsync(a=>a.Id==userId));
        }
        public async Task<string> GetRolesByUserIdAsync(long id)
        {
           var result = await _appRole.GetAllAsync();
           var result2 = await _appUser.GetAllAsync();
            var userRole = await _appUser.GetByFilterAsync(a=>a.Id==id);
            foreach (var item in result)
            {
                if (userRole.RoleId==item.Id)
                {
                    return item.RoleName;
                }

            }
            //Düzeltilecek
            return null;
        }
        public async Task<UserAndRoleDto> GetUserAndRole(long userId)
        {
            return await _appUser.GetUserAndRole(userId);
        }

        public async Task<List<UserAndRoleDto>> GetUsersAndRoles()
        {
            return await _appUser.GetUsersAndRoles(); 
        }
        [Authorize(Roles = "Admin" + "," + "Magaza")]
        public async Task<IResult> Update(AppUser user)
        {
            user.Password = HtmlScriptRegex(user.Password);
            string hashedPassword = HashPassword(user.Password, privateKey + oldkey);
            user.Password = hashedPassword;
            _appUser.Update(user, await _appUser.FindAsync(user.Id));
            return new SuccessResult();
        }
        [Authorize(Roles = "Admin" + "," + "Magaza")]
        public async  Task<IResult> UpdateAll(AppUser user)
        {
            user.Password = HtmlScriptRegex(user.Password);
            //string hash = user.Password.GetHashCode() + hashCode.GetHashCode().ToString();
            // var hashPass = user.Password + hashCode;
            string hashedPassword = HashPassword(user.Password, privateKey + oldkey);
            user.Password = hashedPassword;
            _appUser.UpdateAll(user);
            return new SuccessResult();
        }
    }
}
