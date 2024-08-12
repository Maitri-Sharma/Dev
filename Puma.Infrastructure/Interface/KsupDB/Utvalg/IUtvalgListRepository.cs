using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.BusinessEntity.UtvalgList;
using Puma.DataLayer.DatabaseModel;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Interface.KsupDB.Utvalg
{
    public interface IUtvalgListRepository : IKsupDBGenericRepository<utvalglist>
    {
        Task<UtvalgList> GetUtvalgListSimple(int listId);

        // Task SaveUtvalgList(UtvalgList list, string userName);
        Task<int> GetParentUtvalgListId(int utvalgListId);
        Task UpdateAntallInList(int UtvalgListId);

        Task<UtvalgsListCollection> SearchUtvalgListWithChildrenById(int utvalglistId, bool includeReols = true);
        Task AddSistOppdatertToLists(UtvalgsListCollection utvalgsListCollection);

        Task<UtvalgsListCollection> SearchUtvalgListWithChildrenByKundeNummer(string kundeNummer, SearchMethod searchMethod, bool includeReols = true);

        Task<List<long>> GetUtvalgListReolIDs(int listId);

        Task<bool> UtvalgListNameExists(string utvalglistname);

        Task<bool> ListHasMemberLists(int listID);

        Task<bool> ListHasParentList(int utvalgListId);

        Task<UtvalgsListCollection> SearchUtvalgListByUserID(string userID, SearchMethod searchMethod);

        Task<UtvalgsListCollection> SearchUtvalgListSimple(string utvalglistname, SearchMethod searchMethod);

        Task<UtvalgList> GetUtvalgList(Int64 listId, bool getParentList = true, bool getMemberUtvalg = false);

        Task<List<string>> GetDistinctPRSByListID(long utvalglistId);

        Task<UtvalgsListCollection> SearchUtvalgListSimpleById(int utvalglistid);

        Task<UtvalgsListCollection> SearchUtvalgListSimpleByIdAndCustNoAgreeNo(int utvalglistid, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisListsy, bool includeChildrenUtvalg = false);

        Task<UtvalgsListCollection> SearchUtvalgListSimpleByIDAndCustomerNo(string utvalglistname, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg);

        Task<UtvalgsListCollection> SearchUtvalgListSimpleByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod);

        Task<UtvalgReceiverList> GetUtvalgListReceivers(int listid);

        Task UpdateUtvalgListForIntegration(UtvalgList utvalgList, string username);

        Task UpdateAllowDouble(long listId, bool allowDouble);

        Task UpdateIsBasis(long listId, bool isBasis, int basedOn);

        Task<UtvalgsListCollection> GetUtvalgListSimpleByKundeNr(string KundeNummer);

        Task<List<CampaignDescription>> GetUtvalgListParentCampaigns(int listId);

        Task<List<CampaignDescription>> GetUtvalgListCampaigns(int listId);

        Task<UtvalgsListCollection> SearchUtvalgListWithoutReferences(string Utvalglistname, SearchMethod searchMethod, string customerNumber, bool canHaveEmptyCustomerNumber);

        Task<UtvalgsListCollection> SearchUtvalgListWithChildren(string Utvalglistname, SearchMethod searchMethod);

        Task<UtvalgList> GetUtvalgListWithAllReferences(int UtvalglistId);

        Task<UtvalgList> GetUtvalgListWithChildren(int listId, bool getParentListMemberUtvalg = false);

        Task DeleteCampaignList(int listId, int BasedOn);

        Task<bool> DeleteUtvalgList(int UtvalgListId, bool withChildren, string userName);

        Task<bool> CheckAndDeleteUtvalgListIfEmpty(int UtvalgListId, string userName);

        Task DeleteEmptyUtvalgLists();

        Task<int> GetNrOfDoubleCoverageReolsForList(int listId);
        Task<UtvalgsListCollection> FindUtvalgListsWhithCustomerNumberRestrictions(string listName, string customerNumber);

        Task<List<int>> GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg();

        Task<List<int>> GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer();

        Task<List<int>> GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg();

        Task<bool> IsUtvalgConnectedToList(int utvalgId);

        Task<bool> IsUtvalgConnectedToParentList(int utvalgId);

        Task<UtvalgList> GetRootUtvalgListWithAllReferences(int utvalgId);

        Task<bool> HasListDemSegUtvalgDescendant(int utvalgListId);

        Task<int> SaveUtvalgList(Puma.Shared.UtvalgList utvalglistData);

        Task<List<long>> GetIdsForFordelingToQue(Puma.Shared.UtvalgList utvalgList);

        Task<List<long>> GetListsToRefreshDueToUpdateToBasisList(Puma.Shared.UtvalgList utvalgList);
        Task<List<long>> GetListsToRefreshDueToUpdateToBasisListChild(UtvalgList utvalgList);

        Task SendBasisUtvalgFordelingToQue(List<long> ids, string utvalgType);

        Task SaveUtvalgListModifications(int listid, string info, string userName);

        Task<int> UpdateUtvalgList(Puma.Shared.UtvalgList utvalglistData);

        Task<long> SaveUtvalgListData(Puma.Shared.UtvalgList utvalglistData, string userName);

        Task<UtvalgList> GetUtvalgListNoChild(int listId);

        Task UpdateListLogo(long utvalgListId, string username, string logo);

        Task<string> GetUtvalgListName(int utvalgListId);

        Task CopySaveUtvalgListCopies(UtvalgList newUtvalgList, UtvalgList oldUtvalgList, string user, string suffix, bool forceSave, bool forceOrdreInfo);

        Task CopySaveUtvalgCopies(UtvalgList newUtvalgList, List<Puma.Shared.Utvalg> orgUtvalgList, string user, string suffix, bool forceSave, bool forceOrdreInfo);

        Task<List<long>> GetListsToRefreshDueToUpdate(Puma.Shared.UtvalgList utvalgList);

        Task<bool> DeleteUpdatedListIfEmpty(string userName, Puma.Shared.UtvalgList oldParentList);

        Task<string> CreateNewNameWithSuffixForced(string name, int maxNameLength, bool isList, List<string> newNameList, string suffix);


        Task<UtvalgsListCollection> SearchUtvalgListByIsBasis(string utvalglistname, int onlyBasisLists, SearchMethod searchMethod);

        Task UpdateAntallForList(int listId, double antall);

        Task<long> UpdateUtvalgListDistributionInfo(Puma.Shared.UtvalgList utvalglistData, string userName);

        Task<int> ChangeParentOfList(ChangeOfParentList changeOfParentList);

        Task<int> AcceptAllChangesForList(int listId, string userName);

        Task<UtvalgList> GetUtvalgListWithChildrenData(int listId, bool getParentListMemberUtvalg = false);

        List<long> GetUtvalgIdsFromListId(long listId);

        Task DeleteRoutesOfUtvalgs(string utvalgIds, string routeIds);

        Task UpdateAntallInformation(string type, long Id, Int64 antall);
    }
}
