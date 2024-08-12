using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class KspUDBUnitOfWork : IKspUDBUnitOfWork
    {
        private readonly KspuDBContext context;
        private bool disposed;
        private Dictionary<string, object> repositories;

        public KspUDBUnitOfWork(KspuDBContext context)
        {
            this.context = context;
        }

      

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Commit()
        {
           await context.SaveChangesAsync();
        }
        public void Rollbakc()
        {
            context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public KsupDBGenericRepository<T> Repository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(KsupDBGenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);
                repositories.Add(type, repositoryInstance);
            }
            return (KsupDBGenericRepository<T>)repositories[type];
        }
    }
}

