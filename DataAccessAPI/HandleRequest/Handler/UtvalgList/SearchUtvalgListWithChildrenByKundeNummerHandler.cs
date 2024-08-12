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
    /// SearchUtvalgListWithChildrenByKundeNummerHandler
    /// </summary>
    public class SearchUtvalgListWithChildrenByKundeNummerHandler : IRequestHandler<RequestSearchUtvalgListWithChildrenByKundeNummer, List<ResponseSearchUtvalgListWithChildrenByKundeNummer>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListWithChildrenByKundeNummerHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgListWithChildrenByKundeNummerHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// mapper
        /// </exception>
        public SearchUtvalgListWithChildrenByKundeNummerHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListWithChildrenByKundeNummerHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        public async Task<List<ResponseSearchUtvalgListWithChildrenByKundeNummer>> Handle(RequestSearchUtvalgListWithChildrenByKundeNummer request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListWithChildrenByKundeNummer from Repository");
            var resultData = await _utvalgListRepository.SearchUtvalgListWithChildrenByKundeNummer(request.kundeNummer, request.searchMethod, request.includeReols);
            List<ResponseSearchUtvalgListWithChildrenByKundeNummer> result = null;
            if (resultData?.Any() == true)
            {
                result = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListWithChildrenByKundeNummer>>(resultData);

                if (request.onlyBasisUtvalglist)
                    return result.Where(x => x.IsBasis == true).ToList();
            }

            return result;

        }
    }
}
