using AutoMapper;
using DataAccessAPI.HandleRequest.Request.AvisDekning;
using DataAccessAPI.HandleRequest.Response.AvisDekning;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.AvisDekning
{
    /// <summary>
    /// GetAvisDekningHandler
    /// </summary>
    public class GetAvisDekningHandler : IRequestHandler<RequestGetAvisDekning, List<ResponseAvisDekning>>
    {
        /// <summary>
        /// The avis dekning repository
        /// </summary>
        private readonly IAvisDekningRepository _avisDekningRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAvisDekningHandler> _logger;

        /// <summary>
        /// IMapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvisDekningHandler"/> class.
        /// </summary>
        /// <param name="avisDekningRepository">The avis dekning repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// avisDekningRepository
        /// or
        /// mapper
        /// </exception>
        public GetAvisDekningHandler(IAvisDekningRepository avisDekningRepository, ILogger<GetAvisDekningHandler> logger, IMapper mapper)
        {
            _avisDekningRepository = avisDekningRepository ?? throw new ArgumentNullException(nameof(avisDekningRepository));
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
        public async Task<List<ResponseAvisDekning>> Handle(RequestGetAvisDekning request, CancellationToken cancellationToken)
        {
            List<ResponseAvisDekning> responseAvisDeknings = null;
            _logger.LogDebug("Calling GetAvisDekning from Repository");
            var avisDekningsData = await _avisDekningRepository.GetAvisDekning(request.reolId);

            if (avisDekningsData?.Any() == true)
            {
                responseAvisDeknings = _mapper.Map<List<Puma.Shared.AvisDekning>, List<ResponseAvisDekning>>(avisDekningsData);
            }

            return responseAvisDeknings;
        }
    }
}
