using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
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
    public class GetUtvalgListParentCampaignsHandler : IRequestHandler<RequestGetUtvalgListParentCampaigns, ResponseGetUtvalgListParentCampaigns>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListParentCampaignsHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgListParentCampaignsHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListParentCampaignsHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ResponseGetUtvalgListParentCampaigns> Handle(RequestGetUtvalgListParentCampaigns request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListParentCampaigns from Repository");
            var utvalgListData = await _utvalgListRepository.GetUtvalgListParentCampaigns(request.listId);
            ResponseGetUtvalgListParentCampaigns response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.CampaignDescription>, ResponseGetUtvalgListParentCampaigns>(utvalgListData);
            }
            return response;
            
        }
    }
}
