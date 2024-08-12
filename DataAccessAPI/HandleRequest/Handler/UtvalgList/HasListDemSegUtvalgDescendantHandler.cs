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
    public class HasListDemSegUtvalgDescendantHandler : IRequestHandler<RequestHasListDemSegUtvalgDescendant, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<HasListDemSegUtvalgDescendantHandler> _logger;

        public HasListDemSegUtvalgDescendantHandler(IUtvalgListRepository utvalgListRepository, ILogger<HasListDemSegUtvalgDescendantHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestHasListDemSegUtvalgDescendant request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling HasListDemSegUtvalgDescendant from Repository");
            await _utvalgListRepository.HasListDemSegUtvalgDescendant(request.listId);
            return true;
            
        }
    }
}
