using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB.Reol
{
    public interface IReolRepository : IKsupDBGenericRepository<utvalg>
    {
        Task<bool> TableExists(string tableName);
        Task<Puma.Shared.Reol> GetReol(long reolId, PumaEnum.NotFoundAction actionIfNotFound = PumaEnum.NotFoundAction.ThrowException, bool getAvisDekning = true, string reolTableName = "");

        Task<ReolCollection> GetAllReolsFromTable(string tableName);
        Task<List<long>> GetAllReolIds();

        Task<List<int>> GetAllReolIDSASObjectIds(string WhereClause);

        Task<ReolCollection> GetReolsInFylke(string fylkeId);

        Task<ReolCollection> GetReolsInKommune(string kommuneId);

        Task<ReolCollection> GetReolsByTeamNr(string teamNr);

        Task<ReolCollection> GetReolsInTeam(List<string> teamName);

        Task<ReolCollection> GetReolsInPostNr(string postnummer);

        Task<ReolCollection> GetReolsFromReolIDs(long[] ReolIDs);

        Task<ReolCollection> SearchReolByReolName(string reolName);

        Task<ReolCollection> GetReolsByNameSearch(string reolName);

        Task<ReolCollection> SearchReolPostboksByReolName(string reolName, string kommuneName);

        Task<ReolCollection> GetReolsPostboksByNameSearch(string reolName, string kommuneName);

        Task<ReolCollection> GetReolsFromReolIDString(string ids);

        Task<Puma.Shared.Utvalg> GetReolerBySegmenterSearch(DemographyOptions options);

        Task<ReolCollection> GetReolerByKommuneSearch(string kummuneIder);

        Task<ReolCollection> GetReolerByFylkeSearch(string fylkeIder);

        Task<ReolCollection> GetReolerByTeamSearch(string teamNames);

        Task<ReolCollection> GetReolerByPostalZoneSearch(string postalZone);

        Task<ReolCollection> GetReolerByPostboksSearch(string postboks);

        Task<List<long>> ValidateAndGetNewPIBs(long oldReolID);

        Task<Puma.Shared.Reol> GetReolFromReolCollectionByReolId(long reolid, ReolCollection reolData, PumaEnum.NotFoundAction actionIfNotFound = PumaEnum.NotFoundAction.ThrowException, bool getAvisDekning = true);

        Task<AntallInformation> GetAntallFromDataRow(DataRow row);

        Task<Puma.Shared.Utvalg> GetReolerByDemographySearch(DemographyOptions options, Puma.Shared.Utvalg utvalg, bool isFromKundeweb = false);

        Task<Puma.Shared.Utvalg> GetReolerByIndex_DemographySearch(Puma.Shared.Utvalg utvalg, DemographyOptions options);

        Task<Puma.Shared.Utvalg> GetReolerByIndexOrg_DemographySearch(Puma.Shared.Utvalg utvalg, DemographyOptions options);

        Task<Puma.Shared.Reol> GetReolFromReolID(long reolID, bool getAvisDekning = true);

        Task<ReolCollection> GetOrCreateReolCache(string tableName);
    }

}
