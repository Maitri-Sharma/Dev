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
    public class GetUtvalgCampaignsByUtvalgIdHandler : IRequestHandler<RequestGetUtlvagCampaignsByUtvalgId, List<ResponseGetUtlvagCampaignsByUtvalgId>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgCampaignsByUtvalgIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgCampaignsByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgCampaignsByUtvalgIdHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetUtlvagCampaignsByUtvalgId>> Handle(RequestGetUtlvagCampaignsByUtvalgId request, CancellationToken cancellationToken)
        {
            var utvalgData = await _utvalgRepository.GetUtvalgCampaigns(request.UtvalgId);
            List<ResponseGetUtlvagCampaignsByUtvalgId> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<CampaignDescription>, List<ResponseGetUtlvagCampaignsByUtvalgId>>(utvalgData.OrderBy(x=>x.DistributionDate).ToList()).ToList();
            }
            return response;
        }
    }
}
