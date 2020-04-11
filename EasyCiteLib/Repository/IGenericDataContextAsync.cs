using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Repository
{
    public interface IGenericDataContextAsync<TEntity> where TEntity : class
    {
        DbContext Context { get; }
        DbSet<TEntity> DataSet { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }

    public class GenericDataContextAsync<TEntity> : IGenericDataContextAsync<TEntity> where TEntity : class
    {
        private DbContext _context;
        public DbContext Context
        {
            get => _context;
        }

        private DbSet<TEntity> _dataSet = null;
        public DbSet<TEntity> DataSet
        {
            get => _dataSet;
        }

        public GenericDataContextAsync(EasyCiteDbContext context)
        {
            _context = context;
            _dataSet = Context.Set<TEntity>();
        }

        public int SaveChanges() => Context.SaveChanges();
        public Task<int> SaveChangesAsync() => Context.SaveChangesAsync();
    }
}