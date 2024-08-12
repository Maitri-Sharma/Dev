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
    public class GetReolsFromReolIDStringHandler : IRequestHandler<RequestGetReolsFromReolIDString, List<ResponseGetReolsFromReolIDString>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolsFromReolIDStringHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetReolsFromReolIDStringHandler(IReolRepository reolRepository, ILogger<GetReolsFromReolIDStringHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetReolsFromReolIDString>> Handle(RequestGetReolsFromReolIDString request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolsFromReolIDString> responseGetReolsFromReolIDString = null;
            _logger.LogDebug("Calling GetReolsFromReolIDString from Repository");
            var reolsData = await _reolRepository.GetReolsFromReolIDString(request.ids);
            if (reolsData?.Any() == true)
            {
                responseGetReolsFromReolIDString = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsFromReolIDString>>(reolsData);
            }

            return responseGetReolsFromReolIDString;
        }
    }
}
