using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Database;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(CinemaContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }    

    public async Task<IEnumerable<T>> GetCollectionAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;
        
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }
        
        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        
        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id, string includeProperties = "")
    {
        var result = await _dbSet.FindAsync(id);
        foreach(var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            await _context.Entry(result).Reference(property).LoadAsync();
        }

        return result;
    }

    public async Task<T> AddAsync(T entity)
    {
        //TODO: when adding an Entity with an existing navigation property it removes this navigation property from the existing attached entity and
        //attaches it to the added one. This has to be fixed!!!
        var result = _dbSet.Add(entity).Entity;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var result = _dbSet.Update(entity).Entity;            
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task DeleteAsync(int id)
    {
        var entityToRemove = await _dbSet.FindAsync(id);
        _dbSet.Remove(entityToRemove);

        await _context.SaveChangesAsync();
    }
}
