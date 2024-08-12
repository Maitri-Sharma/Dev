using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Reol;
using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Reol
{
    /// <summary>
    /// GetReolerBySegmenterSearchHandler
    /// </summary>
    public class GetReolerBySegmenterSearchHandler : IRequestHandler<RequestGetReolerBySegmenterSearch, ResponseGetReolerBySegmenterSearch>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerBySegmenterSearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerBySegmenterSearchHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerBySegmenterSearchHandler(IReolRepository reolRepository, ILogger<GetReolerBySegmenterSearchHandler> logger, IMapper mapper)
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
        public async Task<ResponseGetReolerBySegmenterSearch> Handle(RequestGetReolerBySegmenterSearch request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetReolerBySegmenterSearch from Repository");
            var reolerData = await _reolRepository.GetReolerBySegmenterSearch(request.options);
            if (reolerData != null)
                return _mapper.Map<Puma.Shared.Utvalg, ResponseGetReolerBySegmenterSearch>(reolerData);

            return null;
        }
    }
}
