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
    public class GetReolsInKommuneHandler : IRequestHandler<RequestGetReolsInKommune, List<ResponseGetReolsInKommune>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolsInKommuneHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolsInKommuneHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolsInKommuneHandler(IReolRepository reolRepository, ILogger<GetReolsInKommuneHandler> logger, IMapper mapper)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<List<ResponseGetReolsInKommune>> Handle(RequestGetReolsInKommune request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolsInKommune> responseGetReolsInKommunes = null;
            _logger.LogDebug("Calling GetReolsInKommune from Repository");
            var reolsInKommuneData = await _reolRepository.GetReolsInKommune(request.kommuneId);
            if (reolsInKommuneData?.Any() == true)
            {
                responseGetReolsInKommunes = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsInKommune>>(reolsInKommuneData);
            }

            return responseGetReolsInKommunes;
        }
    }
}
