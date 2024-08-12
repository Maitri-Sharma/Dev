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
    /// GetReolsInTeamHandler
    /// </summary>
    public class GetReolsInTeamHandler : IRequestHandler<RequestGetReolsInTeam, List<ResponseGetReolsInTeam>>
    {
        /// <summary>
        /// IReolRepository
        /// </summary>

        private readonly IReolRepository _reolRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetReolsInTeamHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReolsInTeamHandler"/> class.
        /// </summary>
        /// <param name="reolRepository">The reol repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// reolRepository
        /// or
        /// mapper
        /// </exception>
        public GetReolsInTeamHandler(IReolRepository reolRepository, ILogger<GetReolsInTeamHandler> logger, IMapper mapper)
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
        public async Task<List<ResponseGetReolsInTeam>> Handle(RequestGetReolsInTeam request, CancellationToken cancellationToken)
        {
            List<ResponseGetReolsInTeam> responseGetReolsInTeam = null;
            _logger.LogDebug("Calling GetReolsInTeam from Repository");
            var reolsData = await _reolRepository.GetReolsInTeam(request.teamName);
            if (reolsData?.Any() == true)
            {
                responseGetReolsInTeam = _mapper.Map<List<Puma.Shared.Reol>, List<ResponseGetReolsInTeam>>(reolsData);
            }

            return responseGetReolsInTeam;
        }
    }
}
