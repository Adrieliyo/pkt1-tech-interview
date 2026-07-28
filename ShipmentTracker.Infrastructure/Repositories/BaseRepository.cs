using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        internal AppDbContext Context;
        internal DbSet<TEntity> dbSet;

        public BaseRepository(AppDbContext context)
        {
            this.Context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            else
                return await query.ToListAsync();
        }

        public virtual async ValueTask<TEntity> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.SingleOrDefaultAsync(predicate);
        }

        public virtual async Task Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            dbSet.AttachRange(entitiesToUpdate);
            Context.Entry(entitiesToUpdate).State = EntityState.Modified;
        }
    }
}
