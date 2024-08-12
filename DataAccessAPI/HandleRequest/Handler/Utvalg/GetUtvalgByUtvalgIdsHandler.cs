using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// GetUtvalgByUtvalgIdsHandler
    /// </summary>
    public class GetUtvalgByUtvalgIdsHandler : IRequestHandler<RequestGetUtvalgByUtvalgIds, List<ResponseGetUtlvagByUtvalgId>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgByUtvalgIdsHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgByUtvalgIdsHandler"/> class.
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
        public GetUtvalgByUtvalgIdsHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgByUtvalgIdsHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetUtlvagByUtvalgId>> Handle(RequestGetUtvalgByUtvalgIds request, CancellationToken cancellationToken)
        {
            List<ResponseGetUtlvagByUtvalgId> responseGetUtlvagByUtvalgIds = new List<ResponseGetUtlvagByUtvalgId>();

            _logger.LogDebug("Calling GetUtvalg from Repository");
            foreach (var itemUtvalgId in request.UtvalgIds)
            {
                var utvalgData = await _utvalgRepository.GetUtvalg(Convert.ToInt32(itemUtvalgId));
                if (utvalgData != null)
                {
                    ResponseGetUtlvagByUtvalgId response = _mapper.Map<Puma.Shared.Utvalg, ResponseGetUtlvagByUtvalgId>(utvalgData);
                    responseGetUtlvagByUtvalgIds.Add(response);
                }
            }
            return responseGetUtlvagByUtvalgIds;
        }
    }
}
