using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class GetUtvalgByUtvalgIdHandler : IRequestHandler<RequestGetUtvalgByUtvalgId, ResponseGetUtlvagByUtvalgId>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgByUtvalgIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        private readonly IConfigurationRepository _configurationRepository;

        public GetUtvalgByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgByUtvalgIdHandler> logger, IMapper mapper,
            IConfigurationRepository configurationRepository)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _configurationRepository = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseGetUtlvagByUtvalgId> Handle(RequestGetUtvalgByUtvalgId request, CancellationToken cancellationToken)
        {
            ResponseGetUtlvagByUtvalgId response = null;
            _logger.LogDebug("Calling GetUtvalg from Repository");

            var utvalgData = await _utvalgRepository.GetUtvalg(request.utlvagId);
            if (utvalgData != null)
            {
                response = _mapper.Map<Puma.Shared.Utvalg, ResponseGetUtlvagByUtvalgId>(utvalgData);
                if (request.GetsummarizeData)
                {
                    response.basicDetails = await _utvalgRepository.formatReportData(utvalgData, false, false, false, "", 0, 4, "pdf");
                }
            }

            return response;
        }
    }
}
