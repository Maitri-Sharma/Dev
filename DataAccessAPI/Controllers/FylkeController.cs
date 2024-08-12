#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Fylke;
using DataAccessAPI.HandleRequest.Response.Fylke;
using DataAccessAPI.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FylkeController : ControllerBase
    {
        #region Variables
        private readonly ILogger<FylkeController> _logger;
        private static Dictionary<string, FylkeCollection> _FylkeCache;
        private readonly ConfigController configController;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="FylkeController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// mapper
        /// </exception>
        public FylkeController(ILogger<FylkeController> logger, ILogger<ConfigController> loggerConfig,
            IMapper mapper, IMediator mediator)
        {
            _logger = logger;
            configController = new ConfigController(loggerConfig);
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// To fetch Fylke data
        /// </summary>
        /// <returns>FylkeCollection data</returns>
        [HttpGet]
        [ProducesResponseType(typeof(FylkeCollection), (int)HttpStatusCode.OK)]
        [Route("GetAllFylkes")]
        public IActionResult GetAllFylkes()
        {
            _logger.BeginScope("Inside into GetAllFylkes");
            FylkeCollection allFylkes = new FylkeCollection();
            FylkeCollection result = new FylkeCollection();
            string table = configController.CurrentReolTableName;

            if (_FylkeCache == null)
                _FylkeCache = new Dictionary<string, FylkeCollection>();
            if (!_FylkeCache.ContainsKey(table))
            {
                #region Old Code
                //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                //{
                //    Fylker = dbhelper.FillDataTable("kspu_gdb.getallfylkes", CommandType.StoredProcedure, null);
                //}

                //foreach (DataRow row in Fylker.Rows)
                //    allFylkes.Add(GetFylkeFromDataRow(row)); 
                #endregion
                _FylkeCache[table] = allFylkes;

                var fylkerData = _mediator.Send(new RequestGetAllFylkes()).Result;
                var data = _mapper.Map<List<ResponseGetAllFylkes>, List<Puma.Shared.Fylke>>(fylkerData);
                allFylkes.AddRange(data);
                _FylkeCache[table] = allFylkes;
            }
            result.AddRange(_FylkeCache[table]);
            return Ok(result);
        }

        /// <summary>
        /// To fill data in Fylke object
        /// </summary>
        /// <param name="FylkeId"> Fylke ID to fetch data from database</param>
        /// <returns>Fylke data</returns>
        [ProducesResponseType(typeof(ResponseGetFylkes), (int)HttpStatusCode.OK)]
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetFylke(string FylkeId)
        {
            _logger.BeginScope("Inside into GetFylke");
            RequestGetFylke request = new RequestGetFylke() { FylkeId = FylkeId };

            return Ok(_mediator.Send(request).Result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fill data in Fylke object
        /// </summary>
        /// <param name="row">Instance of Datarow</param>
        /// <returns>Fylke data</returns>
        private static Fylke GetFylkeFromDataRow(DataRow row)
        {
            Fylke f = new Fylke();
            f.FylkeID = row["r_fylke_id"].ToString();
            f.FylkeName = row["r_fylke"].ToString();
            return f;
        }

        #endregion
    }
}
