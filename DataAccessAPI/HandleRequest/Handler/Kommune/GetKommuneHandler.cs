

using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Kommune;
using DataAccessAPI.HandleRequest.Response.Kommune;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kommune
{
    /// <summary>
    /// GetKommuneHandler
    /// </summary>
    public class GetKommuneHandler : IRequestHandler<RequestGetKommune, ResponseGetKommune>
    {
        /// <summary>
        /// The kommune repository
        /// </summary>
        private readonly IKommuneRepository _kommuneRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetKommuneHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetKommuneHandler"/> class.
        /// </summary>
        /// <param name="kommuneRepository">The kommune repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// kommuneRepository
        /// or
        /// mapper
        /// </exception>
        public GetKommuneHandler(IKommuneRepository kommuneRepository, ILogger<GetKommuneHandler> logger, IMapper mapper)
        {
            _kommuneRepository = kommuneRepository ?? throw new ArgumentNullException(nameof(kommuneRepository));
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
        public async Task<ResponseGetKommune> Handle(RequestGetKommune request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetKommune from Repository");
            var komuneData = await _kommuneRepository.GetKommune(request.KommuneId);

            ResponseGetKommune responseGetKommune = _mapper.Map<Puma.Shared.Kommune, ResponseGetKommune>(komuneData);

            return responseGetKommune;
        }
    }
}
