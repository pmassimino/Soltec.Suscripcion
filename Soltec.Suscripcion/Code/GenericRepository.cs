using Microsoft.EntityFrameworkCore;
using Soltec.Suscripcion.Data;

namespace Soltec.Suscripcion.Service
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Save();
    }
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private SuscripcionContext _context = null;
        private DbSet<T> table = null;
        public GenericRepository()
        {
            this._context = new SuscripcionContext();
            table = _context.Set<T>();
        }
        public GenericRepository(SuscripcionContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return table;
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public void Insert(T obj)
        {
            table.Add(obj);
        }
        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
