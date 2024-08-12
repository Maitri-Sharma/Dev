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
using System.Text;
using System.Linq;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class KapasitetRepository : KsupDBGenericRepository<AddressPointsState>, IKapasitetRepository
    {
        private readonly ILogger<KapasitetRepository> _logger;
        public readonly string Connctionstring;

        public KapasitetRepository(KspuDBContext context, ILogger<KapasitetRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods

        /// <summary>
        /// Gets kapacity dates from database.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// Inputparamdates are included in result. 
        /// <returns>KapasitetDato List</returns>
        public async Task<List<KapasitetDato>> GetKapasitetDatoer(DateTime fromDate, DateTime toDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetKapasitetDatoer");
            DataTable kapasitetDato;
            List<KapasitetDato> result = new List<KapasitetDato>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDate;

            npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDate;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                kapasitetDato = dbhelper.FillDataTable("kspu_db.getkapasitetdatoer", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in kapasitetDato.Rows)
                result.Add(GetkapasitetdatoFromDataRow(row));

            _logger.LogInformation("Number of row returned {0}", result);

            _logger.LogDebug("Exiting from GetKapasitetDatoer");
            return result;
        }

        /// <summary>
        ///     ''' Get Total Antall
        ///     ''' </summary>
        ///     ''' <param name="id">Utvalg ID\List ID</param>
        ///     ''' <param name="type">U\L</param>
        ///     ''' <returns> Antall count</returns>
        public async Task<long> GetTotalAntall(long id, string type)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetTotalAntall");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = id;

            npgsqlParameters[1] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = type;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_db.gettotalantall", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Returned result {0}", result);

            _logger.LogDebug("Exiting from GetTotalAntall");
            return Convert.ToInt64(result);
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
        public async Task<List<LackingCapacity>> GetDatesLackingCapacity(DateTime fromDate, DateTime toDate, long id, string type, string receiverType, int weight, double thickness = 0.0)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetDatesLackingCapacity");
            DataTable lackingCapacity;
            List<LackingCapacity> result = new List<LackingCapacity>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[7];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDate;

            npgsqlParameters[1] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDate;

            npgsqlParameters[2] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[2].Value = id;

            npgsqlParameters[3] = new NpgsqlParameter("p_receivertype", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[3].Value = receiverType;

            npgsqlParameters[4] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[4].Value = weight;

            npgsqlParameters[5] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[5].Value = thickness;

            npgsqlParameters[6] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = type;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                lackingCapacity = dbhelper.FillDataTable("kspu_db.getdateslackingcapacity", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in lackingCapacity.Rows)
            {
                LackingCapacity lc = new LackingCapacity();
                lc.Dato = (DateTime)row["dato"];
                lc.HouseholdsLackingCapacity = (long)row["Antall"];
                result.Add(lc);
            }

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetDatesLackingCapacity");
            return result;
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
        public async Task<List<KapasitetRuter>> GetRuterLackingCapacity(List<string> dates, long id, string type, string receiverType, int weight, double thickness = 0.0)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetRuterLackingCapacity");
            DataTable kapasitetRuter;
            List<KapasitetRuter> result = new List<KapasitetRuter>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];

            if (dates == null || dates.Count == 0)
                return new List<KapasitetRuter>();

            string strdates = string.Concat(dates.Select(x => string.Format("cast('{0}' as date), ", x)));

            npgsqlParameters[0] = new NpgsqlParameter("p_dates", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = strdates.Substring(0,strdates.Length-2);



            npgsqlParameters[1] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = id;

            npgsqlParameters[2] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = type;

            npgsqlParameters[3] = new NpgsqlParameter("p_receivertype", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[3].Value = receiverType;

            npgsqlParameters[4] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[4].Value = weight;

            npgsqlParameters[5] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[5].Value = thickness;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                kapasitetRuter = dbhelper.FillDataTable("kspu_db.getruterlackingcapacity", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow row in kapasitetRuter.Rows)
                result.Add(GetkapasitetruterFromDataRow(row));

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetRuterLackingCapacity");
            return result;
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
        public async Task SubtractRestkapasitetAbsoluttDag(List<long> ruteIds, int restvekt, DateTime dato, string mottakerType, double thickness)
        {
            await Task.Run(() => { });
            if (ruteIds == null || ruteIds.Count == 0)
                return;

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE KSPU_DB.kapasitetruter");
            sql.Append(" SET RESTVEKT = RESTVEKT - " + restvekt + ", restantall = restantall - 1, RESTTHICKNESS = RESTTHICKNESS - " + thickness);
            sql.Append(" WHERE dato = '" + dato.Date + "' ");
            sql.Append(" AND mottakertype = '" + mottakerType + "' ");
            sql.Append(" AND ");
            AddInClauseValues(sql, ruteIds);


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(sql.ToString(), CommandType.Text, null);
            }

        }

        private void AddInClauseValues(StringBuilder sql, List<long> ruteIds)
        {
            sql.Append(" ( reolnr IN (");
            StringBuilder ids = new StringBuilder();
            bool addMore = false;
            foreach (long rId in ruteIds)
            {
                if (addMore)
                {
                    ids.Append(") OR reolnr IN (");
                    addMore = false;
                }
                else if (ids.Length > 0)
                    ids.Append(",");

                ids.Append(rId);
                if (ids.Length > 900)
                {
                    sql.Append(ids.ToString());
                    ids = new StringBuilder();
                    addMore = true;
                }
            }
            ids.Append(") ) ");
            sql.Append(ids.ToString()); // appending to sql
        }







        //public async Task SubtractRestkapasitetAbsoluttDag(List<long> ruteIds, int restvekt, DateTime dato, string mottakerType)
        //{
        //    await Task.Run(() => { });
        //    _logger.LogDebug("Preparing the data for SubtractRestkapasitetAbsoluttDag");


        //    int? output;
        //    int result;

        //    if (ruteIds == null || ruteIds.Count == 0)
        //        return;

        //    foreach (var id in ruteIds)
        //    {
        //        NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
        //        #region Parameter assignement

        //        npgsqlParameters[0] = new NpgsqlParameter("p_ruteids", NpgsqlTypes.NpgsqlDbType.Bigint);
        //        npgsqlParameters[0].Value = id;

        //        npgsqlParameters[1] = new NpgsqlParameter("p_restvekt", NpgsqlTypes.NpgsqlDbType.Integer);
        //        npgsqlParameters[1].Value = restvekt;

        //        npgsqlParameters[2] = new NpgsqlParameter("p_mottakertype", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
        //        npgsqlParameters[2].Value = mottakerType;

        //        npgsqlParameters[3] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Date);
        //        npgsqlParameters[3].Value = dato;

        //        npgsqlParameters[4] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
        //        npgsqlParameters[4].Direction = ParameterDirection.Output;


        //        #endregion

        //        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
        //        {
        //            output = dbhelper.ExecuteNonQuery("kspu_db.subtractrestkapasitetabsoluttdag", CommandType.StoredProcedure, npgsqlParameters);
        //        }
        //        if (output == null)
        //        {
        //            _logger.LogWarning("Kunne ikke redusere restvekt og -kapasitet på utvalg, dato:" + dato.ToShortDateString() + ", restvekt:" + restvekt + ", mottakertype:" + mottakerType);
        //        }
        //        else
        //        {
        //            result = Convert.ToInt32(npgsqlParameters[5].Value);
        //            _logger.LogInformation(string.Format("Result returned: {0} ", result));
        //        }
        //    }

        //    _logger.LogDebug("Exiting from SubtractRestkapasitetAbsoluttDag");
        //}



        /// <summary>
        ///     ''' Return dates that can be used by sperrefrist.
        ///     ''' Length is set by dayCount. Max 365
        ///     ''' </summary>
        ///     ''' <param name="dato"></param>
        ///     ''' <param name="dayCount"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<List<DateTime>> GetSperrefristDates(DateTime dato, int dayCount)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetSperrefristDates");
            List<DateTime> sperrefristdays = new List<DateTime>();
            DateTime d = dato;
            while (sperrefristdays.Count < dayCount)
            {
                if (!(d.DayOfWeek.Equals(DayOfWeek.Saturday) | d.DayOfWeek.Equals(DayOfWeek.Sunday)))
                    sperrefristdays.Add(d);
                d = d.AddDays(1);
            }
            return sperrefristdays;
        }

        //public async Task SubtractRestkapasitetSperrefrist(List<long> ruteIds, int restvekt, DateTime dato, string mottakertype)
        //{
        //    await Task.Run(() => { });
        //    _logger.LogDebug("Preparing the data for SubtractRestkapasitetSperrefrist");

        //    int? output;
        //    int result;

        //    // get all possible dates for delivery
        //    foreach (DateTime dag in GetSperrefristDates(dato, 3).Result)
        //    {
        //        NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];

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

        //        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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

        //    }
        //    _logger.LogDebug("Exiting from SubtractRestkapasitetSperrefrist");
        //}


        public async Task SubtractRestkapasitetSperrefrist(List<long> ruteIds, int restvekt, DateTime dato, string mottakertype, double restthickness)
        {
            // get all possible dates for delivery
            foreach (DateTime dag in GetSperrefristDates(dato, 1).Result)
            {
                // get ruteIds for this day
                List<long> currDayRuteIds = GetPossibleRuteidsOnDate(ruteIds, dag, restvekt, mottakertype, restthickness);
                if (currDayRuteIds.Count == 0)
                    continue;

                // subtract capacity
                await SubtractRestkapasitetAbsoluttDag(currDayRuteIds, restvekt, dag, mottakertype, restthickness);

                // substract these ids from input ids
                if (ruteIds.Count > currDayRuteIds.Count)
                {
                    IEnumerable<long> tmpRuteIds = ruteIds.Except(currDayRuteIds);
                    ruteIds = tmpRuteIds.ToList();
                }
            }
        }

        private List<long> GetPossibleRuteidsOnDate(List<long> ruteIds, DateTime dato, int restvekt, string mottakertype, double restthickness)
        {
            if (ruteIds == null || ruteIds.Count == 0)
                return new List<long>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select reolnr  ");
            sql.Append(" from KSPU_DB.kapasitetruter ");
            sql.Append(" where restvekt >= " + restvekt + " ");
            sql.Append(" and restthickness >= " + restthickness + " ");
            sql.Append(" and restantall > 0 ");
            sql.Append(" and dato ='" + dato.Date + "' ");
            sql.Append(" and mottakertype = '" + mottakertype + "'  ");
            sql.Append(" and ");
            AddInClauseValues(sql, ruteIds);
            sql.Append(" order by reolnr  ");

            DataTable dt = null;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dt = dbhelper.FillDataTable(sql.ToString(), CommandType.Text, null);
            }


            List<long> result = new List<long>();
            foreach (DataRow row in dt.Rows)
                result.Add(Convert.ToInt64(row["reolnr"]));
            return result;
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
            kDato.Dato = (DateTime)row["r_dato"];
            kDato.UkeNr = Convert.ToInt32(row["r_ukenr"]);
            if (row["r_distribusjonsdag"].ToString().ToUpper().Equals("Y"))
                kDato.Distribusjonsdag = true;
            else
                kDato.Distribusjonsdag = false;
            if (row["r_virkedag"].ToString().ToUpper().Equals("Y"))
                kDato.Virkedag = true;
            else
                kDato.Virkedag = false;
            if (row["r_isearlyweekfirstday"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsEarlyWeekFirstDay = true;
            else
                kDato.IsEarlyWeekFirstDay = false;
            if (row["r_isearlyweeksecondday"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsEarlyWeekSecondDay = true;
            else
                kDato.IsEarlyWeekSecondDay = false;
            if (row["r_ismidweekfirstday"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsMidWeekFirstDay = true;
            else
                kDato.IsMidWeekFirstDay = false;
            if (row["r_ismidweeksecondday"].ToString().Trim().ToUpper().Equals("Y"))
                kDato.IsMidWeekSecondDay = true;
            else
                kDato.IsMidWeekSecondDay = false;
            return kDato;
        }

        /// <summary>
        ///  Update data in kapasitetruter table
        /// </summary>
        /// <param name="Id">Id of utvalg or utval list</param>
        /// <param name="Type">Pass "U" for utvalg and "L" for list</param>
        /// <param name="Vekt">Weight</param>
        /// <param name="Thickness">Thickness</param>
        /// <returns></returns>
        public async Task UpdateKapasitorData(Int64 Id, string Type, Int64 Vekt, decimal Thickness)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for UpdateKapasitorData");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = Id;

                npgsqlParameters[1] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[1].Value = Vekt;

                npgsqlParameters[2] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = Type;

                npgsqlParameters[3] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[3].Value = Thickness;


                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updatekapacitordata", CommandType.StoredProcedure, npgsqlParameters);
                }

                _logger.LogDebug("Exiting from UpdateKapasitorData");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateKapasitorData: " + exception.Message);
            }
        }

        /// <summary>
        /// Fill KapasitetRuter object
        /// </summary>
        /// <param name="row">instance of Datarow</param>
        /// <returns>KapasitetRuter data</returns>
        private KapasitetRuter GetkapasitetruterFromDataRow(DataRow row)
        {
            KapasitetRuter kRuter = new KapasitetRuter();
            kRuter.Dato = (DateTime)row["r_dato"];
            kRuter.RuteNr = Convert.ToInt64(row["r_reolnr"]);
            kRuter.RestVekt = Convert.ToInt16(row["r_restvekt"]);
            kRuter.RestAntall = Convert.ToInt16(row["r_restantall"]);
            kRuter.MottakerType = row["r_mottakertype"].ToString();
            kRuter.RestThickness = Convert.ToInt16(row["r_restthickness"]);
            return kRuter;
        }
        #endregion
    }
}
