

using AutoMapper;
using DataAccessAPI.HandleRequest.Request.ReolerKommune;
using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.ReolerKommune
{
    /// <summary>
    /// GetReolerKommuneHandler
    /// </summary>
    public class GetReolerKommuneHandler : IRequestHandler<RequestGetReolerKommune, ResponseGetReolerKommune>
    {

        /// <summary>
        /// The reoler kommune repository
        /// </summary>
        private readonly IReolerKommuneRepository _reolerKommuneRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerKommuneHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolerKommuneHandler"/> class.
        /// </summary>
        /// <param name="reolerKommuneRepository">The reoler kommune repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolerKommuneRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolerKommuneHandler(IReolerKommuneRepository reolerKommuneRepository, ILogger<GetReolerKommuneHandler> logger, IMapper mapper)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<ResponseGetReolerKommune> Handle(RequestGetReolerKommune request, CancellationToken cancellationToken)
        {
            var reolKomuneData = await _reolerKommuneRepository.GetReolerKommune(request.ReolId, request.KommuneId);

            return _mapper.Map<Puma.Shared.ReolerKommune, ResponseGetReolerKommune>(reolKomuneData);

        }
    }
}
