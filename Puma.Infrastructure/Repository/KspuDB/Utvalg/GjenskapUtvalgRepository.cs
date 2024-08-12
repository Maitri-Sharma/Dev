using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.DataLayer.Helper;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Repository.KspuDB.Utvalg
{
    public class GjenskapUtvalgRepository : KsupDBGenericRepository<utvalg>, IGjenskapUtvalgRepository
    {
        private readonly ILogger<GjenskapUtvalgRepository> _logger;
        public readonly string Connctionstring;
        public GjenskapUtvalgRepository(KspuDBContext context, ILogger<GjenskapUtvalgRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods

        public bool UtvalgRecreationProblemTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALGRECREATIONPROBLEM").Result;
            }
        }

        public bool UtvalgRecreationRuntimeTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_RUNTIME").Result;
            }
        }

        public bool UtvalgRecreationLogTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_LOG").Result;
            }
        }

        public bool UtvalgRecreationWorstCasesTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_WORST_CASES").Result;
            }
        }
        public bool UtvalgRecreationVerificationTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_VERIFICATION").Result;
            }
        }

        public Task<bool> TableExistsforrecreateutvalg(string tableName)
        {
            try
            {
                if (!tableName.Contains("."))
                {
                    _logger.LogDebug("Preparing the data for TableExists1");
                    object cnt;
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        cnt = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    //OracleCommand cmd = new OracleCommand(" SELECT COUNT(*) FROM ALL_TABLES WHERE TABLE_NAME = :tableName ");
                    //AddParameterString(cmd, "tableName", tableName.ToUpper(), 30);
                    //object cnt = ExecuteScalar(cmd);
                    if (cnt is int)
                        return Task.FromResult(System.Convert.ToInt32(cnt) > 0);
                    _logger.LogDebug("Exiting from TableExistsforrecreateutvalg");
                    return Task.FromResult(false);
                }
                else
                {
                    object cnt;
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        cnt = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    //OracleCommand cmd = new OracleCommand(" SELECT COUNT(*) FROM ALL_TABLES WHERE TABLE_NAME = :tableName AND OWNER = :owner ");
                    //AddParameterString(cmd, "tableName", tableName.Split(".")(1).ToUpper(), 30);
                    //AddParameterString(cmd, "owner", tableName.Split(".")(0).ToUpper(), 30);
                    //object cnt = ExecuteScalar(cmd);
                    if (cnt is decimal)
                        return Task.FromResult(System.Convert.ToInt32(cnt) > 0);
                    _logger.LogDebug("Exiting from TableExistsforrecreateutvalg");
                    return Task.FromResult(false);
                }
                
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in TableExistsforrecreateutvalg:", exception.Message);
                throw;
            }
         
        }

        public async Task CreateUtvalgRecreationProblemTable()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for CreateUtvalgRecreationProblemTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationProblemTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateUtvalgRecreationProblemTable:", exception.Message);
                throw;
            }
            
            GrantUtvalgRecreationProblemPermissions();
            _logger.LogDebug("Exiting from CreateUtvalgRecreationProblemTable");
        }

        public void ClearUtvalgRecreationProblemTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALGRECREATIONPROBLEM").Wait();
        }

        public async Task CreateUtvalgRecreationRuntimeTable()
        {
            await Task.Run(() => { });
            //StringBuilder sql = new StringBuilder();
            _logger.LogDebug("Preparing the data for CreateUtvalgRecreationRuntimeTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationRuntimeTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateUtvalgRecreationRuntimeTable:", exception.Message);
                throw;
            }
           
            GrantUtvalgRecreationRuntimePermissions();
            _logger.LogDebug("Exiting from CreateUtvalgRecreationRuntimeTable");
        }

        public async Task ClearUtvalgRecreationRuntimeTable()
        {

            await DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_RUNTIME");
        }

        public async Task CreateUtvalgRecreationLogTable()
        {
            await Task.Run(() => { });
            //StringBuilder sql = new StringBuilder();

            _logger.LogDebug("Preparing the data for CreateUtvalgRecreationLogTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationLogTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateUtvalgRecreationLogTable:", exception.Message);
                throw;
            }
            GrantUtvalgRecreationLogPermissions();
            _logger.LogDebug("Exiting from CreateUtvalgRecreationLogTable");
        }

        public async Task ClearUtvalgRecerationLogTable()
        {
           await DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_LOG");
        }

        public async Task CreateUtvalgRecreationVerficationTable()
        {
            await Task.Run(() => { });
            //StringBuilder sql = new StringBuilder();

            _logger.LogDebug("Preparing the data for CreateUtvalgRecreationRuntimeTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationVerficationTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateUtvalgRecreationRuntimeTable:", exception.Message);
                throw;
            }
            GrantUtvalgRecreationVerificationPermissions();
            _logger.LogDebug("Exiting from CreateUtvalgRecreationRuntimeTable");
        }

        public async Task CreateUtvalgRecreationWorstCases()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for CreateUtvalgRecreationWorstCases");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationWorstCases", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateUtvalgRecreationWorstCases:", exception.Message);
                throw;
            }
            GrantUtvalgRecreationWorstCasesPermissions();
            _logger.LogDebug("Exiting from CreateUtvalgRecreationWorstCases");
        }

        public async Task ClearUtvalgRecerationVerificationTable()
        {
           await DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_VERIFICATION");
        }

        public async Task ClearUtvalgRecerationWorstCasesTable()
        {
           await DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_WORST_CASES");
        }

        public async Task<string> Get_Utvalg_ByID_SQL(Nullable<int> processId, string ID, bool forLocking)
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Preparing the data for Get_Utvalg_ByID_SQL");
                object result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = ID;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.Get_Utvalg_ByID_SQL", CommandType.StoredProcedure, npgsqlParameters);
                }
                string selectPart = "SELECT UTVALGID";
                if (processId.HasValue)
                    selectPart += ", " + processId.Value.ToString();

                //StringBuilder sql = new StringBuilder();
                //sql.AppendLine(selectPart);
                //sql.AppendLine("FROM KSPU_DB.Utvalg");
                //sql.AppendLine("WHERE NOT lower(ReolMapName) = lower(:CurrentReolMapName)");
                //// Fjerne alle kampanjer/arvingsutvalg
                //sql.AppendLine("AND (basedon=0 or basedon IS NULL) ");
                //sql.AppendLine("AND UtvalgID = " + ID);
                //sql.AppendLine("AND antall > 0");
                //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
                //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
                //sql.AppendLine("ORDER BY UTVALGID DESC");

                //return sql.ToString();
                _logger.LogDebug("Exiting from CreateUtvalgRecreationRuntimeTable");
                return null;
                
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Get_Utvalg_ByID_SQL:", exception.Message);
                throw;
            }
        }

        public async Task<string> Get_Utvalg_Tilbud_Or_Bestilt_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking)
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Preparing the data for Get_Utvalg_Tilbud_Or_Bestilt_SQL");
                object result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = maxNrOfUtvalg;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.Get_Utvalg_Tilbud_Or_Bestilt_SQL", CommandType.StoredProcedure, npgsqlParameters);
                }
                string sqlPart = "SELECT UTVALGID";
                if (processId.HasValue)
                    sqlPart += ", " + processId.Value.ToString();

                System.Text.StringBuilder sql = new System.Text.StringBuilder();

                //sql.AppendLine(sqlPart);
                //sql.AppendLine("FROM KSPU_DB.Utvalg U");
                //sql.AppendLine("WHERE NOT lower(U.ReolMapName) = lower(:CurrentReolMapName)");
                //sql.AppendLine("AND (lower(OrdreType) = lower('T') OR lower(OrdreType) = lower('O'))");
                //sql.AppendLine("AND ((U.distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR U.distribusjonsdato IS NULL)");
                //// Fjerne alle kampanjer/arvingsutvalg
                //sql.AppendLine("AND (U.basedon=0 or U.basedon IS NULL) ");
                //sql.AppendLine("AND (U.UTVALGLISTID=0 or U.UTVALGLISTID IS NULL) ");
                //sql.AppendLine("AND U.antall > 0 ");
                //sql.Append("AND INNLEVERINGSDATO >= :dato ");
                //sql.AppendLine("AND rownum <= " + maxNrOfUtvalg.ToString());
                //sql.AppendLine("AND U.UtvalgId NOT IN");
                //sql.AppendLine("(");
                //sql.AppendLine("SELECT UtvalgId");
                //sql.AppendLine("FROM KSPU_DB.UTVALGRECREATIONPROBLEM");
                //sql.AppendLine(")");
                //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
                //sql.AppendLine("ORDER BY U.UTVALGID DESC");
                _logger.LogDebug("Exiting from Get_Utvalg_Tilbud_Or_Bestilt_SQL");
                return sql.ToString();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Get_Utvalg_Tilbud_Or_Bestilt_SQL:", exception.Message);
                throw;
            }
        }

        public async Task<string> Get_UtvalgListID_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for Get_UtvalgListID_Utvalg_In_List_SQL");
                object result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = maxNrOfUtvalg;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.Get_Utvalg_Tilbud_Or_Bestilt_SQL", CommandType.StoredProcedure, npgsqlParameters);
                }

                string selectPart = "SELECT distinct UTVALGLISTID";
                if (processId.HasValue)
                    selectPart += ", " + processId.Value.ToString();

                //StringBuilder sql = new StringBuilder();
                //sql.AppendLine(selectPart);
                //sql.AppendLine("FROM KSPU_DB.Utvalg");
                //sql.AppendLine("WHERE NOT lower(ReolMapName) = lower(:CurrentReolMapName)");
                //// Fjerne alle kampanjer/arvingsutvalg
                //sql.AppendLine("AND (basedon=0 or basedon IS NULL) ");
                //sql.AppendLine("AND NOT UtvalgListID IS NULL");
                //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
                //sql.AppendLine("AND antall > 0");
                //sql.AppendLine("AND rownum <= " + maxNrOfUtvalg.ToString());
                //sql.AppendLine("AND UtvalgId NOT IN");
                //sql.AppendLine("(");
                //sql.AppendLine("SELECT UP.UtvalgId");
                //sql.AppendLine("FROM KSPU_DB.UTVALGRECREATIONPROBLEM UP");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som er eldre en angitt dato og ikke lagret av systembruker
                //sql.AppendLine("AND UTVALGID IN");
                //sql.AppendLine("(");
                //sql.AppendLine("    SELECT DISTINCT UM.UTVALGID");
                //sql.AppendLine("    FROM KSPU_DB.UTVALGMODIFICATION UM WHERE UM.MODIFICATIONDATE > :moddato AND UPPER(UM.USERID) <> UPPER(:bruker)");
                //sql.AppendLine(")");
                //if (IsOrderedorDelivered)
                //{
                //    sql.AppendLine("AND (lower(OrdreType) = lower('T') OR lower(OrdreType) = lower('O'))");
                //    sql.Append("AND INNLEVERINGSDATO >= :dato ");
                //}
                //else
                //    sql.AppendLine("AND (OrdreType is null or (lower(OrdreType) <> lower('T') AND lower(OrdreType) <> lower('O')))");
                //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
                //sql.AppendLine("ORDER BY UTVALGLISTID DESC");

                //return sql.ToString();
                _logger.LogDebug("Exiting from Get_Utvalg_Tilbud_Or_Bestilt_SQL");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Get_Utvalg_Tilbud_Or_Bestilt_SQL:", exception.Message);
                throw;
            }
        }


        public async Task<string> Get_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking)
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Preparing the data for Get_Utvalg_In_List_SQL");
                string selectPart = "SELECT UTVALGID";
                if (processId.HasValue)
                    selectPart += ", " + processId.Value.ToString();

                //StringBuilder sql = new StringBuilder();
                //sql.AppendLine(selectPart);
                //sql.AppendLine("FROM KSPU_DB.Utvalg");
                //sql.AppendLine("WHERE NOT lower(ReolMapName) = lower(:CurrentReolMapName)");
                //// Fjerne alle kampanjer/arvingsutvalg
                //sql.AppendLine("AND (basedon=0 or basedon IS NULL) ");
                //sql.AppendLine("AND NOT UtvalgListID IS NULL");
                //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
                //sql.AppendLine("AND antall > 0");
                //sql.AppendLine("AND rownum <= " + maxNrOfUtvalg.ToString());
                //sql.AppendLine("AND UtvalgId NOT IN");
                //sql.AppendLine("(");
                //sql.AppendLine("SELECT UP.UtvalgId");
                //sql.AppendLine("FROM KSPU_DB.UTVALGRECREATIONPROBLEM UP");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som er eldre en angitt dato og ikke lagret av systembruker
                //sql.AppendLine("AND UTVALGID IN");
                //sql.AppendLine("(");
                //sql.AppendLine("    SELECT DISTINCT UM.UTVALGID");
                //sql.AppendLine("    FROM KSPU_DB.UTVALGMODIFICATION UM WHERE UM.MODIFICATIONDATE > :moddato AND UPPER(UM.USERID) <> UPPER(:bruker)");
                //sql.AppendLine(")");
                //if (IsOrderedorDelivered)
                //    sql.AppendLine("AND (lower(OrdreType) = lower('T') OR lower(OrdreType) = lower('O'))");
                //else
                //    sql.AppendLine("AND (OrdreType is null or (lower(OrdreType) <> lower('T') AND lower(OrdreType) <> lower('O')))");
                //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
                //sql.AppendLine("ORDER BY UTVALGID DESC");

                //return sql.ToString();
                _logger.LogDebug("Exiting from Get_Utvalg_In_List_SQL");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Get_Utvalg_In_List_SQL:", exception.Message);
                throw;
            }
        }

        public async Task<string> Get_Andre_Utvalg_Recently_First_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking)
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Preparing the data for Get_Andre_Utvalg_Recently_First_SQL");
                object result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = maxNrOfUtvalg;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.Get_Andre_Utvalg_Recently_First_SQL", CommandType.StoredProcedure, npgsqlParameters);
                }
                string selectPart = "SELECT UTVALGID";
                if (processId.HasValue)
                    selectPart += ", " + processId.Value.ToString();

                //StringBuilder sql = new StringBuilder();
                //sql.AppendLine(selectPart);
                //sql.AppendLine("FROM KSPU_DB.Utvalg U");
                //sql.AppendLine("WHERE NOT lower(U.ReolMapName) = lower(:CurrentReolMapName)");
                //sql.AppendLine("AND (U.basedon=0 or U.basedon is null) ");
                //sql.AppendLine("AND antall > 0");
                //sql.AppendLine("AND rownum <= " + maxNrOfUtvalg.ToString());
                //sql.AppendLine("AND ((U.distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR U.distribusjonsdato IS NULL)");
                //sql.AppendLine("AND U.UtvalgId NOT IN");
                //sql.AppendLine("(");
                //sql.AppendLine("SELECT UP.UtvalgId");
                //sql.AppendLine("FROM KSPU_DB.UTVALGRECREATIONPROBLEM UP");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som er eldre en angitt dato og ikke lagret av systembruker
                //sql.AppendLine("AND UTVALGID IN");
                //sql.AppendLine("(");
                //sql.AppendLine("    SELECT DISTINCT UM.UTVALGID");
                //sql.AppendLine("    FROM KSPU_DB.UTVALGMODIFICATION UM WHERE UM.MODIFICATIONDATE > :moddato AND UPPER(UM.USERID) <> UPPER(:bruker)");
                //sql.AppendLine(")");
                //sql.AppendLine("AND (ORDRETYPE IS NULL OR (lower(ORDRETYPE) <> lower('T') AND lower(ORDRETYPE) <> lower('O')))");
                //sql.AppendLine("AND UTVALGLISTID IS NULL");
                //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
                //sql.AppendLine("ORDER BY UTVALGID DESC");

                //return sql.ToString();
                _logger.LogDebug("Exiting from Get_Andre_Utvalg_Recently_First_SQL");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Get_Andre_Utvalg_Recently_First_SQL:", exception.Message);
                throw;
            }
        }

        /// <summary>
        ///     ''' Returns the sql-part which make sure the process only recreats the utvalgs it has locked
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<string> GetLockingSQLPredicatePart(bool forLocking, Nullable<int> processId)
        {
            try
            {
                await Task.Run(() => { });
                _logger.LogDebug("Preparing the data for GetLockingSQLPredicatePart");
                StringBuilder result = new StringBuilder();
                if (forLocking)
                    result.AppendLine("AND utvalgId NOT IN");
                else
                    result.AppendLine("AND utvalgId IN");

                result.AppendLine("(");
                result.AppendLine("SELECT utvalgId");
                result.AppendLine("FROM kspu_db.utvalg_recreation_runtime");
                if (!forLocking)
                    result.AppendLine("WHERE ProcessId = " + processId.ToString());
                result.AppendLine(")");
                _logger.LogDebug("Exiting from GetLockingSQLPredicatePart");
                return result.ToString();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetLockingSQLPredicatePart:", exception.Message);
                throw;
            }
        }

        public async Task<DataTable> GetAllRowsInUtvalgRecreationLogTable()
        {
            await Task.Run(() => { });
            return new DataTable();
        }

        public async Task<DataTable> GetAllRowsInUtvalgRecreationVerificationTable()
        {
            await Task.Run(() => { });
            return new DataTable();
        }

        public async Task<DataTable> GetSomeRowsInWorstCasesLogTable(bool onlyOrder, DateTime fromDate, DateTime toDate, bool bWhereDistDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetSomeRowsInWorstCasesLogTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_onlyOrder", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = onlyOrder;

            npgsqlParameters[1] = new NpgsqlParameter("p_fromDate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = fromDate;

            npgsqlParameters[2] = new NpgsqlParameter("p_toDate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[2].Value = toDate;

            npgsqlParameters[3] = new NpgsqlParameter("p_bWhereDistDate", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = bWhereDistDate;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.GetSomeRowsInWorstCasesLogTable", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogDebug("Exiting from GetSomeRowsInWorstCasesLogTable");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetSomeRowsInWorstCasesLogTable:", exception.Message);
                throw;
            }
            //string strWhere = " where ";
            //string strAnd = "";
            //if (onlyOrder)
            //{
            //    // Lovlige verdier er Registrert, Godkjent, Inlevert eller Kansellert. R /G / I / K
            //    strWhere = strWhere + " ORDRETYPE = 'O' AND ORDRESTATUS in ('R', 'G', 'I') ";
            //    strAnd = " and ";
            //}
            //if (bWhereDistDate)
            //    // distribusjonsdato BETWEEN TO_DATE('01.02/2013','DD.MM.YYYY') And TO_DATE('10.12.2013','DD.MM.YYYY')
            //    strWhere = strWhere + strAnd + " distribusjonsdato BETWEEN TO_DATE('" + fromDate + "','DD.MM.YYYY') And TO_DATE('" + toDate + "','DD.MM.YYYY')";
            return await GetAllRowsInTableWhereAndOrderBy("KSPU_DB.UTVALG_RECREATION_WORST_CASES", string.Empty, " order by bestemorlisteid, morlisteid, utvalgid");
        }

        public async Task<DataTable> GetAllRowsInTable(string tableName)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllRowsInTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;


            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.getallrowsintable", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogDebug("Exiting from GetAllRowsInTable");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllRowsInTable:", exception.Message);
                throw;
            }

            //return GetDataTable(new OracleCommand("select * from " + tableName));
            return result;
        }


        public async Task<DataTable> GetAllRowsInTableWhereAndOrderBy(string tableName, string strWhere, string orderBy)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllRowsInTableWhereAndOrderBy");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_strwhere", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = strWhere;

            npgsqlParameters[2] = new NpgsqlParameter("p_orderby", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = orderBy;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.GetAllRowsInTableWhereAndOrderBy", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllRowsInTableWhereAndOrderBy:", exception.Message);
                throw;
            }
            //return GetDataTable(new OracleCommand("select * from " + tableName + strWhere + orderBy));
            _logger.LogDebug("Exiting from GetAllRowsInTable");
            return result;
        }

        public async Task<DataTable> GetAllRowsWithLessThan1500InRecreationLogTable()
        {
            await Task.Run(() => { });
            return new DataTable();
        }

        public async Task<DataTable> GetAllRowsInWorstCasesLogTable()
        {
            await Task.Run(() => { });
            return new DataTable();
        }
        public async Task<DataTable> GetReols(string sql)
        {
            await Task.Run(() => { });
            //OracleCommand cmd = new OracleCommand(sql.ToLower());
            //AddParameterString(cmd, "currentreolmapname", DAConfig.CurrentReolTableName, 1000);
            //if (sql.ToLower().IndexOf(":dato") > -1)
            //    AddParameterDate(cmd, "dato", DateTime.Today.AddMonths(Config.NumberOfMonthsSinceDelivery));
            //if (sql.ToString().ToLower().IndexOf(":moddato") > -1)
            //    AddParameterDate(cmd, "moddato", DateTime.Today.AddMonths(Config.NumberOfMonthsSinceModified));
            //if (sql.ToString().ToLower().IndexOf(":bruker") > -1)
            //    AddParameterString(cmd, "bruker", Config.SystemUserName, 50);
            //if (sql.ToString().ToLower().IndexOf(":fromdate") > -1 & sql.ToString().ToLower().IndexOf(":todate") > -1)
            //{
            //    AddParameterDate(cmd, "fromdate", DateTime.Today);
            //    AddParameterDate(cmd, "todate", DateTime.Today.AddDays(Config.IgnoreNrOfDaysToDelivery));
            //}
            //return GetDataTable(cmd);
            return null;
        }
        public async Task DeleteFromOldUtvalgGeometryIfPresent(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for DeleteFromOldUtvalgGeometryIfPresent");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.deletefromoldutvalggeometryifpresent", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteFromOldUtvalgGeometryIfPresent:", exception.Message);
                throw;
            }
            _logger.LogInformation("Number of row returned: ", result.Rows.Count);
            _logger.LogDebug("Exiting from DeleteFromOldUtvalgGeometryIfPresent");
        }


        public async Task<bool> IsOldUtvalgGeometryPresent(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsOldUtvalgGeometryPresent");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_gdb.custom_isoldutvalggeometrypresent", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsOldUtvalgGeometryPresent:", exception.Message);
                throw;
            }
            // TODO: - Bruk ArcObjects
            // OracleCommand cmd = new OracleCommand("Select utvalgid from kspu_gdb.oldutvalggeometry where utvalgid = :utvalgId");

            //return GetDataTable(cmd).Rows.Count > 0;
            _logger.LogDebug("Exiting from IsOldUtvalgGeometryPresent");
            return result.Rows.Count > 0;
        }

        public async Task<bool> IsChangedBeforRecreate(long piUtvalgID, DateTime LastModifiedDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsChangedBeforRecreate");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = piUtvalgID;

            npgsqlParameters[1] = new NpgsqlParameter("p_lastmodifieddate", NpgsqlTypes.NpgsqlDbType.Timestamp);
            npgsqlParameters[1].Value = LastModifiedDate;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.ischangedbeforrecreate", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsChangedBeforRecreate:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from IsChangedBeforRecreate");
            // OracleCommand cmd = new OracleCommand(" SELECT MAX(UM.MODIFICATIONDATE) FROM KSPU_DB.UTVALGMODIFICATION UM WHERE UTVALGID= " + piUtvalgID);

            //object val = ExecuteScalar(cmd);

            if (result != null && (result) is DBNull)
            {
                DateTime moDate = (DateTime)result;
                if (moDate.CompareTo(LastModifiedDate) == 0)
                    return false;
                else
                    return true;
            }

            return false;
        }

        public async Task<int> MaxAntallUtvalgKandidaterForGjenskap(string sqlString)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for MaxAntallUtvalgKandidaterForGjenskap");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_sqlstring", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = sqlString;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.MaxAntallUtvalgKandidaterForGjenskap", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in MaxAntallUtvalgKandidaterForGjenskap:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from MaxAntallUtvalgKandidaterForGjenskap");
            return System.Convert.ToInt32(result);
        }

        public async Task LockUtvalgRows(string insertSelect)
        {
            await Task.Run(() => { });
        }

        public async Task RemoveUtvalgLocks(string utvalgIdSqlSelect)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for RemoveUtvalgLocks");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_sqlstring", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = utvalgIdSqlSelect;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.RemoveUtvalgLocks", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in RemoveUtvalgLocks:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from RemoveUtvalgLocks");
        }

        public async Task<string> Get_Utvalg_In_List_ByID_SQL(Nullable<int> processId, string ID, bool forLocking)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for Get_Utvalg_In_List_ByID_SQL");
            string whereCondition = await GetLockingSQLPredicatePart(forLocking,processId);
            string selectPart = "SELECT UTVALGID";
            if (processId.HasValue)
                selectPart += ", " + processId.Value.ToString();

            return selectPart.ToString();
        }




        public async Task InsertIntoUtvalgWorstCasesLog(Puma.Shared.Utvalg utv, double antAvvik, double arealAvvik, long antDiff, TempResultData resData)
        {
            await Task.Run(() => { });
        }
        //public bool MustStartNewRecreationProcess
        //{
        //    get
        //    {
        //        return NumberOfUtvalgToRecreate() > 0;
        //    }
        //}


        public async Task<int> NumberOfUtvalgToRecreate()
        {
            await Task.Run(() => { });
            return 1;
        }

        public async Task<int> NumberOfUtvalgInRecreationProblemTable()
        {
            await Task.Run(() => { });
            return 1;
        }

        public async Task<int> NrOfUtvalgWithOldReolMapAndAntallZero()
        {
            await Task.Run(() => { });
            return 1;
        }

        public async Task<string> FindOldReolmap(string strUtvalgID)
        {
            await Task.Run(() => { });
            return string.Empty;
        }

        public async Task<string> addBoxesText(string strBoxes, string strReolId)
        {
            await Task.Run(() => { });
            return string.Empty;
        }

        public async Task<string> FindAllRemovedBoxes(string strUtvalgID, string strOldReolMapName)
        {
            await Task.Run(() => { });
            return string.Empty;
        }

        public async Task<string> FindAllNewBoxes(string strUtvalgID, string strOldReolMapName)
        {
            await Task.Run(() => { });
            return string.Empty;
        }

        public async Task<DataTable> GetAllRowsInTableOrderBy(string tableName, string orderBy)
        {
            await Task.Run(() => { });
            return new DataTable();
        }

        public async Task ReleaseLocks()
        {
            await Task.Run(() => { });
        }

        #endregion

        #region Private Methods
        private void GrantUtvalgRecreationLogPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_LOG").Wait();
        }


        private void GrantUtvalgRecreationVerificationPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_VERIFICATION").Wait();
        }

        private void GrantUtvalgRecreationWorstCasesPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_WORST_CASES").Wait();
        }


        public async Task DeleteFromTableBasedOnUtvalgId(string tableName, DataRow dr)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for DeleteFromTableBasedOnUtvalgId");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_dr", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = dr;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.GetXnrOfRowsFromTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteFromTableBasedOnUtvalgId:", exception.Message);
                throw;
            }
            //OracleCommand cmd = new OracleCommand("delete from " + tableName + " where UtvalgId = " + System.Convert.ToInt32(dr("UtvalgId")).ToString());
            //ExecuteNonQuery(cmd);
            _logger.LogDebug("Exiting from DeleteFromTableBasedOnUtvalgId");
        }

        public async Task DeleteAllRowsInTable(string tableName)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for DeleteAllRowsInTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = tableName;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleteallrowsintable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteAllRowsInTable:", exception.Message);
                throw;
            }
            //OracleCommand cmd = new OracleCommand("delete from " + tableName);
            //ExecuteNonQuery(cmd);
            _logger.LogDebug("Exiting from DeleteAllRowsInTable");
        }

        private void GrantUtvalgRecreationRuntimePermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_RUNTIME").Wait();
        }

        public async Task<bool> FailureOnUtvalgRegistred(int utvalgid)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object count;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    count = dbhelper.ExecuteNonQuery("kspu_db.FailureOnUtvalgRegistred", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FailureOnUtvalgRegistred:", exception.Message);
                throw;
            }
            //StringBuilder sql = new StringBuilder();

            //sql.AppendLine("select count(UTVALGID) from KSPU_DB.UTVALGRECREATIONPROBLEM WHERE UTVALGID = :UTVALGID");
            //OracleCommand cmd = new OracleCommand(sql.ToString());
            //AddParameterInteger(cmd, "UTVALGID", utvalgid);

            //object count = ExecuteScalar(cmd);
            if (count == null | (count) is DBNull)
                return false;
            _logger.LogDebug("Exiting from FailureOnUtvalgRegistred");
            return System.Convert.ToInt32(count) > 0;
        }

        public async Task UpdateRecreationFailure(int utvalgId, string errorText)
        {
            await Task.Run(() => { });

            //StringBuilder sql = new StringBuilder();
            //sql = new StringBuilder();
            //sql.AppendLine("update KSPU_DB.UTVALGRECREATIONPROBLEM");
            //sql.AppendLine("set ERRORTEXT = :ERRORTEXT WHERE UTVALGID = :UTVALGID");
            //OracleCommand cmd = new OracleCommand(sql.ToString());
            //AddParameterInteger(cmd, "UTVALGID", utvalgId);
            //AddParameterString(cmd, "ERRORTEXT", errorText, 500);

            //ExecuteNonQuery(cmd);
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object count;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    count = dbhelper.ExecuteNonQuery("kspu_db.UpdateRecreationFailure", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateRecreationFailure:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from UpdateRecreationFailure");
        }
        private void GrantUtvalgRecreationProblemPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALGRECREATIONPROBLEM").Wait();
        }

        public async Task GrantStandardPermissionsOnTable(string tableName)
        {
            await Task.Run(() => { });
            //ExecuteNonQuery(new OracleCommand("grant select on " + tableName + " to viewer"));
            //ExecuteNonQuery(new OracleCommand("grant select on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant update on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant insert on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant delete on " + tableName + " to editor"));

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.GrantStandardPermissionsOnTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GrantStandardPermissionsOnTable:", exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from GrantStandardPermissionsOnTable");
        }

        public async Task InsertRecreationFailure(int utvalgId, string errorText)
        {
            await Task.Run(() => { });
        }

        public async Task InsertIntoUtvalgRecreationLog(int utvalgId, string utvalgName, string typeGjenskaping)
        {
            await Task.Run(() => { });
        }

        public async Task InsertIntoUtvalgRecreationVerification(int utvalgId, string utvalgName, string ordrereferanse, string kundenummer, DateTime innleveringsDato, int antallForGjenskaping, int antallEtterGjenskaping)
        {
            await Task.Run(() => { });
        }

        public async Task<DataTable> GetXnrOfRowsFromTable(string tableName, int nrOfRows)
        {
            await Task.Run(() => { });

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_nrofrows", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = nrOfRows;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.GetXnrOfRowsFromTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetXnrOfRowsFromTable:", exception.Message);
                throw;
            }
            // return GetDataTable(new OracleCommand("select * from " + tableName + " where rownum <= " + nrOfRows.ToString()));
            return result;
        }

        #endregion
    }
}

