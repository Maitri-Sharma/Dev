using DataAccessAPI.HandleRequest.Handler.Utvalg;
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
    public class GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalgHandler : IRequestHandler<RequestGetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalgHandler> _logger;

        public GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalgHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalgHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(RequestGetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg from Repository");
            await _utvalgListRepository.GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg();
            return true;
            
        }
    }
}
