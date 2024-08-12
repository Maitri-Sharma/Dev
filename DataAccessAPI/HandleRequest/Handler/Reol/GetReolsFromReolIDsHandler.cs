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
    /// <summary>
    /// GetReolsFromReolIDsHandler
    /// </summary>
    public class GetReolsFromReolIDsHandler : IRequestHandler<RequestGetReolsFromReolIDs, List<ResponseGetReolsFromReolIDString>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolsFromReolIDsHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolsFromReolIDsHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolsFromReolIDsHandler(IReolRepository reolRepository, ILogger<GetReolsFromReolIDsHandler> logger, IMapper mapper)
        {
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async  Task<List<ResponseGetReolsFromReolIDString>> Handle(RequestGetReolsFromReolIDs request, CancellationToken cancellationToken)
        {
        //    List<ResponseGetReolsFromReolIDs> responseGetReolsFromReolIDs = null;
        //    _logger.LogDebug("Calling GetReolsFromReolIDs from Repository");
        //    var reolsData = await _reolRepository.GetReolsFromReolIDs(request.ReolIDs);
        //    if (reolsData?.Any() == true)
        //    {
        //        responseGetReolsFromReolIDs = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsFromReolIDs>>(reolsData);
        //    }

        //    return responseGetReolsFromReolIDs;

            List<ResponseGetReolsFromReolIDString> responseGetReolsFromReolIDs = null;
            _logger.LogDebug("Calling GetReolsFromReolIDString from Repository");
            string reolIds = string.Join(",", request.ReolIDs);
            var reolsData = await _reolRepository.GetReolsFromReolIDString(reolIds);
            if (reolsData?.Any() == true)
            {
                responseGetReolsFromReolIDs = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsFromReolIDString>>(reolsData);
            }

            return responseGetReolsFromReolIDs;
        }
    }
}
