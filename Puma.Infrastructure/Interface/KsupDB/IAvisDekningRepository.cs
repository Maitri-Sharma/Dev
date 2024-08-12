using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IAvisDekningRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<bool> AvisExists(string utgave);

        Task<List<string>> GetAllUtgaver();
        Task<List<Avis>> GetPaperList();

        Task<DataTable> GetCoverageList(string[] feltnavn);

        Task<AvisDekningCollection> GetAvisDekning(long reolId);

    }
}
