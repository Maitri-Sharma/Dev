using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Team;
using DataAccessAPI.HandleRequest.Response.Team;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Team
{
    /// <summary>
    /// GetAllTeamHandler
    /// </summary>
    public class GetAllTeamHandler : IRequestHandler<RequestGetAllTeam, List<ResponseGetAllTeam>>
    {
        /// <summary>
        /// The team repository
        /// </summary>
        private readonly ITeamRepository _teamRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllTeamHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTeamHandler"/> class.
        /// </summary>
        /// <param name="teamRepository">The team repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// teamRepository
        /// or
        /// mapper
        /// </exception>
        public GetAllTeamHandler(ITeamRepository teamRepository, ILogger<GetAllTeamHandler> logger, IMapper mapper)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
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
        public async Task<List<ResponseGetAllTeam>> Handle(RequestGetAllTeam request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllTeam> responseGetAllTeam = null;
            _logger.LogDebug("Calling GetAllTeam from Repository");
            var teamsData = await _teamRepository.GetAllTeam();
            if (teamsData?.Any() == true)
            {
                responseGetAllTeam = _mapper.Map<List<Puma.Shared.Team>, List<ResponseGetAllTeam>>(teamsData);
            }

            return responseGetAllTeam;
        }
    }
}
