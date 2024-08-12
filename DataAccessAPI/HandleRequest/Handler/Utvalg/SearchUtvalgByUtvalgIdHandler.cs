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
    public class SearchUtvalgByUtvalgIdHandler : IRequestHandler<RequestSearchUtvalgDetailById, List<ResponseGetUtlvagDetailById>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByUtvalgIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByUtvalgIdHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetUtlvagDetailById>> Handle(RequestSearchUtvalgDetailById request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByUtvalgId from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByUtvalgId(request.UtlvagId, request.includeReols);
            List<ResponseGetUtlvagDetailById> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseGetUtlvagDetailById>>(utvalgData).ToList();
                if (!string.IsNullOrWhiteSpace(request.customerNos))
                    return response.Where(x => x.KundeNummer.ToLower() == request.customerNos.ToLower()).ToList();
            }
            return response;
        }
    }
}
