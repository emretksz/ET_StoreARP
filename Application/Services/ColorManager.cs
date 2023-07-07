using Application.Interface;
using Core.Utilities.Results;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Services
{
    [Authorize(Roles = "Admin" + "," + "Magaza" + "," + "Depo")]
    public class ColorManager : IColorServices
    {
        private readonly IColorDal _colorRepository;

        public ColorManager(IColorDal colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<long> Add(Color color)
        {
            try
            {
                if (color.ColorName!=null)
                {
                 var query=   await _colorRepository.CreateAsyncReturnId(color);
                    return query;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public async Task<IResult> Delete(Color color)
        {
             _colorRepository.Remove(color);
            return new SuccessResult();
        }

        public async Task<IDataResults<List<Color>>> GetAll(Expression<Func<Color, bool>> filter = null)
        {
            return new SuccessDataResult<List<Color>>(await _colorRepository.GetAllAsync(), "Renkler Geldi");

        }

        public async Task<IDataResults<Color>> GetById(long colorId)
        {
            try
            {
                if (colorId != 0)
                {
                    return new SuccessDataResult<Color>(await _colorRepository.GetByFilterAsync(a => a.Id == colorId));
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IResult> Update(Color color)
        {
            try
            {
                if (color != null)
                {
                    var result = await _colorRepository.GetByFilterAsync(a => a.Id == color.Id);
                    _colorRepository.Update(color, result);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }

        public  async Task<IResult> UpdateAll(Color color)
        {
            try
            {
                if (color != null)
                {
                    //var result = await _colorRepository.GetByFilterAsync(a => a.Id == color.Id);
                    _colorRepository.UpdateAll(color);
                    return new SuccessResult();
                }
                return new ErrorResult();
            }
            catch (Exception)
            {

                return new ErrorResult();
            }
        }
    }
}
