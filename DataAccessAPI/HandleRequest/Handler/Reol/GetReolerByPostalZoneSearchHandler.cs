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
    /// GetReolerByPostalZoneSearchHandler
    /// </summary>
    public class GetReolerByPostalZoneSearchHandler : IRequestHandler<RequestGetReolerByPostalZoneSearch, List<ResponseGetReolerByPostalZoneSearch>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerByPostalZoneSearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerByPostalZoneSearchHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerByPostalZoneSearchHandler(IReolRepository reolRepository, ILogger<GetReolerByPostalZoneSearchHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetReolerByPostalZoneSearch>> Handle(RequestGetReolerByPostalZoneSearch request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolerByPostalZoneSearch> responseGetReolerByPostalZoneSearch = null;
            _logger.LogDebug("Calling GetReolerByPostalZoneSearch from Repository");
            var reolsData = await _reolRepository.GetReolerByPostalZoneSearch(request.postalZone);
            if (reolsData?.Any() == true)
            {
                responseGetReolerByPostalZoneSearch = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolerByPostalZoneSearch>>(reolsData);
            }

            return responseGetReolerByPostalZoneSearch;
        }
    }
}
