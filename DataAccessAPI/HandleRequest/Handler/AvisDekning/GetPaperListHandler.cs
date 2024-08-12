using AutoMapper;
using DataAccessAPI.HandleRequest.Request.AvisDekning;
using DataAccessAPI.HandleRequest.Response.AvisDekning;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.AvisDekning
{
    /// <summary>
    /// GetPaperListHandler
    /// </summary>
    public class GetPaperListHandler : IRequestHandler<RequestGetPaperList, List<ResponseAvis>>
    {
        /// <summary>
        /// The avis dekning repository
        /// </summary>
        private readonly IAvisDekningRepository _avisDekningRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetPaperListHandler> _logger;

        /// <summary>
        /// IMapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// GetPaperListHandler
        /// </summary>
        /// <param name="avisDekningRepository"></param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper"></param>
        public GetPaperListHandler(IAvisDekningRepository avisDekningRepository, ILogger<GetPaperListHandler> logger, IMapper mapper)
        {
            _avisDekningRepository = avisDekningRepository ?? throw new ArgumentNullException(nameof(avisDekningRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<ResponseAvis>> Handle(RequestGetPaperList request, CancellationToken cancellationToken)
        {
            List<ResponseAvis> responseAvis = null;
            _logger.LogDebug("Calling GetPaperList from Repository");
            var avisData = await _avisDekningRepository.GetPaperList();
            if (avisData?.Any() == true)
            {
                responseAvis = _mapper.Map<List<Avis>, List<ResponseAvis>>(avisData);
            }

            return responseAvis;
        }
    }
}
