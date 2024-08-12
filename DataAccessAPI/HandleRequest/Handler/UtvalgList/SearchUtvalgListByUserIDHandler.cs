using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    public class SearchUtvalgListByUserIDHandler : IRequestHandler<RequestSearchUtvalgListByUserID, List<ResponseSearchUtvalgListByUserID>>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListByUserIDHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgListByUserIDHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListByUserIDHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<ResponseSearchUtvalgListByUserID>> Handle(RequestSearchUtvalgListByUserID request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListByUserID from Repository");
            var utvalgListData = await _utvalgListRepository.SearchUtvalgListByUserID(request.UserId, request.SearchMethod);
            List<ResponseSearchUtvalgListByUserID> response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListByUserID>>(utvalgListData).ToList();
            }
            return response;
            
        }
    }
}
