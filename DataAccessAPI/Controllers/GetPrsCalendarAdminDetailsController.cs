#region Namespaces
using DataAccessAPI.HandleRequest.Request.Calendar;
using DataAccessAPI.HandleRequest.Response.Calendar;
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using MediatR;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class GetPrsCalendarAdminDetailsController : ControllerBase
    {
        #region Variables
        private readonly ILogger<GetPrsCalendarAdminDetailsController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPrsCalendarAdminDetailsController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediator">The mediator.</param>
        public GetPrsCalendarAdminDetailsController(ILogger<GetPrsCalendarAdminDetailsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Getting the AdminCalender Data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"> List of address point to be save</param>
        /// <returns> List of GetPrsAdminCalendarData</returns>
        [HttpGet("GetPRSAdminCalendar", Name = nameof(GetPRSAdminCalendar))]
        public List<GetPrsAdminCalendarData> GetPRSAdminCalendar(DateTime fromDate, DateTime toDate)
        {
            _logger.BeginScope("Inside into GetPRSAdminCalendar");

            DataTable prsAdminData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            List<GetPrsAdminCalendarData> result = new List<GetPrsAdminCalendarData>();
            npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDate;
            npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDate;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                prsAdminData = dbhelper.FillDataTable("kspu_db.getprsadmincalender", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in prsAdminData.Rows)
                result.Add(GetPRSAdminDataFromDataRow(row));

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetPRSAdminCalendar");
            return result;
        }

        [HttpGet("GetPRSAdminCalendarDayDetail", Name = nameof(GetPRSAdminCalendarDayDetail))]
        public GetPrsAdminCalendarData GetPRSAdminCalendarDayDetail(DateTime FindDate)
        {
            _logger.BeginScope("Inside into GetPRSAdminCalendarDayDetail");
            DataTable dt = new DataTable();
            DataSet resultDs = new DataSet();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("dato_in", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = FindDate;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                resultDs = dbhelper.FillDataSet("kspu_db.sp_prscalendardatafortheday", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (resultDs.Tables.Count < 1)
                _logger.LogError(new Exception("Select command  returned no data table."), "Det oppsto en feil ved kjøring av en spørring mot databasen.");
            else
                dt = resultDs.Tables[0];


            GetPrsAdminCalendarData result = new GetPrsAdminCalendarData();
            if (dt.Rows.Count == 1)
            {
                foreach (DataRow row in dt.Rows)
                    result = GetPRSAdminDataFromDataRow(row);
            }

            return result;
        }


        /// <summary>
        /// Gets the PRS admin data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="vekt">The vekt.</param>
        /// <param name="distribusjonstype">The distribusjonstype.</param>
        /// <param name="startDato">The start dato.</param>
        /// <param name="sluttDato">The slutt dato.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetPrsAdminData), (int)HttpStatusCode.OK)]
        [Route("GetPrsAdminData")]
        public IActionResult GetPrsAdminData(int id, string type, int vekt, string distribusjonstype, DateTime startDato,
            DateTime sluttDato, double thickness)
        {
            _logger.BeginScope("Inside into GetPrsAdminData");
            RequestGetPrsAdminData request = new RequestGetPrsAdminData()
            {
                Distribusjonstype = distribusjonstype,
                Id = id,
                SluttDato = sluttDato,
                StartDato = startDato,
                Thickness = thickness,
                Type = type,
                Vekt = vekt
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// Gets the PRS calendar detail.
        /// </summary>
        /// <param name="findDate">The find date.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetPrsCalendarDetail), (int)HttpStatusCode.OK)]
        [Route("GetPrsCalendarDetail")]
        public IActionResult GetPrsCalendarDetail(DateTime findDate)
        {

            RequestGetPrsCalendarDetail request = new RequestGetPrsCalendarDetail()
            {
                FindDate = findDate
            };
            return Ok(_mediator.Send(request).Result);
        }


        /// <summary>
        /// Gets Rest Capacity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="vekt">The vekt.</param>
        /// <param name="distribusjonstype">The distribusjonstype.</param>
        /// <param name="startDato">The start dato.</param>
        /// <param name="sluttDato">The slutt dato.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseGetRestcapacity), (int)HttpStatusCode.OK)]
        [Route("GetRestcapacity")]
        public IActionResult GetRestcapacity(int id, string type, int vekt, string distribusjonstype, DateTime startDato,
            DateTime sluttDato, double thickness)
        {

            RequestGetRestcapacity request = new RequestGetRestcapacity()
            {
                Distribusjonstype = distribusjonstype,
                Id = id,
                SluttDato = sluttDato,
                StartDato = startDato,
                Thickness = thickness,
                Type = type,
                Vekt = vekt
            };
            return Ok(_mediator.Send(request).Result);
        }

        /// <summary>
        /// Get Rute Info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="vekt">The vekt.</param>
        /// <param name="distribusjonstype">The distribusjonstype.</param>
        /// <param name="valgtDato">The valg dato.</param>
        /// <param name="ruteIDs">Rute IDs.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseGetRuteinfo), (int)HttpStatusCode.OK)]
        [Route("GetRuteinfo")]
        public IActionResult GetRuteinfo([FromBody] List<long> ruteIDs, int id, string type, int vekt, string distribusjonstype, DateTime valgtDato, double thickness)
        {

            RequestGetRuteinfo request = new RequestGetRuteinfo()
            {
                Distribusjonstype = distribusjonstype,
                Id = id,
                Thickness = thickness,
                ValgtDato = valgtDato,
                Type = type,
                Vekt = vekt,
                RuteIDs = ruteIDs
            };
            return Ok(_mediator.Send(request).Result);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Populates a list of GetPRSAdminCalendar from the datarow.
        /// </summary>
        /// <param name="row">An instance of IDataReader</param>
        /// <returns>GetPrsAdminCalendarData</returns>
        private GetPrsAdminCalendarData GetPRSAdminDataFromDataRow(DataRow row)
        {
            Utils utils = new Utils();
            GetPrsAdminCalendarData calData = new GetPrsAdminCalendarData();
            string FrequencyType;
            string IsEarlyWeekFirstDay;
            string IsEarlyWeekSecondDay;
            string IsMidWeekFirstDay;
            string IsMidWeekSecondDay;

            //calData.Dato = utils.GetDateFromRow(row,"r_dato");   //(DateTime)row["r_dato"];
            calData.Dato = (DateTime)row["r_dato"];
            FrequencyType = row["r_frequencytype"].ToString();
            IsEarlyWeekFirstDay = row["r_isearlyweekfirstday"].ToString();
            IsEarlyWeekSecondDay = row["r_isearlyweeksecondday"].ToString();
            IsMidWeekFirstDay = row["r_ismidweekfirstday"].ToString();
            IsMidWeekSecondDay = row["r_ismidweeksecondday"].ToString();

            if (!string.IsNullOrEmpty(FrequencyType))
                calData.FrequencyType = FrequencyType.ToUpper();

            if (!string.IsNullOrEmpty(IsEarlyWeekFirstDay))
            {
                if ((IsEarlyWeekFirstDay.Trim().ToUpper().Equals("Y")))
                    calData.IsEarlyWeekFirstDay = true;
                else
                    calData.IsEarlyWeekFirstDay = false;
            }
            else
                calData.IsEarlyWeekFirstDay = false;

            if (!string.IsNullOrEmpty(IsEarlyWeekSecondDay))
            {
                if ((IsEarlyWeekSecondDay.Trim().ToUpper().Equals("Y")))
                    calData.IsEarlyWeekSecondDay = true;
                else
                    calData.IsEarlyWeekSecondDay = false;
            }
            else
                calData.IsEarlyWeekSecondDay = false;

            if (!string.IsNullOrEmpty(IsMidWeekFirstDay))
            {
                if ((IsMidWeekFirstDay.Trim().ToUpper().Equals("Y")))
                    calData.IsMidWeekFirstDay = true;
                else
                    calData.IsMidWeekFirstDay = false;
            }
            else
                calData.IsMidWeekFirstDay = false;

            if (!string.IsNullOrEmpty(IsMidWeekSecondDay))
            {
                if (row["r_ismidweeksecondday"].ToString().Trim().ToUpper().Equals("Y"))
                    calData.IsMidWeekSecondDay = true;
                else
                    calData.IsMidWeekSecondDay = false;
            }
            else
                calData.IsMidWeekSecondDay = false;

            if (row["r_isholiday"].ToString().Trim().ToUpper().Equals("Y"))
                calData.IsHoliday = true;
            else
                calData.IsHoliday = false;
            calData.LastModifiedDate = (DateTime)row["r_lastmodifieddate"];
            calData.WeekNo = (int)row["r_weekno"];
            return calData;

        }

        #endregion
    }
}
