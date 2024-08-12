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
    public class SearchUtvalgByNameHandler : IRequestHandler<RequestSearchUtvalgByName, List<ResponseSearchUtvalgByName>>
    {

        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByNameHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgByNameHandler"/> class.
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
        public SearchUtvalgByNameHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByNameHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchUtvalgByName>> Handle(RequestSearchUtvalgByName request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgSimple from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgSimple(request.UtvalgName, request.SearchMethod);
            List<ResponseSearchUtvalgByName> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<UtvalgSearchResult>, List<ResponseSearchUtvalgByName>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
