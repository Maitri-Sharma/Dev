#region Namespaces
using DataAccessAPI.Controllers;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Reol;
using DataAccessAPI.HandleRequest.Response.Reol;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using static Puma.Shared.PumaEnum;
#endregion

namespace DataAccessAPI
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReolController : ControllerBase

    {
        #region Variables
        private readonly ILogger<ReolController> _logger;
        private readonly ConfigController configController;
        private static Dictionary<string, ReolCollection> _ReolCache;
        private static Dictionary<string, Dictionary<long, Reol>> _ReolDictionaryCache;
        private static Dictionary<string, string> _ReolJSonCache;
        private string _ReolTableName;
      
        private readonly IMediator _mediator;
        #endregion

        #region Constructors       
        /// <summary>
        /// Initializes a new instance of the <see cref="ReolController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="reoltablename">The reoltablename.</param>
        /// <exception cref="System.Exception">Illegal character " + c + " in reolTableName " + _ReolTableName + " specified in ReolTableController. Table name was rejected for security reasons.</exception>
        public ReolController(ILogger<ReolController> logger, ILogger<ConfigController> loggerConfig, IMediator mediator, string reoltablename = "")
        {
            _logger = logger;
            _mediator = mediator;

            configController = new ConfigController(loggerConfig);
            if (reoltablename != "")
            {
                _ReolTableName = reoltablename;
            }
            else
            {
                _ReolTableName = configController.CurrentReolTableName;
            }
            //Guard against SQL insertion attacks:
            foreach (char c in _ReolTableName.Trim().ToLower().ToCharArray())
            {
                if (!"abcdefghijklmnopqrstuvwxyz_0123456789.".Contains(c))
                    throw new Exception("Illegal character " + c + " in reolTableName " + _ReolTableName + " specified in ReolTableController. Table name was rejected for security reasons.");
            }
        }

        //[ActivatorUtilitiesConstructorAttribute]
        //public ReolController(string reolTableName, ILogger<ReolController> logger)
        //{
        //    _logger = logger;
        //    _ReolTableName = reolTableName;
        //    // Guard against SQL insertion attacks:
        //    foreach (char c in reolTableName.Trim().ToLower().ToCharArray())
        //    {
        //        if (!"abcdefghijklmnopqrstuvwxyz_0123456789.".Contains(c))
        //            throw new Exception("Illegal character " + c + " in reolTableName " + reolTableName + " specified in ReolTableController. Table name was rejected for security reasons.");
        //    }
        //}

        #endregion

        #region Public Methods
        public readonly string NewUtvalgName = "Påbegynt utvalg";

        /// <summary>
        /// Rebuild Reol Cache
        /// </summary>
        [HttpGet("RebuildReolCache", Name = nameof(RebuildReolCache))]
        public void RebuildReolCache()
        {
            _logger.BeginScope("Inside into RebuildReolCache");

            Dictionary<string, ReolCollection> newCache = new Dictionary<string, ReolCollection>();

            // Cache reoler for current and previous reol table. (Previous is to speed up recreation)
            string currentTable = configController.CurrentReolTableName;
            string previousTable = configController.PreviousReolTableName;
            newCache[currentTable] = GetAllReolsFromTable(currentTable);
            newCache[previousTable] = GetAllReolsFromTable(previousTable);

            Dictionary<string, Dictionary<long, Reol>> newDictionaryCache = new Dictionary<string, Dictionary<long, Reol>>();
            newDictionaryCache[currentTable] = CreateReolDictionary(newCache[currentTable]);
            newDictionaryCache[previousTable] = CreateReolDictionary(newCache[previousTable]);

            _ReolCache = newCache;
            _ReolDictionaryCache = newDictionaryCache;

            _logger.LogInformation("Number of row returned _ReolCache: ", _ReolCache.Count, "and _ReolDictionaryCache: ", _ReolDictionaryCache.Count);

            _logger.LogDebug("Exiting from RebuildReolCache");
        }

        /// <summary>
        /// Get Reol 
        /// </summary>
        [HttpGet("GetReol", Name = nameof(GetReol))]
        public Reol GetReol(long reolId, PumaEnum.NotFoundAction actionIfNotFound = PumaEnum.NotFoundAction.ThrowException, bool getAvisDekning = true)
        {
            _logger.BeginScope("Inside into GetReol");
            Dictionary<long, Reol> allReoler = GetReolDictionary();
            if (allReoler.ContainsKey(reolId))
            {
                _logger.LogDebug("Exiting from GetReol");
                return allReoler[reolId];
            }
            else
            {
                if (actionIfNotFound == PumaEnum.NotFoundAction.ThrowException)
                    throw new Exception("Fant ikke reolen med reolid " + reolId + " i tabellen " + _ReolTableName + ".");
                _logger.LogDebug("Exiting from GetReol");
                return null/* TODO Change to default(_) if this is not a reference type */;

            }
        }
        [HttpGet("TableExists", Name = nameof(TableExists))]
        public bool TableExists(string tableName)
        {
            _logger.BeginScope("Inside into UtvalgHasRecreatedBefore");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName.Split(".")[1];

            npgsqlParameters[1] = new NpgsqlParameter("p_schemaname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = tableName.Split(".")[0];

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result == DBNull.Value)
                return false;
            return System.Convert.ToInt32(result) > 0;
        }


        /// <summary>
        /// Get All Reols
        /// </summary>
        [HttpGet("GetAllReols", Name = nameof(GetAllReols))]
        public ReolCollection GetAllReols()
        {
            _logger.BeginScope("Inside into GetAllReols");
            if (_ReolCache == null)
                _ReolCache = new Dictionary<string, ReolCollection>();
            if (_ReolDictionaryCache == null)
                _ReolDictionaryCache = new Dictionary<string, Dictionary<long, Reol>>();
            if (!_ReolCache.ContainsKey(_ReolTableName))
            {
                _ReolCache[_ReolTableName] = GetAllReolsFromTable(_ReolTableName);
                _ReolDictionaryCache[_ReolTableName] = CreateReolDictionary(_ReolCache[_ReolTableName]);
            }
            _logger.LogDebug("Inside into GetAllReols");
            return _ReolCache[_ReolTableName];
        }

        /// <summary>
        /// Get All Reol IDs
        /// </summary>
        /// <returns>Reol ID List</returns>
        [HttpGet("GetAllReolIds", Name = nameof(GetAllReolIds))]
        public List<long> GetAllReolIds()
        {
            try
            {
                _logger.BeginScope("Inside into GetAllReolIds");
                DataTable reolIds;
                List<long> result = new List<long>();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = _ReolTableName;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    reolIds = dbhelper.FillDataTable("kspu_gdb.custom_getallreolids", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow row in reolIds.Rows)
                    result.Add(long.Parse(row["r_reol_id"].ToString()));

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetAllReolIds");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllReolIds: " + exception.Message);
                return null;
            }
        }

        [HttpGet("GetAllReolJSON", Name = nameof(GetAllReolJSON))]
        public string GetAllReolJSON()
        {
            if (_ReolJSonCache == null)
                _ReolJSonCache = new Dictionary<string, string>();

            if (!_ReolJSonCache.ContainsKey(_ReolTableName))
            {
                _ReolJSonCache[_ReolTableName] = GetAllReolsJSONFromTable(_ReolTableName);
            }
            return _ReolJSonCache[_ReolTableName];
        }

        /// <summary>
        /// Get All Reol IDs as Object IDs
        /// </summary>
        /// <param name="WhereClause"></param>
        /// <returns>Reol ID List</returns>
        [HttpGet("GetAllReolIDSASObjectIds", Name = nameof(GetAllReolIDSASObjectIds))]
        public List<int> GetAllReolIDSASObjectIds(string WhereClause)
        {
            try
            {
                _logger.BeginScope("Inside into GetAllReolIDSASObjectIds");
                DataTable reolIds;
                List<int> result = new List<int>();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_whereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = WhereClause;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    reolIds = dbhelper.FillDataTable("kspu_gdb.custom_getallreolidsasobjectids", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow datarow in reolIds.Rows)
                    result.Add(Convert.ToInt32(long.Parse(datarow["r_objectid"].ToString())));

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetAllReolIDSASObjectIds");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllReolIDSASObjectIds: " + exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Reol in Fylke
        /// </summary>
        /// <param name="fylkeId"></param>
        /// <returns>Reol ID</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolsInFylke>), (int)HttpStatusCode.OK)]
        [Route("GetReolsInFylke")]
        public IActionResult GetReolsInFylke(string fylkeId)
        {
            _logger.BeginScope("Inside into GetReolsInFylke");
            RequestGetReolsInFylke request = new RequestGetReolsInFylke()
            {
                fylkeId = fylkeId
            };

            return Ok(_mediator.Send(request).Result);
            #region Old Code
            ////_logger.LogDebug("Inside into from GetReolsInFylke");
            ////ReolCollection reolCollection = GetAllReols();
            ////_logger.LogInformation("Number of row returned: ", reolCollection.Count);
            ////_logger.LogDebug("Exiting from GetReolsInFylke");
            ////return reolCollection.GetReolsInFylke(fylkeId); 
            #endregion


        }

        /// <summary>
        /// Get Reol in Kommune
        /// </summary>
        /// <param name="kommuneId"></param>
        /// <returns>Reol ID</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolsInKommune>), (int)HttpStatusCode.OK)]
        [Route("GetReolsInKommune")]
        public IActionResult GetReolsInKommune(string kommuneId)
        {
            _logger.BeginScope("Inside into GetReolsInKommune");
            RequestGetReolsInKommune request = new RequestGetReolsInKommune()
            {
                kommuneId = kommuneId
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get Reol by team no
        /// </summary>
        /// <param name="teamNr"></param>
        /// <returns>Reol ID</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolsByTeamNr>), (int)HttpStatusCode.OK)]
        [Route("GetReolsByTeamNr")]
        public IActionResult GetReolsByTeamNr(string teamNr)
        {
            _logger.BeginScope("Inside into GetReolsByTeamNr");
            RequestGetReolsByTeamNr request = new RequestGetReolsByTeamNr()
            {
                teamNr = teamNr
            };

            return Ok(_mediator.Send(request).Result);

        }

        /// <summary>
        /// Get Reol by team name
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns>Reol ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<ResponseGetReolsInTeam>), (int)HttpStatusCode.OK)]
        [Route("GetReolsInTeam")]
        public IActionResult GetReolsInTeam([FromBody] List<string> teamName)
        {
            _logger.BeginScope("Inside into GetReolsInTeam");
            #region Old Code
            //_logger.LogDebug("Inside into from GetReolsInKommune");
            //ReolCollection reolCollection = GetAllReols();
            //_logger.LogInformation("Number of row returned: ", reolCollection.Count);
            //_logger.LogDebug("Exiting from GetReolsInKommune");
            //return reolCollection.GetReolsInTeam(teamName); 
            #endregion

            RequestGetReolsInTeam request = new RequestGetReolsInTeam()
            {
                teamName = teamName
            };

            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// Get All Reol IDs by Post No.
        /// </summary>
        /// <param name="postnummer"></param>
        /// <returns>Reol ID List</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolsInPostNr>), (int)HttpStatusCode.OK)]
        [Route("GetReolsInPostNr")]
        public IActionResult GetReolsInPostNr(string postnummer)
        {
            _logger.BeginScope("Inside into GetReolsInPostNr");
            #region Old Code
            //try
            //{
            //    _logger.LogDebug("Inside into GetReolsInPostNr");
            //    DataTable reolIds;
            //    ReolCollection result = new ReolCollection();
            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //    npgsqlParameters[0] = new NpgsqlParameter("p_postnummer", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            //    npgsqlParameters[0].Value = postnummer;

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolIds = dbhelper.FillDataTable("kspu_gdb.getreolsinpostnr", CommandType.StoredProcedure, npgsqlParameters);
            //    }

            //    foreach (DataRow datarow in reolIds.Rows)
            //    {
            //        foreach (Reol reol in GetAllReols())
            //        {
            //            if (!datarow.IsNull("r_reol_id"))
            //            {
            //                if (reol.ReolId == long.Parse(datarow["r_reol_id"].ToString()))
            //                    result.Add(reol);
            //            }
            //        }
            //    }

            //    _logger.LogInformation("Number of row returned: ", result.Count);

            //    _logger.LogDebug("Exiting from GetReolsInPostNr");

            //    return result;
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetReolsInPostNr: " + exception.Message);
            //    return null;
            //} 
            #endregion
            RequestGetReolsInPostNr request = new RequestGetReolsInPostNr()
            {
                postnummer = postnummer
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get Reol data from Reol ID
        /// </summary>
        /// <param name="reolID"></param>
        /// <param name="getAvisDekning"></param> 
        /// <returns>Reol ID List</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetReolFromReolID), (int)HttpStatusCode.OK)]
        [Route("GetReolFromReolID")]
        public IActionResult GetReolFromReolID(long reolID, bool getAvisDekning = true)
        {
            _logger.BeginScope("Inside into GetReolFromReolID");
            #region Old Code
            //try
            //{
            //    _logger.LogDebug("Inside into GetReolFromReolID");
            //    DataTable reolData;
            //    DataRow datarow;
            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

            //    npgsqlParameters[0] = new NpgsqlParameter("p_getavisdekning", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = getAvisDekning;

            //    npgsqlParameters[1] = new NpgsqlParameter("r_reolid", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[1].Value = reolID;

            //    npgsqlParameters[2] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[2].Value = _ReolTableName;

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_gdb.getreolfromreolid", CommandType.StoredProcedure, npgsqlParameters);
            //    }

            //    datarow = reolData.Rows[0];

            //    _logger.LogInformation("Number of row returned: ", datarow.Table.Rows.Count);

            //    _logger.LogDebug("Exiting from GetReolFromReolID");

            //    return GetReolFromDataRow(datarow);
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetReolFromReolID: " + exception.Message);
            //    return null;
            //} 
            #endregion
            RequestGetReolFromReolID request = new RequestGetReolFromReolID()
            {
                getAvisDekning = getAvisDekning,
                reolID = reolID
            };

            return Ok(_mediator.Send(request).Result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<ResponseGetReolsFromReolIDString>), (int)HttpStatusCode.OK)]
        [Route("GetReolsFromReolIDs")]
        public IActionResult GetReolsFromReolIDs(long[] ReolIDs)
        {
            _logger.BeginScope("Inside into GetReolsFromReolIDs");
            #region Old Code
            //_logger.LogDebug("Inside into GetReolsFromReolIDs");
            //ReolCollection result = new ReolCollection();
            //foreach (long reolId in ReolIDs)
            //    result.Add(this.GetReol(reolId));

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from GetReolsFromReolIDs");
            //return result; 
            #endregion

            RequestGetReolsFromReolIDs request = new RequestGetReolsFromReolIDs()
            {
                ReolIDs = ReolIDs
            };

            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchReolByReolName>), (int)HttpStatusCode.OK)]
        [Route("SearchReolByReolName")]
        public IActionResult SearchReolByReolName(string reolName)
        {
            _logger.BeginScope("Inside into SearchReolByReolName");
            // return GetAllReols().GetReolsByNameSearch(reolName);
            RequestSearchReolByReolName request = new RequestSearchReolByReolName()
            {
                reolName = reolName
            };

            return Ok(_mediator.Send(request).Result);
        }
        [HttpGet("GetReolsByNameSearch", Name = nameof(GetReolsByNameSearch))]
        public ReolCollection GetReolsByNameSearch(string reolName)
        {
            return GetAllReols().GetReolsByNameSearch(reolName);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchReolPostboksByReolName>), (int)HttpStatusCode.OK)]
        [Route("SearchReolPostboksByReolName")]
        public IActionResult SearchReolPostboksByReolName(string reolName, string kommuneName)
        {
            _logger.BeginScope("Inside into SearchReolPostboksByReolName");
            // return GetAllReols().GetReolsPostboksByNameSearch(reolName, kommuneName);
            RequestSearchReolPostboksByReolName request = new RequestSearchReolPostboksByReolName()
            {
                ReolName = reolName,
                KommuneName = kommuneName
            };

            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet("GetReolsPostboksByNameSearch", Name = nameof(GetReolsPostboksByNameSearch))]
        public ReolCollection GetReolsPostboksByNameSearch(string reolName, string kommuneName)
        {
            return GetAllReols().GetReolsPostboksByNameSearch(reolName, kommuneName);
        }

        /// <summary>
        /// Get Reol data from Reol ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>ReolCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolsFromReolIDString>), (int)HttpStatusCode.OK)]
        [Route("GetReolsFromReolIDString")]
        public IActionResult GetReolsFromReolIDString(string ids)
        {
            _logger.BeginScope("Inside into GetReolsFromReolIDString");
            #region Old Code
            //try
            //{
            //    _logger.LogDebug("Inside into GetReolsFromReolIDString");
            //    DataTable reolData;
            //    ReolCollection result = new ReolCollection();
            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //    npgsqlParameters[0] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = _ReolTableName;

            //    npgsqlParameters[1] = new NpgsqlParameter("p_ids", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[1].Value = ids;

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_gdb.getreolsfromreolidstring", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    foreach (DataRow datarow in reolData.Rows)
            //    {
            //        Reol reol = GetReolFromDataRow(datarow);
            //        result.Add(reol);
            //    }

            //    _logger.LogInformation("Number of row returned: ", result.Count);

            //    _logger.LogDebug("Exiting from GetReolsFromReolIDString");

            //    return result;
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetReolsFromReolIDString: " + exception.Message);
            //    return null;
            //} 
            #endregion

            RequestGetReolsFromReolIDString request = new RequestGetReolsFromReolIDString()
            {
               ids = ids
            };

            return Ok(_mediator.Send(request).Result);
        }



        [HttpPost]
        [ProducesResponseType(typeof(ResponseGetReolerBySegmenterSearch), (int)HttpStatusCode.OK)]
        [Route("GetReolerBySegmenterSearch")]
        public IActionResult GetReolerBySegmenterSearch([FromBody] DemographyOptions options)
        {
            _logger.BeginScope("Inside into GetReolerBySegmenterSearch");
            #region Old Code
            //_logger.LogDebug("Inside into GetReolerBySegmenterSearch");
            //DataTable reolData;
            //Utvalg utvalg = new Utvalg();
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];


            //npgsqlParameters[0] = new NpgsqlParameter("p_sqlwhereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = options.SQLWhereClause;

            //npgsqlParameters[1] = new NpgsqlParameter("p_sqlwhereclausegeography", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = options.SQLWhereClauseGeography;

            //npgsqlParameters[2] = new NpgsqlParameter("p_sqlorderby", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = options.SQLOrderby;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolData = dbhelper.FillDataTable("kspu_gdb.testsegment", CommandType.StoredProcedure, npgsqlParameters);
            //}


            //utvalg.Reoler = new ReolCollection();
            ////ReolCollection reols = new ReolCollection();

            //ReolController daReol = new ReolController(_logger, _loggerconfigcontroller, configController.CurrentReolTableName);
            //foreach (DataRow row in reolData.Rows)
            //{
            //    long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "r_reol_id"));
            //    Reol r = daReol.GetReol(reolid, NotFoundAction.ReturnNothing, true);
            //    //reols.Add(r);
            //    utvalg.Reoler.Add(r);

            //}
            //return utvalg; 
            #endregion
            RequestGetReolerBySegmenterSearch request = new RequestGetReolerBySegmenterSearch()
            {
                options = options
            };
            return Ok(_mediator.Send(request).Result);

        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGetReolerBySegmenterSearch), (int)HttpStatusCode.OK)]
        [Route("GetReolerByDemographySearch")]
        public IActionResult GetReolerByDemographySearch([FromBody] RequestGetReolerByDemographySearch options,bool isFromKundeWeb = false)
        {
            _logger.BeginScope("Inside into GetReolerByDemographySearch");
            #region Old Code
            //// TODO: Skal spørringa inneholde mottakerkriterier? Får vist reoler med ingen mottakere..Men mottakere kan endres etter at søket gjort!

            ////string SQL = "select main.reol_id from kspu_gdb." + Config.Demografi_Maintable + "  main, kspu_gdb." + Config.Demografi_Indekstable + "  indeks";
            ////SQL += " where main.reol_id = indeks.reol_id AND ";
            ////SQL += options.SQLWhereClause;
            ////SQL += options.SQLWhereClauseGeography;
            ////SQL += options.SQLOrderby;

            //// debug
            //// Logger.LogMessage("Demografi SQL = " + SQL)
            //_logger.LogDebug("Inside into GetReolerByDemographySearch");
            //DataTable reolData;
            //Utvalg utvalg = new Utvalg();
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];


            //npgsqlParameters[0] = new NpgsqlParameter("p_sqlwhereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = options.SQLWhereClause;

            //npgsqlParameters[1] = new NpgsqlParameter("p_sqlwhereclausegeography", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = options.SQLWhereClauseGeography;

            //npgsqlParameters[2] = new NpgsqlParameter("p_sqlorderby", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = options.SQLOrderby;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolData = dbhelper.FillDataTable("kspu_gdb.getreolerbydemographysearch", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //UtvalgReceiverList urLst = new UtvalgReceiverList();
            ////new UtvalgReceiverList();
            //List<UtvalgReceiverList> result = new List<UtvalgReceiverList>();

            ////if ((utvalg == null))
            ////utvalg = utvalg.CreateNewUtvalg(NewUtvalgName, "");

            ////else
            //// fjern eksisterende reoler fra utvalget
            ////utvalg.Reoler = new ReolCollection();

            //long sum = 0;
            ////DAReolTable daReol = new DAReolTable(DAConfig.CurrentReolTableName);
            //ReolController daReol = new ReolController(_logger, _loggerconfigcontroller, configController.CurrentReolTableName);
            //foreach (DataRow row in reolData.Rows)
            //{
            //    long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "r_reol_id"));
            //    Reol r = daReol.GetReol(reolid);
            //    utvalg.Reoler.Add(r);

            //    // tester mot maxantall
            //    if (options.MaxAntall > 0)
            //    {
            //        sum += r.Antall.GetTotalAntall(utvalg.Receivers);
            //        if (sum > options.MaxAntall)
            //            break;// hopp ut av løkka
            //    }
            //}

            //return utvalg; 
            #endregion
            options.IsFromKundeWeb = isFromKundeWeb;
            return Ok(_mediator.Send(options).Result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolerByKommuneSearch>), (int)HttpStatusCode.OK)]
        [Route("GetReolerByKommuneSearch")]
        public IActionResult GetReolerByKommuneSearch(string kummuneIder)
        {
            _logger.BeginScope("Inside into GetReolerByKommuneSearch");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataTable reolData;
            //string currentTable = configController.CurrentReolTableName;
            ////OracleCommand cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + kummuneIder);
            //// AddParameterString(cmd, "reolid", ReolID, 10)
            //npgsqlParameters[0] = new NpgsqlParameter("p_kummuneIder", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = kummuneIder;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolData = dbhelper.FillDataTable("kspu_gdb.GetReolerByKommuneSearch", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////DataTable reolData = GetDataTable(cmd);
            //ReolCollection result = new ReolCollection();
            //foreach (DataRow row in reolData.Rows)
            //{
            //    Reol r = GetReolFromDataRow(row);
            //    result.Add(r);
            //}

            //return result; 
            #endregion
            RequestGetReolerByKommuneSearch request = new RequestGetReolerByKommuneSearch()
            {
                kummuneIder = kummuneIder
            };
            return Ok(_mediator.Send(request).Result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolerByFylkeSearch>), (int)HttpStatusCode.OK)]
        [Route("GetReolerByFylkeSearch")]
        public IActionResult GetReolerByFylkeSearch(string fylkeIder)
        {
            _logger.BeginScope("Inside into GetReolerByFylkeSearch");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataTable reolData;
            //string currentTable = configController.CurrentReolTableName;

            //if (Strings.InStr(Strings.UCase(fylkeIder), "FYLKEID") != 0)
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_fylkeIder", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = fylkeIder;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByFylkeSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //// cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + fylkeIder);
            //else
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_fylkeIder", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = fylkeIder;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE FylkeID IN(" + fylkeIder + ")");
            //// AddParameterString(cmd, "reolid", ReolID, 10)
            ////DataTable reolData = GetDataTable(cmd);
            //ReolCollection result = new ReolCollection();
            //foreach (DataRow row in reolData.Rows)
            //{
            //    Reol r = GetReolFromDataRow(row);
            //    result.Add(r);
            //}

            //return result; 
            #endregion
            RequestGetReolerByFylkeSearch request = new RequestGetReolerByFylkeSearch()
            {
                fylkeIder = fylkeIder
            };
            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolerByTeamSearch>), (int)HttpStatusCode.OK)]
        [Route("GetReolerByTeamSearch")]
        public IActionResult GetReolerByTeamSearch(string teamNames)
        {
            _logger.BeginScope("Inside into GetReolerByTeamSearch");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataTable reolData;
            //string currentTable = configController.CurrentReolTableName;

            //if (Strings.InStr(Strings.UCase(teamNames), "TEAMNAME") != 0)
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_teamNames", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = teamNames;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByTeamSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + teamNames);
            //else
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_teamNames", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = teamNames;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByTeamSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //// cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE TeamName= '" + teamNames + "'");
            //// 
            ////DataTable reolData = GetDataTable(cmd);
            //ReolCollection result = new ReolCollection();
            //foreach (DataRow row in reolData.Rows)
            //{
            //    Reol r = GetReolFromDataRow(row);
            //    result.Add(r);
            //}

            //return result; 
            #endregion

            RequestGetReolerByTeamSearch request = new RequestGetReolerByTeamSearch()
            {
               teamNames = teamNames
            };
            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolerByPostalZoneSearch>), (int)HttpStatusCode.OK)]
        [Route("GetReolerByPostalZoneSearch")]
        public IActionResult GetReolerByPostalZoneSearch(string postalZone)
        {
            _logger.BeginScope("Inside into GetReolerByPostalZoneSearch");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataTable reolData;
            //string currentTable = configController.CurrentReolTableName;
            //if (Strings.InStr(Strings.UCase(postalZone), "POSTNR") != 0)
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_postalZone", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = postalZone;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostalZoneSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + postalZone);
            //else
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_postalZone", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = postalZone;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostalZoneSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE Postnr= '" + postalZone + "'");
            //// Postnr in (2019, 2050)
            ////DataTable reolData = GetDataTable(cmd);
            //ReolCollection result = new ReolCollection();
            //foreach (DataRow row in reolData.Rows)
            //{
            //    Reol r = GetReolFromDataRow(row);
            //    result.Add(r);
            //}

            //return result; 
            #endregion
            RequestGetReolerByPostalZoneSearch request = new RequestGetReolerByPostalZoneSearch()
            {
                postalZone = postalZone
            };
            return Ok(_mediator.Send(request).Result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetReolerByPostboksSearch>), (int)HttpStatusCode.OK)]
        [Route("GetReolerByPostboksSearch")]
        public IActionResult GetReolerByPostboksSearch(string postboks)
        {
            _logger.BeginScope("Inside into GetReolerByPostboksSearch");
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //DataTable reolData;
            //string currentTable = configController.CurrentReolTableName;

            //if (Strings.InStr(Strings.UCase(postboks), "POSTNR") != 0)
            //{

            //    npgsqlParameters[0] = new NpgsqlParameter("p_postboks", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = postboks;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}

            ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + postboks);
            //else
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_postboks", NpgsqlTypes.NpgsqlDbType.Varchar);
            //    npgsqlParameters[0].Value = postboks;


            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
            //    }

            //}
            //// cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE Postnr= '" + postboks + "'");
            //// Postnr in (2019, 2050)
            //// DataTable reolData = GetDataTable(cmd);
            //ReolCollection result = new ReolCollection();
            //foreach (DataRow row in reolData.Rows)
            //{
            //    Reol r = GetReolFromDataRow(row);
            //    result.Add(r);
            //}

            //return result; 
            #endregion

            RequestGetReolerByPostboksSearch request = new RequestGetReolerByPostboksSearch()
            {
                postboks = postboks
            };
            return Ok(_mediator.Send(request).Result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGetReolerBySegmenterSearch), (int)HttpStatusCode.OK)]
        [Route("GetReolerByIndexedDemographySearch")]
        public IActionResult GetReolerByIndexedDemographySearch([FromBody] RequestGetReolerByIndexedDemographySearch options)
        {
            _logger.BeginScope("Inside into GetReolerByIndexedDemographySearch");
           
            return Ok(_mediator.Send(options).Result);
        }
        #endregion

        #region Private Methods
        private string GetAllReolsJSONFromTable(string tableName)
        {
            _logger.BeginScope("Inside into GetAllReolsJSONFromTable");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            StringBuilder result = new StringBuilder();
            DataTable reols;

            npgsqlParameters[0] = new NpgsqlParameter("_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                reols = dbhelper.FillDataTable("kspu_gdb.custom_getallreolinjsonformat", CommandType.StoredProcedure, npgsqlParameters);
            }
            int count = 1;
            foreach (DataRow row in reols.Rows)
            {
                if (count == 1)
                {
                    result.Append("[");
                }
                result.Append(row.ItemArray[0].ToString());
                result.Append(count < reols.Rows.Count ? "," : "]");
                count += 1;
            }
            _logger.LogInformation("Number of row returned {0}", result);
            _logger.LogDebug("Exiting from GetAllReolsJsonFromTable");

            return result.ToString();
        }

        private ReolCollection GetAllReolsFromTable(string tableName)
        {
            _logger.BeginScope("Inside into GetAllReolsFromTable");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            ReolCollection result = new ReolCollection();
            DataTable reols;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                reols = dbhelper.FillDataTable("kspu_gdb.custom_getallreolsfromtable", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow datarow in reols.Rows)
            {
                Reol reol = GetReolFromDataRow(datarow);
                result.Add(reol);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetAllReolsFromTable");

            return result;
        }

        private Reol GetReolFromDataRow(DataRow row)
        {
            Reol r = new Reol();
            // TODO: Add field names to configuration file
            r.ReolId = long.Parse(row["r_reol_id"].ToString());
            // // Ref: POB 5340 - Tilbakestilling av mappinglogikk Postnr og poststed skal nå hentes fra reolene 
            r.PostalZone = row["r_postnr"].ToString();
            r.PostalArea = row["r_poststed"].ToString();
            r.Name = row["r_reolnavn"].ToString();
            r.ReolNumber = row["r_reolnr"].ToString();
            r.Comment = row["r_kommentar"].ToString();
            r.KommuneId = row["r_kommuneid"].ToString();
            r.Kommune = row["r_kommune"].ToString();
            r.FylkeId = row["r_fylkeid"].ToString();
            r.Fylke = row["r_fylke"].ToString();
            r.TeamNumber = row["r_teamnr"].ToString();
            r.TeamName = row["r_teamnavn"].ToString();
            r.PrisSone = Convert.ToInt32(row["r_prissone"]);
            r.Antall = GetAntallFromDataRow(row);
            // TODO: Uncomment line below before deployment, so that avisdekning is read for reoler. Commented out now to speed up development.
            // If getAvisDekning Then r.AvisDeknings = DAAvisDekning.GetAvisDekning(r.ReolId)
            r.SegmentId = row["r_segment"].ToString();

            r.Description = row["r_beskrivelse"].ToString();
            r.RuteType = row["r_reoltype"].ToString();
            r.PostkontorNavn = row["r_pbkontnavn"].ToString();
            r.PrsEnhetsId = row["r_prsnr"].ToString();
            r.PrsName = row["r_prsnavn"].ToString();
            r.PrsDescription = row["r_prsbeskrivelse"].ToString();
            // Added for RDF
            r.Frequency = row["r_rutedistfreq"].ToString();
            r.SondagFlag = row["r_sondagflag"].ToString();
            if (string.IsNullOrWhiteSpace(r.Description))
                r.Description = row["r_pbkontnavn"].ToString();
            if (string.IsNullOrWhiteSpace(r.Description))
                r.Description = row["r_prsbeskrivelse"].ToString();
            // 08.08.2006 - Reolnavn skal brukes dersom den har verdi, ellers får den beskrivelse verdien
            if (string.IsNullOrWhiteSpace(r.Name))
                r.Name = r.Description;
            return r;
        }

        private static Dictionary<long, Reol> CreateReolDictionary(ReolCollection reoler)
        {
            Dictionary<long, Reol> dict = new Dictionary<long, Reol>();
            foreach (Reol reol in reoler)
                dict[reol.ReolId] = reol;
            return dict;
        }

        private AntallInformation GetAntallFromDataRow(DataRow row)
        {
            AntallInformation antallInfo = new AntallInformation();
            antallInfo.Businesses = Convert.ToInt32(row["r_vh"]);
            antallInfo.FarmersReserved = Convert.ToInt32(row["r_gb_res"]);
            antallInfo.Households = Convert.ToInt32(row["r_hh"]);
            antallInfo.HouseholdsReserved = Convert.ToInt32(row["r_hh_res"]);
            antallInfo.Houses = Convert.ToInt32(row["r_er"]);
            antallInfo.HousesReserved = Convert.ToInt32(row["r_er_res"]);
            antallInfo.PriorityHouseholdsReserved = Convert.ToInt32(row["r_p_hh_u_res"]);
            antallInfo.NonPriorityHouseholdsReserved = Convert.ToInt32(row["r_np_hh_u_res"]);
            antallInfo.PriorityBusinessReserved = Convert.ToInt32(row["r_p_vh_u_res"]);
            antallInfo.NonPriorityBusinessReserved = Convert.ToInt32(row["r_np_vh_u_res"]);

            return antallInfo;
        }

        private Dictionary<long, Reol> GetReolDictionary()
        {
            GetAllReols(); // Makes sure that dictionary has been cached
            return _ReolDictionaryCache[_ReolTableName];
        }
        #endregion

    }
}
