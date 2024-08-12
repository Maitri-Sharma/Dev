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
    public class IsUtvalgConnectedToListHandler : IRequestHandler<RequestIsUtvalgConnectedToList, bool>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IsUtvalgConnectedToListHandler> _logger;

        public IsUtvalgConnectedToListHandler(IUtvalgListRepository utvalgListRepository, ILogger<IsUtvalgConnectedToListHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(RequestIsUtvalgConnectedToList request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling IsUtvalgConnectedToList from Repository");
            await _utvalgListRepository.IsUtvalgConnectedToList(request.utvalgId);
            return true;
            
        }
    }
}
