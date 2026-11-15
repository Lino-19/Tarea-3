using GestorExcursiones.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace GestorExcursiones.Infrastructure.Core;
public class BaseRepository<T> where T : class {
 protected readonly ApplicationDbContext _context; protected readonly DbSet<T> _dbSet;
 public BaseRepository(ApplicationDbContext context){_context=context;_dbSet=context.Set<T>();}
 public virtual Task<List<T>> GetAllAsync()=>_dbSet.AsNoTracking().ToListAsync();
 public virtual ValueTask<T?> GetByIdAsync(int id)=>_dbSet.FindAsync(id);
 public virtual async Task<T> AddAsync(T entity){await _dbSet.AddAsync(entity);await _context.SaveChangesAsync();return entity;}
 public virtual async Task UpdateAsync(T entity){_dbSet.Update(entity);await _context.SaveChangesAsync();}
 public virtual async Task<bool> DeleteAsync(int id){var entity=await _dbSet.FindAsync(id);if(entity is null)return false;_dbSet.Remove(entity);await _context.SaveChangesAsync();return true;}
}
