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
    public interface IColorServices
    {
        Task<IDataResults<List<Color>>> GetAll(Expression<Func<Color, bool>> filter = null);
        Task<IDataResults<Color>> GetById(long colorId);
        Task<long> Add(Color color);
        Task<IResult> Update(Color color);
        Task<IResult> Delete(Color color);
        Task<IResult> UpdateAll(Color color);
   
    }
}
