#region Namespaces
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Handler.UtvalgSaver;
using DataAccessAPI.HandleRequest.Request.SelectionDistribution;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Request.UtvalgSaver;
using DataAccessAPI.HandleRequest.Response.Utvalg;
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
using System.Text;
using static Puma.Shared.PumaEnum;

#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UtvalgController : ControllerBase
    {
        #region Variables
        private readonly ILogger<UtvalgController> _logger;
        private readonly ConfigController configController;
        //private readonly UtvalgListController utvalgListController;
        //private ILogger<UtvalgListController> _loggerUtvalglist;
        private ILogger<ReolController> _loggerreolcontroller;
        private ILogger<ConfigController> _loggerconfigcontroller;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="UtvalgController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="mediator">The mediator.</param>
        public UtvalgController(ILogger<UtvalgController> logger, ILogger<ConfigController> loggerConfig, ILogger<ReolController> loggerreol,
             IMediator mediator = null)
        {
            _logger = logger;
            _mediator = mediator;

            // _loggerUtvalglist = loggerUtvalglist;
            _loggerreolcontroller = loggerreol;
            _loggerconfigcontroller = loggerConfig;
            // _loggerUtvalglist = loggerutvalglist;
            configController = new ConfigController(loggerConfig);
            //utvalgListController = new UtvalgListController(loggerutvalglist, loggerConfig, logger, loggerreol);


        }
        #endregion

        #region Public Methods

        //public readonly string errMsgUtvalgName = "Utvalgsnavnet må ha minst 3 tegn.";
        //public readonly string errMsgIllegalCharsUtv = "Utva.lgsnavnet inneholder ulovlige tegn. Fjern tegnene '<' og '>' dersom eksisterer i navnet.";
        //public readonly string errMsgUtvalgNameWithSpaces = "Utvalgsnavnet kan ikke ha mellomrom i begynnelsen eller slutten av navnet. Fjern mellomrom og prøv på nytt.";
        //public readonly string errMsgUtvalgHasNoReceivers = "Utvalget har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt.";

        [HttpPost("SaveUtvalg", Name = nameof(SaveUtvalg))]
        public IActionResult SaveUtvalg([FromBody] Utvalg utv, string userName, bool saveOldReoler = false, bool skipHistory = false, int forceUtvalgListId = 0)
        {
            _logger.BeginScope("Inside into SaveUtvalg");
            RequestSaveUtvalg request = new RequestSaveUtvalg()
            {
                forceUtvalgListId = forceUtvalgListId,
                saveOldReoler = saveOldReoler,
                skipHistory = skipHistory,
                userName = userName,
                utvalg = utv
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Saves the utvalgs.
        /// </summary>
        /// <param name="requestSaveUtvalgs">The request save utvalgs.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="saveOldReoler">if set to <c>true</c> [save old reoler].</param>
        /// <param name="skipHistory">if set to <c>true</c> [skip history].</param>
        /// <param name="forceUtvalgListId">The force utvalg list identifier.</param>
        /// <returns></returns>
        [HttpPost("SaveUtvalgs", Name = nameof(SaveUtvalgs))]
        public IActionResult SaveUtvalgs([FromBody] RequestSaveUtvalgs requestSaveUtvalgs, string userName, bool saveOldReoler = false, bool skipHistory = false, int forceUtvalgListId = 0)
        {
            _logger.BeginScope("Inside into SaveUtvalgs");

            requestSaveUtvalgs.userName = userName;
            requestSaveUtvalgs.saveOldReoler = saveOldReoler;
            requestSaveUtvalgs.skipHistory = skipHistory;
            requestSaveUtvalgs.forceUtvalgListId = forceUtvalgListId;

            return Ok(_mediator.Send(requestSaveUtvalgs).Result);
        }

        [HttpPost("DecoupleUtvalgsFromLists", Name = nameof(DecoupleUtvalgsFromLists))]
        public IActionResult DecoupleUtvalgsFromLists([FromBody] RequestUtvalgSaver request, string userName)
        {
            _logger.BeginScope("Inside into DecoupleUtvalgsFromLists");
            request.userName = userName;
            //utvalg = utv


            return Ok(_mediator.Send(request).Result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagByUtvalgId>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgs")]
        public IActionResult GetUtvalgs([FromBody] RequestGetUtvalgByUtvalgIds request)
        {
            _logger.BeginScope("Inside into GetUtvalgs");

            return Ok(_mediator.Send(request).Result);
        }

        //[HttpPost("Test", Name = nameof(Test))]
        //public IActionResult Test([FromBody] RequestUtvalgSaverTest request, string userName)
        //{

        //    //request.userName = userName;
        //    //utvalg = utv


        //    return Ok(_mediator.Send(request).Result);
        //}

        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsUtvalgSkrivebeskyttetInDB")]
        public IActionResult IsUtvalgSkrivebeskyttetInDB(int utvalgId)
        {
            _logger.BeginScope("Inside into IsUtvalgSkrivebeskyttetInDB");
            #region Old Code
            //_logger.LogDebug("Inside into IsUtvalgSkrivebeskyttetInDB");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //object result;

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgskrivebeskyttetindb", CommandType.StoredProcedure, npgsqlParameters);
            //}


            //if (result == DBNull.Value)
            //{
            //    _logger.LogInformation("Is Utvalg Skrivebeskyttet In DB: False");
            //    _logger.LogDebug("Exiting from IsUtvalgSkrivebeskyttetInDB");
            //    return false;
            //}
            //_logger.LogInformation("Is Utvalg Skrivebeskyttet In DB: ", (bool)result);
            //_logger.LogDebug("Exiting from IsUtvalgSkrivebeskyttetInDB");
            //return (bool)result; 
            #endregion
            RequestIsUtvalgSkrivebeskyttetInDB request = new RequestIsUtvalgSkrivebeskyttetInDB()
            {
                utvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// GetUtvalg With All References
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>Utvalg data</returns>

        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtlvagDetailById), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgWithAllReferences")]
        public IActionResult GetUtvalgWithAllReferences(int utvalgId)
        {
            _logger.BeginScope("Inside into GetUtvalgWithAllReferences");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgWithAllReferences");

            ////if (utvalgListController.IsUtvalgConnectedToParentList(utvalgId))
            ////{
            ////    return utvalgListController.GetRootUtvalgListWithAllReferences(utvalgId).GetUtvalgDescendant(utvalgId);

            ////}
            ////else
            ////{
            //_logger.LogDebug("Exiting from GetUtvalgWithAllReferences");
            //return GetUtvalg(utvalgId);
            ////} 
            #endregion

            RequestGetUtlvagDetailById request = new RequestGetUtlvagDetailById()
            {
                UtlvagId = utvalgId,
                CurrentReolTableName = configController.CurrentReolTableName
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Used by ErgoInterfaces.UsagePatternMethods
        /// Fetches a datatable containing usage patterns for the specified period.
        /// </summary>
        /// <param name="fromDateInclusive"></param>
        /// <param name="toDateInclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpGet("GetUsagePattern", Name = nameof(GetUsagePattern))]
        public DataTable GetUsagePattern(DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            _logger.LogDebug("Inside into GetUsagePattern");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromdateinclusive", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDateInclusive.Date;

            npgsqlParameters[1] = new NpgsqlParameter("p_todateinclusive", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDateInclusive.Date.AddDays(1);

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                utvData = dbhelper.FillDataTable("kspu_db.getusagepattern", CommandType.StoredProcedure, npgsqlParameters);
            }
            _logger.LogDebug("Exiting from GetUsagePattern");
            return utvData;
        }





        /// <summary>
        /// Get Utvalg Data
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch data from Utvalg table</param>
        /// <param name="getReolsummarizeData">set true if want to get reol summarize Data</param>
        /// <returns>Utvalg data</returns>

        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetUtlvagByUtvalgId), (int)HttpStatusCode.OK)]
        [Route("GetUtvalg")]
        public IActionResult GetUtvalg(int utvalgId,bool getReolsummarizeData=false)
        {
            _logger.BeginScope("Inside into GetUtvalg");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalg");
            //DataTable utvData;
            //Exception exception = null;

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;


            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getutvalg", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //if (utvData.Rows.Count != 1)
            //{
            //    exception = new Exception("Fant ikke unikt utvalg med utvalgsid " + utvalgId + " i databasen.");
            //    throw exception;
            //}

            //_logger.LogInformation("Number of row returned: ", utvData.Rows.Count);

            //_logger.LogDebug("Exiting from GetUtvalg");
            //return GetUtvalgFromDataRow(utvData.Rows[0]); 
            #endregion
            RequestGetUtvalgByUtvalgId request = new RequestGetUtvalgByUtvalgId()
            {
                utlvagId = utvalgId,
                CurretnReolTableName = configController.CurrentReolTableName,
                GetsummarizeData =getReolsummarizeData
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        ///  Return username for user that last saved a utvalg
        ///  </summary>
        ///  <param name="utvalgId"></param>
        ///  <returns></returns>

        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("LastSavedBy")]
        public IActionResult LastSavedBy(int utvalgId)
        {
            _logger.BeginScope("Inside into LastSavedBy");
            #region Old Code
            //_logger.LogDebug("Inside into LastSavedBy");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<string>("kspu_db.lastsavedby", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from LastSavedBy");
            //if ((result) is DBNull)
            //    return null;
            //else
            //    return System.Convert.ToString(result); 
            #endregion
            RequestGetUtlvagLastSavedBy request = new RequestGetUtlvagLastSavedBy()
            {
                UtvalgId = utvalgId,
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Does a utvalg with a spesific name exist in the database?
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <returns>boolean</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UtvalgNameExists")]
        public IActionResult UtvalgNameExists(string utvalgNavn)
        {
            _logger.BeginScope("Inside into UtvalgNameExists");
            #region Old Code
            //_logger.LogDebug("Inside into UtvalgNameExists");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utvalgNavn;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalgnameexists", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from UtvalgNameExists");
            //if (result == null | (result) is DBNull)
            //    return false;
            //return System.Convert.ToInt32(result) > 0; 
            #endregion
            RequestUtvagNameExists request = new RequestUtvagNameExists()
            {
                UtvalgName = utvalgNavn
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// It will return true if any of utvalg name from list exists else will return false
        /// </summary>
        /// <param name="utvalgNavns"></param>
        /// <returns>boolean</returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UtvalgNamesExists")]
        public IActionResult UtvalgNamesExists([FromBody]List<string> utvalgNavns)
        {
            _logger.BeginScope("Inside into UtvalgNamesExists");
            
            RequestUtvagsNameExists request = new RequestUtvagsNameExists()
            {
                utvalgNames = utvalgNavns
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// Search Utvalg Simple
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <param name="searchMethod"></param>
        /// <returns>UtvalgSearchCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgByName>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgSimple")]
        public IActionResult SearchUtvalgSimple(string utvalgNavn, SearchMethod searchMethod)
        {
            _logger.BeginScope("Inside into SearchUtvalgSimple");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgSimple");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgSearchCollection result = new UtvalgSearchCollection();


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            ////npgsqlParameters[0].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            //npgsqlParameters[0].Value = utvalgNavn;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            ////npgsqlParameters[1].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow datarow in utvData.Rows)
            //    result.Add(GetUtvalgSearchResultFromDataRow(datarow));

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgSimple");
            //return result; 
            #endregion
            RequestSearchUtvalgByName request = new RequestSearchUtvalgByName()
            {
                UtvalgName = utvalgNavn,
                SearchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///  Search Utvalg by Utvalg ID
        ///  </summary>
        ///  <param name="userID"></param>
        ///  <param name="searchMethod"></param>
        ///   <returns>UtvalgSearchCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgByUserId>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUserID1")]
        public IActionResult SearchUtvalgByUserID1(string userID, SearchMethod searchMethod)
        {
            _logger.BeginScope("Inside into SearchUtvalgByUserID1");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByUserID");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgSearchCollection result = new UtvalgSearchCollection();


            //npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = userID;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuserid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgSearchResultFromDataRow(row));

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgByUserID");
            //return result; 
            #endregion
            RequestSearchUtvalgByUserId request = new RequestSearchUtvalgByUserId()
            {
                UserId = userID,
                SearchMethod = searchMethod
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search by user id and/or customer number and agreement number, and you may only select Basisutvalg
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="customerNos"></param>
        /// <param name="agreementNos"></param>
        /// <param name="forceCustomerAndAgreementCheck"></param>
        /// <param name="onlyBasisUtvalg"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailByUserIdAndCustNo>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUserIDAndCustNo")]
        public IActionResult SearchUtvalgByUserIDAndCustNo(string userID, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, int onlyBasisUtvalg)
        {
            _logger.BeginScope("Inside into SearchUtvalgByUserIDAndCustNo");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByUserIDAndCustNo");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //UtvalgCollection result = new UtvalgCollection();


            //npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = "%" + userID + "%";

            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = "'" + String.Join("', '", customerNos) + "'";

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = "'" + String.Join("', '", agreementNos) + "'";

            //npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";


            //if (forceCustomerAndAgreementCheck)
            //{
            //    if (utils.CanSearch(customerNos, agreementNos))
            //    {
            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuseridcustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //    }
            //    else
            //        // no search
            //        return result;
            //}
            //else
            //{
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuseridcustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}

            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgSearchFromDataRow(row));

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalgByUserIDAndCustNo");
            //return result; 
            #endregion

            RequestSearchUtvalgByUserIdandCustNo request = new RequestSearchUtvalgByUserIdandCustNo()
            {
                agreementNos = agreementNos,
                customerNos = customerNos,
                forceCustomerAndAgreementCheck = forceCustomerAndAgreementCheck,
                onlyBasisUtvalg = onlyBasisUtvalg,
                userID = userID
            };
            return Ok(_mediator.Send(request).Result);
        }

        protected int GetSequenceNextVal(string sequenceName)
        {
            _logger.BeginScope("Inside into GetSequenceNextVal");
            int nextVal;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_sqlorderby", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = sequenceName;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                nextVal = dbhelper.ExecuteScalar<int>("kspu_gdb.GetSequenceNextVal", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" SELECT " + sequenceName + ".NEXTVAL FROM dual ", trans.Connection, trans);
            //int nextVal = System.Convert.ToInt32(ExecuteScalar(cmd));
            return nextVal;
        }




        /// <summary>
        ///      Search by user name and/or customer number and agreement number, and you may only select Basisutvalg
        ///      </summary>
        ///      <param name="utvalgNavn"></param>
        ///      <param name="customerNos"></param>
        ///      <param name="agreementNos"></param>
        ///      <param name="forceCustomerAndAgreementCheck"></param>
        ///      <param name="extendedInfo"></param>
        ///      <param name="onlyBasisUtvalg"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailByNameAndCustNo>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUtvalgName")]
        public IActionResult SearchUtvalgByUtvalgName(string utvalgNavn, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisUtvalg)
        {
            _logger.BeginScope("Inside into SearchUtvalgByUtvalgName");
            #region Old Code
            //var jsonstring = new StringBuilder();
            //_logger.LogDebug("Inside into SearchUtvalg");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //UtvalgCollection result = new UtvalgCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //if (utvalgNavn != null)
            //{
            //    npgsqlParameters[0].Value = "%" + utvalgNavn + "%";
            //}
            //else
            //{
            //    npgsqlParameters[0].Value = "";
            //}



            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";

            //npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";


            //if (forceCustomerAndAgreementCheck)
            //{
            //    if (utils.CanSearch(customerNos, agreementNos))
            //    {
            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgnamecustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //        }

            //    }
            //    else
            //        // no search
            //        return result;
            //}
            //else
            //{
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgnamecustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //    }

            //}


            //foreach (DataRow row in utvData.Rows)
            //{
            //    Utvalg u = GetUtvalgSearchFromDataRow(row);
            //    // add last saved by user if extendedInfo= true
            //    if (extendedInfo)
            //    {
            //        GetUtvalgModifications(u, false);
            //    }
            //    GetUtvalgReoler(u);
            //    GetUtvalgReceiver(u);
            //    GetUtvalgCriteria(u);
            //    GetUtvalgOldReoler(u, Convert.ToInt32(row["r_utvalgid"]));
            //    result.Add(u);

            //}
            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalg");
            //return result; 
            #endregion

            RequestSearchUtvalgByNameandCustNo request = new RequestSearchUtvalgByNameandCustNo()
            {
                agreementNos = agreementNos,
                customerNos = customerNos,
                forceCustomerAndAgreementCheck = forceCustomerAndAgreementCheck,
                onlyBasisUtvalg = onlyBasisUtvalg,
                UtvalgName = utvalgNavn,
                extendedInfo = extendedInfo,
                currentReolTableBame = configController.CurrentReolTableName
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search Utvalg by Utvalg Name
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <param name="searchMethod"></param>
        /// <param name="includeReoler"></param>
        /// <returns>UtvalgSearchCollection</returns>

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailByName>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalg")]
        public IActionResult SearchUtvalg(string utvalgNavn, SearchMethod searchMethod, bool includeReoler = true)
        {
            _logger.BeginScope("Inside into SearchUtvalg");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalg");
            //DataTable utvData;
            //Utils utils = new Utils();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgCollection result = new UtvalgCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utvalgNavn;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalg", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgFromDataRow(row, includeReoler));

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalg");
            //return result; 
            #endregion
            RequestGetUtvalgDetailByName request = new RequestGetUtvalgDetailByName()
            {
                includeReoler = includeReoler,
                searchMethod = searchMethod,
                UtvalgName = utvalgNavn,
                currentReolTableBame = configController.CurrentReolTableName
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search Utvalg by Utvalg ID
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <param name="includeReols"></param>
        /// <param name="customerNos"></param>
        /// <returns>UtvalgSearchCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailById>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUtvalgId")]
        public IActionResult SearchUtvalgByUtvalgId(int utvalgId, bool includeReols = true,string customerNos = "")
        {
            _logger.BeginScope("Inside into SearchUtvalgByUtvalgId");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByUtvalgId");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgCollection result = new UtvalgCollection();


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgFromDataRow(row, includeReols));

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalgByUtvalgId");
            //return result; 
            #endregion
            RequestSearchUtvalgDetailById request = new RequestSearchUtvalgDetailById()
            {
                includeReols = includeReols,
                UtlvagId = utvalgId,
                CurrentReolTableName = configController.CurrentReolTableName,
                customerNos= customerNos
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get Utvalg Campaigns
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns>CampaignDescription</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailById>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgCampaigns")]
        public IActionResult GetUtvalgCampaigns(int utvalgId)
        {
            _logger.BeginScope("Inside into GetUtvalgCampaigns");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgCampaigns");
            //DataTable utvData;
            //DataTable utvData2;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<CampaignDescription> utvColl = new List<CampaignDescription>();


            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getutvalgcampaignsbybasedon", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    utvColl.Add(GetCampaignDescriptionFromUtvalgDataRow(row, false));

            //NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

            //npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters1[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData2 = dbhelper.FillDataTable("kspu_db.getutvalgcampaignsbywasbasedon", CommandType.StoredProcedure, npgsqlParameters1);
            //}
            //foreach (DataRow row in utvData2.Rows)
            //    utvColl.Add(GetCampaignDescriptionFromUtvalgDataRow(row, true));

            //_logger.LogInformation("Number of row returned: ", utvColl);
            //_logger.LogDebug("Exiting from GetUtvalgCampaigns");
            //return utvColl; 
            #endregion
            RequestGetUtlvagCampaignsByUtvalgId request = new RequestGetUtlvagCampaignsByUtvalgId()
            {
                UtvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// Get Utvalg Name
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns>Utvalg name</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgName")]
        public IActionResult GetUtvalgName(int utvalgId)
        {
            _logger.BeginScope("Inside into GetUtvalgName");
            #region old Code
            //_logger.LogDebug("Inside into GetUtvalgName");
            //string result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[0].Value = utvalgId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<string>("kspu_db.getutvalgname", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (result == null) // | result == DBNull.Value)
            //    return null;
            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from GetUtvalgName");
            //return result; 
            #endregion
            RequestGetUtlvagNameByUtvalgId request = new RequestGetUtlvagNameByUtvalgId()
            {
                UtvagId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search by Utvalg ID and customer number
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <param name="agreementNos"></param>
        /// <param name="customerNos"></param>
        /// <param name="includeReols"></param>
        /// <param name="forceCustomerAndAgreementCheck"></param>
        /// <param name="extendedInfo"></param>
        /// <param name="onlyBasisUtvalg"></param>    
        /// <returns></returns>
        /// <remarks></remarks>

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUtvalgIdAndCustmerNo")]
        public IActionResult SearchUtvalgByUtvalgIdAndCustmerNo(int utvalgId, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool includeReols, bool extendedInfo, int onlyBasisUtvalg)
        {
            _logger.BeginScope("Inside into SearchUtvalgByUtvalgIdAndCustmerNo");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByUtvalgIdAndCustmerNo");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //UtvalgCollection result = new UtvalgCollection();
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = "'" + String.Join("', '", customerNos) + "'";

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = "'" + String.Join("', '", agreementNos) + "'";


            //npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";


            //if (forceCustomerAndAgreementCheck)
            //{
            //    if (utils.CanSearch(customerNos, agreementNos))
            //    {
            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgidandcustmernoandagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //    }
            //    else
            //        // no search
            //        return result;
            //}
            //else
            //{
            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgidandcustmernoandagreementno", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}

            //foreach (DataRow row in utvData.Rows)
            //{
            //    Utvalg u = GetUtvalgFromDataRow(row, includeReols);
            //    // add last saved by user if extendedInfo= true
            //    if (extendedInfo)
            //        GetUtvalgModifications(u, false);
            //    result.Add(u);
            //}

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalgByUtvalgIdAndCustmerNo");
            //return result; 
            #endregion

            RequestSearchUtvalgByUtvalgIdAndCustoNo request = new RequestSearchUtvalgByUtvalgIdAndCustoNo()
            {
                UtvagId = utvalgId,
                agreementNos = agreementNos,
                CurretnReolTableName = configController.CurrentReolTableName,
                customerNos = customerNos,
                onlyBasisUtvalg = onlyBasisUtvalg,
                ExtendInfo = extendedInfo,
                forceCustomerAndAgreementCheck = forceCustomerAndAgreementCheck,
                IncludeReol = includeReols
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Searches the utvalg by kunde nr.
        /// </summary>
        /// <param name="KundeNummer">The kunde nummer.</param>
        /// <param name="searchMethod">The search method.</param>
        /// <param name="includeReols">if set to <c>true</c> [include reols].</param>
        /// <param name="extendedInfo">if set to <c>true</c> [extended information].</param>
        /// <param name="onlyBasisUtvalg">if set to <c>true</c> [extended information].</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgByKundeNr>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByKundeNr")]
        public IActionResult SearchUtvalgByKundeNr(string KundeNummer, SearchMethod searchMethod, bool includeReols = true, bool extendedInfo = true, bool onlyBasisUtvalg = false)
        {
            _logger.BeginScope("Inside into SearchUtvalgByKundeNr");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByKundeNr");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //UtvalgCollection result = new UtvalgCollection();
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = KundeNummer;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbykundenr", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgFromDataRow(row, includeReols));

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalgByKundeNr");
            //return result; 
            #endregion
            RequesSearchUtvalgByKundeNr request = new RequesSearchUtvalgByKundeNr()
            {
                CurrentReolTableName = configController.CurrentReolTableName,
                IncludeReols = includeReols,
                SearchMethod = searchMethod,
                KundeNummer = KundeNummer,
                ExtendedInfo = extendedInfo,
                onlyBasisUtvalg = onlyBasisUtvalg
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search Utvalg by Utvalg list ID
        /// </summary>
        /// <param name="utvalgListId"></param>
        /// <param name="includeReols"></param>
        /// <returns>UtvalgSearchCollection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgByUtvalListId>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByUtvalListId")]
        public IActionResult SearchUtvalgByUtvalListId(int utvalgListId, bool includeReols = true)
        {
            _logger.BeginScope("Inside into SearchUtvalgByUtvalListId");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByUtvalListId");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //UtvalgCollection result = new UtvalgCollection();

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgListId;
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvallistid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow dataRow in utvData.Rows)
            //    result.Add(GetUtvalgFromDataRow(dataRow, includeReols));

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from SearchUtvalgByUtvalListId");
            //return result; 
            #endregion
            RequestSearchUtvalgByUtvalListId request = new RequestSearchUtvalgByUtvalListId()
            {
                CurrentReolTableName = configController.CurrentReolTableName,
                IncludeReols = includeReols,
                UtlvagId = utvalgListId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Search Utvalg by Utvalg list ID
        /// </summary>
        /// <param name="OrdreReferanse"></param>
        /// <param name="includeReols"></param>
        /// <param name="OrdreType"></param>
        /// <param name="searchMethod"></param>
        /// <returns>UtvalgSearchCollection</returns>

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSearchUtvalgByOrdreReferanse>), (int)HttpStatusCode.OK)]
        [Route("SearchUtvalgByOrdreReferanse")]
        public IActionResult SearchUtvalgByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod, bool includeReols = true)
        {
            _logger.BeginScope("Inside into SearchUtvalgByOrdreReferanse");
            #region Old Code
            //_logger.LogDebug("Inside into SearchUtvalgByOrdreReferanse");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            //UtvalgCollection result = new UtvalgCollection();
            //Utils utils = new Utils();

            //npgsqlParameters[0] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = OrdreReferanse;
            //npgsqlParameters[1] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            //npgsqlParameters[1].Value = OrdreType;
            //npgsqlParameters[2] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 10);
            //npgsqlParameters[2].Value = Convert.ToInt32(searchMethod);

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyordrereferanse", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add(GetUtvalgFromDataRow(row, includeReols));

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from SearchUtvalgByOrdreReferanse");
            //return result; 
            #endregion
            RequestSearchUtvalgByOrdreReferanse request = new RequestSearchUtvalgByOrdreReferanse()
            {
                CurrentReolTableName = configController.CurrentReolTableName,
                IncludeReols = includeReols,
                SearchMethod = searchMethod,
                OrdreReferanse = OrdreReferanse,
                OrdreType = OrdreType

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Is Ordered selection, ready for update request
        /// </summary>
        /// <param name="deliveryDate">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsOrderedUtvalgAndListsReadyForUpdateRequest")]
        public IActionResult IsOrderedUtvalgAndListsReadyForUpdateRequest(DateTime deliveryDate)
        {
            _logger.BeginScope("Inside into IsOrderedUtvalgAndListsReadyForUpdateRequest");
            #region Old Code
            //_logger.LogDebug("Inside into IsOrderedUtvalgAndListsReadyForUpdateRequest");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //object result;

            //npgsqlParameters[0] = new NpgsqlParameter("p_deliverydate", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[0].Value = deliveryDate;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.isorderedutvalgandlistsreadyforupdaterequest", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from IsOrderedUtvalgAndListsReadyForUpdateRequest");
            //if (result is decimal)
            //    return System.Convert.ToInt32(result) == 0;
            //return System.Convert.ToInt32(result) == 1; 
            #endregion
            RequestIsOrderedUtvalgAndListsReadyForUpdate request = new RequestIsOrderedUtvalgAndListsReadyForUpdate()
            {
                DeliveryDate = deliveryDate

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Is utvalg list ordered
        /// </summary>
        /// <param name="ID">Utvalg List ID</param>
        /// <returns>True or false</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsUtvalgListOrdered")]
        public IActionResult IsUtvalgListOrdered(long ID)
        {
            _logger.BeginScope("Inside into IsUtvalgListOrdered");
            #region Old Code
            //_logger.LogDebug("Inside into IsUtvalgListOrdered");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //object result;

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = ID;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalglistordered", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from IsUtvalgListOrdered");
            //if (result is decimal)
            //    return System.Convert.ToInt32(result) > 0;
            //return System.Convert.ToInt32(result) < 0; 
            #endregion
            RequestIsUtvalgListOrdered request = new RequestIsUtvalgListOrdered()
            {
                ID = ID

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Is utvalg ordered
        /// </summary>
        /// <param name="ID">Utvalg ID</param>
        /// <returns>True or false</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("IsUtvalgOrdered")]
        public IActionResult IsUtvalgOrdered(long ID)
        {
            _logger.BeginScope("Inside into IsUtvalgOrdered");
            #region Old Code
            //_logger.LogDebug("Inside into IsUtvalgOrdered");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //object result;

            //npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = ID;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgordered", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from IsUtvalgOrdered");
            //if (result is decimal)
            //    return System.Convert.ToInt32(result) > 0;
            //return System.Convert.ToInt32(result) < 0; 
            #endregion
            RequestIsUtvalgOrdered request = new RequestIsUtvalgOrdered()
            {
                ID = ID

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///  Get selection for update request
        ///  </summary>
        ///  <param name="deliveryDate"></param>
        ///  <returns>AutoUpdateMessage list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseOrderedUtvalgAndListsForUpdate>), (int)HttpStatusCode.OK)]
        [Route("GetOrderedUtvalgAndListsForUpdateRequest")]
        public IActionResult GetOrderedUtvalgAndListsForUpdateRequest(DateTime deliveryDate)
        {
            _logger.BeginScope("Inside into GetOrderedUtvalgAndListsForUpdateRequest");
            #region Old Code
            //_logger.LogDebug("Inside into GetOrderedUtvalgAndListsForUpdateRequest");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //List<AutoUpdateMessage> result = new List<AutoUpdateMessage>();

            //npgsqlParameters[0] = new NpgsqlParameter("p_dato", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[0].Value = deliveryDate;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getorderedutvalgandlistsforupdaterequest", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //{
            //    try
            //    {
            //        AutoUpdateMessage id = new AutoUpdateMessage();
            //        id.Id = (int)row["utvalgsid"];
            //        id.IsList = (bool)Interaction.IIf(row["utvalgtype"].ToString().ToLower().Equals("liste"), true, false);
            //        if ((utvData.Columns.Contains("ordrereferanse")) && (!row.IsNull("ordrereferanse")))
            //            id.Ordrereferanse = row["ordrereferanse"].ToString();
            //        result.Add(id);
            //    }

            //    catch (Exception exception)
            //    {
            //        _logger.LogError(exception, exception.Message);
            //        _logger.LogError(exception, "Feil i parsing av oppdateringslista for utvalg og lister. Hoppet over " + row["utvalgsid"] + ".");
            //    }
            //}

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from GetOrderedUtvalgAndListsForUpdateRequest");
            //return result; 
            #endregion
            RequestOrderedUtvalgAndListsForUpdate request = new RequestOrderedUtvalgAndListsForUpdate()
            {
                DeliveryDate = deliveryDate

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// For integration: Get all distinct PRS from reoler in Utvalg.
        /// Uses current ReolMap, independent of recreation done, needed or not.
        /// </summary>
        /// <param name="utvalgid"></param>
        /// <returns>PRS list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetDistinctPRS")]
        public IActionResult GetDistinctPRS(int utvalgid)
        {
            _logger.BeginScope("Inside into GetDistinctPRS");
            #region Old Code
            //_logger.LogDebug("Inside into GetDistinctPRS");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //List<string> result = new List<string>();
            ////var prsField = "prsnr";
            ////int utvalgId = utvalg.UtvalgId;
            //string table = configController.CurrentReolTableName;
            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgid;

            //npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = table;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getdistinctprs", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //{
            //    string s = row[0].ToString();
            //    result.Add(s);
            //}

            //_logger.LogInformation("Number of row returned: ", result);
            //_logger.LogDebug("Exiting from GetDistinctPRS");
            //return result; 
            #endregion
            RequestGetDistinctPRSByUtvalgId request = new RequestGetDistinctPRSByUtvalgId()
            {
                CurrentReolTableName = configController.CurrentReolTableName,
                Utvalgid = utvalgid

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Get Utvalg Reoler Count
        ///     ''' </summary>
        ///     ''' <param name="utvalgID"></param>
        ///     ''' <returns>Reoler count</returns>
        [HttpGet("GetUtvalgReolerCount", Name = nameof(GetUtvalgReolerCount))]
        public int GetUtvalgReolerCount(long utvalgID)
        {
            _logger.LogDebug("Inside into GetUtvalgReolerCount");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgID;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_db.getutvalgreolercount", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result.ToString() == null) // | (result.ToString()) is DBNull)
                return 0;

            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from GetUtvalgReolerCount");
            return System.Convert.ToInt32(result);
        }

        //[HttpGet("GetDemographyIndexInfoForReol", Name = nameof(GetDemographyIndexInfoForReol))]
        //public DataTable GetDemographyIndexInfoForReol(Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        //{
        //    _logger.LogDebug("Inside into GetDemographyIndexInfoForReol");
        //    DataTable utvData;
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
        //    DemographyValuesCollection result = new DemographyValuesCollection();

        //    string table = configController.CurrentReolTableName;
        //    //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
        //    //npgsqlParameters[0].Value = utv.UtvalgId;

        //    npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tablename1;

        //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[1].Value = tablename2;

        //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //    {
        //        utvData = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
        //    }
        //    utvData.Columns.Add("r_budrute"); 
        //    // result.Columns("Budrute").SetOrdinal(1) 'Move column to position 1
        //    utvData.Columns.Add("r_antall", typeof(int));
        //    utvData.Columns.Add("r_fylke", typeof(string));
        //    utvData.Columns.Add("r_kommune", typeof(string));
        //    utvData.Columns.Add("r_team", typeof(string));

        //    ReolCollection sortedReols = new ReolCollection(utv.Reoler);
        //    // sortedReols.SortByReolNumber()

        //    foreach (Reol r in sortedReols)
        //    {
        //        foreach (DataRow row in utvData.Rows)
        //        {
        //            if (r.ReolId.ToString() == row[0].ToString())
        //            {
        //                row["r_budrute"] = r.Name;
        //                row["r_antall"] = r.Antall.GetTotalAntall(utv.Receivers);
        //                row["r_fylke"] = r.Fylke;
        //                row["r_kommune"] = r.Kommune;
        //                row["r_team"] = r.TeamName;
        //            }
        //        }
        //    }

        //    if (utvData.Columns.Contains("r_fylke") && utvData.Columns.Contains("r_kommune") && utvData.Columns.Contains("r_team") && utvData.Columns.Contains("r_reolnr"))
        //        utvData.DefaultView.Sort = "r_fylke, r_kommune, r_team, r_reolnr";

        //    _logger.LogInformation("Number of row returned: ", utvData);
        //    _logger.LogDebug("Exiting from GetDemographyIndexInfoForReol");
        //    return utvData;
        //}

        #region Not in use for now

        //[HttpPost("GetDemographyIndexInfoForReol", Name = nameof(GetDemographyIndexInfoForReol))]
        //public DataTable GetDemographyIndexInfoForReol([FromBody] Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        //{
        //    DataTable result;
        //    //var q = "main";
        //    // int startPos= demographyOpts.SQLOrderby.IndexOf(q);

        //    int startPos = demographyOpts.SQLOrderby.IndexOf(tablename1);
        //    if (startPos == -1)
        //        startPos = demographyOpts.SQLOrderby.IndexOf(tablename1);
        //    startPos += 2;
        //    int endPos = demographyOpts.SQLOrderby.IndexOf(tablename2);
        //    if (endPos == -1)
        //        endPos += 2;
        //    // Dim options As String() = demographyOpts.SQLOrderby.Substring(startPos, endPos - startPos).Replace("main.", "").Replace("indeks.", "").Split("+")
        //    string[] options = demographyOpts.SQLOrderby.Substring(startPos, endPos).Split("");
        //    //string[] options = demographyOpts.SQLOrderby.Substring(startPos, endPos - startPos).Replace("main.", "").Replace("indeks.", "").Split("+");
        //    // Dim sqlSelect As String = "Select reol_id, "
        //    string sqlSelect = "Select main.reol_id, main.reolnr, ";

        //    string comma = "";
        //    string plus = "";
        //    StringBuilder indexSumQuery = new StringBuilder("round((");
        //    ConfigController configController = new ConfigController(_loggerconfigcontroller);
        //    foreach (string a in options)
        //    {
        //        a.Trim();
        //        sqlSelect += comma + "round(" + a + ") as \"" + configController.GetDemografiCriteriaText(a.Split(".")[1]) + "\"";
        //        indexSumQuery.Append(plus);
        //        indexSumQuery.Append(a);
        //        comma = ", ";
        //        plus = "+";
        //    }

        //    indexSumQuery.Append(")/" + options.Length + ") as Indeksverdi");
        //    // gjennomsnittlig indeks fjernes
        //    // sqlSelect += comma & indexSumQuery.ToString()

        //    // Dim sqlInClause As String = " from kspu_gdb.norway_reol where reol_id in ("

        //    //string sqlInClause = " from kspu_gdb." + Config.Demografi_Maintable + " main inner join kspu_gdb." + Config.Demografi_Indekstable + "  indeks ";
        //    //sqlInClause += " on main.reol_id = indeks.reol_id";
        //    // DataTable utvData;
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
        //    npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tablename1;

        //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[1].Value = tablename2;

        //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //    {
        //        result = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
        //    }

        //    if (utv.Reoler.Count > 0)
        //    {
        //        //sqlInClause += " where main.reol_id in (";
        //        //comma = "";

        //        int maxINCounter = 0;
        //        foreach (Reol r in utv.Reoler)
        //        {
        //            maxINCounter = maxINCounter + 1;
        //            if (maxINCounter == 1000)
        //            {
        //                //sqlInClause += ") or main.reol_id in (";
        //                comma = "";
        //                maxINCounter = 0;
        //            }
        //            //sqlInClause += comma + r.ReolId.ToString();
        //            comma = ", ";
        //        }

        //        //sqlInClause += ")";
        //    }
        //    else
        //        //sqlInClause += " where main.reol_id = -1";

        //        //sqlSelect += sqlInClause;

        //        //OracleCommand cmd = new OracleCommand(sqlSelect);
        //        //result = GetDataTable(cmd, true);
        //        result.Columns.Add("Budrute");
        //    // result.Columns("Budrute").SetOrdinal(1) 'Move column to position 1
        //    result.Columns.Add("Antall", typeof(int));
        //    result.Columns.Add("Fylke", typeof(string));
        //    result.Columns.Add("Kommune", typeof(string));
        //    result.Columns.Add("Team", typeof(string));


        //    ReolCollection sortedReols = new ReolCollection(utv.Reoler);
        //    // sortedReols.SortByReolNumber()

        //    foreach (Reol r in sortedReols)
        //    {
        //        foreach (DataRow row in result.Rows)
        //        {
        //            if (r.ReolId.ToString() == row.ItemArray[0].ToString())
        //            {
        //                row["r_budrute"] = r.Name;
        //                row["r_antall"] = r.Antall.GetTotalAntall(utv.Receivers);
        //                row["r_fylke"] = r.Fylke;
        //                row["r_kommune"] = r.Kommune;
        //                row["r_team"] = r.TeamName;
        //            }
        //        }
        //    }

        //    if (result.Columns.Contains("Fylke") && result.Columns.Contains("Kommune") && result.Columns.Contains("Team") && result.Columns.Contains("Reolnr"))
        //        result.DefaultView.Sort = "Fylke, Kommune, Team, Reolnr";

        //    // result.Columns.RemoveAt(0) 'Remove reolid

        //    return result;
        //}


        //[HttpGet("GetDemographyIndexInfo", Name = nameof(GetDemographyIndexInfo))]
        //public DemographyValuesCollection GetDemographyIndexInfo(Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        //{
        //    _logger.LogDebug("Inside into GetDemographyIndexInfo");
        //    DataTable minmaxValues;
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
        //    Utils utils = new Utils();
        //    // select 'aldu_5', min(aldu_5) minimum, max(aldu_5) maks, 'BMW', min(bmw) minimum, max(bmw) maks from kspu_gdb.norway_reol_index a inner join kspu_gdb.tbl_norway_reol_index b on a.reol_id = b.Reol_id;
        //    int startPos = demographyOpts.SQLOrderby.IndexOf("(main");
        //    if (startPos == -1)
        //        startPos = demographyOpts.SQLOrderby.IndexOf("(indeks");
        //    startPos += 1;
        //    int endPos = demographyOpts.SQLOrderby.IndexOf(") /");
        //    string[] options = demographyOpts.SQLOrderby.Substring(startPos, endPos - startPos).Replace("main.", "").Replace("indeks.", "").Split("+");
        //    string sqlSelect = "Select ";
        //    string comma = "";
        //    //foreach (string demovar in options)
        //    //{
        //    //    demovar.Trim();
        //    //sqlSelect += comma + "'" + demovar + "', round(min(" + demovar + ")), round(max(" + demovar + "))";
        //    //comma = ", ";
        //    //}
        //    //sqlSelect += " from kspu_gdb." + Config.Demografi_Maintable + " a inner join kspu_gdb." + Config.Demografi_Indekstable + " b on a.reol_id = b.Reol_id";

        //    DemographyValuesCollection result = new DemographyValuesCollection();

        //    string table = configController.CurrentReolTableName;
        //    npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tablename1;

        //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[1].Value = tablename2;

        //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //    {
        //        minmaxValues = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
        //    }
        //    //if (utv.Reoler.Count > 0)
        //    //    sqlSelect += " where " + utv.Reoler.GetReolIDsInClause("a." + Config.ReolIdFieldName);

        //    //OracleCommand cmd = new OracleCommand(sqlSelect);
        //    //DataTable minmaxValues = GetDataTable(cmd);
        //    //DemographyValuesCollection result = new DemographyValuesCollection();
        //    //Dictionary<string, string> demovarcol = utils.getDemografiVarCollection;
        //    int optionsIndx = 0;
        //    //foreach (DataRow row in minmaxValues.Rows)
        //    //{
        //    //    for (int colindx = 0; colindx <= row.Table.Columns.Count - 1; colindx += 3)
        //    //    {
        //    //        string demovar;
        //    //        //if (demovarcol.ContainsKey(utils.GetStringFromRow(row, colindx)))
        //    //        //    demovar = demovarcol.Item[utils.GetStringFromRow(row, colindx)];
        //    //        //else
        //    //           demovar = utils.GetStringFromRow(row, colindx);
        //    //        DemographyValue DemoValue = new DemographyValue(demovar, utils.GetIntFromRow(row, colindx + 1), utils.GetIntFromRow(row, colindx + 2), Convert.ToString(options.GetValue(optionsIndx)));
        //    //        optionsIndx = optionsIndx + 1;
        //    //        result.Add(DemoValue);
        //    //    }
        //    //}
        //    return result;
        //} 
        #endregion


        /// <summary>
        ///  Get Number Of Budruter In Team By TeamNR
        ///  </summary>
        ///  <param name="teamnr"></param>
        /// <returns>No. of Budruter</returns>

        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("GetNumberOfBudruterInTeamByTeamNR")]
        public IActionResult GetNumberOfBudruterInTeamByTeamNR(string teamnr)
        {
            _logger.BeginScope("Inside into GetNumberOfBudruterInTeamByTeamNR");
            #region Old Code
            //_logger.LogDebug("Inside into GetNumberOfBudruterInTeamByTeamNR");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //object result;

            //npgsqlParameters[0] = new NpgsqlParameter("p_teamnr", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = teamnr;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<int>("kspu_gdb.getnumberofbudruterinteambyteamnr", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from GetNumberOfBudruterInTeamByTeamNR");
            //if (result == null | (result) is DBNull)
            //    return System.Convert.ToString(0);
            //return System.Convert.ToString(result); 
            #endregion
            RequestGetNumberOfBudruterInTeamByTeamNR request = new RequestGetNumberOfBudruterInTeamByTeamNR()
            {
                Teamnr = teamnr
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///  Get Number Of Budruter In Team
        ///  </summary>
        /// <returns>DataTabler</returns>

        //[HttpGet("GetNumberOfBudruterInTeam", Name = nameof(GetNumberOfBudruterInTeam))]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("GetNumberOfBudruterInTeam")]
        public IActionResult GetNumberOfBudruterInTeam()
        {
            _logger.BeginScope("Inside into GetNumberOfBudruterInTeam");
            #region Old Code
            //_logger.LogDebug("Inside into GetNumberOfBudruterInTeam");
            //DataTable dt;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //DataTable result;
            //npgsqlParameters = null;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.FillDataTable("kspu_gdb.getnumberofbudruterinteam", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////dt = new DataTable();
            ////dt = result;
            ////var json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            //_logger.LogInformation("Number of row returned: ", result);

            //return result;
            #endregion
            RequestGetNumberOfBudruterInTeam request = new RequestGetNumberOfBudruterInTeam()
            {

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get Probable Next UtvalgId
        ///  </summary>
        /// <returns>Utvalg ID</returns>


        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("GetProbableNextUtvalgId")]
        public IActionResult GetProbableNextUtvalgId()
        {
            _logger.BeginScope("Inside into GetProbableNextUtvalgId");
            #region Old Code
            //_logger.LogDebug("Inside into GetProbableNextUtvalgId");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters = null;
            //int result = -1;

            //using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<int>("kspu_db.getprobablenextutvalgid", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from GetProbableNextUtvalgId");

            //return result; 
            #endregion
            RequestGetProbableNextUtvalgId request = new RequestGetProbableNextUtvalgId()
            {

            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Updates the logo.
        /// </summary>
        /// <param name="utvalgId">The utvalg identifier.</param>
        /// <param name="logo">The logo.</param>
        /// <param name="userName">Name of the user.</param>
        [HttpPut]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UpdateLogo")]
        public IActionResult UpdateLogo(Int64 utvalgId, string logo, string userName)
        {
            _logger.BeginScope("Inside into UpdateLogo");
            #region Old Code
            //try
            //{
            //    _logger.LogDebug("Inside into UpdateLogo");

            //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            //    int result;
            //    //string modificationInfo = "Update Logo ";

            //    #region Parameter assignement

            //    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[0].Value = utv.UtvalgId;

            //    npgsqlParameters[1] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //    npgsqlParameters[1].Value = utv.Logo;

            //    //npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            //    //modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
            //    //npgsqlParameters[2].Value = modificationInfo;

            //    npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //    npgsqlParameters[3].Value = username;

            //    #endregion

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //        result = dbhelper.ExecuteNonQuery("kspu_db.updatelogoforutvalg", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    SaveUtvalgModifications(utv, username, "Update Logo ");
            //    _logger.LogInformation("Number of row returned: ");

            //    _logger.LogDebug("Exiting from UpdateLogo");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("Error in UpdateLogo: " + ex.Message);
            //} 
            #endregion

            RequestUpdateUtvalgLogo request = new RequestUpdateUtvalgLogo()
            {
                Logo = logo,
                Username = userName,
                UtvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// UpdateUtvalgForIntegration
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        /// <param name="username"></param>
        [HttpGet("UpdateUtvalgForIntegration", Name = nameof(UpdateUtvalgForIntegration))]
        public void UpdateUtvalgForIntegration(Utvalg utv, string username)
        {
            try
            {
                _logger.LogDebug("Inside into UpdateUtvalgForIntegration");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[10];
                int result;
                string modificationInfo = "UpdateUtvalgForIntegration - ";

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
                npgsqlParameters[1].Value = utv.OrdreReferanse;

                npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                npgsqlParameters[2].Value = modificationInfo;

                npgsqlParameters[3] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
                npgsqlParameters[3].Value = utv.OrdreType; // Add parameter in Enu type

                npgsqlParameters[4] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
                npgsqlParameters[4].Value = utv.OrdreStatus;

                npgsqlParameters[5] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[5].Value = utv.InnleveringsDato;

                npgsqlParameters[6] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[6].Value = utv.Avtalenummer;

                npgsqlParameters[7] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[7].Value = utv.DistributionDate;

                npgsqlParameters[8] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)DistributionType.Null);
                npgsqlParameters[8].Value = utv.DistributionType;

                npgsqlParameters[9] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[9].Value = username;
                #endregion

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalgforintegration", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utv.UtvalgId);
                SaveUtvalgModifications(utv, username, "UpdateUtvalgForIntegration - ");

                _logger.LogInformation("Number of row returned: ");

                _logger.LogDebug("Exiting from UpdateUtvalgForIntegration");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateUtvalgForIntegration: " + exception.Message);
            }
        }

        [HttpPost("UpdateReolMapnameForDisconnectedUtvalg", Name = nameof(UpdateReolMapnameForDisconnectedUtvalg))]
        public void UpdateReolMapnameForDisconnectedUtvalg([FromBody] Utvalg utv, string username)
        {

            if (utv.WasBasedOn > 0 | utv.BasedOn > 0)
            {
                _logger.LogDebug("Inside into UpdateReolMapnameForDisconnectedUtvalg");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;
                //string modificationInfo = "UpdateReolMapnameForDisconnectedUtvalg - ";

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Integer);
                if (utv.WasBasedOn > 0)
                    npgsqlParameters[1].Value = utv.WasBasedOn;
                else
                    npgsqlParameters[1].Value = utv.BasedOn;

                //npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                //modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                //npgsqlParameters[2].Value = modificationInfo;

                //npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                //npgsqlParameters[3].Value = username;

                #endregion

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updatereolmapnamefordisconnectedutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Oppdaterer av reolmapnavn under frikobling av utvalg endret ingen rader.Utvalgsid: " + utv.UtvalgId);

                SaveUtvalgModifications(utv, username, "UpdateReolMapnameForDisconnectedUtvalg - ");

                _logger.LogInformation("Number of row returned: ");

                _logger.LogDebug("Exiting from UpdateReolMapnameForDisconnectedUtvalg");

            }
        }


        /// <summary>
        ///  Metoden oppdaterer utvalget med skrivebeskyttelse.
        ///  </summary>
        ///  <param name="utv"></param>
        ///  <param name="username"></param>
        ///  <remarks></remarks>
        [HttpPost("UpdateWriteprotectUtvalg", Name = nameof(UpdateWriteprotectUtvalg))]
        public void UpdateWriteprotectUtvalg(Utvalg utv, string username)
        {
            _logger.LogDebug("Inside into UpdateWriteprotectUtvalg");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            int result;
            string modificationInfo = "UpdateWriteprotectUtvalg - ";

            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            npgsqlParameters[1] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = IIf(utv.Skrivebeskyttet, 1, 0);

            npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
            npgsqlParameters[2].Value = modificationInfo;

            npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[3].Value = username;

            #endregion

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.updatewriteprotectutvalg", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result == 0)
                _logger.LogWarning("Oppdatering av skrivebeskyttelse forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utv.UtvalgId);

            SaveUtvalgModifications(utv, username, "UpdateReolMapnameForDisconnectedUtvalg - ");

            _logger.LogInformation("Number of row returned: ");

            _logger.LogDebug("Exiting from UpdateWriteprotectUtvalg");

        }

        /// <summary>
        /// Metoden laster reolene for å kunne kjøre forretningslaget i frikobling av lister etterpå
        ///  </summary>
        /// <param name="utv"></param>
        ///  <remarks></remarks>
        [HttpGet("GetUtvalgReolerForIntegration", Name = nameof(GetUtvalgReolerForIntegration))]
        public void GetUtvalgReolerForIntegration(Utvalg utv)
        {
            //GetUtvalgReoler(utv);
            //Commented this code because its void method and method is get type so.
        }




        /// <summary>
        ///  Delete Utvalg data
        ///  </summary>
        ///  <param name="utvalgId"> Utvalg ID</param>
        ///  <param name="username"></param>
        ///  <remarks></remarks>
        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("DeleteUtvalg")]
        public IActionResult DeleteUtvalg(int utvalgId, string username)
        {
            _logger.BeginScope("Inside into DeleteUtvalg");
            #region Old Code
            //_logger.LogDebug("Inside into DeleteUtvalg");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            //int result;
            //int utvalgListId = GetUtvalgListId(utvalgId);

            //#region Parameter assignement

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = utvalgListId;

            //npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[3].Value = username;

            //#endregion

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteUtvalg", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //_logger.LogInformation("Number of row returned: ");

            //_logger.LogDebug("Exiting from DeleteUtvalg"); 
            #endregion
            RequestDeleteUtvalgByUtvalgId request = new RequestDeleteUtvalgByUtvalgId()
            {
                UtvalgId = utvalgId,
                UserName = username
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///  Delete old Utvalg data
        ///  </summary>
        ///  <param name="utvalgId"> Utvalg ID</param>
        ///  <remarks></remarks>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("DeleteFromUtvalgOldReol")]
        public IActionResult DeleteFromUtvalgOldReol(int utvalgId)
        {
            _logger.BeginScope("Inside into DeleteFromUtvalgOldReol");
            #region Old Code
            //_logger.LogDebug("Inside into DeleteFromUtvalgOldReol");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            //int result;

            //#region Parameter assignement

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //#endregion

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.deletefromutvalgoldreol", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from DeleteFromUtvalgOldReol"); 
            #endregion
            RequestDeleteFromUtvalgOldReol request = new RequestDeleteFromUtvalgOldReol()
            {
                UtvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// GetUtvalgListId
        /// Metoden returnerer utvalgslisteid til ett utvalg med en gitt utvalgId.
        /// Dersom utvalget ikke har noen utvalgslisteid returneres verdien -1
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgListId")]
        public IActionResult GetUtvalgListId(int utvalgId)
        {
            _logger.BeginScope("Inside into GetUtvalgListId");
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgListId");
            //DataTable utvData;
            //DataRow row;
            //int utvalgListId = -1;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistid", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (utvData.Rows.Count > 0)
            //{
            //    row = utvData.Rows[0];
            //    if (!Convert.IsDBNull(row[0]))
            //        utvalgListId = Convert.ToInt32(row["r_utvalglistid"]);
            //}
            //_logger.LogInformation("Number of row returned: ", utvalgListId);

            //_logger.LogDebug("Exiting from GetUtvalgListId");
            //return utvalgListId; 
            #endregion
            RequestGetUtvalgListIdByUtvalgId request = new RequestGetUtvalgListIdByUtvalgId()
            {
                UtvalgId = utvalgId
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Metoden hent elste ID uten flagg 
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseUtvalgBasisFordeling>), (int)HttpStatusCode.OK)]
        [Route("FindBasisUtvalgFordelingToSend")]
        public IActionResult FindBasisUtvalgFordelingToSend()
        {
            _logger.BeginScope("Inside into FindBasisUtvalgFordelingToSend");
            #region Old Code
            //_logger.LogDebug("Inside into FindBasisUtvalgFordelingToSend");
            //DataTable utvData;
            //List<UtvalgBasisFordeling> result = new List<UtvalgBasisFordeling>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            //npgsqlParameters = null;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.findbasisutvalgfordelingtosend", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //{
            //    UtvalgBasisFordeling ubf = new UtvalgBasisFordeling();
            //    ubf.ID = (int)row["r_utvalgid"];
            //    ubf.Utvalgtype = row["r_utvalgtype"].ToString();
            //    ubf.Dato = (DateTime)row["r_dato"];
            //    result.Add(ubf);
            //}
            //_logger.LogInformation("Number of row returned: ", result);

            //_logger.LogDebug("Exiting from FindBasisUtvalgFordelingToSend");
            //return result; 
            #endregion
            RequestFindBasisUtvalgFordelingToSend request = new RequestFindBasisUtvalgFordelingToSend()
            {
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Metoden oppdaterer BasisUtvalgFordelings tabellen med sendt til OEBS info
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("UpdateBasisUtvalgFordelingOppdatering")]
        public IActionResult UpdateBasisUtvalgFordelingOppdatering(RequestUpdateBasisUtvalgFordelingOppdatering ubf)
        {
            _logger.BeginScope("Inside into UpdateBasisUtvalgFordelingOppdatering");
            #region Old Code
            //_logger.LogDebug("Inside into UpdateBasisUtvalgFordelingOppdatering");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            //int result;

            //#region Parameter assignement

            //npgsqlParameters[0] = new NpgsqlParameter("p_isoebs", NpgsqlTypes.NpgsqlDbType.Integer);
            ////npgsqlParameters[0].Value = IIf(ubf.IsSendtOEBS, 1, 0);

            //npgsqlParameters[1] = new NpgsqlParameter("p_datooebs", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[1].Value = DateTime.Now;

            //npgsqlParameters[2] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[2].Value = ubf.ID;

            //npgsqlParameters[3] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            //npgsqlParameters[3].Value = ubf.Utvalgtype;

            //npgsqlParameters[4] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[4].Direction = ParameterDirection.Output;

            //#endregion

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.updatebasisutvalgfordelingoppdatering", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //if (result == 0)
            //    _logger.LogWarning("Oppdatering av Basisutvalgsfordeling forsøkte å oppdatere et linje som ikke eksisterer i KSPU.Utvalgsid: " + ubf.ID + " Type: " + ubf.Utvalgtype + " Dato: " + ubf.Dato);

            //result = Convert.ToInt32(npgsqlParameters[5].Value);
            //_logger.LogInformation(string.Format("Number of row affected {0} for Userid {1}", result));

            //_logger.LogDebug("Exiting from UpdateBasisUtvalgFordelingOppdatering"); 
            #endregion
            return Ok(_mediator.Send(ubf).Result);
        }

        [HttpGet("UtvalgNamesExists", Name = nameof(UtvalgNamesExists))]
        public string[] UtvalgNamesExists(string[] utvalgNames)
        {
            List<string> existingNames = new List<string>();
            //int i = 0;
            //int j = 0;
            StringBuilder nameSql = new StringBuilder();

            //foreach (string name in utvalgNames)
            //{
            //    nameSql.Append("upper(name) = ");
            //    nameSql.Append(":Name"); nameSql.Append(i.ToString());
            //    nameSql.Append(" OR ");
            //    AddParameterString(cmd, "Name" + i.ToString(), name.ToUpper(), 100);
            //    i += 1;
            //    j += 1;
            //    if (i == 10 || j == utvalgNames.Length)
            //    {
            //        i = 0;
            //        nameSql.Remove(nameSql.Length - 4, 3); // remove last 'OR '
            //        cmd.CommandText = " SELECT NAME FROM KSPU_DB.Utvalg WHERE " + nameSql.ToString();
            //        DataTable result = GetDataTable(cmd);
            //        foreach (DataRow row in result.Rows)
            //            existingNames.Add(GetStringFromRow(row, "name"));

            //        nameSql.Remove(0, nameSql.Length);
            //        if (!cmd.Parameters == null)
            //            cmd.Parameters.Clear();
            //        cmd.CommandText = "";
            //    }
            //}


            return existingNames.ToArray();
        }


        /// <summary>
        ///     ''' Metoden oppretter BasisUtvalgFordelings tabellen med ider som skal legges i køen
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>

        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("CreateBasisUtvalgFordelingOppdatering")]
        private IActionResult CreateBasisUtvalgFordelingOppdatering(RequestCreateBasisUtvalgFordelingOppdatering ubf)
        {
            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            //int result;

            //#region Parameter assignement

            //npgsqlParameters[0] = new NpgsqlParameter("p_InsertDato", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = ubf.Dato;

            //npgsqlParameters[1] = new NpgsqlParameter("p_UtvalgId", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[1].Value = ubf.ID;

            //npgsqlParameters[2] = new NpgsqlParameter("p_UtvalgType", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[2].Value = ubf.Utvalgtype;
            //#endregion Parameter assignement
            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.updatebasisutvalgfordelingoppdatering", CommandType.StoredProcedure, npgsqlParameters);
            //} 
            #endregion

            return Ok(_mediator.Send(ubf).Result);

        }

        /// <summary>
        ///     Metoden sjekker om utvalgid og type allerede finnes i køen
        ///     </summary>
        ///      <param name="ubf"></param>
        ///     <remarks></remarks>
        //[HttpGet("BasisUtvalgFordelingExistsOnQue", Name = nameof(BasisUtvalgFordelingExistsOnQue))]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("BasisUtvalgFordelingExistsOnQue")]
        private IActionResult BasisUtvalgFordelingExistsOnQue(RequestBasisUtvalgFordelingExistsOnQue ubf)
        {
            #region Old Code
            //_logger.LogDebug("Inside into BasisUtvalgFordelingExistsOnQue");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            //object count;
            //npgsqlParameters[1] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = ubf.ID;

            //npgsqlParameters[2] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            //npgsqlParameters[2].Value = ubf.Utvalgtype;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    count = dbhelper.ExecuteNonQuery("kspu_db.BasisUtvalgFordelingExistsOnQue", CommandType.StoredProcedure, npgsqlParameters);
            //}
            ////object count = ExecuteScalar(cmd);
            //if (count == null | (count) is DBNull)
            //    return false;
            //return System.Convert.ToInt32(count) > 0;
            #endregion Old Code
            return Ok(_mediator.Send(ubf).Result);
        }

        /// <summary>
        ///     ''' Metoden oppretter BasisUtvalgFordelings tabellen med ider som skal legges i køen, dersom id og type ikke allerede ligger på køen klar til å sendes.
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>
        [HttpPost("SendBasisUtvalgFordelingToQue", Name = nameof(SendBasisUtvalgFordelingToQue))]
        public void SendBasisUtvalgFordelingToQue([FromBody] List<UtvalgBasisFordeling> ubf)
        {
            _logger.LogDebug("Inside into SendBasisUtvalgFordelingToQue");
            foreach (var itemUbf in ubf)
            {
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_insertdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
                npgsqlParameters[0].Value = itemUbf.Dato;

                npgsqlParameters[1] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = itemUbf.ID;

                npgsqlParameters[2] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
                npgsqlParameters[2].Value = itemUbf.Utvalgtype;

                npgsqlParameters[3] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[3].Direction = ParameterDirection.Output;

                #endregion

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.sendbasisutvalgfordelingtoque", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Oppdatering av Basisutvalgsfordeling forsøkte å oppdatere et linje som ikke eksisterer i KSPU.Utvalgsid: " + itemUbf.ID + " Type: " + itemUbf.Utvalgtype + " Dato: " + itemUbf.Dato);

                result = Convert.ToInt32(npgsqlParameters[5].Value);
                _logger.LogInformation(string.Format("Number of row affected {0}: ", result));

                _logger.LogDebug("Exiting from SendBasisUtvalgFordelingToQue");
            }

        }

        /// <summary>
        ///     ''' Checks if distribution is in the next x days ( X = Config.IgnoreNrOfDaysToDelivery ) 
        ///     ''' </summary>
        ///     ''' <param name="idsU"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks> Will also check if any contents of the list has distribution in the next x days</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [Route("CheckIfUtvalgListsDistributionIsToClose")]
        public IActionResult CheckIfUtvalgListsDistributionIsToClose(int[] idsU)
        {
            #region Old Code
            //_logger.LogDebug("Inside into CheckIfUtvalgListsDistributionIsToClose");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

            //Dictionary<int, int> resultDict = new Dictionary<int, int>();
            //foreach (string idList in CreateInClauses(idsU, 1000))
            //{
            //    {
            //        npgsqlParameters[0] = new NpgsqlParameter("p_idlist", NpgsqlTypes.NpgsqlDbType.Varchar);
            //        npgsqlParameters[0].Value = idList;

            //        npgsqlParameters[1] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Varchar);
            //        npgsqlParameters[1].Value = DateTime.Today;

            //        npgsqlParameters[2] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Varchar);
            //        // npgsqlParameters[2].Value = DateTime.Today.AddDays(Config.IgnoreNrOfDaysToDelivery);

            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.checkifutvalglistsdistributionistoclose", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        foreach (DataRow row in utvData.Rows)
            //        {
            //            int id = (int)row["r_id"];
            //            resultDict[id] = id;
            //        }
            //    }
            //}
            //_logger.LogInformation("Number of row returned: ", resultDict);

            //_logger.LogDebug("Exiting from CheckIfUtvalgListsDistributionIsToClose");
            //return new List<int>(resultDict.Values); 
            #endregion
            RequestCheckIfUtvalgListsDistributionIsToClose request = new RequestCheckIfUtvalgListsDistributionIsToClose()
            {
                Idus = idsU
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Check If UtvalgLists Need On The Fly Update
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks> Will also check if any contents of the list has distribution in the next x days</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [Route("CheckIfUtvalgListsNeedOnTheFlyUpdate")]
        public IActionResult CheckIfUtvalgListsNeedOnTheFlyUpdate(int[] utvalgListIDs)
        {
            #region Old Code
            //_logger.LogDebug("Inside into CheckIfUtvalgListsNeedOnTheFlyUpdate");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

            //Dictionary<int, int> resultDict = new Dictionary<int, int>();
            //utvalgListIDs = ReplaceCampaignListIDsWithBasisListIDs(utvalgListIDs);
            //foreach (int id in CheckIfParentUtvalgListsNeedOnTheFlyUpdate(utvalgListIDs))
            //    resultDict[id] = id;
            //foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            //{
            //    {
            //        npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Varchar);
            //        npgsqlParameters[0].Value = str;

            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            utvData = dbhelper.FillDataTable("kspu_db.CheckIfUtvalgListsNeedOnTheFlyUpdate", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        foreach (DataRow row in utvData.Rows)
            //        {
            //            int id = (int)row["utvalglistid"];
            //            resultDict[id] = id;
            //        }
            //    }
            //}
            //_logger.LogInformation("Number of row returned: ", resultDict);

            //_logger.LogDebug("Exiting from CheckIfUtvalgListsDistributionIsToClose");
            //return new List<int>(resultDict.Values); 
            #endregion
            RequestCheckIfUtvalgListsNeedOnTheFlyUpdate request = new RequestCheckIfUtvalgListsNeedOnTheFlyUpdate()
            {
                utvalgListIDs = utvalgListIDs
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Find Expired Utvalg IDs
        ///     ''' </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [Route("FindExpiredUtvalgIDs")]
        public IActionResult FindExpiredUtvalgIDs()
        {
            #region Old Code
            //_logger.LogDebug("Inside into FindExpiredUtvalgIDs");
            //DataTable utvData;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            //List<int> result = new List<int>();

            //string table = configController.CurrentReolTableName;
            //npgsqlParameters[0] = new NpgsqlParameter("p_dato", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //npgsqlParameters[0].Value = DateTime.Today.AddMonths(-24);

            //npgsqlParameters[1] = new NpgsqlParameter("p_dato2", NpgsqlTypes.NpgsqlDbType.Timestamp);
            //npgsqlParameters[1].Value = DateTime.Today.AddMonths(-2);

            //npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[2].Value = "kundeweb_u";

            //npgsqlParameters[3] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[3].Value = true;

            //npgsqlParameters[4] = new NpgsqlParameter("p_systemuser", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[4].Value = "SystemUser";

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    utvData = dbhelper.FillDataTable("kspu_db.findexpiredutvalgids", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in utvData.Rows)
            //    result.Add((int)row["r_utvalgid"]);

            //_logger.LogInformation("Number of row returned: ", result.Count);
            //_logger.LogDebug("Exiting from FindExpiredUtvalgIDs");
            //return result; 
            #endregion
            RequestFindExpiredUtvalgIds request = new RequestFindExpiredUtvalgIds()
            {
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///      Gets a list of the reol ids in the UtvalgReol table for a given utvalg.
        ///      </summary>
        ///      <param name="utvalgID"></param>
        ///      <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<long>), (int)HttpStatusCode.OK)]
        [Route("GetUtvalgReolIDs")]
        public IActionResult GetUtvalgReolIDs(int utvalgID)
        {
            #region Old Code
            //_logger.LogDebug("Inside into GetUtvalgReolIDs");
            //DataTable reolID;
            //List<long> result = new List<long>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utvalgID;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolID = dbhelper.FillDataTable("kspu_db.getutvalgreolids", CommandType.StoredProcedure, npgsqlParameters);
            //}
            //foreach (DataRow row in reolID.Rows)
            //    result.Add((long)(Convert.ToDouble(row["r_reolid"])));
            //_logger.LogInformation("Number of row returned: ", result.Count);

            //_logger.LogDebug("Exiting from GetUtvalgReolIDs");
            //return result; 
            #endregion
            RequestGetUtvalgReolIDs request = new RequestGetUtvalgReolIDs()
            {
                UtlvagId = utvalgID
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Metoden frikobler et utvalget brukt fra integrasjon mot Ordre2, Normal forretningslagskall til KSPU.BusinessLayer.BasisUtvalgManager.DisconnectUtvalg fungerte ikke i integrasjon.
        ///     ''' (do NOT chanhe to anchestor)
        ///     ''' </summary>
        ///     ''' <param name="utv"></param>
        ///     ''' <param name="username"></param>
        ///     ''' <remarks></remarks>
        [HttpGet("UpdateDisconnectUtvalgForIntegration", Name = nameof(UpdateDisconnectUtvalgForIntegration))]
        public void UpdateDisconnectUtvalgForIntegration(Utvalg utv, string username)
        {


        }


        /// <summary>
        /// Adds the selections for process.
        /// </summary>
        /// <param name="selectionId">The selection identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("AddSelectionsForProcess")]
        public IActionResult AddSelectionsForProcess(int selectionId)
        {

            RequestAddSelectionDistribution request = new RequestAddSelectionDistribution()
            {
                SelectionId = selectionId
            };
            return Ok(_mediator.Send(request).Result);
        }

        #endregion

        #region Private Methods
        private Utvalg GetUtvalgSearchFromDataRow(DataRow row)
        {
            Utvalg utv = new Utvalg();
            Utils utils = new Utils();
            utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);
            utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
            utv.Name = Convert.ToString(row["r_name"]);
            utv.Logo = row["r_logo"].ToString();
            utv.OrdreReferanse = Convert.ToString(row["r_ordrereferanse"]);
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), "Null");
            utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), "Null");
            utv.InnleveringsDato = Convert.ToDateTime(Convert.IsDBNull(row["r_innleveringsdato"]) ? null : (DateTime?)(row["r_innleveringsdato"]));
            utv.ReolMapName = Convert.ToString(row["r_reolmapname"]);
            utv.AntallWhenLastSaved = (long)Convert.ToDouble(row["r_antall"]);
            utv.OldReolMapName = Convert.ToString(row["r_oldreolmapname"]);
            utv.Skrivebeskyttet = Convert.ToInt32(row["r_skrivebeskyttet"]) > 0;
            utv.Weight = Convert.ToInt32(row["r_vekt"]);
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));
            utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), "Null");
            utv.ArealAvvik = Convert.ToInt32(row["r_arealAvvik"]);
            utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;
            utv.BasedOn = Convert.ToInt32(row["r_basedon"]);
            utv.WasBasedOn = Convert.ToInt32(row["r_wasbasedon"]);
            utv.Thickness = Convert.ToDouble(Convert.IsDBNull(row["r_thickness"]) ? null : (Double?)(row["r_thickness"]));
            //if (row["r_utvalglistid"] != null)
            //{
            //    utv.List = new UtvalgList();
            //    utv.List.ListId = Convert.ToInt32(Convert.IsDBNull(row["r_utvalglistid"]) ? null : (Decimal?)(row["r_utvalglistid"]));
            //}
            utv.ListId = Convert.ToString(row["r_utvalglistid"]);
            return utv;
        }

        private UtvalgSearchResult GetUtvalgSearchResultFromDataRow(DataRow row)
        {
            UtvalgSearchResult utv = new UtvalgSearchResult();
            Utils utils = new Utils();
            utv.UtvalgName = Convert.ToString(row["r_name"]);
            utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);
            utv.Antall = Convert.ToInt32(row["r_antall"]);
            utv.ReolCount = GetUtvalgReolerCount(utv.UtvalgId);
            utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;
            utv.BasedOn = Convert.ToInt32(row["r_basedOn"]);
            if (row["r_utvalgListId"] != null)
                //utv.List = DAUtvalgList.GetUtvalgListSimple(GetIntFromRow(row, "UtvalgListId"));
                utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), "Null");
            return utv;
        }

        //private Utvalg GetUtvalgFromDataRow(DataRow row, bool includeReols = true)
        //{
        //    Utvalg utv = new Utvalg();
        //    Utils utils = new Utils();
        //    utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);



        //    utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
        //    utv.Name = Convert.ToString(row["r_name"]);
        //    utv.Logo = Convert.ToString(row["r_logo"]);
        //    utv.OrdreReferanse = Convert.ToString(row["r_ordrereferanse"]);
        //    if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null))))
        //        utv.OrdreType = 0;
        //    else
        //        utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null);
        //    if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null))))
        //        utv.OrdreStatus = 0;
        //    else
        //        utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null);

        //    utv.InnleveringsDato = Convert.ToDateTime(Convert.IsDBNull(row["r_innleveringsdato"]) ? null : (DateTime?)(row["r_innleveringsdato"]));
        //    utv.ReolMapName = Convert.ToString(row["r_reolMapName"]);
        //    utv.AntallWhenLastSaved = (long)Convert.ToDouble(row["r_antall"]);
        //    utv.ArealAvvik = Convert.ToInt32(row["r_arealavvik"]);
        //    utv.Weight = Convert.ToInt32(row["r_vekt"]);
        //    utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));



        //    if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null))))
        //        utv.DistributionType = 0;
        //    else
        //        utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null);
        //    utv.Skrivebeskyttet = Convert.ToInt32(row["r_skrivebeskyttet"]) > 0;
        //    //if (Convert.ToString(row["r_utvalglistid"]) != null)

        //    //{
        //    //utv.List = new UtvalgList();
        //    //utv.List.ListId = Convert.ToInt32(Convert.IsDBNull(row["r_utvalglistid"]) ? null : (Decimal?)(row["r_utvalglistid"]));
        //    //utv.List = DAUtvalgList.GetUtvalgList((int)row["UtvalgListId"]);
        //    //if (!utv.List.MemberUtvalgs.Contains(utv))
        //    //    utv.List.MemberUtvalgs.Add(utv);
        //    //}

        //    utv.ListId = Convert.ToString(row["r_utvalglistid"]);
        //    utv.OldReolMapName = Convert.ToString(row["r_oldreolmapname"]);
        //    //utv.Avtalenummer = Convert.ToInt32(row["r_avtalenummer"]);
        //    if (String.IsNullOrWhiteSpace(Convert.ToString((row, "r_avtalenummer"))))
        //        utv.Avtalenummer = 0;
        //    else
        //        utv.Avtalenummer = Convert.ToInt32(Convert.IsDBNull(row["r_avtalenummer"]));
        //    //utv.Avtalenummer = (Convert.IsDBNull(row["r_avtalenummer"]) ? null : (int?)(row["r_avtalenummer"]));
        //    utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;

        //    utv.BasedOn = Convert.ToInt32(row["r_basedOn"]);
        //    utv.WasBasedOn = Convert.ToInt32(row["r_wasBasedOn"]);
        //    utv.Thickness = Convert.ToDouble(Convert.IsDBNull(row["r_thickness"]) ? null : (Double?)(row["r_thickness"]));

        //    GetUtvalgModifications(utv, true);
        //    if (includeReols)
        //        GetUtvalgReoler(utv);
        //    GetUtvalgReceiver(utv);
        //    GetUtvalgKommune(utv);
        //    GetUtvalgDistrict(utv);
        //    GetUtvalgPostalZone(utv);
        //    GetUtvalgCriteria(utv);
        //    utv.SetInitialData();
        //    return utv;
        //}
        [HttpGet("GetUtvalgReceiver", Name = nameof(GetUtvalgReceiver))]
        public void GetUtvalgReceiver([FromBody] Utvalg utv, int utvalgid)
        {
            _logger.LogDebug("Inside into GetUtvalgReceiver");
            DataTable reciever;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            UtvalgReceiver r = new UtvalgReceiver();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            if (utv.BasedOn > 0)
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.BasedOn;
            }
            else
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utvalgid;    //utv.UtvalgId;
            }

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                reciever = dbhelper.FillDataTable("kspu_db.getutvalgreceiver", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow row in reciever.Rows)
            {
                r.ReceiverId = (ReceiverType)utils.GetEnumFromRow(row, "r_receiverid", typeof(ReceiverType));
                //utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), "Null");
                r.Selected = utils.GetBooleanFromRow(row, "r_selected");
                utv.Receivers.Add(r);
            }
        }

        [HttpGet("GetUtvalgKommune", Name = nameof(GetUtvalgKommune))]
        private void GetUtvalgKommune(Utvalg utv)
        {
            _logger.LogDebug("Inside into GetUtvalgKommune");
            DataTable komm;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                komm = dbhelper.FillDataTable("kspu_db.getutvalgkommune", CommandType.StoredProcedure, npgsqlParameters);
            }


            foreach (DataRow row in komm.Rows)
            {
                UtvalgKommune k = new UtvalgKommune();
                k.KommuneId = utils.GetStringFromRow(row, "r_kommuneid");
                k.KommuneMapName = utils.GetStringFromRow(row, "r_kommuneMapname");
                utv.Kommuner.Add(k);
            }
            _logger.LogInformation("Number of row returned: ", komm.Rows.Count);

            _logger.LogDebug("Exiting from GetUtvalgKommune");
        }

        [HttpGet("GetUtvalgDistrict", Name = nameof(GetUtvalgDistrict))]
        private void GetUtvalgDistrict(Utvalg utv)
        {
            _logger.LogDebug("Inside into GetUtvalgDistrict");
            DataTable dis;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                dis = dbhelper.FillDataTable("kspu_db.getutvalgdistrict", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in dis.Rows)
            {
                UtvalgDistrict d = new UtvalgDistrict();
                d.DistrictId = utils.GetStringFromRow(row, "r_districtid");
                d.DistrictMapName = utils.GetStringFromRow(row, "r_districtmapname");
                utv.Districts.Add(d);
            }
        }


        [HttpGet("GetUtvalgPostalZone", Name = nameof(GetUtvalgPostalZone))]
        private void GetUtvalgPostalZone(Utvalg utv)
        {
            _logger.LogDebug("Inside into GetUtvalgPostalZone");
            DataTable pos;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                pos = dbhelper.FillDataTable("kspu_db.getutvalgpostalzone", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in pos.Rows)
            {
                UtvalgPostalZone p = new UtvalgPostalZone();
                p.PostalZone = utils.GetIntFromRow(row, "r_postalzone");
                p.PostalZoneMapName = utils.GetStringFromRow(row, "r_postalZonemapname");
                utv.PostalZones.Add(p);
            }
        }

        [HttpGet("GetUtvalgCriteria", Name = nameof(GetUtvalgCriteria))]
        private void GetUtvalgCriteria(Utvalg utv)
        {
            _logger.LogDebug("Inside into GetUtvalgCriteria");
            DataTable cri;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                cri = dbhelper.FillDataTable("kspu_db.getutvalgcriteria", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in cri.Rows)
            {
                UtvalgCriteria c = new UtvalgCriteria();
                c.CriteriaId = utils.GetIntFromRow(row, "r_criteriaid");
                c.Criteria = utils.GetStringFromRow(row, "r_criteria");
                //c.CriteriaType = (CriteriaType)utils.GetEnumFromRow(row, "r_criteriatype", typeof(CriteriaType));
                utv.Criterias.Add(c);
            }
        }


        [HttpGet("GetUtvalgModifications", Name = nameof(GetUtvalgModifications))]
        private void GetUtvalgModifications(Utvalg utv, bool bAsc)
        {
            DataTable modification;
            Utils utils = new Utils();
            //string SortOrdrer = "ASC";
            //if (!bAsc)
            //    SortOrdrer = "DESC";
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                modification = dbhelper.FillDataTable("kspu_db.getutvalgmodifications", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow row in modification.Rows)
            {
                UtvalgModification m = new UtvalgModification();
                m.UserId = utils.GetStringFromRow(row, "UserId");
                m.ModificationId = utils.GetIntFromRow(row, "UtvalgModificationId");
                m.ModificationTime = utils.GetTimestampFromRow(row, "ModificationDate");
                utv.Modifications.Add(m);
            }
        }

        /// <summary>
        /// Saves the many utvalg.
        /// </summary>
        /// <param name="utvList">The utv list.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpPost("SaveManyUtvalg", Name = nameof(SaveManyUtvalg))]
        public IActionResult SaveManyUtvalg([FromBody] UtvalgList utvList, string userName)
        {
            _logger.BeginScope("Inside into SaveManyUtvalg");
            RequestSaveManyUtvalgs request = new RequestSaveManyUtvalgs()
            {
                UtvalgList = utvList,
                UserName = userName

            };

            return Ok(_mediator.Send(request).Result);
        }


        private static void SaveUtvalgKommuner(Utvalg utv)
        {
            int res;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                res = dbhelper.ExecuteNonQuery("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgKommune WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgKommune (UtvalgId, KommuneId, KommuneMapName) VALUES (:UtvalgId, :KommuneId, :KommuneMapName) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterString(insertCmd, "KommuneId", "", 4);
            //AddParameterString(insertCmd, "KommuneMapName", "", 50);
            foreach (UtvalgKommune k in utv.Kommuner)
            {
                //insertCmd.Parameters.Item("KommuneId").Value = k.KommuneId;
                //insertCmd.Parameters.Item("KommuneMapName").Value = k.KommuneMapName;
                //ExecuteNonQuery(insertCmd);
            }
        }


        private static void SaveUtvalgReceiver(Utvalg utv)
        {
            int res;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                res = dbhelper.ExecuteNonQuery("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgReceiver WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgReceiver (UtvalgId, ReceiverId, Selected) VALUES (:UtvalgId, :ReceiverId, :Selected) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterInteger(insertCmd, "ReceiverId", 0);
            //AddParameterInteger(insertCmd, "Selected", 0);
            foreach (UtvalgReceiver r in utv.Receivers)
            {
                //insertCmd.Parameters.Item("ReceiverId").Value = r.ReceiverId;
                //insertCmd.Parameters.Item("Selected").Value = System.Convert.ToInt32(r.Selected);
                //ExecuteNonQuery(insertCmd);
            }
        }


        private void SaveUtvalgAntallToInheritors(Utvalg utv)
        {
            int res;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                res = dbhelper.ExecuteNonQuery("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand updateCmd = new OracleCommand(" UPDATE KSPU_DB.Utvalg SET Antall = :Antall WHERE BasedOn = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(updateCmd, "UtvalgID", utv.UtvalgId);
            //AddParameterLong(updateCmd, "Antall", utv.AntallWhenLastSaved);
            //ExecuteNonQuery(updateCmd);
        }


        private static void SaveUtvalgDistrict(Utvalg utv)
        {
            DataTable district;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                district = dbhelper.FillDataTable("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgDistrict WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgDistrict (UtvalgId, DistrictId, DistrictMapName) VALUES (:UtvalgId, :DistrictId, :DistrictMapName) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterString(insertCmd, "DistrictId", "", 50);
            //AddParameterString(insertCmd, "DistrictMapName", "", 50);
            foreach (UtvalgDistrict d in utv.Districts)
            {
                //insertCmd.Parameters.Item("DistrictId").Value = d.DistrictId;
                //insertCmd.Parameters.Item("DistrictMapName").Value = d.DistrictMapName;
                //ExecuteNonQuery(insertCmd);
            }
        }

        private void SaveUtvalgPostalZone(Utvalg utv)
        {
            DataTable postalzone;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                postalzone = dbhelper.FillDataTable("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgPostalZone WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgPostalZone (UtvalgId, PostalZone, PostalZoneMapName) VALUES (:UtvalgId, :PostalZone, :PostalZoneMapName) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterInteger(insertCmd, "PostalZone", 0);
            //AddParameterString(insertCmd, "PostalZoneMapName", "", 50);
            foreach (UtvalgPostalZone p in utv.PostalZones)
            {
                //insertCmd.Parameters.Item("PostalZone").Value = p.PostalZone;
                //insertCmd.Parameters.Item("PostalZoneMapName").Value = p.PostalZoneMapName;
                //ExecuteNonQuery(insertCmd);
            }
        }


        private void SaveUtvalgCriteria(Utvalg utv)
        {
            DataTable Criteria;
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgCriteria WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgCriteria (UtvalgId, CriteriaId, Criteria, CriteriaType) VALUES (:UtvalgId, :CriteriaId, :Criteria, :CriteriaType) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterInteger(insertCmd, "CriteriaId", 0);
            //AddParameterString(insertCmd, "Criteria", "", 255);
            //AddParameterInteger(insertCmd, "CriteriaType", 0);
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                Criteria = dbhelper.FillDataTable("kspu_gdb.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (UtvalgCriteria c in utv.Criterias)
            {
                if (c.CriteriaId == 0)
                    //  c.CriteriaId = GetSequenceNextVal("KSPU_DB.CriteriaId_Seq", trans);
                    // insertCmd.Parameters.Item("CriteriaId").Value = c.CriteriaId;
                    if (c.Criteria == null)
                        c.Criteria = "";
                //insertCmd.Parameters.Item("Criteria").Value = c.Criteria;
                //insertCmd.Parameters.Item("CriteriaType").Value = System.Convert.ToInt32(c.CriteriaType);
                //ExecuteNonQuery(insertCmd);
            }
        }

        //private void GetUtvalgReoler(Utvalg utv)
        //{
        //    int utvalgId = utv.UtvalgId;
        //    if ((utv.BasedOn > 0))
        //    {
        //        // Fetch reoler from the Basis Utvalg that this utvalg is based on
        //        utvalgId = utv.BasedOn;
        //        GetUtvalgReolMapNames(utv, utvalgId);
        //    }
        //    ReolController daCurrentReoler = new ReolController(_loggerreolcontroller, _loggerconfigcontroller);
        //    if (utv.OldReolMapName != "")
        //        // Utvalg was recreated by batch job, read ReolerBeforeRecreation from table UtvalgOldReol
        //        GetUtvalgOldReoler(utv, utvalgId);
        //    if (utv.ReolMapName.ToUpper() != configController.CurrentReolTableName.ToUpper())
        //    {
        //        if (utv.ReolerBeforeRecreation == null)
        //        {
        //            // Recreate utvalg on the fly based on ids
        //            utv.ReolerBeforeRecreation = new ReolCollection();
        //            if (daCurrentReoler.TableExists(utv.ReolMapName))
        //            {
        //                ReolController daOldReoler = new ReolController(_loggerreolcontroller, _loggerconfigcontroller, utv.ReolMapName);
        //                foreach (long reolid in GetUtvalgReolIDs(utvalgId))
        //                {
        //                    // Supportsak #621937 - ignore missing reolids, even if it should not happen... 
        //                    Reol Oldr = daOldReoler.GetReol(reolid, NotFoundAction.ReturnNothing, false);
        //                    if (Oldr != null)
        //                        utv.ReolerBeforeRecreation.Add(Oldr);
        //                }
        //            }
        //            else
        //                utv.OldReolMapMissing = true;
        //        }
        //        // Current recreation strategy: Get new reoler with same reolids, ignore missing reolids.
        //        List<long> reolIDsToUse;
        //        if (utv.OldReolMapName != "")
        //            reolIDsToUse = GetUtvalgOldReolIDs(utvalgId);
        //        else
        //            reolIDsToUse = GetUtvalgReolIDs(utvalgId);
        //        foreach (long reolid in reolIDsToUse)
        //        {
        //            Reol r = daCurrentReoler.GetReol(reolid, NotFoundAction.ReturnNothing);
        //            if (r != null)
        //                utv.Reoler.Add(r);
        //        }
        //    }
        //    else
        //        // No recreation is necessary
        //        foreach (long reolid in GetUtvalgReolIDs(utvalgId))
        //            utv.Reoler.Add(daCurrentReoler.GetReol(reolid));


        //}

        private List<long> GetUtvalgOldReolIDs(int utvalgId)
        {
            _logger.LogDebug("Inside into GetUtvalgOldReolIDs");
            DataTable reolID;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                reolID = dbhelper.FillDataTable("kspu_db.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow row in reolID.Rows)
                // result.Add((long)row["r_reolid"]);
                result.Add(utils.GetLongFromRow(row, "r_reolid"));
            _logger.LogInformation("Number of row returned: ", result);

            _logger.LogDebug("Exiting from GetUtvalgOldReolIDs");
            return result;
        }


        private void GetUtvalgReolMapNames(Utvalg utv, int utvalgId)
        {
            _logger.LogDebug("Inside into GetUtvalgReolMapNames");
            DataTable result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_gdb.getutvalgreolids", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                utv.ReolMapName = row["r_reolmapname"].ToString();
                utv.OldReolMapName = row["r_oldreolmapname"].ToString();
            }
            _logger.LogInformation("Number of row returned: ", result.Rows.Count);

            _logger.LogDebug("Exiting from GetUtvalgReolMapNames");
        }

        private CampaignDescription GetCampaignDescriptionFromUtvalgDataRow(DataRow row, bool isDisconnected)
        {
            CampaignDescription utv = new CampaignDescription();
            Utils utils = new Utils();
            utv.ID = utils.GetIntFromRow(row, "r_utvalgid");
            utv.Name = utils.GetStringFromRow(row, "r_name");
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_OrdreType", typeof(OrdreType), "Null");
            utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_OrdreStatus", typeof(OrdreStatus), "Null");
            //utv.DistributionDate = (DateTime)row["r_Distribusjonsdato"];
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));
            utv.IsDisconnected = isDisconnected;
            return utv;
        }

        //private void GetUtvalgOldReoler(Utvalg utv, int utvalgId)
        //{
        //    utv.ReolerBeforeRecreation = new ReolCollection();
        //    ReolController DAReolTable = new ReolController(_loggerreolcontroller, _loggerconfigcontroller, utv.OldReolMapName);
        //    if (!string.IsNullOrWhiteSpace(utv.OldReolMapName) && DAReolTable.TableExists(utv.OldReolMapName))
        //    {
        //        //ReolController daOldReoler = new ReolController(utv.OldReolMapName);
        //        foreach (long reolID in GetUtvalgOldReolIDs(utvalgId))
        //        {
        //            var reolData = DAReolTable.GetReol(reolID, NotFoundAction.ReturnNothing/* Conversion error: Set to default value for this argument */, false);
        //            if (reolData != null)
        //                utv.ReolerBeforeRecreation.Add(reolData);

        //        }
        //    }
        //    else
        //        utv.OldReolMapMissing = true;


        //}

        /// <summary>
        /// Save Utvalg Reoler
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        private void SaveUtvalgReoler(Utvalg utv)
        {
            try
            {
                _logger.LogDebug("Inside into SaveUtvalgReoler");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = 0;
                #endregion

                foreach (Reol r in utv.Reoler)
                {
                    npgsqlParameters[1].Value = r.ReolId;

                    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgreoler", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                _logger.LogInformation("Number of row returned: ");

                _logger.LogDebug("Exiting from SaveUtvalgReoler");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }

        /// <summary>
        /// Save Utvalg Previous Reoler
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        private void SaveUtvalgPreviousReoler(Utvalg utv)
        {
            try
            {
                _logger.LogDebug("Inside into SaveUtvalgPreviousReoler");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = 0;
                #endregion

                foreach (Reol r in utv.ReolerBeforeRecreation)
                {
                    npgsqlParameters[1].Value = r.ReolId;

                    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgpreviousreoler", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                _logger.LogInformation("Number of row returned: ");

                _logger.LogDebug("Exiting from SaveUtvalgPreviousReoler");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }

        /// <summary>
        /// SaveUtvalgModifications
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        /// <param name="userName"></param>
        /// <param name="modificationInfo"></param>
        [HttpPost("SaveUtvalgModifications", Name = nameof(SaveUtvalgModifications))]
        internal void SaveUtvalgModifications(Utvalg utv, string userName, string modificationInfo = "")
        {
            try
            {
                _logger.LogDebug("Inside into SaveUtvalgModifications");

                UtvalgModification utvalgModification = new UtvalgModification();

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                int result;

                #region Parameter assignement
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
                npgsqlParameters[1].Value = utvalgModification.UserId;

                //npgsqlParameters[2] = new NpgsqlParameter("p_info", NpgsqlTypes.NpgsqlDbType.Varchar);
                //npgsqlParameters[2].Value = utvalgModification.mo;

                npgsqlParameters[2] = new NpgsqlParameter("p_info", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                npgsqlParameters[2].Value = modificationInfo;
                #endregion

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgmodifications", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogInformation("Number of row returned: ", result);

                _logger.LogDebug("Exiting from saveutvalgmodifications");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }




        /// <summary>
        /// Save Utvalg Previous Reoler
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        private void UpdateOldReolMapName(Utvalg utv)
        {
            try
            {
                _logger.LogDebug("Inside into UpdateOldReolMapName");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_oldreolmapname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                if (utv.OldReolMapName.Length != 0)
                    npgsqlParameters[1].Value = utv.OldReolMapName;
                else
                    npgsqlParameters[1].Value = null;

                #endregion

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateoldreolmapname", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogInformation("Number of row returned: ");

                _logger.LogDebug("Exiting from UpdateOldReolMapName");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }

        /// <summary>
        /// Utvalg Has Recreated Before
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        private bool UtvalgHasRecreatedBefore(int utvalgId)
        {
            _logger.LogDebug("Inside into UtvalgHasRecreatedBefore");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalghasrecreatedbefore", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Number of row returned: ", result);

            _logger.LogDebug("Exiting from UtvalgHasRecreatedBefore");
            if (result == DBNull.Value)
                return false;
            return System.Convert.ToInt32(result) > 0;
        }


        /// <summary>
        ///     ''' Checks if distribution is in the next x days ( X = Config.IgnoreNrOfDaysToDelivery ) 
        ///     ''' </summary>
        ///     ''' <param name="idsU"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        [HttpGet("CheckIfUtvalgsDistributionIsToClose", Name = nameof(CheckIfUtvalgsDistributionIsToClose))]
        public List<int> CheckIfUtvalgsDistributionIsToClose(int[] idsU)
        {
            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            Utils utils = new Utils();
            DataTable utvData;
            foreach (string idList in CreateInClauses(idsU, 1000))
            {
                StringBuilder sql = new StringBuilder();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                //object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_idsu", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = Convert.ToInt32(idList);

                npgsqlParameters[1] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[1].Value = DateTime.Today;

                npgsqlParameters[2] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[2].Value = DateTime.Today.AddDays(3);

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.checkifutvalgsdistributionistoclose", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow row in utvData.Rows)
                {
                    int id = utils.GetIntFromRow(row, "r_utvalgid");
                    resultDict[id] = id;
                }
            }
            return new List<int>(resultDict.Values);
        }


        /// <summary>
        ///    Checks if on the fly is needed for a set of utvalg ids
        ///      </summary>
        ///      <param name="idsU"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>

        [HttpGet("CheckIfUtvalgsNeedOnTheFlyUpdate", Name = nameof(CheckIfUtvalgsNeedOnTheFlyUpdate))]
        public List<int> CheckIfUtvalgsNeedOnTheFlyUpdate(int[] idsU)
        {
            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            Utils utils = new Utils();
            DataTable utvData;
            foreach (string idList in CreateInClauses(idsU, 1000))
            {
                StringBuilder sql = new StringBuilder();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                //object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_idsu", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = Convert.ToInt32(idList);

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.checkifutvalgsneedontheflyupdate", CommandType.StoredProcedure, npgsqlParameters);
                }


                foreach (DataRow row in utvData.Rows)
                {
                    int id = utils.GetIntFromRow(row, "r_utvalgid");
                    resultDict[id] = id;
                }
            }
            return new List<int>(resultDict.Values);
        }



        [HttpPost("AcceptAllChangesForList", Name = nameof(AcceptAllChangesForList))]
        public IActionResult AcceptAllChangesForList(int ListId, string userName)
        {
            _logger.BeginScope("Inside into AcceptAllChangesForList");
            RequestAcceptAllChangesOfList request = new RequestAcceptAllChangesOfList()
            {
                ListId = ListId,
                UserName = userName
            };

            return Ok(_mediator.Send(request).Result);
        }

        private List<string> CreateInClauses(IEnumerable<int> ids, int maxLength)
        {
            List<string> bolker = new List<string>();
            StringBuilder idsULsb = new StringBuilder();
            foreach (int id in ids)
            {
                if (id > 0)
                {
                    idsULsb.Append(id.ToString());
                    if (idsULsb.Length > maxLength)
                    {
                        bolker.Add(idsULsb.ToString());
                        idsULsb.Remove(0, idsULsb.Length);
                    }
                    else
                        idsULsb.Append(",");
                }
            }
            if (idsULsb.Length > 0)
            {
                idsULsb.Remove(idsULsb.Length - 1, 1);
                bolker.Add(idsULsb.ToString());
            }
            return bolker;
        }

        /// <summary>
        ///     ''' GetUtvalgListId
        ///     ''' Metoden returnerer utvalgslisteid til ett utvalg med en gitt utvalgId.
        ///     ''' Dersom utvalget ikke har noen utvalgslisteid returneres verdien -1
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>

        private List<int> CheckIfParentUtvalgListsNeedOnTheFlyUpdate(IEnumerable<int> utvalgListIDs)
        {
            _logger.LogDebug("Inside into CheckIfParentUtvalgListsNeedOnTheFlyUpdate");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            List<int> result = new List<int>();
            foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = str;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.checkifparentutvalglistsneedontheflyupdate", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    result.Add((int)row["r_utvalglistid"]);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from CheckIfParentUtvalgListsNeedOnTheFlyUpdate");
            return result;

        }

        /// <summary>
        ///     ''' Returns a new list of utvalglistids containing the same ids as the input, except campaign list ids have been replaced with their basis list id.
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private List<int> ReplaceCampaignListIDsWithBasisListIDs(IEnumerable<int> utvalgListIDs)
        {
            _logger.LogDebug("Inside into ReplaceCampaignListIDsWithBasisListIDs");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            Dictionary<int, bool> resultDict = new Dictionary<int, bool>();
            foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = str;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.replacecampaignlistidswithbasislistids", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                {
                    int oldId = (int)row["r_utvalglistid"];
                    int newId = (int)row["r_basedon"];
                    if ((newId > 0))
                        resultDict[newId] = true;
                    else
                        resultDict[oldId] = true;
                }
            }
            _logger.LogInformation("Number of row returned: ", resultDict);

            _logger.LogDebug("Exiting from ReplaceCampaignListIDsWithBasisListIDs");
            return new List<int>(resultDict.Keys);

        }
        #endregion
    }
}
