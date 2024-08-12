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
    /// SearchUtvalgListWithChildrenHandler
    /// </summary>
    public class SearchUtvalgListWithChildrenHandler : IRequestHandler<RequestSearchUtvalgListWithChildren, List<ResponseSearchUtvalgListWithChildren>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListWithChildrenHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgListWithChildrenHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// mapper
        /// </exception>
        public SearchUtvalgListWithChildrenHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListWithChildrenHandler> logger, IMapper mapper)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<List<ResponseSearchUtvalgListWithChildren>> Handle(RequestSearchUtvalgListWithChildren request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListWithChildren from Repository");
            var resultData = await _utvalgListRepository.SearchUtvalgListWithChildren(request.Utvalglistname,request.searchMethod);
            List<ResponseSearchUtvalgListWithChildren> result = null;
            if (resultData?.Any() == true)
            {
                result = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListWithChildren>>(resultData);
            }

            return result;
        }
    }
}
