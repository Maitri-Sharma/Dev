using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// SearchUtvalgListSimpleByOrdreReferanseHandler
    /// </summary>
    public class SearchUtvalgListSimpleByOrdreReferanseHandler : IRequestHandler<RequestSearchUtvalgListSimpleByOrdreReferanse, List<ResponseSearchUtvalgListSimpleByOrdreReferanse>>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListSimpleByOrdreReferanseHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgListSimpleByOrdreReferanseHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// logger
        /// or
        /// mapper
        /// </exception>
        public SearchUtvalgListSimpleByOrdreReferanseHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListSimpleByOrdreReferanseHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
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
        public async Task<List<ResponseSearchUtvalgListSimpleByOrdreReferanse>> Handle(RequestSearchUtvalgListSimpleByOrdreReferanse request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListSimpleByOrdreReferanse from Repository");
            var utvalgListData = await _utvalgListRepository.SearchUtvalgListSimpleByOrdreReferanse(request.OrdreReferanse, request.OrdreType, request.SearchMethod);
            List<ResponseSearchUtvalgListSimpleByOrdreReferanse> response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListSimpleByOrdreReferanse>>(utvalgListData).ToList();
            }
            return response;
            
        }
    }
}
