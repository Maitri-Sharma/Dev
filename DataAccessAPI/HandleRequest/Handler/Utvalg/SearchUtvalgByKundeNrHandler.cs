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
    public class SearchUtvalgByKundeNrHandler : IRequestHandler<RequesSearchUtvalgByKundeNr, List<ResponseSearchUtvalgByKundeNr>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByKundeNrHandler> _logger;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByKundeNrHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByKundeNrHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ResponseSearchUtvalgByKundeNr>> Handle(RequesSearchUtvalgByKundeNr request, CancellationToken cancellationToken)
        {

            _logger.LogDebug("Calling SearchUtvalgByKundeNr from Repository");

            var utvalgData = await _utvalgRepository.SearchUtvalgByKundeNr(request.KundeNummer, request.SearchMethod, request.IncludeReols, request.ExtendedInfo);

            List<ResponseSearchUtvalgByKundeNr> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseSearchUtvalgByKundeNr>>(utvalgData).ToList();

                if (request.onlyBasisUtvalg)
                {
                    return response.Where(x => x.IsBasis == true).ToList();
                }
            }
            return response;
        }
    }
}
