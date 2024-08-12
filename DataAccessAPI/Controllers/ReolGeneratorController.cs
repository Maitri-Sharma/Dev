using DataAccessAPI.Extensions;
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Repository.KspuDB.Reol;
using System;
using System.Data;

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReolGeneratorController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ReolGeneratorController> _logger;
        //private string DBConnectionString;
        private string _DBConnectionString;
        private IReolGeneratorRepository _reolGeneratorRepository;
        public readonly IConfigurationRepository _configurationRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReolGeneratorController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="reolGeneratorRepository"></param>
        public ReolGeneratorController(ILogger<ReolGeneratorController> logger, IReolGeneratorRepository reolGeneratorRepository)
        {
            _logger = logger;
            _reolGeneratorRepository = reolGeneratorRepository;

        }
        #endregion

        #region Public Methods
        [HttpGet("CalculateStatistics", Name = nameof(CalculateStatistics))]
        public bool CalculateStatistics(string tableName, bool gdbuser = false)
        {
            _logger.LogDebug("Inside into CalculateStatistics");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.calculatestatistics", CommandType.StoredProcedure, npgsqlParameters);
                }
                // RunSQL("ANALYZE TABLE " + tableName + " COMPUTE STATISTICS");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }
        //[HttpGet("DropTable", Name = nameof(DropTable))]
        //public bool DropTable(string tableName, bool gdbuser = false)
        //{
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
        //    object result;
        //    if (gdbuser)
        //        SetGDBUserAsConnStr();
        //    else
        //        SetDBUserAsConnStr();
        //    try
        //    {
        //        npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
        //        npgsqlParameters[0].Value = tableName;

        //        using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
        //        {
        //            result = dbhelper.ExecuteScalar<bool>("kspu_gdb.droptable", CommandType.StoredProcedure, npgsqlParameters);
        //        }
        //        //RunSQL("DROP TABLE " + tableName);
        //    }
        //    catch (Exception exception)
        //    {
        //        if (exception.Message.Contains("ORA-00942"))
        //            // Logger.LogWarning(tableName & " does not exist")
        //            return true;
        //        else
        //        {
        //            _logger.LogError(exception, exception.Message);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        [HttpGet("DoesTableExist", Name = nameof(DoesTableExist))]
        public bool DoesTableExist(string tableName, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_gdb.doestableexist", CommandType.StoredProcedure, npgsqlParameters);
            }

            if (result == null | (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;

            //return RunScalarSQL("select count(table_name) from user_tables where table_name = '" + tableName.ToUpper() + "'") > 0;
        }

        [HttpGet("DoesViewExist", Name = nameof(DoesViewExist))]
        public bool DoesViewExist(string viewName, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            npgsqlParameters[0] = new NpgsqlParameter("p_viewname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = viewName;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_gdb.doesviewexist", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result == null | (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;
            //return RunScalarSQL("select count(view_name) from user_views where view_name = '" + viewName.ToUpper() + "'") > 0;
        }

        [HttpGet("RenameTable", Name = nameof(RenameTable))]
        public bool RenameTable(string fromTableName, string toTableName, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_fromtablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = fromTableName;

                npgsqlParameters[1] = new NpgsqlParameter("p_totablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = toTableName;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.renametable", CommandType.StoredProcedure, npgsqlParameters);
                }
                //RunSQL("ALTER TABLE " + fromTableName + " RENAME TO  " + toTableName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-00942"))
                {
                }
                else
                {
                    _logger.LogError(ex.Message + " " + ex.Source + " SQL: " + "ALTER TABLE " + fromTableName + " RENAME TO  " + toTableName);
                    return false;
                }
            }
            return true;
        }

        [HttpGet("CreateIndex", Name = nameof(CreateIndex))]
        public bool CreateIndex(string IndexName, string TablenameWithFields, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_IndexName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = IndexName;

                npgsqlParameters[1] = new NpgsqlParameter("p_TablenameWithFields", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = TablenameWithFields;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.createindex", CommandType.StoredProcedure, npgsqlParameters);
                }
                // Example CreateIndex("REOL_ADR_INDEX","REOL_ADR(GKID)")
                //RunSQL("CREATE INDEX " + IndexName + " ON " + TablenameWithFields);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpGet("DropIndex", Name = nameof(DropIndex))]
        public bool DropIndex(string IndexName, bool GDBUser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (GDBUser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_IndexName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = IndexName;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.dropindex", CommandType.StoredProcedure, npgsqlParameters);
                }
                // Example DropIndex("TMP_REOL_ADR_INDEX")
                //RunSQL("DROP INDEX " + IndexName);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpGet("UpdateIndex", Name = nameof(UpdateIndex))]
        public bool UpdateIndex(string indexName, string tableNameWithFields, bool gdbuser = false)
        {
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            DropIndex(indexName, gdbuser);
            CreateIndex(indexName, tableNameWithFields, gdbuser);
            return true;
        }

        [HttpGet("Truncate_Table", Name = nameof(Truncate_Table))]
        public bool Truncate_Table(string piTableNAme, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            try
            {
                if (gdbuser)
                    SetGDBUserAsConnStr();
                else
                    SetDBUserAsConnStr();
                npgsqlParameters[0] = new NpgsqlParameter("p_piTableNAme", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = piTableNAme;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.truncatetable", CommandType.StoredProcedure, npgsqlParameters);
                }
                // ---Sletter verdier som ligger i tabellen
                //RunSQL("TRUNCATE TABLE " + piTableNAme);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }

            return true;
        }

        [HttpGet("GrantSelect", Name = nameof(GrantSelect))]
        public bool GrantSelect(string tableName, string toUser, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            try
            {
                if (gdbuser)
                    SetGDBUserAsConnStr();
                else
                    SetDBUserAsConnStr();
                npgsqlParameters[0] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;

                npgsqlParameters[1] = new NpgsqlParameter("p_toUser", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = toUser;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.grantselect", CommandType.StoredProcedure, npgsqlParameters);
                }
                // Example GrantSelect("GKREOLOVERFORING_TEMP", Config.KSPU_GDBUser)
                //RunSQL("GRANT SELECT ON " + tableName + " TO " + toUser);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        // Grant privileges
        // Public Shared Function GrantSelectAsDBUser(ByVal tableName As String, ByVal toUser As String) As Boolean
        // SetDBUserAsConnStr()
        // Return GrantSelect(tableName, toUser)
        // End Function

        // Public Shared Function GrantSelectAsGDBUser(ByVal tableName As String, ByVal toUser As String) As Boolean
        // SetGDBUserAsConnStr()
        // Return GrantSelect(tableName, toUser)
        // End Function
        // ---Add VarCHAR2 felt
        [HttpGet("AddVarCHAR2FieldToTable", Name = nameof(AddVarCHAR2FieldToTable))]
        public bool AddVarCHAR2FieldToTable(string piTableNAme, string piFeltNAme, int piLength)
        {
            _logger.LogDebug("Inside into AddVarCHAR2FieldToTable");
            try
            {
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_pitablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = piTableNAme;

                npgsqlParameters[1] = new NpgsqlParameter("p_pifeltname", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = piFeltNAme;

                npgsqlParameters[2] = new NpgsqlParameter("p_pilength", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[2].Value = piLength;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.addvarchar2fieldtotable", CommandType.StoredProcedure, npgsqlParameters);
                }
                // ---Legger til felt
                //RunSQL("ALTER TABLE " + piTableNAme + " ADD " + piFeltNAme + " VARCHAR2(" + piLength + ")");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }

            return true;
        }
        [HttpGet("getFields", Name = nameof(getFields))]
        public string getFields(string fromTable, bool asGDB, string prefix)
        {
            DataTable table;
            // select column_name from USER_TAB_COLUMNS where upper(table_name) = upper('input_boksanlegg');
            if (asGDB)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            npgsqlParameters[0] = new NpgsqlParameter("p_fromtable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;

            npgsqlParameters[1] = new NpgsqlParameter("p_prefix", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = prefix;
            //OracleCommand cmd = new OracleCommand("select column_name from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' ");

            //DataTable table = GetDataTable(cmd);
            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                table = dbhelper.FillDataTable("kspu_db.getFields", CommandType.StoredProcedure, npgsqlParameters);
            }
            string str = "";
            foreach (DataRow row in table.Rows)
                str += prefix + "." + row.ItemArray[0] + ",";
            return str.TrimEnd(',');
        }
        [HttpGet("GetRapportInfo", Name = nameof(GetRapportInfo))]
        public string GetRapportInfo(string tablename)
        {
            DataTable table;
            SetGDBUserAsConnStr();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_table", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                table = dbhelper.FillDataTable("kspu_db.getrapportinfo", CommandType.StoredProcedure, npgsqlParameters);
            }
            //return RunSQLStringReturn("SELECT * FROM " + Config.SluttRapportTableName);
            return Convert.ToString(table.Rows.Count);
        }
        [HttpPost("CalculateTotalFields", Name = nameof(CalculateTotalFields))]
        public bool CalculateTotalFields(string tablename, string sumSQL)
        {
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tablename;

                npgsqlParameters[1] = new NpgsqlParameter("p_sumsql", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = sumSQL;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.calculatetotalfields", CommandType.StoredProcedure, npgsqlParameters);
                }
                //var UpdateSQL = "UPDATE " + tablename + " SET " + sumSQL;
                //RunSQL(UpdateSQL);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }
        [HttpPost("UpdateTotalFields", Name = nameof(UpdateTotalFields))]
        public bool UpdateTotalFields(string tableName, string fromTable)
        {
            string totalFieldsTableA = getTotalFields(tableName, "A.");
            string totalFieldsTableB = getTotalFields(fromTable, "B.");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //var updateSQL = "UPDATE " + tableName + " A " + "SET (" + totalFieldsTableA + ")=(SELECT " + totalFieldsTableB + " FROM " + fromTable + " B " + "WHERE A." + Config.ReolIdFieldName + "=B." + Config.ReolIdFieldName + ") WHERE EXISTS (SELECT 1 FROM " + fromTable + " B " + "WHERE A." + Config.ReolIdFieldName + "=B." + Config.ReolIdFieldName + ")";
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;

                npgsqlParameters[1] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = fromTable;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.updatetotalfields", CommandType.StoredProcedure, npgsqlParameters);
                }
                //RunSQL(updateSQL);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }

        [HttpGet("GetInReolStringForStatRestPoints", Name = nameof(GetInReolStringForStatRestPoints))]
        public string GetInReolStringForStatRestPoints()
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            DataTable result;
            //var selSQL = "select distinct(" + Config.ReolIdFieldName + ") from " + Config.MergedReolPunkter + " minus select " + Config.ReolIdFieldName + " from " + Config.tmp_reolmap_adr;
            try
            {
                SetGDBUserAsConnStr();
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = null;

                npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = null;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.FillDataTable("kspu_db.GetInReolStringForStatRestPoints", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleDataReader reader = ExecuteReader(new OracleCommand(selSQL));

                //if (reader.HasRows)
                //{
                //    string InString = Config.ReolIdFieldName + " IN (";
                //    string whereclause = InString;
                //    string comma = "";
                //    string orString = "";
                //    int numStrings = 0;
                //    while (reader.Read())
                //    {
                //        if (!reader.IsDBNull(0))
                //        {
                //            if (numStrings == 50)
                //            {
                //                numStrings = 0;
                //                comma = "";
                //                whereclause += ") or " + InString;
                //            }
                //            whereclause += comma + "'" + reader.GetString(0) + "'";
                //        }
                //        comma = ",";
                //        numStrings += 1;
                //    }
                //    whereclause += ")";
                //    return whereclause;
                //}
                //else
                //   return Config.ReolIdFieldName + " = '-1'";
                return "1";
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return "Error";
            }
            finally
            {
                SetDBUserAsConnStr();
            }
        }

        [HttpPost("InsertMissingReols", Name = nameof(InsertMissingReols))]
        public bool InsertMissingReols(string statTableName, string RouteTableName)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            //string statFields = GetStatisticFields(statTableName, null/* Conversion error: Set to default value for this argument */, "'" + Config.ReolIdFieldName.ToUpper + "'", "|| '=0'");
            //string insertStr = "INSERT INTO " + statTableName + "(" + Config.ReolIdFieldName + ") SELECT " + Config.ReolIdFieldName + " FROM " + RouteTableName + " MINUS SELECT " + Config.ReolIdFieldName + " FROM " + statTableName;
            //var statField = statFields.Substring(0, statFields.IndexOf("="));
            //string updateStr = "UPDATE " + statTableName + " SET " + statFields + " WHERE " + statField + " IS NULL";

            try
            {
                SetGDBUserAsConnStr();
                npgsqlParameters[1] = new NpgsqlParameter("p_statTableName", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = statTableName;

                npgsqlParameters[1] = new NpgsqlParameter("p_RouteTableName", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = RouteTableName;

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.InsertMissingReols", CommandType.StoredProcedure, npgsqlParameters);
                }
                //RunSQL(insertStr);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL(updateStr);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpGet("IsXYPresent", Name = nameof(IsXYPresent))]
        public bool IsXYPresent(string TableName)
        {
            //var CreateSQL = "SELECT COUNT(*) ANTALL FROM " + TableName + " WHERE X <> 0 AND Y <> 0 ";
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = TableName;

                SetDBUserAsConnStr();
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.isxypresentantall", CommandType.StoredProcedure, npgsqlParameters);
                }
                bool XYPresent = true;
                if (XYPresent)
                {
                    int SwitchedXY = 0;
                    using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                    {
                        result = dbhelper.ExecuteScalar<int>("kspu_db.isxypresent", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    //int SwitchedXY = RunScalarSQL("SELECT COUNT(*) FROM  " + TableName + " WHERE X > 2000000");
                    if (SwitchedXY > 0)
                    {
                        _logger.LogWarning("Det er " + SwitchedXY.ToString() + " forekomster i  " + TableName + " som har snudd XY");
                        using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                        {
                            result = dbhelper.ExecuteScalar<int>("kspu_db.isxypresentupdate", CommandType.StoredProcedure, npgsqlParameters);
                        }
                        //RunSQL("UPDATE  " + TableName + " SET  X = Y, Y= X WHERE X > 2000000");
                        //RunSQL("COMMIT");
                    }
                }
                return XYPresent;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            finally
            {
                SetDBUserAsConnStr();
            }
        }

        [HttpPost("CreateRestPointsGK", Name = nameof(CreateRestPointsGK))]
        public bool CreateRestPointsGK()
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            //var CreateSQL = "Create table " + Config.tmp_restpoints_gk + " as select * from " + Config.tmp_restpoints + " where " + Config.ReolIdFieldName + " in (select distinct(" + Config.ReolIdFieldName + ") from " + Config.tmp_restpoints + " minus select " + Config.ReolIdFieldName + " from " + Config.tmp_reolmap_adr + ")";
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = null;

                SetDBUserAsConnStr();
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.CreateRestPointsGK", CommandType.StoredProcedure, npgsqlParameters);
                }
                SetGDBUserAsConnStr();
                //RunSQL(CreateSQL);

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            finally
            {
                SetDBUserAsConnStr();
            }
        }

        [HttpPost("UpdateReolPointsGK", Name = nameof(UpdateReolPointsGK))]
        public bool UpdateReolPointsGK()
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            object result;
            //var updateSQL = "UPDATE " + Config.tmp_restpoints_gk + " set(HUSH, BEF) = (select 1/count(*) HUSH,1/count(*) BEF from " + Config.tmp_restpoints_gk + ")";
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = null;

                SetGDBUserAsConnStr();
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.CreateRestPointsGK", CommandType.StoredProcedure, npgsqlParameters);
                }
                //RunSQL(updateSQL);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }

        [HttpPost("CreateStatisticTable", Name = nameof(CreateStatisticTable))]
        public bool CreateStatisticTable(string statTableName, string baseStatTableName, bool RoundNum, string statTotFields)
        {
            string[] statFields = GetStatisticFields(baseStatTableName).Split(",");

            bool percentIndexCalc = statTotFields != "";
            //Collection AvargeTotalFields = Config.AvargeTotalMapping;

            string SQL = "Create Table " + statTableName + " as select a.reol_id";
            string SelectSQL = " ";
            string commaSign = ", ";
            foreach (string field in statFields)
            {
                if (field.ToUpper().IndexOf("SN") > -1)
                {
                    string totalField;
                    try
                    {
                        //totalField = AvargeTotalFields[field];
                        totalField = "0";
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                        return false;
                    }
                    // Beregner snitt
                    // (decode(sum(a.gktoreolfaktor*b.TOTAL_ANTA), 0, 1,sum(a.gktoreolfaktor*b.TOTAL_ANTA))) SNITT_ALDE 
                    // For å unngå deling på null.
                    SelectSQL += commaSign + "sum(a.gktoreolfaktor*b." + totalField + "*" + field + ")/(decode(sum(a.gktoreolfaktor*b." + totalField + "), 0, 1,sum(a.gktoreolfaktor*b." + totalField + "))) " + field;
                }
                else if (percentIndexCalc)
                {
                    // Beregner prosent/index verdier for ruter avledet av grunnkrets sin prosent/index tabell
                    if (field.ToUpper().IndexOf("TOT") > -1)
                    {
                        string calculateString = "sum(a.gktoreolfaktor*b." + field + ") ";
                        if (RoundNum)
                            calculateString = "round(" + calculateString.TrimEnd() + ") ";
                        SelectSQL += commaSign + calculateString + field;
                    }
                    else
                    {
                        string totalField = getTotalField(statTotFields, field);
                        if (totalField == "")
                        {
                            string calculateString = "sum(a.gktoreolfaktor*b." + field + ") ";
                            if (RoundNum)
                                calculateString = "round(" + calculateString.TrimEnd() + ") ";
                            SelectSQL += commaSign + calculateString + field;
                        }
                        else
                        {
                            // sum(a.gktoreolfaktor*b.total_anta*b.aldu_5)/(decode(sum(a.gktoreolfaktor*b.total_anta), 0, 1,sum(a.gktoreolfaktor*b.total_anta))) aldu_5
                            string calculateString = "sum(a.gktoreolfaktor*b." + totalField + "*b." + field + ")/(decode(sum(a.gktoreolfaktor*b." + totalField + "), 0, 1,sum(a.gktoreolfaktor*b." + totalField + "))) ";
                            if (RoundNum)
                                calculateString = "round(" + calculateString.TrimEnd() + ") ";
                            SelectSQL += commaSign + calculateString + field;
                        }
                    }
                }
                else
                {
                    // Omberegner antall for ruter avledet av grunnkrets.
                    string calculateString = "sum(a.gktoreolfaktor*b." + field + ") ";
                    if (RoundNum)
                        calculateString = "round(" + calculateString.TrimEnd() + ") ";
                    SelectSQL += commaSign + calculateString + field;
                }
                commaSign = ", ";
            }

            //string fromSQL = " from " + Config.KSPU_DBUser + "." + Config.tmp_gkreoloverforing + " a inner join " + baseStatTableName + " b on a.gkid=b.gkrets_id Group by a.reol_id";

            //SQL += SelectSQL + fromSQL;
            try
            {
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = null;
                //RunSQL(SQL);
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.CreateRestPointsGK", CommandType.StoredProcedure, npgsqlParameters);
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }

        [HttpPost("CreateAddressReolTable", Name = nameof(CreateAddressReolTable))]
        public bool CreateAddressReolTable()
        {
            try
            {
                _ = _reolGeneratorRepository.CreateAddressReolTable().Result;
                //SetDBUserAsConnStr();
                //string calcfld = "A." + Config.calcfield;
                //RunSQL("CREATE TABLE " + Config.tmp_reol_adr + " (ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR) " + "AS SELECT A.ADRNR,A.REOL_ID,A.GKRETS_ID,SUBSTR(A.GKRETS_ID,1,4)," + calcfld + "," + calcfld + "," + calcfld + " FROM " + Config.KSPU_GDBUser + "." + Config.tmp_reolmap_adr + " A");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        //[HttpPost("UpdateAddressReolTable", Name = nameof(UpdateAddressReolTable))]
        //public bool UpdateAddressReolTable()
        //{
        //    try
        //    {
        //        //_reolGeneratorRepository.UpdateAddressReolTable().Result;
        //        string restpointGk =  _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);
        //        string tmp_reol_adr =  _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
        //        string KSPU_GDBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.kspu_gdbuser);
        //        string tmp_restpoints_gk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);

        //        string calcfld = "(select 1/count(*) from kspu_gdb." + restpointGk + ")";
        //        string insertQuery = "INSERT INTO " + tmp_reol_adr + "(ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR) " + "SELECT 0,A.REOL_ID,A.GKRETS_ID,SUBSTR(A.GKRETS_ID,1,4)," + calcfld + "," + calcfld + "," + calcfld + " FROM " + KSPU_GDBUser + "." + tmp_restpoints_gk + " A";

        //        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
        //        {
        //            dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
        //        }


        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.LogError(exception, exception.Message);
        //        return false;
        //    }
        //    return true;
        //}
        [HttpPost("UpdateAddressReolTable", Name = nameof(UpdateAddressReolTable))]
        public bool UpdateAddressReolTable()
        {
            try
            {
                if (_reolGeneratorRepository == null)
                {
                    _ = _reolGeneratorRepository.UpdateAddressReolTable().Result;
                }
                // ReolGeneratorRepository repo = new ReolGeneratorRepository();
                //var data = repo.UpdateAddressReolTable();

            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateAddressReolPostboxTable", Name = nameof(CreateAddressReolPostboxTable))]
        public bool CreateAddressReolPostboxTable()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_reol_adr_postbox + " (ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR) " + "AS SELECT A.ADRNRGAB,A.REOL_ID,B.GKRETS_ID,SUBSTR(B.GKRETS_ID,1,4),B.BEF,B.BEF, B.BEF " + "FROM " + Config.input_boksanlegg + " A, KSPU_GDB.GAB_ADR_P_BOLIG B " + "WHERE (B.ADRNR = A.ADRNRGAB) AND (A.REOL_ID NOT LIKE '% %' AND A.REOL_ID IS NOT NULL) ");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("InsertAddressReolPostboxTable", Name = nameof(InsertAddressReolPostboxTable))]
        public bool InsertAddressReolPostboxTable()
        {
            try
            {
                SetDBUserAsConnStr();
                // RunSQL("INSERT INTO " + Config.tmp_reol_adr + " SELECT * " + "FROM " + Config.tmp_reol_adr_postbox);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSumPopTable", Name = nameof(CreateSumPopTable))]
        public bool CreateSumPopTable()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_sumbefgk + " AS SELECT GKID, SUM(BEF) AS SUMBEF FROM " + Config.tmp_reol_adr + " GROUP BY GKID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSumPopReol", Name = nameof(CreateSumPopReol))]
        public bool CreateSumPopReol()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_sumbefreol + " AS SELECT REOL_ID,SUM(BEF) SUMBEF " + "FROM " + Config.tmp_reol_adr + " GROUP BY REOL_ID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("UpdateAddrReolTabWithSumPopGKTab", Name = nameof(UpdateAddrReolTabWithSumPopGKTab))]
        public bool UpdateAddrReolTabWithSumPopGKTab()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("UPDATE " + Config.tmp_reol_adr + " A " + "SET A.BEFGKFAKTOR = (SELECT (A.BEF/DECODE(B.SUMBEF,0,1,B.SUMBEF)) FROM " + Config.tmp_sumbefgk + " B " + "WHERE B.GKID= A.GKID)");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("UpdateAddrReolTabWithSumPopReolTab", Name = nameof(UpdateAddrReolTabWithSumPopReolTab))]
        public bool UpdateAddrReolTabWithSumPopReolTab()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("UPDATE " + Config.tmp_reol_adr + " A " + "SET A.BEFREOLFAKTOR = (SELECT (A.BEF/DECODE(B.SUMBEF,0,1,B.SUMBEF)) FROM " + Config.tmp_sumbefreol + " B " + "WHERE B.REOL_ID= A.REOL_ID)");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateTmpInputReolpunkterAdrNrGab", Name = nameof(CreateTmpInputReolpunkterAdrNrGab))]
        public bool CreateTmpInputReolpunkterAdrNrGab()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("create table " + Config.tmp_input_reolpunkter + " as (select " + Config.ReolIdFieldName + ", " + Config.adressenr_pas + " from " + Config.KSPU_DBUser + "." + Config.input_reolpunkter + " where (to_number(x) = 0 and to_number(y) = 0) and to_number(" + Config.adressenr_pas + ") <> 0 )");
                //return RunScalarSQL("SELECT COUNT(*) ANTALL FROM " + Config.tmp_input_reolpunkter) > 0;
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateTmpInputReolpunkterWithValidXY", Name = nameof(CreateTmpInputReolpunkterWithValidXY))]
        public bool CreateTmpInputReolpunkterWithValidXY()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("create table " + Config.tmp_input_reolpunkter + " as (select " + Config.ReolIdFieldName + ", x, y from " + Config.KSPU_DBUser + "." + Config.input_reolpunkter + " where (to_number(x) <> 0 and to_number(y) <> 0))");
            }
            // RunSQL("GRANT Select ON " & Config.tmp_input_reolpunkter & " TO " & Config.DB_role_Viewer)
            // RunSQL("GRANT Select,delete,update,insert ON " & Config.tmp_input_reolpunkter & " TO " & Config.DB_role_Editor)
            // RunSQL("commit")
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateTmpInputBoksanleggAdrNrGab", Name = nameof(CreateTmpInputBoksanleggAdrNrGab))]
        public bool CreateTmpInputBoksanleggAdrNrGab()
        {
            try
            {
                SetGDBUserAsConnStr();
                _ = _reolGeneratorRepository.CreateTmpInputBoksanleggAdrNrGab().Result;
                // RunSQL("create table " + Config.tmp_input_boksanlegg + " as (select " + Config.ReolIdFieldName + ", " + Config.adressenr_pas + " from " + Config.KSPU_DBUser + "." + Config.input_boksanlegg + " where (to_number(x) = 0 and to_number(y) = 0) and to_number(" + Config.adressenr_pas + ") <> 0)");

                //return RunScalarSQL("SELECT COUNT(*) ANTALL FROM " + Config.tmp_input_boksanlegg) > 0;
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }

        }

        [HttpPost("CreateTmpInputBoksanleggWithValidXY", Name = nameof(CreateTmpInputBoksanleggWithValidXY))]
        public bool CreateTmpInputBoksanleggWithValidXY()
        {
            try
            {
                SetGDBUserAsConnStr();
                // RunSQL("create table " + Config.tmp_input_boksanlegg + " as (select " + Config.ReolIdFieldName + ", X, Y from " + Config.KSPU_DBUser + "." + Config.input_boksanlegg + " where (to_number(x) <> 0 and to_number(y) <> 0))");
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateGKReolOverforing", Name = nameof(CreateGKReolOverforing))]
        public bool CreateGKReolOverforing()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_gkreoloverforing + " (REOL_ID, GKID, GKTOREOLFAKTOR,REOLTOGKFAKTOR) " + "AS SELECT REOL_ID, GKID,SUM(BEFGKFAKTOR),SUM(BEFREOLFAKTOR) FROM " + Config.tmp_reol_adr + " GROUP BY REOL_ID, GKID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateIndexOnGKGKReolOverforing", Name = nameof(CreateIndexOnGKGKReolOverforing))]
        public bool CreateIndexOnGKGKReolOverforing()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE INDEX " + Config.idx_gkreoloverforing + " ON " + Config.tmp_gkreoloverforing + "(" + Config.ReolIdFieldName + ")");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateIndexOnReolGKReolOverforing", Name = nameof(CreateIndexOnReolGKReolOverforing))]
        public bool CreateIndexOnReolGKReolOverforing()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE INDEX " + Config.idx_reolgkreoloverforing + " ON " + Config.tmp_gkreoloverforing + "(GKID)");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("Oppdater_tmp_reol_adrMedRiktigReolID", Name = nameof(Oppdater_tmp_reol_adrMedRiktigReolID))]
        public bool Oppdater_tmp_reol_adrMedRiktigReolID(string pinyReolID)
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("UPDATE " + Config.tmp_reol_adr + " SET " + Config.ReolIdFieldName + "='" + pinyReolID + "' WHERE ADRNR IN(SELECT reol_id from " + Config.tmp_reolid + ")");

                //RunSQL("COMMIT");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("AntallAdrePunkterITem_reol_adr", Name = nameof(AntallAdrePunkterITem_reol_adr))]
        public int AntallAdrePunkterITem_reol_adr(string sReolID)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetDBUserAsConnStr();
                //OracleCommand cmd = new OracleCommand("select antadr from " + Config.tmp_reolantadr + " where " + Config.ReolIdFieldName + " = '" + sReolID + "'");
                npgsqlParameters[0] = new NpgsqlParameter("p_sReolID", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = sReolID;
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    table = dbhelper.FillDataTable("kspu_db.CreateRestPointsGK", CommandType.StoredProcedure, npgsqlParameters);
                }

                string str = "";

                foreach (DataRow row in table.Rows)
                    str = row.ItemArray[0].ToString();
                if (str != "")
                    return System.Convert.ToInt32(str);
                else
                    return 0;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed selecting data, " + exception.Message);
                throw new Exception("Error running SQL!", exception);
            }
        }

        [HttpPost("CreateTMP_REOLID", Name = nameof(CreateTMP_REOLID))]
        public bool CreateTMP_REOLID()
        {
            SetDBUserAsConnStr();
            //RunSQL("CREATE TABLE " + Config.tmp_reolid + " (" + Config.ReolIdFieldName + " VARCHAR2(16 BYTE) NOT NULL ENABLE)");

            return true;
        }

        [HttpPost("CreateReolAntAdrTable", Name = nameof(CreateReolAntAdrTable))]
        public bool CreateReolAntAdrTable()
        {
            SetDBUserAsConnStr();
            // RunSQL("CREATE TABLE " + Config.tmp_reolantadr + " AS (SELECT " + Config.ReolIdFieldName + ", COUNT(" + Config.ReolIdFieldName + ") ANTADR FROM " + Config.tmp_reol_adr + " GROUP BY " + Config.ReolIdFieldName + ")");
            return true;
        }

        [HttpPost("InsertInto_TMP_REOLID", Name = nameof(InsertInto_TMP_REOLID))]
        public bool InsertInto_TMP_REOLID(string piReolID)
        {
            SetDBUserAsConnStr();

            //RunSQL("INSERT INTO " + Config.tmp_reolid + " values('" + piReolID + "')");
            //RunSQL("COMMIT");

            return true;
        }


        [HttpPost("CreateSumHH", Name = nameof(CreateSumHH))]
        public bool CreateSumHH()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_sumhh_komm + " AS SELECT KOMMUNEID, SUM(HH) SUM_HH FROM " + Config.input_reoler + " GROUP BY KOMMUNEID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateReolKommFaktor", Name = nameof(CreateReolKommFaktor))]
        public bool CreateReolKommFaktor()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_reol_komm_faktor + " AS SELECT a.REOL_ID, a.HH/DECODE(b.SUM_HH,0,1,SUM_HH) ANDEL FROM " + Config.input_reoler + " a" + " INNER JOIN " + Config.tmp_sumhh_komm + " b ON a.KOMMUNEID=b.KOMMUNEID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateAdjustReols", Name = nameof(CreateAdjustReols))]
        public bool CreateAdjustReols()
        {
            try
            {
                SetDBUserAsConnStr();

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateAvisdekning", Name = nameof(CreateAvisdekning))]
        public bool CreateAvisdekning()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_avisdekning + " AS SELECT a.REOL_ID, b.UTGAVE, b.EKSEMPLAR*c.ANDEL EKSEMPLAR,b.PROSENT*c.ANDEL PROSENT FROM " + Config.input_reoler + " a INNER JOIN " + Config.input_avisdekning + " b ON a.KOMMUNEID=b.KOMMUNEID" + " INNER JOIN " + Config.tmp_reol_komm_faktor + " c ON c.REOL_ID=a.REOL_ID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("createAvisMappingTable", Name = nameof(createAvisMappingTable))]
        public bool createAvisMappingTable()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("CREATE TABLE AVISFIELDMAPPING AS SELECT DISTINCT(UTGAVE), 'A164' FELTNAVN FROM INPUT_AVISDEKNING");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("updateAvisMappingTable", Name = nameof(updateAvisMappingTable))]
        public bool updateAvisMappingTable()
        {
            try
            {
                SetDBUserAsConnStr();
                //RunSQL("UPDATE AVISFIELDMAPPING SET FELTNAVN = 'A' || ROWNUM");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("createPostCoverageTable", Name = nameof(createPostCoverageTable))]
        public bool createPostCoverageTable()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE TMP_PSTDEK_TAB (KOMMID VARCHAR2(4) NOT NULL, DEKNING NUMBER(10, 2), CONSTRAINT TMP_PSTDEK_TAB PRIMARY KEY (KOMMID))");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpGet("createNewspaperTable", Name = nameof(createNewspaperTable))]
        public bool createNewspaperTable()
        {
            try
            {
                SetDBUserAsConnStr();
                //DataTable result = getData("SELECT FELTNAVN || ' NUMBER(10,2), ' FROM AVISFIELDMAPPING");
                //System.Text.StringBuilder Sql = new System.Text.StringBuilder();
                //Sql.Append("CREATE TABLE AVISDEKNING_KOMM(KOMMID VARCHAR2(4) NOT NULL, ");
                //foreach (DataRow row in result.Rows)
                //    Sql.Append(row.Item(0).ToString);
                //Sql.Append(" CONSTRAINT AVISDEKNING_KOMM_PK PRIMARY KEY(KOMMID))");
                //RunSQL(Sql.ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("InsertDataToPostCoverTable", Name = nameof(InsertDataToPostCoverTable))]
        public bool InsertDataToPostCoverTable()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("INSERT INTO TMP_PSTDEK_TAB SELECT KOMMUNEID AS KOMMID,(SUM(HH)/(SUM(HH)+SUM(HH_RES))*100) DEKNING FROM NORWAY_REOL GROUP BY KOMMUNEID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("InsertKommIDsToNewsPaperTable", Name = nameof(InsertKommIDsToNewsPaperTable))]
        public bool InsertKommIDsToNewsPaperTable()
        {
            try
            {
                SetDBUserAsConnStr();
                // RunSQL("INSERT INTO AVISDEKNING_KOMM(KOMMID) SELECT DISTINCT(KOMMUNEID) FROM INPUT_AVISDEKNING ORDER BY KOMMUNEID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("UpdateNewsPaperTable", Name = nameof(UpdateNewsPaperTable))]
        public bool UpdateNewsPaperTable()
        {
            try
            {
                SetDBUserAsConnStr();
                int numNewspapers = SelectCount("AVISFIELDMAPPING");
                //for (int newspaper = 1; newspaper <= numNewspapers; newspaper++)
                // RunSQL("UPDATE AVISDEKNING_KOMM A SET A" + newspaper.ToString().Trim() + "=(SELECT PROSENT FROM INPUT_AVISDEKNING B WHERE UTGAVE IN (SELECT UTGAVE FROM AVISFIELDMAPPING WHERE FELTNAVN = 'A" + newspaper.ToString().Trim() + "') AND A.KOMMID = B.KOMMUNEID)");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSegmentTempTable_1", Name = nameof(CreateSegmentTempTable_1))]
        public bool CreateSegmentTempTable_1()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_segment_1 + " AS SELECT REOL_ID,GKID,SUM(BEF) ANT_BEF FROM " + Config.KSPU_DBUser + "." + Config.tmp_reol_adr + " GROUP BY REOL_ID,GKID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSegmentTempTable_2", Name = nameof(CreateSegmentTempTable_2))]
        public bool CreateSegmentTempTable_2()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_segment_2 + " AS SELECT a.REOL_ID, a.GKID,a.ANT_BEF,b.SEGMENT FROM " + Config.tmp_segment_1 + " a," + Config.KSPU_DBUser + "." + Config.input_segmenter + " b" + " WHERE a.GKID=b.GKRETS_ID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSegmentTempTable_3", Name = nameof(CreateSegmentTempTable_3))]
        public bool CreateSegmentTempTable_3()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_segment_3 + " AS SELECT REOL_ID, SEGMENT,SUM(ANT_BEF) SUM_BEF FROM " + Config.tmp_segment_2 + " GROUP BY REOL_ID,SEGMENT");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        // Finds the segment value corresponding to each reol
        [HttpPost("CreateReolSegmentTable", Name = nameof(CreateReolSegmentTable))]
        public bool CreateReolSegmentTable()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_reol_segment + " AS SELECT REOL_ID, MAX(SEGMENT) SEGMENT FROM " + "(SELECT REOL_ID,SEGMENT FROM " + Config.tmp_segment_3 + " WHERE (REOL_ID,SUM_BEF) IN " + "(SELECT REOL_ID,MAX(SUM_BEF) SUM_BEF FROM " + Config.tmp_segment_3 + " GROUP BY REOL_ID)) " + "GROUP BY REOL_ID");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        // Finds the segment value corresponding to each reol
        [HttpPost("CreateInputReolerSegmentTable", Name = nameof(CreateInputReolerSegmentTable))]
        public bool CreateInputReolerSegmentTable()
        {
            try
            {
                SetGDBUserAsConnStr();
                //RunSQL("CREATE TABLE " + Config.tmp_input_reoler_segment + " AS SELECT A.*,B.SEGMENT " + "FROM " + Config.KSPU_DBUser + "." + Config.input_reoler + " A left join " + Config.tmp_reol_segment + " B " + "on (A." + Config.ReolIdFieldName + " = B." + Config.ReolIdFieldName + ")");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateAntallBefOgReolTabell", Name = nameof(CreateAntallBefOgReolTabell))]
        public bool CreateAntallBefOgReolTabell()
        {
            SetGDBUserAsConnStr();
            //RunSQL("CREATE TABLE " + Config.tmp_AntallBef_ReolTableName + " "
            //+ "as select " + Config.ReolIdFieldName + ",sum(BEFOLKN) BEFOLKN,sum(HUSHOLDN) HUSHOLDN from " + Config.tmp_100mGrid_Reol + " group by " + Config.ReolIdFieldName);

            return true;
        }

        [HttpPost("DeleteAntallBefReolTabell", Name = nameof(DeleteAntallBefReolTabell))]
        public bool DeleteAntallBefReolTabell()
        {
            SetGDBUserAsConnStr();
            try
            {
                //RunSQL("DROP TABLE " + Config.tmp_AntallBef_ReolTableName);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        [HttpPost("CreateSluttRapportTabell", Name = nameof(CreateSluttRapportTabell))]
        public bool CreateSluttRapportTabell()
        {
            SetGDBUserAsConnStr();
            //RunSQL("CREATE TABLE " + Config.SluttRapportTableName + " (RAPPORTTEKST  VARCHAR2(2048) NOT NULL)");
            return true;
        }

        [HttpPost("SkrivTilSluttRapportTabellen", Name = nameof(SkrivTilSluttRapportTabellen))]
        public bool SkrivTilSluttRapportTabellen(string piTekst)
        {
            SetGDBUserAsConnStr();
            //RunSQL("INSERT INTO " + Config.SluttRapportTableName + " (RAPPORTTEKST) VALUES('" + piTekst + "')");

            return true;
        }

        [HttpPost("GetRapportInfo", Name = nameof(GetRapportInfo))]
        public string GetRapportInfo()
        {
            SetGDBUserAsConnStr();
            //return RunSQLStringReturn("SELECT * FROM " + Config.SluttRapportTableName);
            return null;
        }

        [HttpGet("LoggSluttRapport", Name = nameof(LoggSluttRapport))]
        public bool LoggSluttRapport(string tablename)
        {
            DataTable table;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            try
            {
                SetGDBUserAsConnStr();
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[0].Value = tablename;
                //OracleCommand cmd = new OracleCommand("select * from " + Config.SluttRapportTableName);
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    table = dbhelper.FillDataTable("kspu_db.LoggSluttRapport", CommandType.StoredProcedure, npgsqlParameters);
                }

                //string str = "";

                //foreach (DataRow row in table.Rows)
                //{
                //    //if (IsDBNull(row.ItemArray[0]))
                //    //{
                //    //    str = row.ItemArray[0];

                //    //    _logger.LogMessage(str);
                //    //}
                //}

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }

        [HttpPost("SlettSluttRapportTabell", Name = nameof(SlettSluttRapportTabell))]
        public bool SlettSluttRapportTabell()
        {
            //return DropTable(Config.SluttRapportTableName, true);
            return true;
        }


        public string GDBDBConnectionString
        {
            get
            {
                return "Data Source=PUMA;User Id=kspu_gdb;Password=kspu_gdb;Integrated Security= no";
            }
        }
        public string DBDBConnectionString
        {
            get
            {
                return "Data Source=PUMA;User Id=kspu_db;Password=kspu_db;Integrated Security=no";
            }
        }

        #endregion

        #region Private Methods
        private bool AnalyzeTable(string tableName, bool gdbuser = false)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    table = dbhelper.FillDataTable("kspu_db.AnalyzeTable", CommandType.StoredProcedure, npgsqlParameters);
                }
                // Example AnalyzeTable("REOL_ADR")
                // RunSQL("ANALYZE TABLE " + tableName + " COMPUTE STATISTICS");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        //private  DataTable getData(string sql)
        //{
        //    try
        //    {
        //        return DABase.GetDataTable(new OracleCommand(sql));
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //        return null/* TODO Change to default(_) if this is not a reference type */;
        //    }
        //}
        [HttpGet("getAliasTable", Name = nameof(getAliasTable))]
        public DataTable getAliasTable()
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetGDBUserAsConnStr();
                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    table = dbhelper.FillDataTable("kspu_db.getAliasTable", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleCommand cmd = new OracleCommand("Select tabell, felt, alias from feltalias order by tabell");
                return table;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
        }

        [HttpGet("GetDublikateReolkrets", Name = nameof(GetDublikateReolkrets))]
        public DataTable GetDublikateReolkrets()
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetGDBUserAsConnStr();

                using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    table = dbhelper.FillDataTable("kspu_db.GetDublikateReolkrets", CommandType.StoredProcedure, npgsqlParameters);
                }

                //OracleCommand cmd = new OracleCommand(" select reol_id,count(reol_id) from " + Config.tmp_imported_2 + " group by reol_ID having count(reol_id)>1 ");

                return table;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
        }

        //private static string RunSQLStringReturn(string SQL)
        //{
        //    OracleDataReader dataReader = ExecuteReader(new OracleCommand(SQL));
        //    string str = "";
        //    while (dataReader.Read())
        //        str += dataReader.GetOracleString(0) + Constants.vbCrLf;
        //    return str;
        //}
        protected string DBConnectionString
        {
            set
            {
                _DBConnectionString = value;
            }
        }

        private void SetGDBUserAsConnStr()                                            // Connect as GDB
        {
            DBConnectionString = GDBDBConnectionString;

        }
        private void SetDBUserAsConnStr()                                             // Connect as DB
        {
            DBConnectionString = DBDBConnectionString;
        }

        private int SelectCount(string tablename)
        {
            _logger.LogDebug("Inside into SelectCount");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename;
            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_db.selectcount", CommandType.StoredProcedure, npgsqlParameters);
            }
            //return ExecuteScalar(new OracleCommand("Select Count(*) from " + tablename));

            if (result.ToString() == null) // | (result.ToString()) is DBNull)
                return 0;

            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from SelectCount");
            return System.Convert.ToInt32(result);
        }

        private string getTotalField(string totFieldString, string fieldname)
        {
            try
            {
                int totFieldInd = 0;
                string[] totFields = totFieldString.Split(new Char[] { ',', '=' });
                foreach (string field in totFields)
                {
                    if (field.ToUpper().IndexOf(fieldname.ToUpper()) > -1)
                        return totFields[totFieldInd - 1].Trim();
                    totFieldInd += 1;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return "";
        }

        private string getTotalFields(string fromTable, string prefix = "")
        {
            SetGDBUserAsConnStr();
            DataTable table;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;
            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                table = dbhelper.FillDataTable("kspu_db.selectcount", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand("select column_name from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' " + "and column_name like 'TOT%' order by column_name");


            string str = "";
            foreach (DataRow row in table.Rows)
                str += prefix + row.ItemArray[0] + ",";
            return str.TrimEnd(',');
        }

        private string GetStatisticFields(string fromTable, string prefix = "", string AdditionalIgnoreFields = "", string additionSelect = "")
        {
            SetGDBUserAsConnStr();
            DataTable table;
            if (AdditionalIgnoreFields != "" & !AdditionalIgnoreFields.StartsWith(","))
                AdditionalIgnoreFields = "," + AdditionalIgnoreFields;
            //OracleCommand cmd = new OracleCommand("select column_name " + additionSelect + " from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' " + "and column_name not like 'SEG%' AND column_name not " + "in (" + Config.IgnoreFields + AdditionalIgnoreFields + ") order by column_id");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;
            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                table = dbhelper.FillDataTable("kspu_db.GetStatisticFields", CommandType.StoredProcedure, npgsqlParameters);
            }

            string str = "";
            foreach (DataRow row in table.Rows)
                str += prefix + row.ItemArray[0] + ",";
            return str.TrimEnd(',');
        }



        #endregion
    }
}
