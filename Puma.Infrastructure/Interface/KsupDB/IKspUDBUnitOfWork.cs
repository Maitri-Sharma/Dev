using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IKspUDBUnitOfWork
    {
        Task Commit();

        void Rollbakc();

    }
}
