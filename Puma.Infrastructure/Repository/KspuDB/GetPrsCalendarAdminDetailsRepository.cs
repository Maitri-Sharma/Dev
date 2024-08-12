using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Puma.DataLayer.DatabaseModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using Puma.DataLayer.Helper;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class GetPrsCalendarAdminDetailsRepository : KsupDBGenericRepository<AddressPointsState>, IGetPrsCalendarAdminDetailsRepository
    {
        private readonly ILogger<GetPrsCalendarAdminDetailsRepository> _logger;
        public readonly string Connctionstring;

        public GetPrsCalendarAdminDetailsRepository(KspuDBContext context, ILogger<GetPrsCalendarAdminDetailsRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods

        /// <summary>
        /// Getting the AdminCalender Data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"> List of address point to be save</param>
        /// <returns> List of GetPrsAdminCalendarData</returns>

        public async Task<List<GetPrsAdminCalendarData>> GetPRSAdminCalendar(DateTime fromDate, DateTime toDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetPRSAdminCalendar");

            DataTable prsAdminData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            List<GetPrsAdminCalendarData> result = new List<GetPrsAdminCalendarData>();
            npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDate;
            npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDate;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                prsAdminData = dbhelper.FillDataTable("kspu_db.getprsadmincalender", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in prsAdminData.Rows)
                result.Add(GetPRSAdminDataFromDataRow(row));

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetPRSAdminCalendar");
            return result;
        }


        public async Task<GetPrsAdminCalendarData> GetPRSAdminCalendarDayDetail(DateTime FindDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetPRSAdminCalendarDayDetail");
            DataTable dt = new DataTable();
            DataSet resultDs = new DataSet();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("dato_in", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = FindDate;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        public async Task<List<DateTime>> GetDateWiseBookedCapacity(DateTime fromDate, DateTime toDate, long id, string type, string receiverType, int weight, double thickness = 0.0)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for GetDateWiseBookedCapacity");

                DataTable prsAdminData;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[7];
                List<DateTime> result = new List<DateTime>();
                npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[0].Value = fromDate;
                npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[1].Value = toDate;
                npgsqlParameters[2] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[2].Value = id;
                npgsqlParameters[3] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[3].Value = type;
                npgsqlParameters[4] = new NpgsqlParameter("p_receivertype", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[4].Value = receiverType;
                npgsqlParameters[5] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[5].Value = weight;
                npgsqlParameters[6] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
                npgsqlParameters[6].Value = thickness;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    prsAdminData = dbhelper.FillDataTable("kspu_db.getdatewisecapaciybooked", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow row in prsAdminData.Rows)
                    result.Add(Convert.ToDateTime(row["r_dato"]));

                _logger.LogInformation("Number of row returned {0}", result.Count);

                _logger.LogDebug("Exiting from GetDateWiseBookedCapacity");
                return result;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error in GetDateWiseBookedCapacity");
            }
            return null;
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
