#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Team;
using DataAccessAPI.HandleRequest.Response.Team;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {

        #region Variables
        private readonly ILogger<TeamController> _logger;
        private static List<TeamKommuneKey> _teamKommuneKeyCache = null;
        private readonly ConfigController configController;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors              
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">mediator</exception>
        public TeamController(ILogger<TeamController> logger, ILogger<ConfigController> loggerConfig,
            IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            configController = new ConfigController(loggerConfig);
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Search Team data on the basis of team name
        /// </summary>
        /// <param name="teamNavn">Team Name</param>
        /// <returns>TeamCollection data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BydelCollection), (int)HttpStatusCode.OK)]
        [Route("SearchTeam")]
        public IActionResult SearchTeam(string teamNavn)
        {
            _logger.BeginScope("Inside into SearchTeam");
            #region Old Code
            //_logger.LogDebug("Inside into SearchTeam");
            //DataTable teamCollection;
            //TeamCollection result = new TeamCollection();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //if (_TeamCache == null)
            //    _TeamCache = new Dictionary<string, Team>();

            //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = configController.CurrentReolTableName;

            //npgsqlParameters[1] = new NpgsqlParameter("p_teamnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 34);
            //npgsqlParameters[1].Value = teamNavn;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    teamCollection = dbhelper.FillDataTable("kspu_gdb.searchteam", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //if (teamCollection?.Rows?.Count == 0)
            //    return null/* TODO Change to default(_) if this is not a reference type */;

            //foreach (DataRow datarow in teamCollection?.Rows)
            //{
            //    string TeamNr = datarow["r_teamnr"].ToString();
            //    if (_TeamCache.ContainsKey(TeamNr))
            //        result.Add(_TeamCache[TeamNr]);
            //    else
            //    {
            //        _TeamCache[TeamNr] = new Team(datarow["r_teamnr"].ToString(), datarow["r_teamnavn"].ToString());
            //        result.Add(_TeamCache[TeamNr]);
            //    }
            //}

            //_logger.LogInformation("Number of row returned: ", result.Count);

            //_logger.LogDebug("Exiting from SearchTeam");
            //return result; 
            #endregion
            RequestSearchTeam request = new RequestSearchTeam()
            {
                teamNavn = teamNavn
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get all team data
        /// </summary>
        /// <returns>TeamCollection </returns>

        [HttpGet]
        [ProducesResponseType(typeof(BydelCollection), (int)HttpStatusCode.OK)]
        [Route("GetAllTeam")]
        public IActionResult GetAllTeam()
        {
            _logger.BeginScope("Inside into GetAllTeam");
            #region Old Code
            //_logger.LogDebug("Inside into GetAllTeam");
            //DataTable teamCollection;
            //TeamCollection result = new TeamCollection();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = configController.CurrentReolTableName;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    teamCollection = dbhelper.FillDataTable("kspu_gdb.getallteam", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (teamCollection?.Rows?.Count == 0)
            //    throw new Exception("Fant ikke noe Team i databasen.");


            //foreach (DataRow datarow in teamCollection?.Rows)
            //{
            //    string TeamNr = datarow["r_teamnr"].ToString();
            //    if (_TeamCache.ContainsKey(TeamNr))
            //        result.Add(_TeamCache[TeamNr]);
            //    else
            //    {
            //        _TeamCache[TeamNr] = new Team(datarow["r_teamnr"].ToString(), datarow["r_teamnavn"].ToString());
            //        result.Add(_TeamCache[TeamNr]);
            //    }
            //}

            //_logger.LogInformation("Number of row returned: ", result.Count);

            //_logger.LogDebug("Exiting from GetAllTeam");
            //return result; 
            #endregion
            RequestGetAllTeam request = new RequestGetAllTeam()
            {
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get all team Kommune keys
        /// </summary>
        /// <returns>TeamKommuneKey List</returns>

        [HttpGet]
        [ProducesResponseType(typeof(List<TeamKommuneKey>), (int)HttpStatusCode.OK)]
        [Route("GetAllTeamKommuneKeys")]
        public IActionResult GetAllTeamKommuneKeys()
        {
            _logger.BeginScope("Inside into GetAllTeamKommuneKeys");
            if (_teamKommuneKeyCache?.Any()  == true)
                return Ok(_teamKommuneKeyCache);
            #region Old Code
            //_logger.LogDebug("Inside into GetAllTeamKommuneKeys");
            //DataTable teamKommuneKey;
            //List<TeamKommuneKey> result = new List<TeamKommuneKey>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = configController.CurrentReolTableName;

            //if (_teamKommuneKeyCache != null)
            //    return _teamKommuneKeyCache;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    teamKommuneKey = dbhelper.FillDataTable("kspu_gdb.getallteamkommunekeys", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow datarow in teamKommuneKey.Rows)
            //    result.Add(new TeamKommuneKey(System.Convert.ToString(datarow["r_teamnr"]), System.Convert.ToString(datarow["r_kommuneid"])));

            //_teamKommuneKeyCache = result;

            //_logger.LogInformation("Number of row returned: ", result.Count);

            //_logger.LogDebug("Exiting from GetAllTeamKommuneKeys");
            //return _teamKommuneKeyCache; 
            #endregion
            List<TeamKommuneKey> result = new List<TeamKommuneKey>();

            RequestGetAllTeamKommuneKeys request = new RequestGetAllTeamKommuneKeys()
            {
            };
            var teamKomuneKes = _mediator.Send(request).Result;
            result = _mapper.Map<List<ResponseGetAllTeamKommuneKeys>, List<TeamKommuneKey>>(teamKomuneKes);
            _teamKommuneKeyCache = result;
            return Ok(result);
        }

        #endregion
    }
}
