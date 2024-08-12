using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IRecreateRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task CreateTable(string tablename, string basedOn = "");

        Task<bool> DropTable(string tablename);
    }
}
