using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Reol;
using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Reol
{
    /// <summary>
    /// SearchReolPostboksByReolNameHandler
    /// </summary>
    public class SearchReolPostboksByReolNameHandler : IRequestHandler<RequestSearchReolPostboksByReolName, List<ResponseSearchReolPostboksByReolName>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchReolPostboksByReolNameHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchReolPostboksByReolNameHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public SearchReolPostboksByReolNameHandler(IReolRepository reolRepository, ILogger<SearchReolPostboksByReolNameHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
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
        public async Task<List<ResponseSearchReolPostboksByReolName>> Handle(RequestSearchReolPostboksByReolName request, CancellationToken cancellationToken)
        {
            List<ResponseSearchReolPostboksByReolName> responseSearchReolPostboksByReolName = null;
            _logger.LogDebug("Calling SearchReolPostboksByReolName from Repository");
            var reolsData = await _reolRepository.SearchReolPostboksByReolName(request.ReolName,request.KommuneName);
            if (reolsData?.Any() == true)
            {
                responseSearchReolPostboksByReolName = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseSearchReolPostboksByReolName>>(reolsData);
            }

            return responseSearchReolPostboksByReolName;
        }
    }
}
