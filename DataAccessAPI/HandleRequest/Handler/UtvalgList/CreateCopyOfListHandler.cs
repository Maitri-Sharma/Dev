using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// CreateCopyOfListHandler
    /// </summary>
    public class CreateCopyOfListHandler : IRequestHandler<RequestCreateCopyOfList, ResponseCreateCopyOfUtalgList>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveUtvalgListHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCopyOfListHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// logger
        /// or
        /// mapper
        /// or
        /// mediator
        /// </exception>
        public CreateCopyOfListHandler(IUtvalgListRepository utvalgListRepository, ILogger<SaveUtvalgListHandler> logger, IMapper mapper, IMediator mediator)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Exception">Please pass valid list id</exception>
        public async Task<ResponseCreateCopyOfUtalgList> Handle(RequestCreateCopyOfList request, CancellationToken cancellationToken)
        {

            var utvalgListData = await _utvalgListRepository.GetUtvalgListWithAllReferences(Convert.ToInt32(request.ListId));

            if (utvalgListData == null)
            {
                throw new Exception("Please pass valid list id");

            }

            RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(utvalgListData);

            requestSaveUtvalgList.Name = request.ListName;
            requestSaveUtvalgList.KundeNummer = request.kundeNumber;
            requestSaveUtvalgList.ParentListId = 0;
            requestSaveUtvalgList.userName = request.userName;
            requestSaveUtvalgList.ListId = 0;
            requestSaveUtvalgList.IsBasis = false;
            requestSaveUtvalgList.OrdreType = OrdreType.Null;
            requestSaveUtvalgList.OrdreReferanse = string.Empty;
            requestSaveUtvalgList.Thickness = 0;
            requestSaveUtvalgList.Weight = 0;
            requestSaveUtvalgList.BasedOn = 0;
            var newListData = await _mediator.Send(requestSaveUtvalgList);

            Puma.Shared.UtvalgList listData = _mapper.Map<ResponseSearchUtvalgListSimpleById, Puma.Shared.UtvalgList>(newListData);
            listData.MemberUtvalgs = new List<Puma.Shared.Utvalg>();

            string prefix = "_" + request.CustomText;
            if (request.SelectionCriteria == SelectionCriteria.Date)
            {
                prefix = "_" + DateTime.Now.ToString("ddMMyy");
            }

            else if (request.SelectionCriteria == SelectionCriteria.DateTime)
            {
                prefix = "_" + DateTime.Now.ToString("ddMMyy") + "_" + DateTime.Now.ToString("HH:mm");

            }

            //Create new selection of each existin selection
            #region Old Code to save selections
            //foreach (var itemUtvalg in utvalgListData?.MemberUtvalgs)
            //{
            //    string utvalgName = await _utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

            //    RequestSaveUtvalg requestSaveUtvalg = new RequestSaveUtvalg();
            //    requestSaveUtvalg.utvalg = itemUtvalg;
            //    requestSaveUtvalg.utvalg.Name = utvalgName;
            //    requestSaveUtvalg.utvalg.KundeNummer = request.kundeNumber;
            //    requestSaveUtvalg.utvalg.ListId = newListData.ListId.ToString();
            //    requestSaveUtvalg.userName = request.userName;
            //    requestSaveUtvalg.utvalg.UtvalgId = 0;
            //    var utvalgData = await _mediator.Send(requestSaveUtvalg);

            //    listData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));
            //} 
            #endregion


            if (utvalgListData?.MemberUtvalgs?.Any() == true)
            {
                RequestListSaveUtvalgs requestListSaveUtvalgs = new RequestListSaveUtvalgs();
                requestListSaveUtvalgs.utvalgs = new List<Puma.Shared.Utvalg>();
                foreach (var itemUtvalg in utvalgListData?.MemberUtvalgs)
                {
                    string utvalgName = await _utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

                    Puma.Shared.Utvalg utvalg = new();
                    utvalg = itemUtvalg;
                    utvalg.Name = utvalgName;
                    utvalg.KundeNummer = request.kundeNumber;
                    utvalg.ListId = newListData.ListId.ToString();
                    utvalg.UtvalgId = 0;
                    utvalg.IsBasis = false;
                    utvalg.Thickness = 0;
                    utvalg.Weight = 0;
                    utvalg.OrdreType = OrdreType.Null;
                    utvalg.OrdreReferanse = string.Empty;
                    utvalg.BasedOn = 0;
                    //var utvalgData = await _mediator.Send(requestSaveUtvalg);
                    requestListSaveUtvalgs.utvalgs.Add(utvalg);
                    //listData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));
                }
                requestListSaveUtvalgs.userName = request.userName;

                List<ResponseSaveUtvalg> lstUtvalgResponse = await _mediator.Send(requestListSaveUtvalgs);

                listData.MemberUtvalgs.AddRange(_mapper.Map<List<ResponseSaveUtvalg>, List<Puma.Shared.Utvalg>>(lstUtvalgResponse));
            }

            //Create Member list
            if (utvalgListData.MemberLists?.Any() == true)
            {
                listData.MemberLists = new List<Puma.Shared.UtvalgList>();
                foreach (var item in utvalgListData.MemberLists)
                {
                    listData.MemberLists.Add(await MemberListCreation(item, request.kundeNumber, prefix, newListData.ListId, request.userName));
                }
            }


            //After everything is saved update its parent antall
            await _utvalgListRepository.UpdateAntallInList(newListData.ListId);

            //var getNewListData = await _utvalgListRepository.GetUtvalgListWithAllReferences(newListData.ListId);

            return _mapper.Map<Puma.Shared.UtvalgList, ResponseCreateCopyOfUtalgList>(listData);



        }

        /// <summary>
        /// Members the list creation.
        /// </summary>
        /// <param name="utvalgList">The utvalg list.</param>
        /// <param name="kundeNumber">The kunde number.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="ParentListId">The parent list identifier.</param>
        /// <param name="userName">Name of the user.</param>
        public async Task<Puma.Shared.UtvalgList> MemberListCreation(Puma.Shared.UtvalgList utvalgList, string kundeNumber, string prefix, int ParentListId, string userName)
        {
            Puma.Shared.UtvalgList memberListData = new Puma.Shared.UtvalgList();

            string utvalgListName = await _utvalgListRepository.CreateNewNameWithSuffixForced(utvalgList.Name, 50, true, new List<string>(), prefix);

            RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(utvalgList);
            requestSaveUtvalgList.Name = utvalgListName;
            requestSaveUtvalgList.KundeNummer = kundeNumber;
            requestSaveUtvalgList.ParentListId = ParentListId;
            requestSaveUtvalgList.userName = userName;
            requestSaveUtvalgList.ListId = 0;
            requestSaveUtvalgList.IsBasis = false;
            requestSaveUtvalgList.OrdreType = OrdreType.Null;
            requestSaveUtvalgList.OrdreReferanse = string.Empty;
            requestSaveUtvalgList.Thickness = 0;
            requestSaveUtvalgList.Weight = 0;
            requestSaveUtvalgList.BasedOn = 0;
            var listData = await _mediator.Send(requestSaveUtvalgList);

            memberListData = _mapper.Map<ResponseSearchUtvalgListSimpleById, Puma.Shared.UtvalgList>(listData);
            memberListData.MemberUtvalgs = new List<Puma.Shared.Utvalg>();



            if (utvalgList?.MemberUtvalgs?.Any() == true)
            {
                RequestListSaveUtvalgs requestListSaveUtvalgs = new RequestListSaveUtvalgs();
                requestListSaveUtvalgs.utvalgs = new List<Puma.Shared.Utvalg>();
                foreach (var itemUtvalg in utvalgList?.MemberUtvalgs)
                {
                    string utvalgName = await _utvalgListRepository.CreateNewNameWithSuffixForced(itemUtvalg.Name, 50, false, new List<string>(), prefix);

                    Puma.Shared.Utvalg utvalg = new();

                    //RequestSaveUtvalg requestSaveUtvalg = new RequestSaveUtvalg();
                    utvalg = itemUtvalg;
                    utvalg.Name = utvalgName;
                    utvalg.KundeNummer = kundeNumber;
                    utvalg.ListId = listData.ListId.ToString();
                    // userName = userName;
                    utvalg.UtvalgId = 0;
                    utvalg.IsBasis = false;
                    utvalg.Thickness = 0;
                    utvalg.Weight = 0;
                    utvalg.OrdreType = OrdreType.Null;
                    utvalg.OrdreReferanse = string.Empty;
                    utvalg.BasedOn = 0;
                    requestListSaveUtvalgs.utvalgs.Add(utvalg);

                    //var utvalgData = await _mediator.Send(requestSaveUtvalg);

                    //memberListData.MemberUtvalgs.Add(_mapper.Map<ResponseSaveUtvalg, Puma.Shared.Utvalg>(utvalgData));

                }

                requestListSaveUtvalgs.userName = userName;

                List<ResponseSaveUtvalg> lstUtvalgResponse = await _mediator.Send(requestListSaveUtvalgs);

                memberListData.MemberUtvalgs.AddRange(_mapper.Map<List<ResponseSaveUtvalg>, List<Puma.Shared.Utvalg>>(lstUtvalgResponse));
            }

            if (utvalgList.MemberLists?.Any() == true)
            {
                memberListData.MemberLists = new List<Puma.Shared.UtvalgList>();
                foreach (var itemMemberLists in utvalgList.MemberLists)
                {
                    memberListData.MemberLists.Add(await MemberListCreation(itemMemberLists, kundeNumber, prefix, listData.ListId, userName));
                }
            }

            return memberListData;
        }


    }


}
