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
    public interface IKommuneRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<KommuneCollection> GetAllKommunes();

        Task<bool> KommuneExists(string Kommunenavn);

        Task<string> GetKommuneID(string Kommunenavn, string FylkeNavn);

        Task SetIsKommuneUniqueProperty(KommuneCollection kommunes);

        Task<Kommune> GetKommune(string KommuneId);

       
    }
}
