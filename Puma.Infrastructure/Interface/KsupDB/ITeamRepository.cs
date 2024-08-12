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
    public interface ITeamRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<TeamCollection> SearchTeam(string teamNavn);
        Task<TeamCollection> GetAllTeam();

        Task<List<TeamKommuneKey>> GetAllTeamKommuneKeys();
    }
}
