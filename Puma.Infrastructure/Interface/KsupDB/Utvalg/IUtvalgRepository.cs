using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.BusinessEntity.Utvalg;
using Puma.DataLayer.DatabaseModel;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Interface.KsupDB.Utvalg
{
    public interface IUtvalgRepository : IKsupDBGenericRepository<utvalg>
    {
        Task<bool> IsUtvalgSkrivebeskyttetInDB(int utvalgId);

        Task<Puma.Shared.Utvalg> GetUtvalgWithAllReferences(int utvalgId);

        Task<DataTable> GetUsagePattern(DateTime fromDateInclusive, DateTime toDateInclusive);

        Task<Puma.Shared.Utvalg> GetUtvalg(int utvalgId);

        Task<string> LastSavedBy(int utvalgId);

        Task<bool> UtvalgNameExists(string utvalgNavn);

        Task<UtvalgSearchCollection> SearchUtvalgSimple(string utvalgNavn, SearchMethod searchMethod);

        Task<UtvalgSearchCollection> SearchUtvalgByUserID1(string userID, SearchMethod searchMethod);

        Task<UtvalgCollection> SearchUtvalgByUserIDAndCustNo(string userID, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, int onlyBasisUtvalg);

        Task<UtvalgCollection> SearchUtvalgByUtvalgName(string utvalgNavn, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisUtvalg);

        Task<UtvalgCollection> SearchUtvalg(string utvalgNavn, SearchMethod searchMethod, bool includeReoler = true);

        Task<UtvalgCollection> SearchUtvalgByUtvalgId(int utvalgId, bool includeReols = true);

        Task<List<CampaignDescription>> GetUtvalgCampaigns(int utvalgId);

        Task<UtvalgCollection> SearchUtvalgByUtvalgIdAndCustmerNo(int utvalgId, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool includeReols, bool extendedInfo, int onlyBasisUtvalg);

        Task<UtvalgCollection> SearchUtvalgByKundeNr(string KundeNummer, SearchMethod searchMethod, bool includeReols = true, bool extendedInfo = true);

        Task<UtvalgCollection> SearchUtvalgByUtvalListId(Int64 utvalgListId, bool includeReols = true);
        Task<UtvalgCollection> SearchUtvalgByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod, bool includeReols = true);
        Task<bool> IsOrderedUtvalgAndListsReadyForUpdateRequest(DateTime deliveryDate);

        Task<bool> IsUtvalgListOrdered(long ID);
        Task<bool> IsUtvalgOrdered(long ID);

        Task<List<AutoUpdateMessage>> GetOrderedUtvalgAndListsForUpdateRequest(DateTime deliveryDate);

        Task<List<string>> GetDistinctPRS(int utvalgid);

        Task<int> GetUtvalgReolerCount(long utvalgID);
        Task<DemographyValuesCollection> GetDemographyIndexInfo(Puma.Shared.Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2);

        Task<string> GetNumberOfBudruterInTeamByTeamNR(string teamnr);

        Task<DataTable> GetNumberOfBudruterInTeam();

        Task<int> GetProbableNextUtvalgId();

        Task UpdateLogo(Int64 utvalgId, string logo, string username);

        Task UpdateUtvalgForIntegration(Puma.Shared.Utvalg utv, string username);

        Task GetUtvalgReolerForIntegration(Puma.Shared.Utvalg utv);

        Task UpdateReolMapnameForDisconnectedUtvalg(Puma.Shared.Utvalg utv, string username);

        Task UpdateWriteprotectUtvalg(Puma.Shared.Utvalg utv, string username);

        Task DeleteUtvalg(int utvalgId, string username);

        Task DeleteFromUtvalgOldReol(int utvalgId);

        Task<int> GetUtvalgListId(int utvalgId);

        Task<List<UtvalgBasisFordeling>> FindBasisUtvalgFordelingToSend();

        Task UpdateBasisUtvalgFordelingOppdatering(UtvalgBasisFordeling ubf);

        Task<string[]> UtvalgNamesExists(string[] utvalgNames);

        Task CreateBasisUtvalgFordelingOppdatering(UtvalgBasisFordeling ubf);

        Task<bool> BasisUtvalgFordelingExistsOnQue(UtvalgBasisFordeling ubf);

        Task SendBasisUtvalgFordelingToQue(UtvalgBasisFordeling ubf);

        Task<List<int>> CheckIfUtvalgListsDistributionIsToClose(int[] idsU);

        Task<List<int>> CheckIfUtvalgListsNeedOnTheFlyUpdate(IEnumerable<int> utvalgListIDs);

        Task<List<int>> FindExpiredUtvalgIDs();

        Task<List<long>> GetUtvalgReolIDs(int utvalgID);

        Task GetUtvalgReceiver(Puma.Shared.Utvalg utv);

        //Task GetUtvalgKommune(Puma.Shared.Utvalg utv);

        //Task GetUtvalgDistrict(Puma.Shared.Utvalg utv);

        //Task GetUtvalgPostalZone(Puma.Shared.Utvalg utv);

        Task GetUtvalgCriteria(Puma.Shared.Utvalg utv);

        Task GetUtvalgModifications(Puma.Shared.Utvalg utv, bool bAsc);

        Task GetUtvalgReoler(Puma.Shared.Utvalg utv);
        Task GetUtvalgOldReoler(Puma.Shared.Utvalg utv, int utvalgId);
        Task SaveUtvalgModifications(Puma.Shared.Utvalg utv, string userName, string modificationInfo = "");

        Task<string> GetUtvalgName(int utvalgId);

        Task<List<long>> GetUtvalgOldReolIDs(int utvalgId);

        Task<int> GetSequenceNextVal(string sequenceName);

        Task<int> SaveUtvalg(Puma.Shared.Utvalg utvalgData);

        Task<int> UpdateUtvalg(Puma.Shared.Utvalg utvalgData);

        Task SaveUtvalgReoler(Puma.Shared.Utvalg utv);

        Task SaveUtvalgKommuner(Puma.Shared.Utvalg utv);
        Task SaveUtvalgReceiver(Puma.Shared.Utvalg utv);
        Task SaveUtvalgDistrict(Puma.Shared.Utvalg utv);
        Task SaveUtvalgPostalZone(Puma.Shared.Utvalg utv);
        Task SaveUtvalgCriteria(Puma.Shared.Utvalg utv);
        Task SaveUtvalgAntallToInheritors(Puma.Shared.Utvalg utv);
        Task<bool> UtvalgHasRecreatedBefore(int utvalgId);
        Task SaveUtvalgPreviousReoler(Puma.Shared.Utvalg utv);
        Task UpdateOldReolMapName(Puma.Shared.Utvalg utv);

        Task UpdateDisconnectUtvalgForIntegration(Puma.Shared.Utvalg utv, string username);

        Task<long> SaveUtvalgData(Puma.Shared.Utvalg utvData, string userName, bool saveOldReoler=false, bool skipHistory=false, int forceUtvalgListId=0);

        Task<List<UtvalgIdWrapper>> GetUtvalgsToRefreshDueToUpdate(Puma.Shared.Utvalg basisUtvalg);

        //Task<Puma.Shared.Utvalg> GetUtvalgAllData(int utvalgId);

        Task SaveMultipleUtvalgData(List<Puma.Shared.Utvalg> utvsData, string userName, bool saveOldReoler = false, bool skipHistory = false, int forceUtvalgListId = 0);

        Task<List<BasicDetail>> formatReportData(Puma.Shared.Utvalg utvalg, bool showBusiness, bool showHouseholds, bool showHouseholdsReserved, string strDayDetails, int level, int uptolevel, string reportType);

        Task<bool> UtvalgsNameExists(List<string> utvalgNavns);
    }
}
