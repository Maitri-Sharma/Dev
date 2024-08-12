#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Kommune;
using DataAccessAPI.HandleRequest.Response.Kommune;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
#endregion

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KommuneController : ControllerBase
    {
        #region Variables
        private readonly ILogger<KommuneController> _logger;
        private static Dictionary<string, KommuneCollection> _KommuneCache;
        private static bool _IsUniqueKommuneSet;
        private readonly ConfigController configController;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Properties
        public static bool IsUniqueKommuneSet
        {
            get
            {
                return _IsUniqueKommuneSet;
            }
        }

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="KommuneController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        public KommuneController(ILogger<KommuneController> logger, ILogger<ConfigController> loggerConfig,
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
        /// To get all Kommune data
        /// </summary>
        /// <returns>KommuneCollection</returns>

        [HttpGet]
        [ProducesResponseType(typeof(KommuneCollection), (int)HttpStatusCode.OK)]
        [Route("GetAllKommunes")]
        public IActionResult GetAllKommunes()
        {
            _logger.BeginScope("Inside into GetAllKommunes");
            KommuneCollection result = new KommuneCollection();
            string table = configController.CurrentReolTableName;
            KommuneCollection allKommuner = new KommuneCollection();

            if (_KommuneCache == null)
                _KommuneCache = new Dictionary<string, KommuneCollection>();

            if ((!_KommuneCache.ContainsKey(table)))
            {
                #region Old Code
                //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                //npgsqlParameters[0].Value = table;
                //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                //{
                //    kommuner = dbhelper.FillDataTable("kspu_gdb.getallkommunes", CommandType.StoredProcedure, npgsqlParameters);
                //    allKommuner = new KommuneCollection();
                //    foreach (DataRow row in kommuner.Rows)
                //        allKommuner.Add(GetKommuneFromDataRow(row));
                //    _KommuneCache[table] = allKommuner;
                //} 
                #endregion
                var komuneData = _mediator.Send(new RequestGetAllKommunes()).Result;
                var mapData = _mapper.Map<List<ResponseGetAllKommunes>, List<Kommune>>(komuneData);
                allKommuner.AddRange(mapData);
                _KommuneCache[table] = allKommuner;
            }

            result.AddRange(_KommuneCache[table]);

            return Ok(result);
        }

        /// <summary>
        /// Check if Kommune Exists
        /// </summary>
        /// <param name="Kommunenavn">Kommune name to check in table</param>
        /// <returns>Kommune Exist or not</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("KommuneExists")]
        public IActionResult KommuneExists(string Kommunenavn)
        {
            _logger.BeginScope("Inside into KommuneExists");
            #region Old Code
            //_logger.LogDebug("Inside into KommuneExists");
            //int result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_kommunenavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = Kommunenavn;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<int>("kspu_gdb.kommuneexists", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned {0}", result);

            //_logger.LogDebug("Exiting from KommuneExists");

            //return (Convert.ToInt32(result) > 0); 
            #endregion
            RequestKommuneExists request = new RequestKommuneExists()
            {
                Kommunenavn = Kommunenavn
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// To fetch Kommune ID from Current Reol Table
        /// </summary>
        /// <param name="Kommunenavn">Kommune name to check in table</param>
        /// <param name="FylkeNavn">Flyke name to check in table</param>
        /// <returns>Kommune ID</returns>

        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("GetKommuneID")]
        public IActionResult GetKommuneID(string Kommunenavn, string FylkeNavn)
        {
            _logger.BeginScope("Inside into GetKommuneID");
            #region Old Code
            //_logger.LogDebug("Inside into GetKommuneID");
            //DataTable kommuneID;
            //string kID;
            //Exception exception = null;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_kommunenavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = Kommunenavn.ToUpper();

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    kommuneID = dbhelper.FillDataTable("kspu_gdb.kommuneexists", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (kommuneID.Rows.Count == 0)
            //{
            //    exception = new Exception("Fant ikke kommunen med Kommunenavn " + Kommunenavn + " i databasen.");
            //    _logger.LogError(exception, exception.Message);
            //    throw exception;
            //}

            //if (kommuneID.Rows.Count > 1)
            //{
            //    if (FylkeNavn == null)
            //    {
            //        exception = new Exception("Fant flere kommuner i databasen med Kommunenavn " + Kommunenavn + ".");
            //        _logger.LogError(exception, exception.Message);
            //        throw exception;
            //    }
            //    else
            //    {
            //    }
            //    // retrive the first
            //    kID = kommuneID.Rows[0].ToString(); //GetStringFromRow(kommuneID.Rows[0], "KommuneID");
            //}
            //else
            //    kID = kommuneID.Rows[0].ToString();

            //_logger.LogInformation("Number of row returned {0}", kID);

            //_logger.LogDebug("Exiting from KommuneExists");
            //return kID; 
            #endregion
            RequestGetKommuneID request = new RequestGetKommuneID()
            {
                Kommunenavn = Kommunenavn,
                FylkeNavn = FylkeNavn
            };
            return Ok(_mediator.Send(request).Result);

        }

        /// <summary>
        /// To set Kommune property
        /// </summary>
        /// <param name="kommunes">Kommune name to check in table</param>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("SetIsKommuneUniqueProperty")]
        public IActionResult SetIsKommuneUniqueProperty(KommuneCollection kommunes)
        {
            _logger.BeginScope("Inside into SetIsKommuneUniqueProperty");
            _IsUniqueKommuneSet = true;
            RequestSetIsKommuneUniqueProperty request = new RequestSetIsKommuneUniqueProperty()
            {
            };
            return Ok(_mediator.Send(request).Result);
           
        }

        /// <summary>
        /// To fetch Kommune data from Current Reol Table
        /// </summary>
        /// <param name="KommuneId">Kommune ID to check in table</param>
        /// <returns>Kommune Data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetKommune), (int)HttpStatusCode.OK)]
        [Route("GetKommune")]
        public IActionResult GetKommune(string KommuneId)
        {
            _logger.BeginScope("Inside into GetKommune");
            RequestGetKommune request = new RequestGetKommune()
            {
                KommuneId = KommuneId
            };

            return Ok(_mediator.Send(request).Result);
        }

        #endregion

        #region Private Methods

        ///// <summary>
        ///// To check Kommune name is unique 
        ///// </summary>
        ///// <param name="komm">Kommune ID to check in table</param>
        ///// <returns>True if unique else false</returns>
        //private bool IsKommuneNameUnique(Kommune komm)
        //{
        //    foreach (Kommune k in GetAllKommunes())
        //    {
        //        if (k.KommuneID != komm.KommuneID)
        //        {
        //            if (k.KommuneName.ToLower().Trim() == komm.KommuneName.Trim().ToLower())
        //                return false;
        //        }
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Fill Kommune object
        ///// </summary>
        ///// <param name="row">Object of Datarow</param>
        ///// <returns>Kommune data</returns>
        //private Kommune GetKommuneFromDataRow(DataRow row)
        //{
        //    Kommune k = new Kommune();
        //    k.KommuneID = row["r_kommuneid"].ToString();
        //    k.KommuneName = row["r_kommune"].ToString();
        //    k.FylkeID = row["r_fylkeid"].ToString();
        //    k.FylkeName = row["r_fylke"].ToString();
        //    return k;
        //}

        #endregion
    }
}
