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
    /// Handler
    /// </summary>
    
    public class GetReolsInFylkeHandler : IRequestHandler<RequestGetReolsInFylke, List<ResponseGetReolsInFylke>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolsInFylkeHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolsInFylkeHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolsInFylkeHandler(IReolRepository reolRepository, ILogger<GetReolsInFylkeHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetReolsInFylke>> Handle(RequestGetReolsInFylke request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolsInFylke> responseGetReolsInFylkes = null;
            _logger.LogDebug("Calling GetReolerByFylkeSearch from Repository");
            var reolsInFyleksData = await _reolRepository.GetReolerByFylkeSearch(request.fylkeId);
            if (reolsInFyleksData?.Any() == true)
            {
                responseGetReolsInFylkes = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsInFylke>>(reolsInFyleksData);
            }

            return responseGetReolsInFylkes;
        }
    }
}
