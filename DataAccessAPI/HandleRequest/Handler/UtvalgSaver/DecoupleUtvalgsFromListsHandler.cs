using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using static Puma.Shared.PumaEnum;
using Puma.Infrastructure.Repository.KspuDB.Utvalg;
using MediatR;
using DataAccessAPI.HandleRequest.Request.UtvalgSaver;
using DataAccessAPI.HandleRequest.Response.UtvalgSaver;
using System.Threading;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgSaver
{
    /// <summary>
    /// DecoupleUtvalgsFromListsHandler
    /// </summary>
    public class DecoupleUtvalgsFromListsHandler : IRequestHandler<RequestUtvalgSaver, string>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;


        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DecoupleUtvalgsFromListsHandler> _logger;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;


        /// <summary>
        /// Initializes a new instance of the <see cref="DecoupleUtvalgsFromListsHandler" /> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">utvalgRepository
        /// or
        /// logger
        /// or
        /// utvalgListRepository
        /// or
        /// mapper</exception>
        public DecoupleUtvalgsFromListsHandler(IUtvalgRepository utvalgRepository, ILogger<DecoupleUtvalgsFromListsHandler> logger,
              IUtvalgListRepository utvalgListRepository,
               IMapper mapper, IMediator mediator)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }


        /// <summary>
        /// Decouples the utvalgs from lists.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="skippedUtvalgs">The skipped utvalgs.</param>
        /// <param name="decoupledUtvalgs">The decoupled utvalgs.</param>
        /// <param name="listsWithRemovedUtvalgsInDb">The lists with removed utvalgs in database.</param>
        /// <param name="deletedLists">The deleted lists.</param>
        public async Task DecoupleUtvalgsFromLists(List<WorkingListEntry> entries, string userName, List<Puma.Shared.Utvalg> skippedUtvalgs, List<Puma.Shared.Utvalg> decoupledUtvalgs, List<Puma.Shared.UtvalgList> listsWithRemovedUtvalgsInDb, List<Puma.Shared.UtvalgList> deletedLists)
        {
            _logger.LogDebug("Inside into DecoupleUtvalgsFromListsHandler");
            ValidateUtvalgsToBeDecoupledFromList(entries);
            List<long> oebsNotifications = new List<long>();
            try
            {
                if (skippedUtvalgs == null) skippedUtvalgs = new List<Puma.Shared.Utvalg>();
                if (decoupledUtvalgs == null) decoupledUtvalgs = new List<Puma.Shared.Utvalg>();
                if (listsWithRemovedUtvalgsInDb == null) listsWithRemovedUtvalgsInDb = new List<Puma.Shared.UtvalgList>();
                if (deletedLists == null) deletedLists = new List<Puma.Shared.UtvalgList>();
                //First list down all disting list ids

                List<int> distinctListIds = entries.Select(x => x.UtvalgListId).Distinct().ToList();

                //Loop on distinct list to fetch all reference data\
                foreach (var itemListIds in distinctListIds)
                {
                    Puma.Shared.UtvalgList simpleList = await _utvalgListRepository.GetUtvalgListWithAllReferences(itemListIds);
                    foreach (WorkingListEntry entry in entries.Where(x => x.UtvalgListId == itemListIds && x.IsUtvalg))
                    {

                        if (!string.IsNullOrWhiteSpace(entry.Utvalg.ListId))
                        {
                            entry.Utvalg.List = simpleList;
                        }

                        if (entry.Utvalg.List == null) continue;

                        Puma.Shared.UtvalgList list = entry.Utvalg.List;
                        entry.UtvalgList = simpleList;

                        _logger.LogDebug("Inside into GetUtvalgListWithChildren");
                        list = simpleList;

                        Puma.Shared.UtvalgList parentList = list.ParentList;

                        // 2012.01.12 ekskludere Tilbud fra sjekken under, altså kun hindre dette dersom lista er knytet til Ordre.
                        //if (list.OrdreType == OrdreType.O || list.OrdreType == OrdreType.T
                        //    || (parentList != null && (parentList.OrdreType == OrdreType.O || parentList.OrdreType == OrdreType.T)))
                        if (list.OrdreType == OrdreType.O || (parentList != null && (parentList.OrdreType == OrdreType.O)))
                        {
                            skippedUtvalgs.Add(entry.Utvalg);
                            continue;
                        }

                        decoupledUtvalgs.Add(entry.Utvalg);

                        int reloadListId = list.ListId;

                        if (parentList != null)
                        {
                            listsWithRemovedUtvalgsInDb.Add(parentList);
                        }

                        entry.Utvalg.List = null;
                        entry.Utvalg.ListId = "0";
                        entry.Utvalg.ListName = "";
                        // Tilbud er åpnet opp for frikobling og må da nullstille ordreinformasjonen om et utvalg frikobles med Ordreinformasjon
                        if (entry.Utvalg.OrdreType == OrdreType.T)
                        {
                            entry.Utvalg.OrdreType = OrdreType.Null;
                            entry.Utvalg.OrdreReferanse = "";
                            entry.Utvalg.OrdreStatus = OrdreStatus.Null;
                        }
                        _logger.LogDebug("Inside into SaveUtvalg");
                        _ = await _utvalgRepository.SaveUtvalgData(entry.Utvalg, userName, true);

                        if (list.MemberLists.Count == 0 && list.MemberUtvalgs.Count == 0 && (!list.IsBasis || list.ListsBasedOnMe.Count == 0))
                        {
                            // Delete empty lists
                            _logger.LogDebug("Inside into DeleteUtvalgList");
                            bool parentWasDeleted = await _utvalgListRepository.DeleteUtvalgList(list.ListId, false, userName);
                            deletedLists.Add(list);
                            if (parentList != null)
                            {
                                if (listsWithRemovedUtvalgsInDb.Contains(parentList)) listsWithRemovedUtvalgsInDb.Remove(list);
                                parentList.MemberLists.Remove(list);
                                if (parentWasDeleted)
                                {
                                    deletedLists.Add(parentList);
                                    if (listsWithRemovedUtvalgsInDb.Contains(parentList))
                                        listsWithRemovedUtvalgsInDb.Remove(parentList);
                                }
                                else
                                {
                                    //RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(parentList);
                                    _ = await _utvalgListRepository.SaveUtvalgListData(parentList, userName);
                                    //requestSaveUtvalgList.userName = userName;
                                   // _ = await _mediator.Send(requestSaveUtvalgList);
                                    if (!listsWithRemovedUtvalgsInDb.Contains(parentList))
                                        listsWithRemovedUtvalgsInDb.Add(parentList);
                                }
                            }
                        }
                        else
                        {   // 2012.03.05 Supportsak #620279 Ergo sak 2929367  - antall blir 0 når man har med en nykopiert liste i liste... tvinger ny kalkulering for å fikse dette
                            list.Antall = list.CalculateAntall();
                            //RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(list);
                            //requestSaveUtvalgList.userName = userName;
                            //_ = await _mediator.Send(requestSaveUtvalgList);
                            _ = await _utvalgListRepository.SaveUtvalgListData(list, userName);
                            if (!listsWithRemovedUtvalgsInDb.Contains(list))
                                listsWithRemovedUtvalgsInDb.Add(list);
                            if (parentList != null)
                            {
                                // 2012.03.05 Supportsak #620279 Ergo sak 2929367  - antall blir 0 når man har med en nykopiert liste i liste... tvinger ny kalkulering for å fikse dette
                                parentList.Antall = parentList.CalculateAntall();
                                //requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(parentList);
                                //requestSaveUtvalgList.userName = userName;
                                //_ = await _mediator.Send(requestSaveUtvalgList);

                                _ = await _utvalgListRepository.SaveUtvalgListData(parentList, userName);
                                if (!listsWithRemovedUtvalgsInDb.Contains(parentList))
                                    listsWithRemovedUtvalgsInDb.Add(parentList);

                            }
                        }

                        //2012.03.05 Supportsak #620279 Ergo sak 2929367 Kopiering av lister i lister gir feil ved frakobling - list.MemberLists.Count og list.MemberUtvalgs.Count er ikke riktig uten relasting... 
                        _logger.LogDebug("Inside into GetUtvalgListWithChildren");
                        //Puma.Shared.UtvalgList nlist = await _utvalgListRepository.GetUtvalgListWithAllReferences(reloadListId);
                        // Puma.Shared.UtvalgList nparentList = await _utvalgListRepository.GetUtvalgListWithChildrenData(reloadListId);
                        //if (reloadParentListId > -1)
                        //{
                        //    Puma.Shared.UtvalgList newParentList = await _utvalgListRepository.GetUtvalgListWithChildren(reloadParentListId);
                        //}


                        Puma.Shared.UtvalgList nlist = await _utvalgListRepository.GetUtvalgListWithChildrenData(reloadListId);
                        Puma.Shared.UtvalgList nparentList = nlist.ParentList;
                        //if (reloadParentListId > -1)
                        //{
                        //    Puma.Shared.UtvalgList newParentList = await _utvalgListRepository.GetUtvalgListWithChildrenData(reloadParentListId);
                        //}

                        if (nparentList != null)
                        {
                            nparentList.Antall = nparentList.CalculateAntall();
                            //RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(nparentList);
                            //requestSaveUtvalgList.userName = userName;
                            //_ = await _mediator.Send(requestSaveUtvalgList);
                            _ = await _utvalgListRepository.SaveUtvalgListData(nparentList, userName);
                        }
                    }
                }

                #region Old working Code 
                ////foreach (WorkingListEntry entry in entries)
                ////{

                ////    if (!entry.IsUtvalg) continue;
                ////    if (!string.IsNullOrWhiteSpace(entry.Utvalg.ListId))
                ////    {
                ////        entry.Utvalg.List = await _utvalgListRepository.GetUtvalgListSimple(Convert.ToInt32(entry.Utvalg.ListId));
                ////    }
                ////    if (entry.Utvalg.List == null) continue;

                ////    Puma.Shared.UtvalgList list = entry.Utvalg.List;
                ////    entry.UtvalgList = await _utvalgListRepository.GetUtvalgListSimple(entry.UtvalgListId);
                ////    //2012.03.05 Supportsak #620279 Ergo sak 2929367 Kopiering av lister i lister gir feil ved frakobling - list.MemberLists.Count og list.MemberUtvalgs.Count er ikke riktig uten relasting...                     
                ////    _logger.LogDebug("Inside into GetUtvalgListWithChildren");
                ////    list = await _utvalgListRepository.GetUtvalgListWithAllReferences(entry.Utvalg.List.ListId);

                ////    Puma.Shared.UtvalgList parentList = list.ParentList;

                ////    // 2012.01.12 ekskludere Tilbud fra sjekken under, altså kun hindre dette dersom lista er knytet til Ordre.
                ////    //if (list.OrdreType == OrdreType.O || list.OrdreType == OrdreType.T
                ////    //    || (parentList != null && (parentList.OrdreType == OrdreType.O || parentList.OrdreType == OrdreType.T)))
                ////    if (list.OrdreType == OrdreType.O || (parentList != null && (parentList.OrdreType == OrdreType.O)))
                ////    {
                ////        skippedUtvalgs.Add(entry.Utvalg);
                ////        continue;
                ////    }
                ////    if (list.IsBasis)
                ////    {
                ////        _logger.LogDebug("Inside into GetUtvalgListCampaigns");
                ////        await GetUtvalgListCampaigns(list);
                ////        foreach (CampaignDescription c in list.ListsBasedOnMe)
                ////            if (c.OrdreType == PumaEnum.OrdreType.T)
                ////                oebsNotifications.Add(c.ID);
                ////    }
                ////    if (parentList != null && parentList.IsBasis)
                ////    {
                ////        _logger.LogDebug("Inside into GetUtvalgListCampaigns");
                ////        await GetUtvalgListCampaigns(parentList);
                ////        foreach (CampaignDescription c in parentList.ListsBasedOnMe)
                ////            if (c.OrdreType == PumaEnum.OrdreType.T)
                ////                oebsNotifications.Add(c.ID);
                ////    }
                ////    decoupledUtvalgs.Add(entry.Utvalg);

                ////    int reloadListId = list.ListId;

                ////    if (parentList != null)
                ////    {
                ////        listsWithRemovedUtvalgsInDb.Add(parentList);
                ////    }

                ////    entry.Utvalg.List = null;
                ////    entry.Utvalg.ListId = "0";
                ////    entry.Utvalg.ListName = "";
                ////    // Tilbud er åpnet opp for frikobling og må da nullstille ordreinformasjonen om et utvalg frikobles med Ordreinformasjon
                ////    if (entry.Utvalg.OrdreType == OrdreType.T)
                ////    {
                ////        entry.Utvalg.OrdreType = OrdreType.Null;
                ////        entry.Utvalg.OrdreReferanse = "";
                ////        entry.Utvalg.OrdreStatus = OrdreStatus.Null;
                ////    }
                ////    _logger.LogDebug("Inside into SaveUtvalg");
                ////    _ = await _utvalgRepository.SaveUtvalgData(entry.Utvalg,userName);

                ////    if (list.MemberLists.Count == 0 && list.MemberUtvalgs.Count == 0 && (!list.IsBasis || list.ListsBasedOnMe.Count == 0))
                ////    {
                ////        // Delete empty lists
                ////        _logger.LogDebug("Inside into DeleteUtvalgList");
                ////        bool parentWasDeleted = await _utvalgListRepository.DeleteUtvalgList(list.ListId, false, userName);
                ////        deletedLists.Add(list);
                ////        if (parentList != null)
                ////        {
                ////            if (listsWithRemovedUtvalgsInDb.Contains(parentList)) listsWithRemovedUtvalgsInDb.Remove(list);
                ////            parentList.MemberLists.Remove(list);
                ////            if (parentWasDeleted)
                ////            {
                ////                deletedLists.Add(parentList);
                ////                if (listsWithRemovedUtvalgsInDb.Contains(parentList))
                ////                    listsWithRemovedUtvalgsInDb.Remove(parentList);
                ////            }
                ////            else
                ////            {
                ////                RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(parentList);
                ////                requestSaveUtvalgList.userName = userName;
                ////                _ = await _mediator.Send(requestSaveUtvalgList);
                ////                if (!listsWithRemovedUtvalgsInDb.Contains(parentList))
                ////                    listsWithRemovedUtvalgsInDb.Add(parentList);
                ////            }
                ////        }
                ////    }
                ////    else
                ////    {   // 2012.03.05 Supportsak #620279 Ergo sak 2929367  - antall blir 0 når man har med en nykopiert liste i liste... tvinger ny kalkulering for å fikse dette
                ////        list.Antall = list.CalculateAntall();
                ////        RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(list);
                ////        requestSaveUtvalgList.userName = userName;
                ////        _ = await _mediator.Send(requestSaveUtvalgList);
                ////        if (!listsWithRemovedUtvalgsInDb.Contains(list))
                ////            listsWithRemovedUtvalgsInDb.Add(list);
                ////        if (parentList != null)
                ////        {
                ////            // 2012.03.05 Supportsak #620279 Ergo sak 2929367  - antall blir 0 når man har med en nykopiert liste i liste... tvinger ny kalkulering for å fikse dette
                ////            parentList.Antall = parentList.CalculateAntall();
                ////            requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(parentList);
                ////            requestSaveUtvalgList.userName = userName;
                ////            _ = await _mediator.Send(requestSaveUtvalgList);
                ////            if (!listsWithRemovedUtvalgsInDb.Contains(parentList))
                ////                listsWithRemovedUtvalgsInDb.Add(parentList);

                ////        }
                ////    }

                ////    //2012.03.05 Supportsak #620279 Ergo sak 2929367 Kopiering av lister i lister gir feil ved frakobling - list.MemberLists.Count og list.MemberUtvalgs.Count er ikke riktig uten relasting... 
                ////    _logger.LogDebug("Inside into GetUtvalgListWithChildren");
                ////    //Puma.Shared.UtvalgList nlist = await _utvalgListRepository.GetUtvalgListWithAllReferences(reloadListId);
                ////    Puma.Shared.UtvalgList nparentList = await _utvalgListRepository.GetUtvalgListWithChildrenData(reloadListId);
                ////    //if (reloadParentListId > -1)
                ////    //{
                ////    //    Puma.Shared.UtvalgList newParentList = await _utvalgListRepository.GetUtvalgListWithChildren(reloadParentListId);
                ////    //}

                ////    if (nparentList != null)
                ////    {
                ////        nparentList.Antall = nparentList.CalculateAntall();
                ////        RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(nparentList);
                ////        requestSaveUtvalgList.userName = userName;
                ////        _ = await _mediator.Send(requestSaveUtvalgList);
                ////    }

                ////} 
                #endregion
            }
            finally
            {
                //UtvalgServiceProxy.UtvalgServiceProxy.OppdaterUtvalgsfordelingDueToChangesInBasisUtvalg(oebsNotifications);
                _logger.LogDebug("Inside into SendBasisUtvalgFordelingToQue");
                await _utvalgListRepository.SendBasisUtvalgFordelingToQue(oebsNotifications, "L");
            }
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<string> Handle(RequestUtvalgSaver request, CancellationToken cancellationToken)
        {
            string result = "";
            List<Puma.Shared.Utvalg> skippedUtvalgs = new List<Puma.Shared.Utvalg>();
            List<Puma.Shared.Utvalg> decouppledUtvalgs = new List<Puma.Shared.Utvalg>();
            List<Puma.Shared.UtvalgList> listWithRemovedUtvalgInDb = new List<Puma.Shared.UtvalgList>();
            List<Puma.Shared.UtvalgList> deletedList = new List<Puma.Shared.UtvalgList>();

            await DecoupleUtvalgsFromLists(request.workingListEntries, request.userName, skippedUtvalgs, decouppledUtvalgs, listWithRemovedUtvalgInDb, deletedList);

            if (skippedUtvalgs?.Any() == true)
            {
                throw new Exception(string.Format(Puma.DataLayer.BusinessEntity.Constants.errMsgListeFrakoblingListInOrdreTilbud, string.Join(",  ", skippedUtvalgs.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).ToList())));
            }
            else if (decouppledUtvalgs?.Any() == true)
            {
                result = string.Format(Puma.DataLayer.BusinessEntity.Constants.MsgDisconnectUtvalgFromList, string.Join("</br> ", decouppledUtvalgs.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).ToList()));
            }

            foreach (var itemChangedUtvalg in listWithRemovedUtvalgInDb.Select(x=>x.ListId).Distinct())
            {
                Puma.Shared.UtvalgList changedList = await _utvalgListRepository.GetUtvalgListWithAllReferences(itemChangedUtvalg);
                changedList.Antall = changedList.CalculateAntall();

                //RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(changedList);
                //requestSaveUtvalgList.userName = request.userName;
                //_ = await _mediator.Send(requestSaveUtvalgList);

                _ = await _utvalgListRepository.SaveUtvalgListData(changedList, request.userName);
            }

            return result;

        }

        /// <summary>
        /// Validates the utvalgs to be decoupled from list.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        private void ValidateUtvalgsToBeDecoupledFromList(List<WorkingListEntry> entries)
        {
            _logger.LogDebug("Inside into ValidateUtvalgsToBeDecoupledFromList");
            if (entries.Count == 0)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgNoUtvalgChecked);
            if (entries.Any(e => !e.IsUtvalg))
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgNotOnlyUtvalgChecked);
            if (entries.Any(e => e.IsUtvalg && string.IsNullOrEmpty(e.Utvalg.ListId)))
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgWithoutParentListChecked);
            // Check if entries contain campaigns
            if (entries.Any(e => (e.IsUtvalg && (e.Utvalg.BasedOn > 0 || (e.Utvalg.List != null && e.Utvalg.List.BasedOn > 0))) || (e.IsUtvalgsListe && e.UtvalgList.BasedOn > 0)))
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgCantDisconnectUtvalgConnectedToBasis);
            if (!ValidateDecouplingFromBasisList(entries).Result)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgCantDisconnectFromBasisList);
        }


        /// <summary>
        /// Validates the decoupling from basis list.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns></returns>
        private async Task<bool> ValidateDecouplingFromBasisList(List<WorkingListEntry> entries)
        {
            List<Puma.Shared.UtvalgList> basisListsToCheck = new List<Puma.Shared.UtvalgList>();
            List<int> checkedIds = new List<int>();
            foreach (WorkingListEntry be in entries.Where(e => e.IsUtvalg && e.Utvalg.IsBasis && e.Utvalg.List != null && e.Utvalg.List.IsBasis))
                ValidateDecouplingFromBasisList_ProcessEntry(be.Utvalg.List, basisListsToCheck, checkedIds);
            foreach (WorkingListEntry be in entries.Where(e => e.IsUtvalgsListe && e.UtvalgList.IsBasis && e.UtvalgList.ParentList != null && e.UtvalgList.ParentList.IsBasis))
                ValidateDecouplingFromBasisList_ProcessEntry(be.UtvalgList.ParentList, basisListsToCheck, checkedIds);

            List<int> distinctListIds = basisListsToCheck.Select(x => x.ListId).Distinct().ToList();

            foreach (int basisListId in distinctListIds)
            {
                // Reread from database to make sure it has all references correct
                Puma.Shared.UtvalgList freshList = await _utvalgListRepository.GetUtvalgListWithAllReferences(basisListId);
                int utvalgCount = 0;
                int utvalgListCount = 0;
                foreach (Puma.Shared.Utvalg utv in freshList.MemberUtvalgs)
                    if (entries.Any(e => e.IsUtvalg && e.Utvalg.UtvalgId == utv.UtvalgId))
                        utvalgCount++;
                foreach (Puma.Shared.UtvalgList list in freshList.MemberLists)
                    if (entries.Any(e => e.IsUtvalgsListe && e.UtvalgList.ListId == list.ListId))
                        utvalgListCount++;
                if (freshList.MemberLists.Count == utvalgListCount && freshList.MemberUtvalgs.Count == utvalgCount)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates the decoupling from basis list process entry.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="basisListsToCheck">The basis lists to check.</param>
        /// <param name="checkedIds">The checked ids.</param>
        private void ValidateDecouplingFromBasisList_ProcessEntry(Puma.Shared.UtvalgList list, List<Puma.Shared.UtvalgList> basisListsToCheck, List<int> checkedIds)
        {
            // Only check each list once
            if (checkedIds.Contains(list.ListId)) return;
            checkedIds.Add(list.ListId);
            GetUtvalgListCampaigns(list).Wait();
            if (list.ListsBasedOnMe.Count > 0)
            {
                // Trying to disconnect a basis utvalg or list from a basis list that has campaigns - only allowed as long as we don't disconnect all utvalgs and lists from the parent list.
                basisListsToCheck.Add(list);
            }
            else if (list.ParentList != null)
            {
                GetUtvalgListCampaigns(list.ParentList).Wait();
                if (list.ParentList.ListsBasedOnMe.Count > 0)
                {
                    if (list.ParentList.MemberUtvalgs.Count == 0 && list.ParentList.MemberLists.Count == 1)
                        basisListsToCheck.Add(list); // List must not be emptied since it's parent has campaigns and has this list as its only member.
                    // This still does not cover all cases - the user could check all basisutvalgs in several basis lists that are part of the same parent list, and that could leave the parent list empty.
                    // We'll cross our fingers and assume that the user would not do this in cases where the parent list has campaigns.
                }
            }
        }

        /// <summary>
        /// Gets the utvalg list campaigns.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        private async Task GetUtvalgListCampaigns(Puma.Shared.UtvalgList utvalgList)
        {
            if (utvalgList.ListsBasedOnMe == null)
            {
                utvalgList.ListsBasedOnMe = await _utvalgListRepository.GetUtvalgListCampaigns(utvalgList.ListId);
            }
        }
    }
}



