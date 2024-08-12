using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.AddressPoints;
using DataAccessAPI.HandleRequest.Response.ArbeidsListState;
using DataAccessAPI.HandleRequest.Response.AvisDekning;
using DataAccessAPI.HandleRequest.Response.ByDel;
using DataAccessAPI.HandleRequest.Response.Fylke;
using DataAccessAPI.HandleRequest.Response.Kapasitet;
using DataAccessAPI.HandleRequest.Response.Kommune;
using DataAccessAPI.HandleRequest.Response.Reol;
using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using DataAccessAPI.HandleRequest.Response.Team;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using DataAccessAPI.Models;
using Puma.DataLayer.BusinessEntity.EC_Data;
using Puma.Shared;

namespace DataAccessAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //// Add as many of these lines as you need to map your objects
            CreateMap<AddressPoint, ResponseAddressPointState>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailById>();
            CreateMap<UtvalgSearchResult, ResponseSearchUtvalgByName>();
            CreateMap<UtvalgSearchResult, ResponseSearchUtvalgByUserId>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailByUserIdAndCustNo>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailByNameAndCustNo>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailByName>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailById>();
            CreateMap<CampaignDescription, ResponseGetUtlvagCampaignsByUtvalgId>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagDetailByUtvalgIdAndCustNo>();
            CreateMap<Puma.Shared.Utvalg, ResponseSearchUtvalgByKundeNr>();
            CreateMap<Puma.Shared.Utvalg, ResponseSearchUtvalgByOrdreReferanse>();
            CreateMap<Puma.Shared.AutoUpdateMessage, ResponseOrderedUtvalgAndListsForUpdate>();
            CreateMap<Puma.Shared.UtvalgBasisFordeling, ResponseUtvalgBasisFordeling>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetUtlvagByUtvalgId>();
            CreateMap<RequestUpdateBasisUtvalgFordelingOppdatering, UtvalgBasisFordeling>();
            CreateMap<RequestCreateBasisUtvalgFordelingOppdatering, UtvalgBasisFordeling>();
            CreateMap<RequestBasisUtvalgFordelingExistsOnQue,UtvalgBasisFordeling>();
            CreateMap<Puma.Shared.Utvalg, ResponseSearchUtvalgByUtvalListId>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgListSimple>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListByUserID>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgListNoChild>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimpleById>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimpleByIDAndCustomerNo>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimpleByOrdreReferanse>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListWithChildrenById>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListWithChildrenByKundeNummer>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListWithChildren>();
            CreateMap<Puma.Shared.UtvalgList, ResponseSearchUtvalgListSimple>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithChildren>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgListSimpleByKundeNr>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgList>();
            CreateMap<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithAllReferences>();
            CreateMap<Puma.Shared.UtvalgList, RequestSaveUtvalgList>();
            CreateMap<ResponseSearchUtvalgListSimpleById,Puma.Shared.UtvalgList>();
            CreateMap<ResponseSaveUtvalg, Puma.Shared.Utvalg>();


            CreateMap<Puma.Shared.UtvalgReceiverList, ResponseGetUtvalgListReceivers>();
            CreateMap<Puma.Shared.CampaignDescription, ResponseGetUtvalgListParentCampaigns>();
            CreateMap<Puma.Shared.Utvalg, ResponseSaveUtvalg>();
            CreateMap<ArbeidsListEntryState, ResponseArbeidsListEntryState>();
            CreateMap<Avis, ResponseAvis>();
            CreateMap<ResponseAvisDekning, AvisDekning>();
            CreateMap<AvisDekning, ResponseAvisDekning>();
            CreateMap<Bydel, ResponseGetAllBydels>();
            CreateMap<ResponseGetAllBydels, Bydel>();
            CreateMap<Fylke, ResponseGetAllFylkes>();
            CreateMap<ResponseGetAllFylkes, Fylke>();
            CreateMap<Fylke, ResponseGetFylkes>();
            CreateMap<Team, ResponseSearchTeam>();
            CreateMap<Team, ResponseGetAllTeam>();
            CreateMap<TeamKommuneKey, ResponseGetAllTeamKommuneKeys>();
            CreateMap<ResponseGetAllTeamKommuneKeys, TeamKommuneKey>();
            CreateMap<KapasitetDato, ResponseGetKapasitetDatoer>();
            CreateMap<LackingCapacity, ResponseGetDatesLackingCapacity>();
            CreateMap<KapasitetRuter, ResponseGetRuterLackingCapacity>();
            CreateMap<Kommune, ResponseGetAllKommunes>();
            CreateMap<ResponseGetAllKommunes, Kommune>();
            CreateMap<Kommune, ResponseGetKommune>();
            CreateMap<ReolerKommune, ResponseGetReolerKommune>();
            CreateMap<ResponseGetReolerKommune, ReolerKommune>();
            CreateMap<ReolerKommune, ResponseGetReolerKommuneByKommuneId>();
            CreateMap<ResponseGetReolerKommuneByKommuneId, ReolerKommune>();
            CreateMap<ReolerKommune, ResponseGetAllReolerKommune>();
            CreateMap<ResponseGetAllReolerKommune, ReolerKommune>();
            CreateMap<RequestSaveUtvalgList, Puma.Shared.UtvalgList>();
            CreateMap<Reol, ResponseGetReolsInFylke>();
            CreateMap<Reol, ResponseGetReolsInKommune>();
            CreateMap<Reol, ResponseGetReolsByTeamNr>();
            CreateMap<Reol, ResponseGetReolsInTeam>();
            CreateMap<Reol, ResponseGetReolsInPostNr>();
            CreateMap<Reol, ResponseGetReolFromReolID>();
            CreateMap<Reol, ResponseSearchReolByReolName>();
            CreateMap<Reol, ResponseSearchReolPostboksByReolName>();
            CreateMap<Reol, ResponseGetReolsFromReolIDString>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetReolerBySegmenterSearch>();
            CreateMap<Puma.Shared.Utvalg, ResponseGetReolerByDemographySearch>();
            CreateMap<Reol, ResponseGetReolerByKommuneSearch>();
            CreateMap<Reol, ResponseGetReolerByFylkeSearch>();
            CreateMap<Reol, ResponseGetReolerByTeamSearch>();
            CreateMap<Reol, ResponseGetReolerByPostalZoneSearch>();
            CreateMap<Reol, ResponseGetReolerByPostboksSearch>();

            //For EC
            CreateMap<OrdreStatusRequestEntity, OrdreStatusRequest>();
            CreateMap<OrdreStatusDataEntity, OrdreStatusData>();
            CreateMap<UtvalgsIdEntity, Models.UtvalgsId>();

            CreateMap<RequestCreateCampaignList, RequestSaveUtvalgList>();
            CreateMap<ResponseSearchUtvalgListSimpleById, ResponseCreateCampaignList>();

            CreateMap<Puma.Shared.UtvalgList, ResponseAddUtvalgsToNewList>();
            CreateMap<Puma.Shared.Utvalg, RequestSaveUtvalg>();
            CreateMap<Puma.Shared.UtvalgList, RequestSaveUtvalgList>();
            CreateMap<Puma.Shared.CampaignDescription, ResponseGetUtvalgListCampaigns>();

            CreateMap<Puma.Shared.UtvalgList, ResponseDisconnectList>();

            CreateMap<Puma.Shared.UtvalgList, ResponseCreateCopyOfUtalgList>();

            CreateMap<Puma.Shared.UtvalgList, RequestSaveUtvalgList>();

            CreateMap<RequestSaveUtvalgListDistributionData, Puma.Shared.UtvalgList>();

        }
    }
}
