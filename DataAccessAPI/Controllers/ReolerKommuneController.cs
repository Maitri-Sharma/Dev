#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.ReolerKommune;
using DataAccessAPI.HandleRequest.Response.ReolerKommune;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReolerKommuneController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ReolerKommuneController> _logger;
        private static Dictionary<ReolerKommuneKey, ReolerKommune> _ReolerKommuneCache;
        private static Dictionary<string, ReolerKommuneCollection> _ReolerKommuneCacheKommune;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors      
        /// <summary>
        /// Initializes a new instance of the <see cref="ReolerKommuneController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        public ReolerKommuneController(ILogger<ReolerKommuneController> logger, IMediator mediator,
            IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// To fetch data from Reoler Kommune table on the basis of Reol ID and Kommune ID
        /// </summary>
        /// <param name="ReolId">Reol ID to check in table</param>
        /// <param name="KommuneId">Kommune ID to check in table</param>
        /// <returns>ReolerKommune data</returns>

        [HttpGet]
        [ProducesResponseType(typeof(ReolerKommune), (int)HttpStatusCode.OK)]
        [Route("GetReolerKommune")]
        public IActionResult GetReolerKommune(long ReolId, string KommuneId)
        {
            _logger.BeginScope("Inside into GetReolerKommune");
            ReolerKommuneKey reolerKommuneKey = new ReolerKommuneKey(ReolId, KommuneId);
            ReolerKommune result = new ReolerKommune();

            if (_ReolerKommuneCache == null)
                _ReolerKommuneCache = new Dictionary<ReolerKommuneKey, ReolerKommune>();
            if (_ReolerKommuneCache.ContainsKey(reolerKommuneKey))
                return Ok(_ReolerKommuneCache[reolerKommuneKey]);

            #region Old Code
            //npgsqlParameters[0] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[0].Value = ReolId;

            //npgsqlParameters[1] = new NpgsqlParameter("p_kommuneid", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            //npgsqlParameters[1].Value = KommuneId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolerKommune = dbhelper.FillDataTable("kspu_gdb.getreolerkommune", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //if (reolerKommune?.Rows?.Count > 1)
            //{
            //    exception = new Exception("Fant flere ReolerKommune i databasen med Reolid " + ReolId + " og KommuneId " + KommuneId + ".");
            //    throw exception;
            //}
            //else if (reolerKommune?.Rows?.Count == 0)
            //{
            //    exception = new Exception("Fant ikke ReolerKommune hvor med Reolid " + ReolId + " og KommuneId " + KommuneId + ".");
            //    throw exception;
            //} 
            #endregion

            RequestGetReolerKommune request = new RequestGetReolerKommune()
            {
                KommuneId = KommuneId,
                ReolId = ReolId
            };
            var reoldData = _mediator.Send(request).Result;
            result = _mapper.Map<ResponseGetReolerKommune, ReolerKommune>(reoldData);
            _ReolerKommuneCache[reolerKommuneKey] = result;

            return Ok(result);
        }

        /// <summary>
        /// To fetch data from Reoler Kommune table on the basis of Kommune ID
        /// </summary>
        /// <param name="KommuneId">Kommune ID to fetch data from Reoler Kommune table</param>
        /// <returns>ReolerKommune data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ReolerKommuneCollection), (int)HttpStatusCode.OK)]
        [Route("GetReolerKommuneByKommuneId")]
        public IActionResult GetReolerKommuneByKommuneId(string KommuneId)
        {
            _logger.BeginScope("Inside into GetReolerKommuneByKommuneId");
            ReolerKommuneCollection result = new ReolerKommuneCollection();
            if (_ReolerKommuneCacheKommune == null)
                _ReolerKommuneCacheKommune = new Dictionary<string, ReolerKommuneCollection>();
            if (_ReolerKommuneCacheKommune.ContainsKey(KommuneId))
                return Ok(_ReolerKommuneCacheKommune[KommuneId]);

            #region Old Code
            //npgsqlParameters[0] = new NpgsqlParameter("p_kommuneid", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            //npgsqlParameters[0].Value = KommuneId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolerKommune = dbhelper.FillDataTable("kspu_gdb.getreolerkommunebykommuneid", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //if (reolerKommune?.Rows?.Count == 0)
            //{
            //    exception = new Exception("Fant ikke ReolerKommune i kommune med Kommuneid " + KommuneId + " i databasen.");
            //    throw exception;
            //}

            //foreach (DataRow dataRow in reolerKommune.Rows)
            //{
            //    result.Add(GetReolerKommuneFromDataRow(dataRow));
            //}

            #endregion
            RequestGetReolerKommuneByKommuneId request = new RequestGetReolerKommuneByKommuneId()
            {
                KommuneId = KommuneId
            };
            var koumneData = _mediator.Send(request).Result;
            var mapData = _mapper.Map<List<ResponseGetReolerKommuneByKommuneId>, List<ReolerKommune>>(koumneData);
            result.AddRange(mapData);
            _ReolerKommuneCacheKommune[KommuneId] = result;
            return Ok(result);
        }

        /// <summary>
        /// To fetch data from Reoler Kommune table
        /// </summary>
        /// <returns>ReolerKommune data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ReolerKommuneCollection), (int)HttpStatusCode.OK)]
        [Route("GetAllReolerKommune")]
        public IActionResult GetAllReolerKommune()
        {
            _logger.BeginScope("Inside into GetAllReolerKommune");
            ReolerKommuneCollection result = new ReolerKommuneCollection();
            ReolerKommuneKey reolerKommuneKey;

            if (_ReolerKommuneCache == null)
                _ReolerKommuneCache = new Dictionary<ReolerKommuneKey, ReolerKommune>();

            RequestGetAllReolerKommune request = new RequestGetAllReolerKommune();

            var komuneData = _mediator.Send(request).Result;
            var mapData = _mapper.Map<List<ResponseGetAllReolerKommune>, List<ReolerKommune>>(komuneData);
            result.AddRange(mapData);

            foreach (var item in result)
            {
                reolerKommuneKey = new ReolerKommuneKey(item.ReolId, item.KommuneId);
                if (!_ReolerKommuneCache.ContainsKey(reolerKommuneKey))
                {
                    _ReolerKommuneCache[reolerKommuneKey] = item;
                }
            }
            #region Old Code
            ////var Persons = _ReolerKommuneCache.Where(kvp => !mapData.Contains(kvp.Value))
            ////      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    reolerKommune = dbhelper.FillDataTable("kspu_gdb.getallreolerkommune", CommandType.StoredProcedure, null);
            //}

            //foreach (DataRow dataRow in reolerKommune.Rows)
            //{
            //    reolerKommuneKey = new ReolerKommuneKey((long)dataRow["r_reolid"], dataRow["r_kommuneid"].ToString());
            //    if (!_ReolerKommuneCache.ContainsKey(reolerKommuneKey))
            //        _ReolerKommuneCache[reolerKommuneKey] = GetReolerKommuneFromDataRow(dataRow);
            //    result.Add(_ReolerKommuneCache[reolerKommuneKey]);
            //}

            //_logger.LogInformation("Number of row returned {0}", result);

            //_logger.LogDebug("Exiting from GetAllReolerKommune"); 
            #endregion
            return Ok(result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To Populate data in Reoler Kommune Object
        /// </summary>
        /// <param name="row">Data row thay has reoler kommune data</param>
        /// <returns>ReolerKommune data</returns>
        private static ReolerKommune GetReolerKommuneFromDataRow(DataRow row)
        {
            ReolerKommune r = new ReolerKommune();
            r.KommuneId = row["r_kommuneid"].ToString();
            r.ReolId = (long)row["r_reolid"];
            r.HH = (int)row["r_HH"];
            r.ER = (int)row["r_ER"];
            r.GB = (int)row["r_GB"];
            r.VH = (int)row["r_VH"];
            r.HH_RES = (int)row["r_HH_RES"];
            r.ER_RES = (int)row["r_ER_RES"];
            r.GB_RES = (int)row["r_GB_RES"];
            return r;
        }
        #endregion
    }
}
