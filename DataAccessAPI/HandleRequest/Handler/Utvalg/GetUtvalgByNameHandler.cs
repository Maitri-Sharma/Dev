using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class GetUtvalgByNameHandler : IRequestHandler<RequestGetUtvalgDetailByName, List<ResponseGetUtlvagDetailByName>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgByNameHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgByNameHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// or
        /// mapper
        /// </exception>
        public GetUtvalgByNameHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgByNameHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
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
        public async Task<List<ResponseGetUtlvagDetailByName>> Handle(RequestGetUtvalgDetailByName request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalg from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalg(request.UtvalgName,request.searchMethod,request.includeReoler);
            List<ResponseGetUtlvagDetailByName> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseGetUtlvagDetailByName>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
