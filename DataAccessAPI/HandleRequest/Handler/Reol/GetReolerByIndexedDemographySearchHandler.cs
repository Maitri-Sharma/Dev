using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Reol;
using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace DataAccessAPI.HandleRequest.Handler.Reol
{
    public class GetReolerByIndexedDemographySearchHandler : IRequestHandler<RequestGetReolerByIndexedDemographySearch, ResponseGetReolerByDemographySearch>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerByDemographySearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reolRepository"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetReolerByIndexedDemographySearchHandler(IReolRepository reolRepository, ILogger<GetReolerByDemographySearchHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseGetReolerByDemographySearch> Handle(RequestGetReolerByIndexedDemographySearch request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetReolerByIndexedDemographySearch from Repository");
            var reolerData = await _reolRepository.GetReolerByIndexOrg_DemographySearch(request.Utvalg, request.options);
            if (reolerData != null)
                return _mapper.Map<Puma.Shared.Utvalg, ResponseGetReolerByDemographySearch>(reolerData);

            return null;
        }
    }
}
