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
    public class GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummerHandler:  IRequestHandler<RequestGetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer, bool>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummerHandler> _logger;

        public GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummerHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummerHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> Handle(RequestGetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
