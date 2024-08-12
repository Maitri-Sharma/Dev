#region Namespaces
using AutoMapper;
using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Kapasitet;
using DataAccessAPI.HandleRequest.Response.Kapasitet;
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
    [Authorize]
    //[ApiController]
    public class KapasitetController : ControllerBase
    {
        #region Variables
        private readonly ILogger<KapasitetController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="KapasitetController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// mapper
        /// </exception>
        public KapasitetController(ILogger<KapasitetController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets kapacity dates from database.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// Inputparamdates are included in result. 
        /// <returns>KapasitetDato List</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetKapasitetDatoer>), (int)HttpStatusCode.OK)]
        [Route("GetKapasitetDatoer")]
        public IActionResult GetKapasitetDatoer(DateTime fromDate, DateTime toDate)
        {
            _logger.BeginScope("Inside into GetKapasitetDatoer");
            #region Old Code
            //_logger.LogDebug("Inside into GetKapasitetDatoer");
            //DataTable kapasitetDato;
            //List<KapasitetDato> result = new List<KapasitetDato>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[0].Value = fromDate;

            //npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[1].Value = toDate;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    kapasitetDato = dbhelper.FillDataTable("kspu_gdb.getkapasitetdatoer", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in kapasitetDato.Rows)
            //    result.Add(GetkapasitetdatoFromDataRow(row));

            //_logger.LogInformation("Number of row returned {0}", result);

            //_logger.LogDebug("Exiting from GetKapasitetDatoer");
            //return result; 
            #endregion
            RequestGetKapasitetDatoer request = new RequestGetKapasitetDatoer()
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Get Total Antall
        ///     ''' </summary>
        ///     ''' <param name="id">Utvalg ID\List ID</param>
        ///     ''' <param name="type">U\L</param>
        ///     ''' <returns> Antall count</returns>
        [HttpGet]
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        [Route("GetTotalAntall")]
        public IActionResult GetTotalAntall(long id, string type)
        {
            _logger.BeginScope("Inside into GetTotalAntall");
            #region Old Code
            //_logger.LogDebug("Inside into GetTotalAntall");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[0].Value = id;

            //npgsqlParameters[1] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = type;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteScalar<long>("kspu_db.gettotalantall", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //_logger.LogInformation("Returned result {0}", result);

            //_logger.LogDebug("Exiting from GetTotalAntall");
            //return Convert.ToInt64(result); 
            #endregion
            RequestGetTotalAntall request = new RequestGetTotalAntall()
            {
                id = id,
                type = type
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get dates lacking capacity
        /// </summary>
        /// <param name="fromDate">Date from</param>
        /// <param name="toDate">Date to</param>
        /// <param name="id">Utvalg ID\List ID</param>
        /// <param name="type">U\L</param>
        /// <param name="receiverType"></param>
        /// <param name="weight">Weight of Order</param>
        /// <param name="thickness">Thicknesss of order</param>
        /// <returns>LackingCapacity list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetDatesLackingCapacity>), (int)HttpStatusCode.OK)]
        [Route("GetDatesLackingCapacity")]
        public IActionResult GetDatesLackingCapacity(DateTime fromDate, DateTime toDate, long id, string type, string receiverType, int weight, double thickness = 0.0)
        {
            _logger.BeginScope("Inside into GetDatesLackingCapacity");
            #region Old Code
            //_logger.LogDebug("Inside into GetDatesLackingCapacity");
            //DataTable lackingCapacity;
            //List<LackingCapacity> result = new List<LackingCapacity>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[7];

            //npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[0].Value = fromDate;

            //npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            //npgsqlParameters[1].Value = toDate;

            //npgsqlParameters[2] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[2].Value = id;

            //npgsqlParameters[3] = new NpgsqlParameter("p_receivertype", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[3].Value = receiverType;

            //npgsqlParameters[4] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[4].Value = weight;

            //npgsqlParameters[5] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            //npgsqlParameters[5].Value = thickness;

            //npgsqlParameters[6] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[6].Value = type;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    lackingCapacity = dbhelper.FillDataTable("kspu_db.getdateslackingcapacity", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in lackingCapacity.Rows)
            //{
            //    LackingCapacity lc = new LackingCapacity();
            //    lc.Dato = (DateTime)row["dato"];
            //    lc.HouseholdsLackingCapacity = (long)row["Antall"];
            //    result.Add(lc);
            //}

            //_logger.LogInformation("Number of row returned {0}", result.Count);

            //_logger.LogDebug("Exiting from GetDatesLackingCapacity");
            //return result; 
            #endregion
            RequestGetDatesLackingCapacity request = new RequestGetDatesLackingCapacity()
            {
                id = id,
                type = type,
                fromDate = fromDate,
                receiverType = receiverType,
                thickness = thickness,
                toDate = toDate,
                weight =weight
            };

            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get routes lacking capacity
        /// </summary>
        /// <param name="dates">Date from</param>
        /// <param name="id">Utvalg ID\List ID</param>
        /// <param name="type">U\L</param>
        /// <param name="receiverType"></param>
        /// <param name="weight">Weight of Order</param>
        /// <param name="thickness">Thicknesss of order</param>
        /// <returns>KapasitetRuter list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseGetRuterLackingCapacity>), (int)HttpStatusCode.OK)]
        [Route("GetRuterLackingCapacity")]
        public IActionResult GetRuterLackingCapacity(List<string> dates, long id, string type, string receiverType, int weight, double thickness = 0.0)
        {
            _logger.BeginScope("Inside into GetRuterLackingCapacity");
            #region Old Code
            //_logger.LogDebug("Inside into GetRuterLackingCapacity");
            //DataTable kapasitetRuter;
            //List<KapasitetRuter> result = new List<KapasitetRuter>();
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];

            //if (dates == null || dates.Count == 0)
            //    return new List<KapasitetRuter>();

            //for (int i = 0; i <= dates.Count - 1; i++)
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_dates" + i, NpgsqlTypes.NpgsqlDbType.Date);
            //    npgsqlParameters[0].Value = dates[i];
            //}

            //npgsqlParameters[1] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            //npgsqlParameters[1].Value = thickness;

            //npgsqlParameters[2] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            //npgsqlParameters[2].Value = id;

            //npgsqlParameters[3] = new NpgsqlParameter("p_receivertype", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[3].Value = receiverType;

            //npgsqlParameters[4] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[4].Value = weight;

            //npgsqlParameters[5] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[5].Value = type;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    kapasitetRuter = dbhelper.FillDataTable("kspu_db.getruterlackingcapacity", CommandType.StoredProcedure, npgsqlParameters);
            //}

            //foreach (DataRow row in kapasitetRuter.Rows)
            //    result.Add(GetkapasitetruterFromDataRow(row));

            //_logger.LogInformation("Number of row returned {0}", result.Count);

            //_logger.LogDebug("Exiting from GetRuterLackingCapacity");
            //return result; 
            #endregion
            RequestGetRuterLackingCapacity request = new RequestGetRuterLackingCapacity()
            {
                dates = dates,
                id = id,
                receiverType = receiverType,
                thickness = thickness,
                type = type,
                weight = weight
            };
            return Ok(_mediator.Send(request).Result);

        }


        /// <summary>
        ///     ''' Metoden reduserer restvekt og restantall på liste av budruter ved å trekke fra utvalgets restvekt og restantall.
        ///     ''' Restkapasitet eller -vekt kan bli negativ
        ///     ''' Tell ned basert på receivertype
        ///     ''' </summary>
        ///     ''' <param name="ruteIds"></param>
        ///     ''' <param name="restvekt"></param>
        ///     ''' <param name="dato"></param>
        ///     ''' <param name="mottakerType"></param>
        ///     ''' <remarks></remarks>
      
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("SubtractRestkapasitetAbsoluttDag")]
        public IActionResult SubtractRestkapasitetAbsoluttDag(List<long> ruteIds, int restvekt, DateTime dato, string mottakerType)
        {
            _logger.BeginScope("Inside into SubtractRestkapasitetAbsoluttDag");
            #region Old Code
            //_logger.LogDebug("Inside into SubtractRestkapasitetAbsoluttDag");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            //int? output;
            //int result;

            //if (ruteIds == null || ruteIds.Count == 0)
            //    return;

            //foreach (var id in ruteIds)
            //{
            //    #region Parameter assignement

            //    npgsqlParameters[0] = new NpgsqlParameter("p_ruteids", NpgsqlTypes.NpgsqlDbType.Bigint);
            //    npgsqlParameters[0].Value = id;

            //    npgsqlParameters[1] = new NpgsqlParameter("p_restvekt", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[1].Value = restvekt;

            //    npgsqlParameters[2] = new NpgsqlParameter("p_mottakertype", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            //    npgsqlParameters[2].Value = mottakerType;

            //    npgsqlParameters[3] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Date);
            //    npgsqlParameters[3].Value = dato;

            //    npgsqlParameters[4] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[4].Direction = ParameterDirection.Output;


            //    #endregion

            //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //    {
            //      output =  dbhelper.ExecuteNonQuery("kspu_db.subtractrestkapasitetabsoluttdag", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //    if (output == null)
            //    {
            //        _logger.LogWarning("Kunne ikke redusere restvekt og -kapasitet på utvalg, dato:" + dato.ToShortDateString() + ", restvekt:" + restvekt + ", mottakertype:" + mottakerType);
            //    }
            //    else
            //    {
            //        result = Convert.ToInt32(npgsqlParameters[5].Value);
            //        _logger.LogInformation(string.Format("Number of row affected {0} ", result));
            //    }
            //}

            //_logger.LogDebug("Exiting from SubtractRestkapasitetAbsoluttDag"); 
            #endregion
            RequestSubtractRestkapasitetAbsoluttDag request = new RequestSubtractRestkapasitetAbsoluttDag()
            {
               dato = dato,
               mottakerType = mottakerType,
               restvekt = restvekt,
               ruteIds = ruteIds
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        ///     ''' Return dates that can be used by sperrefrist.
        ///     ''' Length is set by dayCount. Max 365
        ///     ''' </summary>
        ///     ''' <param name="dato"></param>
        ///     ''' <param name="dayCount"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<DateTime>), (int)HttpStatusCode.OK)]
        [Route("GetSperrefristDates")]
        public IActionResult GetSperrefristDates(DateTime dato, int dayCount)
        {
            _logger.BeginScope("Inside into GetSperrefristDates");
            #region Old Code
            //List<DateTime> sperrefristdays = new List<DateTime>();
            //DateTime d = dato;
            //while (sperrefristdays.Count < dayCount)
            //{
            //    if (!(d.DayOfWeek.Equals(DayOfWeek.Saturday) | d.DayOfWeek.Equals(DayOfWeek.Sunday)))
            //        sperrefristdays.Add(d);
            //    d = d.AddDays(1);
            //}
            //return sperrefristdays; 
            #endregion
            RequestGetSperrefristDates request = new RequestGetSperrefristDates()
            {
               dato = dato,
               dayCount = dayCount
            };
            return Ok(_mediator.Send(request).Result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [Route("SubtractRestkapasitetSperrefrist")]
        public IActionResult SubtractRestkapasitetSperrefrist(List<long> ruteIds, int restvekt, DateTime dato, string mottakertype)
        {
            _logger.BeginScope("Inside into SubtractRestkapasitetSperrefrist");
            #region Old COde
            //_logger.LogDebug("Inside into SubtractRestkapasitetSperrefrist");

            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            //int? output;
            //int result;

            //// get all possible dates for delivery
            //foreach (DateTime dag in GetSperrefristDates(dato, 3))
            //{
            //        #region Parameter assignement

            //        npgsqlParameters[0] = new NpgsqlParameter("p_ruteids", NpgsqlTypes.NpgsqlDbType.Bigint);
            //        npgsqlParameters[0].Value = ruteIds;

            //        npgsqlParameters[1] = new NpgsqlParameter("p_restvekt", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[1].Value = restvekt;

            //        npgsqlParameters[2] = new NpgsqlParameter("p_mottakertype", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            //        npgsqlParameters[2].Value = mottakertype;

            //        npgsqlParameters[3] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Date);
            //        npgsqlParameters[3].Value = dato;

            //        npgsqlParameters[4] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            //        npgsqlParameters[4].Direction = ParameterDirection.Output;


            //        #endregion

            //        using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //        {
            //            output = dbhelper.ExecuteNonQuery("kspu_db.subtractrestkapasitetsperrefrist", CommandType.StoredProcedure, npgsqlParameters);
            //        }
            //        if (output == null)
            //        {
            //            _logger.LogWarning("Kunne ikke redusere restvekt og -kapasitet på utvalg, dato:" + dato.ToShortDateString() + ", restvekt:" + restvekt + ", mottakertype:" + mottakertype);
            //        }
            //        else
            //        {
            //            result = Convert.ToInt32(npgsqlParameters[5].Value);
            //            _logger.LogInformation(string.Format("Number of row affected {0} ", result));
            //        }

            //}
            //_logger.LogDebug("Exiting from SubtractRestkapasitetSperrefrist"); 
            #endregion
            RequestSubtractRestkapasitetSperrefrist request = new RequestSubtractRestkapasitetSperrefrist()
            {
              dato = dato,
              mottakertype = mottakertype,
              restvekt = restvekt,
              ruteIds = ruteIds
            };
            return Ok(_mediator.Send(request).Result);
        }


        #endregion


        #region Private Methods

        /// <summary>
        /// Fill KapasitetDato object
        /// </summary>
        /// <param name="row">Object of Datarow</param>
        /// <returns>KapasitetDato data</returns>
        private KapasitetDato GetkapasitetdatoFromDataRow(DataRow row)
        {
            KapasitetDato kDato = new KapasitetDato();
            kDato.Dato = (DateTime)row["Dato"];
            kDato.UkeNr = (int)row["Ukenr"];
            if (row["Distribusjonsdag"].ToString().ToUpper().Equals("Y"))
                kDato.Distribusjonsdag = true;
            else
                kDato.Distribusjonsdag = false;
            if (row["Virkedag"].ToString().ToUpper().Equals("Y"))
                kDato.Virkedag = true;
            else
                kDato.Virkedag = false;
            if (row["ISEARLYWEEKFIRSTDAY"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsEarlyWeekFirstDay = true;
            else
                kDato.IsEarlyWeekFirstDay = false;
            if (row["ISEARLYWEEKSECONDDAY"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsEarlyWeekSecondDay = true;
            else
                kDato.IsEarlyWeekSecondDay = false;
            if (row["ISMIDWEEKFIRSTDAY"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsMidWeekFirstDay = true;
            else
                kDato.IsMidWeekFirstDay = false;
            if (row["ISMIDWEEKSECONDDAY"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsMidWeekSecondDay = true;
            else
                kDato.IsMidWeekSecondDay = false;
            return kDato;
        }

        /// <summary>
        /// Fill KapasitetRuter object
        /// </summary>
        /// <param name="row">instance of Datarow</param>
        /// <returns>KapasitetRuter data</returns>
        private KapasitetRuter GetkapasitetruterFromDataRow(DataRow row)
        {
            KapasitetRuter kRuter = new KapasitetRuter();
            kRuter.Dato = (DateTime)row["Dato"];
            kRuter.RuteNr = (long)row["ReolNr"];
            kRuter.RestVekt = (int)row["RestVekt"];
            kRuter.RestAntall = (int)row["RestAntall"];
            kRuter.MottakerType = row["MottakerType"].ToString();
            kRuter.RestThickness = (double)row["RestThickness"];
            return kRuter;
        }
        #endregion

    }
}
