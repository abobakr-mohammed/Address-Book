using DataTier.Database;
using DataTier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessTier.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> Table = null;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            Table = _context.Set<T>();
        }

      

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }
        public IEnumerable<T> GetAll()
        {
            return  Table.ToList();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await Table.FindAsync(id);
            return entity;
        }

        public async Task<T> GetByNameAsync(string name)
        {
            var entity = await Table.FindAsync(name);
            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            await Table.AddAsync(entity);
            _context.SaveChanges();
            return entity;
        }
        public T Insert(T entity)
        {
             Table.Add(entity);
            _context.SaveChanges();
            return entity;
        }
        public T Update(T entity)
        {
            Table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            Table.Remove(entity);
            _context.SaveChanges();

        }


    }
}
