

using AutoMapper;
using DataAccessAPI.HandleRequest.Request.ReolerKommune;
using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.ReolerKommune
{
    /// <summary>
    /// GetReolerKommuneByKommuneIdHandler
    /// </summary>
    public class GetReolerKommuneByKommuneIdHandler : IRequestHandler<RequestGetReolerKommuneByKommuneId, List<ResponseGetReolerKommuneByKommuneId>>
    {
        /// <summary>
        /// The reoler kommune repository
        /// </summary>
        private readonly IReolerKommuneRepository _reolerKommuneRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerKommuneByKommuneIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerKommuneByKommuneIdHandler"/> class.
        /// </summary>
        /// <param name="reolerKommuneRepository">The reoler kommune repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolerKommuneRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerKommuneByKommuneIdHandler(IReolerKommuneRepository reolerKommuneRepository, ILogger<GetReolerKommuneByKommuneIdHandler> logger, IMapper mapper)
        {
            _reolerKommuneRepository = reolerKommuneRepository ?? throw new ArgumentNullException(nameof(reolerKommuneRepository));
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
        public async Task<List<ResponseGetReolerKommuneByKommuneId>> Handle(RequestGetReolerKommuneByKommuneId request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolerKommuneByKommuneId> responseGetReolerKommuneByKommuneIds = null;
            _logger.LogDebug("Calling GetReolerKommuneByKommuneId from Repository");
            var reolKomuneByKomuneId = await _reolerKommuneRepository.GetReolerKommuneByKommuneId(request.KommuneId);
            if (reolKomuneByKomuneId?.Any() == true)
            {
                responseGetReolerKommuneByKommuneIds = _mapper.Map<List<Puma.Shared.ReolerKommune>, List<ResponseGetReolerKommuneByKommuneId>>(reolKomuneByKomuneId);
            }
            return responseGetReolerKommuneByKommuneIds;

        }
    }
}
