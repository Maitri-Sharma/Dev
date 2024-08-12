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

namespace DataAccessAPI.HandleRequest.Handler.UtvalgSaver
{
    /// <summary>
    /// CreateNewParentForListHandler
    /// </summary>
    public class CreateNewParentForListHandler : IRequestHandler<RequestCreateNewParentForList, (ResponseGetUtvalgListWithAllReferences utvalgListData, string message)>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;



        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CreateNewParentForListHandler> _logger;


        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewParentForListHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// logger
        /// or
        /// mediator
        /// </exception>
        public CreateNewParentForListHandler(IUtvalgListRepository utvalgListRepository, ILogger<CreateNewParentForListHandler> logger, IMediator mediator)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        /// <exception cref="System.Exception">
        /// </exception>
        public async Task<(ResponseGetUtvalgListWithAllReferences utvalgListData, string message)> Handle(RequestCreateNewParentForList request, CancellationToken cancellationToken)
        {
            int maxListeNameLength = 50;
            ResponseGetUtvalgListWithAllReferences utvalgListData = null;
            if (request.DeletedLists == null)
            {
                request.DeletedLists = new List<Puma.Shared.UtvalgList>();
            }

            Puma.Shared.UtvalgList oldParentList = request.ListToBeChanged.ParentList;
            request.NewParentName = request.NewParentName.Trim();
            if (request.NewParentName.Length < 3)
            {
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgListName);
            }

            if (request.NewParentName.Length > maxListeNameLength)
            {
                throw new Exception(string.Format(Puma.DataLayer.BusinessEntity.Constants.errMsgListNameToLong, maxListeNameLength));
            }

            if (await _utvalgListRepository.UtvalgListNameExists(request.NewParentName))
            {
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgListExists.Replace(@"*", request.NewParentName));
            }

            Puma.Shared.UtvalgList newList = new Puma.Shared.UtvalgList
            {
                Name = request.NewParentName,
                Logo = request.NewParentLogo,
                IsBasis = request.ListToBeChanged.IsBasis
            };

            if (!string.IsNullOrEmpty(request.NewParentCustomer))
            {
                newList.KundeNummer = request.NewParentCustomer;
            }

            if (newList.IsBasis)
            {
                newList.KundeNummer = request.ListToBeChanged.KundeNummer;
            }

            Int64 utvalglistId = await _utvalgListRepository.SaveUtvalgListData(newList, request.UserName);
            request.ListToBeChanged.ParentList = newList;
            await _utvalgListRepository.SaveUtvalgListData(request.ListToBeChanged, request.UserName);
            request.ListToBeChanged.ParentList.AntallWhenLastSaved = request.ListToBeChanged.Antall;
            if (await _utvalgListRepository.DeleteUpdatedListIfEmpty(request.UserName, oldParentList))
            {
                request.DeletedLists.Add(oldParentList);
            }

            if (utvalglistId > 0)
            {
                RequestGetUtvalgListWithAllReferences requestGetUtvalgListWithAllReferences = new()
                {
                    UtvalglistId = Convert.ToInt32(utvalglistId)
                };
                utvalgListData = await _mediator.Send(requestGetUtvalgListWithAllReferences);

            }

            return (utvalgListData, Puma.DataLayer.BusinessEntity.Constants.MsgConnectListInNewList.Replace(@"*", newList.Name));
        }
    }
}
