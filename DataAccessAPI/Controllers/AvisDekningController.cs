#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.AvisDekning;
using DataAccessAPI.HandleRequest.Response.AvisDekning;
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
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AvisDekningController : ControllerBase
    {
        #region Variables

        private readonly ILogger<AvisDekningController> _logger;
        private static List<string> _AllUtgaverCache;
        private static Dictionary<long, AvisDekningCollection> _AvisDekningCache;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="AvisDekningController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// mapper
        /// </exception>
        public AvisDekningController(ILogger<AvisDekningController> logger, IMediator mediator,
            IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));


        }
        #endregion

        #region Public Methods
        /// <summary>
        /// To check if Avis exists
        /// </summary>
        /// <param name="utgave">utgave to fetch data from AvisDekning table</param>
        /// <returns>True or false</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("AvisExists")]
        public IActionResult AvisExists(string utgave)
        {
            _logger.BeginScope("Inside into AvisExists");
            RequestAvisExists request = new RequestAvisExists()
            {
                utgave = utgave
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// To fetch data from AvisDekning table
        /// </summary>
        /// <returns>List of Utgave</returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("GetAllUtgaver")]
        public IActionResult GetAllUtgaver()
        {
            _logger.BeginScope("Inside into GetAllUtgaver");
            //TODO: For cache first will check data here if null then will call mediator and will return result
            if ((_AllUtgaverCache == null))
            {
                RequestGetAllUtgaver request = new RequestGetAllUtgaver();

                _AllUtgaverCache = _mediator.Send(request).Result;
            }

            _logger.LogInformation("Number of row returned: ", _AllUtgaverCache.Count);
            _logger.LogDebug("Exiting from GetAllUtgaver");

            return Ok(_AllUtgaverCache);
        }

        /// <summary>
        /// To fetch data from AVISFIELDMAPPING table
        /// </summary>
        /// <returns>Avis List</returns>

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseAvis>), (int)HttpStatusCode.OK)]
        [Route("GetPaperList")]
        public IActionResult GetPaperList()
        {
            _logger.BeginScope("Inside into GetPaperList");
            RequestGetPaperList request = new RequestGetPaperList();
            return Ok(_mediator.Send(request).Result);

        }

        /// <summary>
        /// To fetch Coverage List
        /// </summary>
        /// <returns>Coverage List</returns>
        [HttpGet("GetCoverageList", Name = nameof(GetCoverageList))]
        public DataTable GetCoverageList(string[] feltnavn)
        {
            _logger.BeginScope("Inside into GetCoverageList");
            DataTable coverage = null;
            if (feltnavn != null && feltnavn.Length > 0)
            {
                StringBuilder feltnavnSql = new StringBuilder();
                foreach (string item in feltnavn)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        feltnavnSql.Append(",nvl(adk.");
                        feltnavnSql.Append(item);
                        feltnavnSql.Append(",0) as ");
                        feltnavnSql.Append(item);
                    }
                }

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    coverage = dbhelper.FillDataTable((string.Format("kspu_db.getcoveragelist", feltnavnSql.ToString())), CommandType.StoredProcedure, null);
                }
            }

            _logger.LogInformation("Number of row returned {0}", coverage.Rows.Count);

            _logger.LogDebug("Exiting from GetCoverageList");
            return coverage;
        }

        /// <summary>
        /// To fetch data from AvisDekning table on the basis of Reol ID
        /// </summary>
        /// <returns>Avis Dekning Collection</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AvisDekningCollection), (int)HttpStatusCode.OK)]
        [Route("GetAvisDekning")]
        public IActionResult GetAvisDekning(long reolId)
        {
            _logger.BeginScope("Inside into GetAvisDekning");
            AvisDekningCollection result = new AvisDekningCollection();
            if (_AvisDekningCache == null)
                _AvisDekningCache = new Dictionary<long, AvisDekningCollection>();
            if (_AvisDekningCache.ContainsKey(reolId))
                return Ok(_AvisDekningCache[reolId]);

            #region Old Code
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[0] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            //npgsqlParameters[0].Value = Convert.ToString(reolId);

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    avisDekning = dbhelper.FillDataTable("kspu_db.getavisdekning", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow dataRow in avisDekning.Rows)
            //{
            //    result.Add(GetAvisDekningFromDataRow(dataRow));
            //} 
            #endregion

            RequestGetAvisDekning request = new RequestGetAvisDekning()
            {
                reolId = reolId
            };

            var data = _mediator.Send(request).Result;
            var avisDekningData = _mapper.Map<List<ResponseAvisDekning>, List<AvisDekning>>(data);
            result.AddRange(avisDekningData);

            _AvisDekningCache[reolId] = result;

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAvisDekning");

            return Ok(result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fetch data from AvisDekning table
        /// </summary>
        /// <returns>Avis List</returns>
        private AvisDekning GetAvisDekningFromDataRow(DataRow dataRow)
        {
            AvisDekning avisDekning = new AvisDekning();
            avisDekning.ReolId = long.Parse(dataRow["r_reolid"].ToString());
            avisDekning.Utgave = dataRow["r_utgave"].ToString();
            // r.Eksemplar = commonRepository.GetOracleNumberFromRow(row, "EKSEMPLAR");
            //r.Prosent = commonRepository.GetOracleNumberFromRow(row, "PROSENT");
            return avisDekning;
        }

        #endregion

    }
}
