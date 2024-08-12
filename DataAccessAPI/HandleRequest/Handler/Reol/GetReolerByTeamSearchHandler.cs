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
    /// GetReolerByTeamSearchHandler
    /// </summary>
    public class GetReolerByTeamSearchHandler : IRequestHandler<RequestGetReolerByTeamSearch, List<ResponseGetReolerByTeamSearch>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerByTeamSearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerByTeamSearchHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerByTeamSearchHandler(IReolRepository reolRepository, ILogger<GetReolerByTeamSearchHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetReolerByTeamSearch>> Handle(RequestGetReolerByTeamSearch request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolerByTeamSearch> responseGetReolerByTeamSearch = null;
            _logger.LogDebug("Calling GetReolerByTeamSearch from Repository");
            var reolsData = await _reolRepository.GetReolerByTeamSearch(request.teamNames);
            if (reolsData?.Any() == true)
            {
                responseGetReolerByTeamSearch = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolerByTeamSearch>>(reolsData);
            }

            return responseGetReolerByTeamSearch;
        }
    }
}
