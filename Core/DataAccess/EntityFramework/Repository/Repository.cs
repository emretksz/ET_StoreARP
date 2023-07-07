using Core.Entities.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.DataAccess.EntityFramework.Repository
{
    public class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IEntity, IBaseEntity, new()
        where TContext : DbContext, new()
    {
        //TContext _context = new TContext();
        //public TContext _context { get; }
        /*     TContext _context = new TContext()*/
        //public Repository(TContext context)
        //{
        //    _context = context;
        //}

        public async Task<List<TEntity>> GetAllAsync()
        {
            using (TContext _context = new TContext())
            {
                return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            using (TContext _context = new TContext())
            {
                return await _context.Set<TEntity>().Where(filter).AsNoTracking().ToListAsync();
            }
        }

        //public async Task<List<TEntity>> GetAllAsync<TKey>(Expression<Func<T, TKey>> selector, OrderByType orderByType = OrderByType.DESC)
        //{
        //    return orderByType == OrderByType.ASC ? await _context.Set<TEntity>().AsNoTracking().OrderBy(selector).ToListAsync() : await _context.Set<TEntity>().AsNoTracking().OrderByDescending(selector).ToListAsync();
        //}

        //public async Task<List<TEntity>> GetAllAsync<TKey>(Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> selector, OrderByType orderByType = OrderByType.DESC)
        //{
        //    return orderByType == OrderByType.ASC ? await _context.Set<TEntity>().Where(filter).AsNoTracking().OrderBy(selector).ToListAsync() : await _context.Set<TEntity>().Where(filter).AsNoTracking().OrderByDescending(selector).ToListAsync();
        //}

        public async Task<TEntity> FindAsync(object id)
        {
            using (TContext _context = new TContext())
            {
                return await _context.Set<TEntity>().FindAsync(id);
            }
        }

        public async Task<TEntity> GetByFilterAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false)
        {
            try
            {
                using (TContext _context = new TContext())
                {
                    return !asNoTracking ? await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(filter) : await _context.Set<TEntity>().FirstOrDefaultAsync(filter);

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IQueryable<TEntity> GetQuery()
        {
            //using (TContext _context = new TContext())
            //{
            //}
            TContext _context = new TContext();
                return _context.Set<TEntity>().AsQueryable();
        }

        public void Remove(TEntity entity)
        {
            using (TContext _context = new TContext())
            {
                _context.Set<TEntity>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public async Task CreateAsync(TEntity entity)
        {
            using (TContext _context = new TContext())
            {
                var added = _context.Entry(entity);
                added.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
            //await _context.Set<TEntity>().AddAsync(entity);
        }
        public async Task<long> CreateAsyncReturnId(TEntity entity)
        {
            try
            {
                using (TContext _context = new TContext())
                {
                    // var result = await _context.Set<TEntity>().AddAsync(entity);
                    var added = _context.Entry(entity);
                    added.State = EntityState.Added;
                    await _context.SaveChangesAsync();
                    return entity.Id;
                }
            }
            catch (Exception ex )
            {

                throw;
            }

        }

        public async void Update(TEntity entity, TEntity unchanged)
        {
            using (TContext _context = new TContext())
            {
                _context.Entry(unchanged).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async void UpdateAll(TEntity entity)
        {
            using (TContext _context = new TContext())
            {
                try
                {

                    var update = _context.Entry(entity);
                    update.State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
 
            //_context.Entry(unchanged).CurrentValues.SetValues(entity);
            //await _context.SaveChangesAsync();

        }
    }
}
