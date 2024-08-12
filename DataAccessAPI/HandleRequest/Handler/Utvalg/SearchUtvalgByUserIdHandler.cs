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
    public class SearchUtvalgByUserIdHandler : IRequestHandler<RequestSearchUtvalgByUserId, List<ResponseSearchUtvalgByUserId>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByUserIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByUserIdHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByUserIdHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchUtvalgByUserId>> Handle(RequestSearchUtvalgByUserId request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByUserID1 from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByUserID1(request.UserId, request.SearchMethod);
            List<ResponseSearchUtvalgByUserId> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<UtvalgSearchResult>, List<ResponseSearchUtvalgByUserId>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
