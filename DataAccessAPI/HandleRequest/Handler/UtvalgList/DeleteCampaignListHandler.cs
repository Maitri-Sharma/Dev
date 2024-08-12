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
    public class DeleteCampaignListHandler : IRequestHandler<RequestDeleteCampaignList, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeleteCampaignListHandler> _logger;

        public DeleteCampaignListHandler(IUtvalgListRepository utvalgListRepository, ILogger<DeleteCampaignListHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public  async Task<bool> Handle(RequestDeleteCampaignList request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling DeleteCampaignList from Repository");
            await _utvalgListRepository.DeleteCampaignList(request.listId,request.BasedOn);
            return true;
        }
    }
}
