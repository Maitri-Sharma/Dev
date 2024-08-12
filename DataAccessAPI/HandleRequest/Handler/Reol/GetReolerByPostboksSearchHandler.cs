using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Reol;
using DataAccessAPI.HandleRequest.Response.Reol;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Reol
{
    public class GetReolerByPostboksSearchHandler : IRequestHandler<RequestGetReolerByPostboksSearch, List<ResponseGetReolerByPostboksSearch>>
    {

        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolerByPostboksSearchHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetReolerByPostboksSearchHandler(IReolRepository reolRepository, ILogger<GetReolerByPostboksSearchHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetReolerByPostboksSearch>> Handle(RequestGetReolerByPostboksSearch request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolerByPostboksSearch> responseGetReolerByPostboksSearch = null;
            _logger.LogDebug("Calling GetReolerByPostboksSearch from Repository");
            var reolsData = await _reolRepository.GetReolerByPostboksSearch(request.postboks);
            if (reolsData?.Any() == true)
            {
                responseGetReolerByPostboksSearch = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolerByPostboksSearch>>(reolsData);
            }

            return responseGetReolerByPostboksSearch;
        }
    }
}
