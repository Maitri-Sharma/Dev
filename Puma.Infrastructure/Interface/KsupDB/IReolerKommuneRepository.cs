using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IReolerKommuneRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<ReolerKommune> GetReolerKommune(long ReolId, string KommuneId);
        Task<ReolerKommuneCollection> GetReolerKommuneByKommuneId(string KommuneId);

        Task<ReolerKommuneCollection> GetAllReolerKommune();
    }
}
