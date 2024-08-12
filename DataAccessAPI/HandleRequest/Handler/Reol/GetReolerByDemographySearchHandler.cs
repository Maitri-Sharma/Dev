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
    public class GetReolerByDemographySearchHandler : IRequestHandler<RequestGetReolerByDemographySearch, ResponseGetReolerByDemographySearch>
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

        public GetReolerByDemographySearchHandler(IReolRepository reolRepository, ILogger<GetReolerByDemographySearchHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseGetReolerByDemographySearch> Handle(RequestGetReolerByDemographySearch request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetReolerByDemographySearch from Repository");
            var reolerData = await _reolRepository.GetReolerByDemographySearch(request.options,request.Utvalg,request.IsFromKundeWeb);
            if (reolerData != null)
                return _mapper.Map<Puma.Shared.Utvalg, ResponseGetReolerByDemographySearch>(reolerData);

            return null;
        }
    }
}
