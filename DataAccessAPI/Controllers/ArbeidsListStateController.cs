#region Namespaces
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.ArbeidsListState;
using DataAccessAPI.HandleRequest.Response.ArbeidsListState;
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
    public class ArbeidsListStateController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ArbeidsListStateController> _logger;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArbeidsListStateController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">mediator</exception>
        public ArbeidsListStateController(ILogger<ArbeidsListStateController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// SAve the address point. If exists then it iwll update otherwise adding new address point
        /// </summary>
        /// <param name="arbeidsListState"> List of arbeidsListState to be saved</param>
        /// <returns>Number of row affecteds</returns>

        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("SaveArbeidsListState")]
        public IActionResult SaveArbeidsListState(List<ArbeidsListEntryState> arbeidsListState)
        {
            _logger.BeginScope("Inside into SaveArbeidsListState");
            #region Old Code
            //_logger.LogDebug("Inside into SaveArbeidsListState");

            //Exception exception = null;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];
            //int result = 0;

            //if (arbeidsListState == null)
            //{
            //    exception = new Exception("arbeidsListState can not be null!");
            //    _logger.LogError(exception, exception.Message);
            //    throw exception;
            //}

            //if (arbeidsListState.Count > 0)
            //{
            //    if (!IsMaximumOneEntryActive(arbeidsListState))
            //    {
            //        exception = new Exception("More than one entry is active in arbeidsListState in sub SaveArbeidsListState");
            //        _logger.LogError(exception, exception.Message);
            //        throw exception;
            //    }

            //    foreach (ArbeidsListEntryState entry in arbeidsListState)
            //    {
            //        #region Parameter assignement

            //        npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //        npgsqlParameters[0].Value = entry.Id;

            //        npgsqlParameters[1] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //        npgsqlParameters[1].Value = entry.Type;

            //        npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Double, 50);
            //        npgsqlParameters[2].Value = entry.UserId;

            //        npgsqlParameters[3] = new NpgsqlParameter("p_active", NpgsqlTypes.NpgsqlDbType.Boolean);
            //        npgsqlParameters[3].Value = entry.Active;

            //        npgsqlParameters[4] = new NpgsqlParameter("p_timecreated", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            //        npgsqlParameters[4].Value = DateTime.Now;

            //        npgsqlParameters[5] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[5].Direction = ParameterDirection.Output;


            //        #endregion

            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            dbhelper.ExecuteNonQuery("kspu_db.insertupdatearbeidsliststate", CommandType.StoredProcedure, npgsqlParameters);

            //            result = Convert.ToInt32(npgsqlParameters[5].Value);
            //            _logger.LogInformation(string.Format("Number of row affected {0}", result));
            //        }
            //    }
            //}

            //_logger.LogDebug("Exiting from SaveArbeidsListState");
            //return result; 
            #endregion

            RequestSaveArbeidsListState request = new RequestSaveArbeidsListState()
            {
                arbeidsListState = arbeidsListState
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="userId">User ID to fetch list of address related to passed user</param>
        /// <returns>List of Address points</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseArbeidsListEntryState>), (int)HttpStatusCode.OK)]
        [Route("GetArbeidsListState")]
        public IActionResult GetArbeidsListState(string userId)
        {
            _logger.BeginScope("Inside into GetArbeidsListState");
            #region Old Code
            //_logger.LogDebug("Inside into GetArbeidsListState");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<ArbeidsListEntryState> result = new List<ArbeidsListEntryState>();
            //DataTable dataTable;

            //npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = userId;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    dataTable = dbhelper.FillDataTable("kspu_db.getarbeidsliststate", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow dataRow in dataTable.Rows)
            //    result.Add(CreateArbeidsListStateEntryFromRow(dataRow));

            //_logger.LogInformation("Number of row returned {0}", result.Count);

            //_logger.LogDebug("Exiting from GetArbeidsListState");

            //return result; 
            #endregion
            RequestGetArbeidsListState request = new RequestGetArbeidsListState()
            {
                userId = userId
            };
            return Ok(_mediator.Send(request).Result);

        }

        #endregion

        #region private Methods

        /// <summary>
        /// Check the active entry in arbeidsListState
        /// </summary>
        /// <param name="arbeidsListState">List of arbeidsListState to check</param>
        /// <returns>True if found records, false otherwise.</returns>
        private static bool IsMaximumOneEntryActive(List<ArbeidsListEntryState> arbeidsListState)
        {
            if (arbeidsListState.Where(c => c.Active == true).Any())
                return true;
            return false;
        }

        /// <summary>
        /// Populates a list of Address List entry state from the datarow.
        /// </summary>
        /// <param name="dataRow">An instance of IDataReader</param>
        /// <returns>ArbeidsListEntryState data</returns>
        private static ArbeidsListEntryState CreateArbeidsListStateEntryFromRow(DataRow dataRow)
        {
            if (dataRow == null)
                throw new Exception("row can not be null for CreateArbeidsListStateEntryFromRow!");

            int id = Convert.ToInt32(dataRow["r_id"]);
            PumaEnum.ListType type = ArbeidsListEntryState.GetTypeValueFromChar(Convert.ToChar(dataRow["r_type"]));
            string userId = Convert.ToString(dataRow["r_userid"]);
            bool active = ArbeidsListEntryState.GetActiveValueFromChar(Convert.ToChar(dataRow["r_active"]));

            return new ArbeidsListEntryState(id, type, userId, active);
        }
        #endregion
    }
}
