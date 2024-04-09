using Application.Persistence;
using Infrasctructure.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable? repositores;
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw new Exception("Error en transacción", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (repositores is null)
            {
                repositores = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!repositores.Contains(type))
            {
                var repositoryType = typeof(RepositoryBase<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                repositores.Add(type, repositoryInstance);
            }

            return (IAsyncRepository<TEntity>)repositores[type]!;
        }
    }
}
