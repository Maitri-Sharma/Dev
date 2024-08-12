using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// SearchUtvalgListWithChildrenByIdHandler
    /// </summary>
    public class SearchUtvalgListWithChildrenByIdHandler : IRequestHandler<RequestSearchUtvalgListWithChildrenById, List<ResponseSearchUtvalgListWithChildrenById>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListWithChildrenByIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgListWithChildrenByIdHandler" /> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository
        /// or
        /// logger
        /// or
        /// mapper</exception>
        public SearchUtvalgListWithChildrenByIdHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListWithChildrenByIdHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseSearchUtvalgListWithChildrenById>> Handle(RequestSearchUtvalgListWithChildrenById request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListWithChildrenById from Repository");
            var resultData = await _utvalgListRepository.SearchUtvalgListWithChildrenById(request.listId,request.includeReols);
            List<ResponseSearchUtvalgListWithChildrenById> result = null;
            if (resultData != null)
            {
                result = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListWithChildrenById>>(resultData);
            }

            return result;

        }
    }
}
