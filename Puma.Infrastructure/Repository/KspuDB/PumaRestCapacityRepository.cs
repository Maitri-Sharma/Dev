using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class PumaRestCapacityRepository : KsupDBGenericRepository<AddressPointsState>, IPumaRestCapacityRepository
    {
        private readonly ILogger<PumaRestCapacityRepository> _logger;
        public readonly string Connctionstring;

        public PumaRestCapacityRepository(KspuDBContext context, ILogger<PumaRestCapacityRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region DELETE KAPASITETDATO TABELLER

        /// <summary>
        /// Deletes all records from INPUT_REOLER
        /// </summary>
        public async Task Kapasitet_Ruter_Dato_AllAsync()
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Inside into Kapasitet_Ruter_Dato_AllAsync");

                int result;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.delete_kapasitet_ruter_dato_all", CommandType.StoredProcedure, null);
                }
                _logger.LogInformation("Number of row returned: ", result);

                _logger.LogDebug("Exiting from Kapasitet_Ruter_Dato_AllAsync");
                //OracleDB.ExecuteNonQuery("KSPU_DELETE.KAPASITET_RUTER_DATO_All", null, OracleDB.TypeOfProc.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Kapasitet_Ruter_Dato_AllAsync " + ex.Message);
                //throw ex;
            }
        }

        #endregion

        #region DELETE KAPASITETDATO TABELLER OG IMPORTER NYE

        /// <summary>
        /// Deletes all records from INPUT_REOLER
        /// </summary>
        public async Task Kapasitet_Delete_And_ImportAsync()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Inside into Kapasitet_Delete_And_ImportAsync");

            int result;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.delete_kapasitet_delete_and_import", CommandType.StoredProcedure, null);
            }
            _logger.LogInformation("Number of row returned: ", result);

            _logger.LogDebug("Exiting from Kapasitet_Delete_And_ImportAsync");
            //OracleDB.ExecuteNonQuery("KSPU_DELETE.KAPASITET_DELETE_AND_IMPORT", null, OracleDB.TypeOfProc.StoredProcedure);
        }

        #endregion

        #region INSERT KAPASITETDATO

        /// <summary>
        /// 
        /// </summary>
        /// <paramname="DATO_IN"></param>
        /// <paramname="UKENR_IN"></param>
        /// <paramname="DISTRIBUSJONSDAG_IN"></param>
        /// <paramname="VIRKEDAG_IN"></param>
        public async Task Kapasitetdato_AddAsync(DateTime dato, int uke_nr, string distribusjonDag, string virkeDag)
        {
            try
            {
                await Task.Run(() => { });
                //return 1;
                _logger.LogDebug("Inside into Kapasitetdato_AddAsync");
                object result;

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];

                npgsqlParameters[0] = new NpgsqlParameter("dato_in", NpgsqlTypes.NpgsqlDbType.Timestamp);
                npgsqlParameters[0].Value = dato;

                npgsqlParameters[1] = new NpgsqlParameter("ukenr_in", NpgsqlTypes.NpgsqlDbType.Smallint);
                npgsqlParameters[1].Value = uke_nr;

                npgsqlParameters[2] = new NpgsqlParameter("distribusjonsdag_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = distribusjonDag;

                npgsqlParameters[3] = new NpgsqlParameter("virkedag_in", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[3].Value = virkeDag;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    try
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.add_kapasitetdato", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        throw;
                    }
                }

                _logger.LogInformation("Number of row returned: " + result);

                _logger.LogDebug("Exiting from Kapasitetdato_AddAsync");


                //var parmList = new List<OracleParameter>();
                //parmList.Add(new OracleParameter("DATO_IN", DATO_IN));
                //parmList.Add(new OracleParameter("UKENR_IN", UKENR_IN));
                //parmList.Add(new OracleParameter("DISTRIBUSJONSDAG_IN", DISTRIBUSJONSDAG_IN));
                //parmList.Add(new OracleParameter("VIRKEDAG_IN", VIRKEDAG_IN));
                //OracleDB.ExecuteNonQuery("KSPU_ADD.KAPASITETDATO", parmList, OracleDB.TypeOfProc.StoredProcedure);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        #endregion

        #region INSERT KAPASITETRUTER

        /// <summary>
        /// 
        /// </summary>
        /// <paramname="DATO_IN"></param>
        /// <paramname="REOLNR_IN"></param>
        /// <paramname="RESTVEKT_IN"></param>
        /// <paramname="RESTANTALL_IN"></param>
        /// <paramname="MOTTAKERTYPE_IN"></param>
        public async Task Kapasitetruter_AddAsync(DateTime dato, long reolNr, int restVekt, int restAntall, string mottakerType, double restThickness)
        {
            await Task.Run(() => { });
            //return 1;
            _logger.LogDebug("Inside into Kapasitetruter_AddAsync");
            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];

            npgsqlParameters[0] = new NpgsqlParameter("dato_in", NpgsqlTypes.NpgsqlDbType.Timestamp);
            npgsqlParameters[0].Value = dato;

            npgsqlParameters[1] = new NpgsqlParameter("reolnr_in", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = reolNr;

            npgsqlParameters[2] = new NpgsqlParameter("restvekt_in", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[2].Value = restVekt;

            npgsqlParameters[3] = new NpgsqlParameter("restantall_in", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[3].Value = restAntall;

            npgsqlParameters[4] = new NpgsqlParameter("mottakertype_in", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = mottakerType;

            npgsqlParameters[5] = new NpgsqlParameter("restthickness_in", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[5].Value = Convert.ToDouble(restThickness);

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.add_kapasitetruter", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    throw;
                }
            }

            _logger.LogInformation("Number of row returned: " + result);

            _logger.LogDebug("Exiting from Kapasitetruter_AddAsync");

            //var parmList = new List<OracleParameter>();
            //parmList.Add(new OracleParameter("DATO_IN", DATO_IN));
            //parmList.Add(new OracleParameter("REOLNR_IN", REOLNR_IN));
            //parmList.Add(new OracleParameter("RESTVEKT_IN", RESTVEKT_IN));
            //parmList.Add(new OracleParameter("RESTANTALL_IN", RESTANTALL_IN));
            //parmList.Add(new OracleParameter("MOTTAKERTYPE_IN", MOTTAKERTYPE_IN));
            //// parmList.Add(new OracleParameter("RESTTHICKNESS_IN", RESTTHICKNESS_IN));
            //OracleDB.ExecuteNonQuery("KSPU_ADD.KAPASITETRUTER", parmList, OracleDB.TypeOfProc.StoredProcedure);
        }

        #endregion

        #region ADD PRS CALENDAR DATA

        /// <summary>
        /// Insert all records from PRS Calendar to PUMA
        /// </summary>
        public async Task Add_PRS_Calendar_To_PumaAsync(DateTime dato, string isHoliday, string isEarlyWeekFirstDay, string isEarlyWeekSecondDay, string isMidWeekFirstDay, string isMidWeekSecondDay, string frequencyType, DateTime lastModificationDate, long weekNo)
        {
            await Task.Run(() => { });
            //return 1;
            _logger.LogDebug("Inside into Add_PRS_Calendar_To_PumaAsync");
            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[9];

            npgsqlParameters[0] = new NpgsqlParameter("dato_in", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = dato;

            npgsqlParameters[1] = new NpgsqlParameter("isholiday_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[1].Value = isHoliday;

            npgsqlParameters[2] = new NpgsqlParameter("isearlyweekfirstday_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[2].Value = isEarlyWeekFirstDay;

            npgsqlParameters[3] = new NpgsqlParameter("isearlyweeksecondday_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[3].Value = isEarlyWeekSecondDay;

            npgsqlParameters[4] = new NpgsqlParameter("ismidweekfirstday_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[4].Value = isMidWeekFirstDay;

            npgsqlParameters[5] = new NpgsqlParameter("ismidweeksecondday_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[5].Value = isMidWeekSecondDay;

            npgsqlParameters[6] = new NpgsqlParameter("frequencytype_in", NpgsqlTypes.NpgsqlDbType.Char);
            npgsqlParameters[6].Value = frequencyType;

            npgsqlParameters[7] = new NpgsqlParameter("weekno_in", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[7].Value = weekNo;

            npgsqlParameters[8] = new NpgsqlParameter("lastmodifieddate_in", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[8].Value = lastModificationDate;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.add_calenderdetailsfromprs", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    throw;
                }
            }

            _logger.LogInformation("Number of row returned: " + result);

            _logger.LogDebug("Exiting from Add_PRS_Calendar_To_PumaAsync");

            //var parmList = new List<OracleParameter>();
            //parmList.Add(new OracleParameter("DATO_IN", DATO_IN));
            //parmList.Add(new OracleParameter("ISHOLIDAY_IN", ISHOLIDAY_IN));
            //parmList.Add(new OracleParameter("ISEARLYWEEKFIRSTDAY_IN", ISEARLYWEEKFIRSTDAY_IN));
            //parmList.Add(new OracleParameter("ISEARLYWEEKSECONDDAY_IN", ISEARLYWEEKSECONDDAY_IN));
            //parmList.Add(new OracleParameter("ISMIDWEEKFIRSTDAY_IN", ISMIDWEEKFIRSTDAY_IN));
            //parmList.Add(new OracleParameter("ISMIDWEEKSECONDDAY_IN", ISMIDWEEKSECONDDAY_IN));
            //parmList.Add(new OracleParameter("FREQUENCYTYPE_IN", FREQUENCYTYPE_IN));
            //parmList.Add(new OracleParameter("LASTMODIFIEDDATE_IN", LASTMODIFIEDDATE_IN));
            //parmList.Add(new OracleParameter("WEEKNO_IN", WEEKNO_IN));
            //OracleDB.ExecuteNonQuery("KSPU_ADD.CALENDERDETAILSFROMPRS", parmList, OracleDB.TypeOfProc.StoredProcedure);
        }

        #endregion

    }
}