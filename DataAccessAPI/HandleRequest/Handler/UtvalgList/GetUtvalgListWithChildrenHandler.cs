using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// GetUtvalgListWithChildrenHandler
    /// </summary>
    public class GetUtvalgListWithChildrenHandler : IRequestHandler<RequestGetUtvalgListWithChildren, ResponseGetUtvalgListWithChildren>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListWithChildrenHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgListWithChildrenHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// mapper
        /// </exception>
        public GetUtvalgListWithChildrenHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListWithChildrenHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ResponseGetUtvalgListWithChildren> Handle(RequestGetUtvalgListWithChildren request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Inside into GetUtvalgListWithChildren");
            var resultData = await _utvalgListRepository.GetUtvalgListWithChildren(request.listId, request.getParentListMemberUtvalg);
            ResponseGetUtvalgListWithChildren result = null;
            if (resultData != null)
            {
                result = _mapper.Map<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithChildren>(resultData);
            }

            return result;
        }
    }
}
