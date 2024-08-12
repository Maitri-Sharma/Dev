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
    public class GetNrOfDoubleCoverageReolsForListHandler : IRequestHandler<RequestGetNrOfDoubleCoverageReolsForList, int>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetNrOfDoubleCoverageReolsForListHandler> _logger;

        public GetNrOfDoubleCoverageReolsForListHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetNrOfDoubleCoverageReolsForListHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<int> Handle(RequestGetNrOfDoubleCoverageReolsForList request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetNrOfDoubleCoverageReolsForList from Repository");
            await _utvalgListRepository.GetNrOfDoubleCoverageReolsForList(request.listId);
            return 1;
            
        }
    }
}
