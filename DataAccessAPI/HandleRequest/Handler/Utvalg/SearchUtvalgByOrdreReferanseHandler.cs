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
    public class SearchUtvalgByOrdreReferanseHandler : IRequestHandler<RequestSearchUtvalgByOrdreReferanse, List<ResponseSearchUtvalgByOrdreReferanse>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByOrdreReferanseHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByOrdreReferanseHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByOrdreReferanseHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchUtvalgByOrdreReferanse>> Handle(RequestSearchUtvalgByOrdreReferanse request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByOrdreReferanse from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByOrdreReferanse(request.OrdreReferanse, request.OrdreType, request.SearchMethod, request.IncludeReols);
            List<ResponseSearchUtvalgByOrdreReferanse> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseSearchUtvalgByOrdreReferanse>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
