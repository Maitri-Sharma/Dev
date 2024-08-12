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
    public class GetAllTeamKommuneKeysHandler : IRequestHandler<RequestGetAllTeamKommuneKeys, List<ResponseGetAllTeamKommuneKeys>>
    {
        /// <summary>
        /// The team repository
        /// </summary>
        private readonly ITeamRepository _teamRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllTeamKommuneKeysHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTeamKommuneKeysHandler"/> class.
        /// </summary>
        /// <param name="teamRepository">The team repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// teamRepository
        /// or
        /// mapper
        /// </exception>
        public GetAllTeamKommuneKeysHandler(ITeamRepository teamRepository, ILogger<GetAllTeamKommuneKeysHandler> logger, IMapper mapper)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetAllTeamKommuneKeys>> Handle(RequestGetAllTeamKommuneKeys request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllTeamKommuneKeys> responseGetAllTeamKommuneKeys = null;
            _logger.LogDebug("Calling GetAllTeamKommuneKeys from Repository");
            var teamsData = await _teamRepository.GetAllTeamKommuneKeys();
            if (teamsData?.Any() == true)
            {
                responseGetAllTeamKommuneKeys = _mapper.Map<List<Puma.Shared.TeamKommuneKey>, List<ResponseGetAllTeamKommuneKeys>>(teamsData);
            }

            return responseGetAllTeamKommuneKeys;
        }
    }
}
