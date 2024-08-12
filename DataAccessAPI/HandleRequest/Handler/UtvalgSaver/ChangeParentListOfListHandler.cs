using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using static Puma.Shared.PumaEnum;
using MediatR;
using System.Threading;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using Newtonsoft.Json;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using Puma.DataLayer.BusinessEntity.UtvalgList;
using System.Linq;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgSaver
{
    /// <summary>
    /// ChangeParentListOfListHandler
    /// </summary>
    public class ChangeParentListOfListHandler : IRequestHandler<RequestChangeParentListOfList, (ResponseGetUtvalgListWithAllReferences utvalgListData, string message)>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;



        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ChangeParentListOfListHandler> _logger;


        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeParentListOfListHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The Mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// utvalgRepository
        /// or
        /// logger
        /// or
        /// Mapper
        /// </exception>
        public ChangeParentListOfListHandler(IUtvalgListRepository utvalgListRepository, ILogger<ChangeParentListOfListHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Exception">
        /// Basislister kan bare kobles til basislister.
        /// or
        /// Utvalgslister kan ikke kobles til basislister. De må først konverteres til basislister.
        /// or
        /// or
        /// </exception>
        public async Task<(ResponseGetUtvalgListWithAllReferences utvalgListData, string message)> Handle(RequestChangeParentListOfList request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Request for ChangeParentListOfList" + JsonConvert.SerializeObject(request));
            ResponseGetUtvalgListWithAllReferences utvalgListData = null;
            if (request.DeletedLists == null)
            {
                request.DeletedLists = new List<Puma.Shared.UtvalgList>();
            }

            Puma.Shared.UtvalgList oldParentList = request.ListToBeChanged.ParentList;
            string message = string.Empty;

            if (request.NewParentList == null)
            {
                request.ListToBeChanged.ParentList = request.NewParentList;
                message = Puma.DataLayer.BusinessEntity.Constants.MsgSaveUtvalgList.Replace(@"*", request.ListToBeChanged.Name);

                //if (request.ListToBeChanged.OrdreType == OrdreType.T)
                //{
                request.ListToBeChanged.OrdreReferanse = null;
                request.ListToBeChanged.OrdreStatus = OrdreStatus.Null;
                request.ListToBeChanged.OrdreType = OrdreType.Null;

                //foreach (Puma.Shared.Utvalg utv in request.ListToBeChanged.GetAllUtvalgs())
                //{
                //    utv.OrdreReferanse = null;
                //    utv.OrdreStatus = OrdreStatus.Null;
                //    utv.OrdreType = OrdreType.Null;
                //    RequestSaveUtvalg requestSave = new RequestSaveUtvalg();
                //    requestSave.utvalg = utv;
                //    requestSave.userName = request.UserName;
                //    _ = _mediator.Send(requestSave);

                //}


                ChangeOfParentList changeOfParentList = new ChangeOfParentList()
                {
                    Antall = -(request.ListToBeChanged.Antall),
                    ListId = request.ListToBeChanged.ListId,
                    NewOrderReference = null,
                    NewOrderStattus = OrdreStatus.Null,
                    NewOrderType = OrdreType.Null,
                    OrderType = request.ListToBeChanged.OrdreType,
                    ParentListId = 0

                };
                _ = await _utvalgListRepository.ChangeParentOfList(changeOfParentList);
                //}
                utvalgListData = _mapper.Map<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithAllReferences>(request.ListToBeChanged);
            }
            else
            {

                if (request.ListToBeChanged.IsBasis && !request.NewParentList.IsBasis)
                {
                    throw new Exception(@"Basislister kan bare kobles til basislister.");
                }

                if (!request.ListToBeChanged.IsBasis && request.NewParentList.IsBasis)
                {
                    throw new Exception(@"Utvalgslister kan ikke kobles til basislister. De må først konverteres til basislister.");
                }

                if (await _utvalgListRepository.ListHasParentList(request.NewParentList.ListId))
                {
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgListHasParentList);
                }

                if (AreSameList(request.ListToBeChanged, request.NewParentList))
                {
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgIsSameLists);
                }

                Puma.Shared.UtvalgList parentList = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.NewParentList.ListId);
                message = Puma.DataLayer.BusinessEntity.Constants.MsgConnectListInList.Replace(@"*", request.NewParentList.Name);

                if (!parentList.HasUtvalgListAsDescendant(request.ListToBeChanged.ListId))
                {
                    //request.ListToBeChanged.ParentList = parentList;

                    //if ((request.ListToBeChanged.OrdreType == OrdreType.T) || (request.NewParentList.OrdreType == OrdreType.T))
                    //{
                    //    request.ListToBeChanged.OrdreReferanse = request.NewParentList.OrdreReferanse;
                    //    request.ListToBeChanged.OrdreStatus = request.NewParentList.OrdreStatus;
                    //    request.ListToBeChanged.OrdreType = request.NewParentList.OrdreType;

                    //    foreach (Puma.Shared.Utvalg utv in request.ListToBeChanged.GetAllUtvalgs())
                    //    {
                    //        utv.OrdreReferanse = request.NewParentList.OrdreReferanse;
                    //        utv.OrdreStatus = request.NewParentList.OrdreStatus;
                    //        utv.OrdreType = request.NewParentList.OrdreType;

                    //        RequestSaveUtvalg requestSave = new RequestSaveUtvalg();
                    //        requestSave.utvalg = utv;
                    //        requestSave.userName = request.UserName;
                    //        _ = _mediator.Send(requestSave);
                    //        //UtvalgSaver.SaveUtvalg(utv, request.userName);
                    //    }
                    //}
                    ChangeOfParentList changeOfParentList = new ChangeOfParentList()
                    {
                        Antall = request.ListToBeChanged.Antall,
                        ListId = request.ListToBeChanged.ListId,
                        NewOrderReference = request.NewParentList.OrdreReferanse,
                        NewOrderStattus = request.NewParentList.OrdreStatus,
                        NewOrderType = request.NewParentList.OrdreType,
                        OrderType = request.ListToBeChanged.OrdreType,
                        ParentListId = request.NewParentList.ListId

                    };
                    _ = await _utvalgListRepository.ChangeParentOfList(changeOfParentList);

                    ReolCollection doubleCoverage = parentList.GetDoubleCoverage(request.ListToBeChanged.GetAllUtvalgs());
                    if (doubleCoverage.Count > 0)
                    {
                        message = Puma.DataLayer.BusinessEntity.Constants.infoMsgDobbeldekning.Replace(@"*", doubleCoverage.Count.ToString()) + message;
                    }

                    //UtvalgServiceProxy.UtvalgServiceProxy.OppdaterUtvalgsfordelingDueToChangesInBasisUtvalg(
                    //    BasisUtvalgListManager.GetListsToRefreshDueToUpdate(parentList));
                    await _utvalgListRepository.SendBasisUtvalgFordelingToQue(await _utvalgListRepository.GetListsToRefreshDueToUpdate(parentList), "L");
                    request.ListToBeChanged.OrdreType = request.NewParentList.OrdreType;
                    request.ListToBeChanged.OrdreReferanse = request.NewParentList.OrdreReferanse;
                    request.ListToBeChanged.OrdreStatus = request.NewParentList.OrdreStatus;
                    if (request.ListToBeChanged.MemberUtvalgs?.Any() == true)
                    {
                        request.ListToBeChanged.MemberUtvalgs.ForEach(x => { x.OrdreStatus = request.NewParentList.OrdreStatus; x.OrdreReferanse = request.NewParentList.OrdreReferanse; x.OrdreType = request.NewParentList.OrdreType; });
                    }
                    if (parentList.MemberLists?.Any() == false)
                        parentList.MemberLists = new List<Puma.Shared.UtvalgList>();

                    parentList.MemberLists.Add(request.ListToBeChanged);

                    utvalgListData = _mapper.Map<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithAllReferences>(parentList);

                }
            }

            //Int64 utvalglistId = await _utvalgListRepository.SaveUtvalgListData(request.ListToBeChanged, request.UserName);
            //if (request.NewParentList == null)
            //{
            //    utvalglistId = request.ListToBeChanged.ListId;
            //}

            if (await _utvalgListRepository.DeleteUpdatedListIfEmpty(request.UserName, oldParentList))
            {
                request.DeletedLists.Add(oldParentList);
            }




            //if (utvalglistId > 0)
            //{


            //}
            return (utvalgListData, message);
        }

        /// <summary>
        /// Ares the same list.
        /// </summary>
        /// <param name="list1">The list1.</param>
        /// <param name="list2">The list2.</param>
        /// <returns></returns>
        private bool AreSameList(Puma.Shared.UtvalgList list1, Puma.Shared.UtvalgList list2)
        {
            return list1 == null && list2 == null || list1 != null && list2 != null && list1.ListId == list2.ListId;
        }



    }
}
