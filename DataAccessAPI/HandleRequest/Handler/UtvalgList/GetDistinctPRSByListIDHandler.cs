using DataAccessAPI.HandleRequest.Request.UtvalgList;
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
    public class GetDistinctPRSByListIDHandler : IRequestHandler<RequestGetDistinctPRSByListID, List<string>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetDistinctPRSByListIDHandler> _logger;

        public GetDistinctPRSByListIDHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetDistinctPRSByListIDHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<List<string>> Handle(RequestGetDistinctPRSByListID request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetDistinctPRSByListID from Repository");
            return await _utvalgListRepository.GetDistinctPRSByListID(request.utvalglistId);
            
        }
    }
}
