using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// UpdateListLogoHandler
    /// </summary>
    public class UpdateListLogoHandler : IRequestHandler<RequestUpdateListLogo, bool>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateListLogoHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// mediator
        /// </exception>
        public UpdateListLogoHandler(IUtvalgListRepository utvalgListRepository, IMediator mediator)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> Handle(RequestUpdateListLogo request, CancellationToken cancellationToken)
        {
            //Update logo of utvalg List
            await _utvalgListRepository.UpdateListLogo(request.ListId, request.UserName, request.Logo);

            //Update Logo of utvalgs

            foreach (var itemUtvalgData in request.requestUpdateUtvaglLogos)
            {
                RequestUpdateUtvalgLogo requestUpdateUtvalgLogo = new()
                {
                    Logo = itemUtvalgData.Logo,
                    Username = request.UserName,
                    UtvalgId = itemUtvalgData.UtvalgId
                };

                _ = await _mediator.Send(requestUpdateUtvalgLogo);
            }


            //Update data for member list
            if (request.MemberList?.Any() == true)
            {
                foreach (var itemMemberList in request.MemberList)
                {
                    await _utvalgListRepository.UpdateListLogo(itemMemberList.ListId, request.UserName, itemMemberList.Logo);

                    foreach (var itemUtvalgData in itemMemberList.requestUpdateUtvaglLogos)
                    {
                        RequestUpdateUtvalgLogo requestUpdateUtvalgLogo = new()
                        {
                            Logo = itemUtvalgData.Logo,
                            Username = request.UserName,
                            UtvalgId = itemUtvalgData.UtvalgId
                        };

                        _ = await _mediator.Send(requestUpdateUtvalgLogo);
                    }
                }
            }

            return true;
        }
    }
}
