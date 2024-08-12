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
    /// GetReolerByFylkeSearchHandler
    /// </summary>
    public class GetReolerByFylkeSearchHandler : IRequestHandler<RequestGetReolerByFylkeSearch, List<ResponseGetReolerByFylkeSearch>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerByFylkeSearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerByFylkeSearchHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerByFylkeSearchHandler(IReolRepository reolRepository, ILogger<GetReolerByFylkeSearchHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetReolerByFylkeSearch>> Handle(RequestGetReolerByFylkeSearch request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolerByFylkeSearch> responseGetReolerByFylkeSearch = null;
            _logger.LogDebug("Calling GetReolerByFylkeSearch from Repository");
            var reolsData = await _reolRepository.GetReolerByFylkeSearch(request.fylkeIder);
            if (reolsData?.Any() == true)
            {
                responseGetReolerByFylkeSearch = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolerByFylkeSearch>>(reolsData);
            }

            return responseGetReolerByFylkeSearch;
        }
    }
}
