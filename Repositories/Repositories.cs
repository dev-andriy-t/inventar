using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RetailingApp.Entities;

namespace RetailingApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }
    }

    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetAllWithDetails();
        IEnumerable<Product> GetProductsByCategory(int categoryId);
    }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Product> GetAllWithDetails()
        {
            return _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Manufacturer)
                .Include(p => p.Location)
                .ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Categories)       
                .Include(p => p.Manufacturer)
                .Include(p => p.Location)
                .Where(p => p.Categories.Any(c => c.Id == categoryId))
                .ToList();
        }

    }
}
