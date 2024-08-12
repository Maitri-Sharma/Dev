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
    /// SearchTeamHandler
    /// </summary>
    public class SearchTeamHandler : IRequestHandler<RequestSearchTeam, List<ResponseSearchTeam>>
    {
        /// <summary>
        /// The team repository
        /// </summary>
        private readonly ITeamRepository _teamRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchTeamHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTeamHandler"/> class.
        /// </summary>
        /// <param name="teamRepository">The team repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fylkeRepository
        /// or
        /// mapper
        /// </exception>
        public SearchTeamHandler(ITeamRepository teamRepository, ILogger<SearchTeamHandler> logger, IMapper mapper)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseSearchTeam>> Handle(RequestSearchTeam request, CancellationToken cancellationToken)
        {
            List<ResponseSearchTeam> responseSearchTeams = null;
            _logger.LogDebug("Calling SearchTeam from Repository");
            var teamsData = await _teamRepository.SearchTeam(request.teamNavn);
            if (teamsData?.Any() == true)
            {
                responseSearchTeams = _mapper.Map<List<Puma.Shared.Team>, List<ResponseSearchTeam>>(teamsData);
            }

            return responseSearchTeams;
        }
    }
}
