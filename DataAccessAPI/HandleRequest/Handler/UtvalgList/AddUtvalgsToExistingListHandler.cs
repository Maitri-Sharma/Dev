using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
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
    /// AddUtvalgsToExistingListHandler
    /// </summary>
    public class AddUtvalgsToExistingListHandler : IRequestHandler<RequestAddUtvalgsToExistingList, bool>
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddUtvalgsToExistingListHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// mapper
        /// </exception>
        public AddUtvalgsToExistingListHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
        public async Task<bool> Handle(RequestAddUtvalgsToExistingList request, CancellationToken cancellationToken)
        {
            foreach (var utv in request.utvalgs)
            {
                utv.ListId = request.utvalgList.ListId.ToString();
                if (string.IsNullOrEmpty(utv.KundeNummer))
                {
                    utv.KundeNummer = request.utvalgList.KundeNummer;
                    utv.KundeNavn = request.utvalgList.KundeNavn;
                }

                if ((utv.OrdreType == OrdreType.T) || (request.utvalgList.OrdreType == OrdreType.T))
                {
                    utv.OrdreReferanse = request.utvalgList.OrdreReferanse;
                    utv.OrdreStatus = request.utvalgList.OrdreStatus;
                    utv.OrdreType = request.utvalgList.OrdreType;
                }

                RequestSaveUtvalg requestSave = new RequestSaveUtvalg()
                {
                    forceUtvalgListId = 0,
                    saveOldReoler =  true,
                    skipHistory = false,
                    userName = request.userName,
                    utvalg = utv
                };

                //RequestSaveUtvalg requestSave = _mapper.Map<Puma.Shared.Utvalg, RequestSaveUtvalg>(utv);
                //requestSave.userName = request.userName;
                _ = await _mediator.Send(requestSave);
            }
            RequestSaveUtvalgList requestSaveUtvalgList = _mapper.Map<Puma.Shared.UtvalgList, RequestSaveUtvalgList>(request.utvalgList);
            if (request.utvalgList?.ParentList != null)
            {
                requestSaveUtvalgList.ParentListId = Convert.ToInt32(request.utvalgList?.ParentList.ListId);
            }
                requestSaveUtvalgList.userName = request.userName;
            _ = await _mediator.Send(requestSaveUtvalgList);

            return true;
        }
    }
}
