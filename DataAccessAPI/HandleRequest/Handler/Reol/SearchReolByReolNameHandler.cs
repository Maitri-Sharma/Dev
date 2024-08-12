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
    public class SearchReolByReolNameHandler : IRequestHandler<RequestSearchReolByReolName, List<ResponseSearchReolByReolName>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchReolByReolNameHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchReolByReolNameHandler(IReolRepository reolRepository, ILogger<SearchReolByReolNameHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchReolByReolName>> Handle(RequestSearchReolByReolName request, CancellationToken cancellationToken)
        {
            List<ResponseSearchReolByReolName> responseSearchReolByReolName = null;
            _logger.LogDebug("Calling SearchReolByReolName from Repository");
            var reolsData = await _reolRepository.SearchReolByReolName(request.reolName);
            if (reolsData?.Any() == true)
            {
                responseSearchReolByReolName = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseSearchReolByReolName>>(reolsData);
            }

            return responseSearchReolByReolName;
        }
    }
}
