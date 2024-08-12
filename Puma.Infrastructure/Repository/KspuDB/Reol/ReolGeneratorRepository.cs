using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB.Reol
{
    /// <summary>
    /// ReolGeneratorRepository
    /// </summary>
    public class ReolGeneratorRepository : KsupDBGenericRepository<utvalg>, IReolGeneratorRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ReolGeneratorRepository> _logger;
        /// <summary>
        /// The connctionstring
        /// </summary>
        public readonly string Connctionstring;
        public string commit;
        public string deleteQuery2;
        public string deleteQuery;
        public string deleteQuery1;
        /// <summary>
        /// The configuration repository
        /// </summary>
        public readonly IConfigurationRepository _configurationRepository;

        public string _DBConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReolGeneratorRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configurationRepository">The configuration repository.</param>
        public ReolGeneratorRepository(KspuDBContext context, ILogger<ReolGeneratorRepository> logger, IConfigurationRepository configurationRepository) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
            _configurationRepository = configurationRepository;

        }

        #region Public Methods
        /// <summary>
        /// Calculates the statistics.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> CalculateStatistics(string tableName, bool gdbuser = false)
        {
            await Task.Run(() => { });
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

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_calculatestatistics", CommandType.StoredProcedure, npgsqlParameters);
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
        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> DropTable(string tableName, bool gdbuser = false)
        {
            await Task.Run(() => { });
            string schema = gdbuser ? "kspu_gdb." : "kspu_db.";
            object result;
            string dropTable = "DROP TABLE " + schema + tableName;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery(dropTable, CommandType.Text, null);
                }
                //RunSQL("DROP TABLE " + tableName);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Doeses the table exist.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> DoesTableExist(string tableName, bool gdbuser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_doestableexist", CommandType.StoredProcedure, npgsqlParameters);
            }

            if (result == null || (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;

        }


        /// <summary>
        /// Doeses the view exist.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> DoesViewExist(string viewName, bool gdbuser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();

            npgsqlParameters[0] = new NpgsqlParameter("p_viewName", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = viewName;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_doesviewexist", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (result == null || (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Renames the table.
        /// </summary>
        /// <param name="fromTableName">Name of from table.</param>
        /// <param name="toTableName">Name of to table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> RenameTable(string fromTableName, string toTableName, bool gdbuser = false)
        {
            await Task.Run(() => { });
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

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_renametable", CommandType.StoredProcedure, npgsqlParameters);
                }
                //RunSQL("ALTER TABLE " + fromTableName + " RENAME TO  " + toTableName);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message + " " + exception.Source + " SQL: " + "ALTER TABLE " + fromTableName + " RENAME TO  " + toTableName);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="IndexName">Name of the index.</param>
        /// <param name="TablenameWithFields">The tablename with fields.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> CreateIndex(string IndexName, string TablenameWithFields, bool gdbuser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;

            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_indexname", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = IndexName;

                npgsqlParameters[1] = new NpgsqlParameter("p_tablenamewithfields", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = gdbuser ? "kspu_gdb." : "kspu_db." + TablenameWithFields;



                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_createindex", CommandType.StoredProcedure, npgsqlParameters);
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


        /// <summary>
        /// Drops the index.
        /// </summary>
        /// <param name="IndexName">Name of the index.</param>
        /// <param name="GDBUser">if set to <c>true</c> [GDB user].</param>
        /// <returns></returns>
        public async Task<bool> DropIndex(string IndexName, bool GDBUser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_indexname", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = GDBUser ? "kspu_gdb." : "kspu_db." + IndexName;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_dropindex", CommandType.StoredProcedure, npgsqlParameters);
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


        /// <summary>
        /// Updates the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="tableNameWithFields">The table name with fields.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> UpdateIndex(string indexName, string tableNameWithFields, bool gdbuser = false)
        {
            await Task.Run(() => { });

            await DropIndex(indexName, gdbuser);
            await CreateIndex(indexName, tableNameWithFields, gdbuser);
            return true;
        }


        /// <summary>
        /// Truncates the table.
        /// </summary>
        /// <param name="piTableNAme">The pi table n ame.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> Truncate_Table(string piTableNAme, bool gdbuser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            try
            {

                npgsqlParameters[0] = new NpgsqlParameter("p_piTableNAme", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = piTableNAme;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Grants the select.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="toUser">To user.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> GrantSelect(string tableName, string toUser, bool gdbuser = false)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            try
            {

                //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                //npgsqlParameters[0].Value = gdbuser ? "kspu_gdb." : "kspu_db." + tableName;

                //npgsqlParameters[1] = new NpgsqlParameter("p_touser", NpgsqlTypes.NpgsqlDbType.Varchar);
                //npgsqlParameters[1].Value = toUser;

                //using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                //{
                //    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_grantselect", CommandType.StoredProcedure, npgsqlParameters);
                //}
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

        /// <summary>
        /// Adds the variable cha r2 field to table.
        /// </summary>
        /// <param name="piTableNAme">The pi table n ame.</param>
        /// <param name="piFeltNAme">The pi felt n ame.</param>
        /// <param name="piLength">Length of the pi.</param>
        /// <returns></returns>
        public async Task<bool> AddVarCHAR2FieldToTable(string piTableNAme, string piFeltNAme, int piLength)
        {
            await Task.Run(() => { });
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

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.custom_addvarchar2fieldtotable", CommandType.StoredProcedure, npgsqlParameters);
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

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="asGDB">if set to <c>true</c> [as GDB].</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public async Task<string> getFields(string fromTable, bool asGDB, string prefix)
        {
            await Task.Run(() => { });
            DataTable table;
            // select column_name from USER_TAB_COLUMNS where upper(table_name) = upper('input_boksanlegg');

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            npgsqlParameters[0] = new NpgsqlParameter("p_fromtable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;

            npgsqlParameters[1] = new NpgsqlParameter("p_prefix", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = prefix;
            //OracleCommand cmd = new OracleCommand("select column_name from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' ");

            //DataTable table = GetDataTable(cmd);
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                table = dbhelper.FillDataTable("kspu_db.getFields", CommandType.StoredProcedure, npgsqlParameters);
            }
            string str = "";
            foreach (DataRow row in table.Rows)
                str += prefix + "." + row.ItemArray[0] + ",";
            return str.TrimEnd(',');
        }

        /// <summary>
        /// Gets the rapport information.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        public async Task<string> GetRapportInfo(string tablename)
        {
            await Task.Run(() => { });
            DataTable table;
            SetGDBUserAsConnStr();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_table", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                table = dbhelper.FillDataTable("kspu_db.getrapportinfo", CommandType.StoredProcedure, npgsqlParameters);
            }
            //return RunSQLStringReturn("SELECT * FROM " + Config.SluttRapportTableName);
            return Convert.ToString(table.Rows.Count);
        }

        /// <summary>
        /// Calculates the total fields.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <param name="sumSQL">The sum SQL.</param>
        /// <returns></returns>
        public async Task<bool> CalculateTotalFields(string tablename, string sumSQL)
        {
            await Task.Run(() => { });
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tablename;

                npgsqlParameters[1] = new NpgsqlParameter("p_sumsql", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = sumSQL;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_gdb.custom_calculatetotalfields", CommandType.StoredProcedure, npgsqlParameters);
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

        /// <summary>
        /// Updates the total fields.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fromTable">From table.</param>
        /// <returns></returns>
        public async Task<bool> UpdateTotalFields(string tableName, string fromTable)
        {
            await Task.Run(() => { });
            string totalFieldsTableA = await getTotalFields(tableName, "A.");
            string totalFieldsTableB = await getTotalFields(fromTable, "B.");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            //var updateSQL = "UPDATE " + tableName + " A " + "SET (" + totalFieldsTableA + ")=(SELECT " + totalFieldsTableB + " FROM " + fromTable + " B " + "WHERE A." + Config.ReolIdFieldName + "=B." + Config.ReolIdFieldName + ") WHERE EXISTS (SELECT 1 FROM " + fromTable + " B " + "WHERE A." + Config.ReolIdFieldName + "=B." + Config.ReolIdFieldName + ")";
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;

                npgsqlParameters[1] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = fromTable;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Gets the in reol string for stat rest points.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetInReolStringForStatRestPoints()
        {
            await Task.Run(() => { });
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

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Inserts the missing reols.
        /// </summary>
        /// <param name="statTableName">Name of the stat table.</param>
        /// <param name="RouteTableName">Name of the route table.</param>
        /// <returns></returns>
        public async Task<bool> InsertMissingReols(string statTableName, string RouteTableName)
        {
            await Task.Run(() => { });
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

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Determines whether [is xy present] [the specified table name].
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        public async Task<bool> IsXYPresent(string TableName)
        {
            await Task.Run(() => { });
            TableName = "kspu_db." + TableName;
            var getAntal = "SELECT COUNT(*) ANTALL FROM " + TableName + " WHERE X <> 0 AND Y <> 0 ";
            object result;
            try
            {
                int XYPresent = 0;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    XYPresent = dbhelper.ExecuteScalar<int>(getAntal, CommandType.Text, null);
                }
                if (XYPresent > 0)
                {
                    int SwitchedXY = 0;
                    var checkXYExists = "SELECT COUNT(*) FROM  " + TableName + " WHERE X > 2000000 ";
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        SwitchedXY = dbhelper.ExecuteScalar<int>(checkXYExists, CommandType.Text, null);
                    }
                    //int SwitchedXY = RunScalarSQL("SELECT COUNT(*) FROM  " + TableName + " WHERE X > 2000000");
                    if (SwitchedXY > 0)
                    {
                        var updateData = "UPDATE  " + TableName + " SET  X = Y, Y= X WHERE X > 2000000";

                        _logger.LogWarning("Det er " + SwitchedXY.ToString() + " forekomster i  " + TableName + " som har snudd XY");
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            result = dbhelper.ExecuteNonQuery(updateData, CommandType.Text, null);
                        }
                    }
                }
                return XYPresent > 0;
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


        /// <summary>
        /// Creates the rest points gk.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateRestPointsGK()
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            string tmp_restpoints_gk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);
            string tmp_restpoints = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints);
            string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
            string tmp_reolmap_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolmap_adr);
            string insertQuery = "Create table " + tmp_restpoints_gk + " as select * from " + tmp_restpoints + " where " + ReolIdFieldName + " in (select distinct(" + ReolIdFieldName + ") from " + tmp_restpoints + " minus select " + ReolIdFieldName + " from " + tmp_reolmap_adr + ")";
            try
            {
               // SetDBUserAsConnStr();
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
                //SetGDBUserAsConnStr();
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


        /// <summary>
        /// Updates the reol points gk.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateReolPointsGK()
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            string tmp_restpoints_gk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);
            string updateQuery = "UPDATE " + tmp_restpoints_gk + " set(HUSH, BEF) = (select 1/count(*) HUSH,1/count(*) BEF from " + tmp_restpoints_gk + ")";
            try
            {


                SetGDBUserAsConnStr();
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(updateQuery, CommandType.Text, null);
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }


        /// <summary>
        /// Creates the statistic table.
        /// </summary>
        /// <param name="statTableName">Name of the stat table.</param>
        /// <param name="baseStatTableName">Name of the base stat table.</param>
        /// <param name="RoundNum">if set to <c>true</c> [round number].</param>
        /// <param name="statTotFields">The stat tot fields.</param>
        /// <returns></returns>
        public async Task<bool> CreateStatisticTable(string statTableName, string baseStatTableName, bool RoundNum, string statTotFields)
        {
            await Task.Run(() => { });
            string[] statFields = GetStatisticFields(baseStatTableName).Result.Split(",");

            bool percentIndexCalc = statTotFields != "";

            string SQL = "Create Table " + statTableName + " as select a.reol_id";
            string SelectSQL = " ";
            string commaSign = ", ";
            foreach (string field in statFields)
            {
                if (field.IndexOf("SN", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    string totalField;
                    try
                    {
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
                    if (field.IndexOf("TOT", StringComparison.OrdinalIgnoreCase) > -1)
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

            try
            {
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = null;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Creates the address reol table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAddressReolTable()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string calcfield = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.calcfield);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string KSPU_GDBUser = "kspu_gdb";//await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_GDBUser);
                string tmp_reolmap_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolmap_adr);
                string calcfld = "A." + calcfield;
                string insertQuery = "CREATE TABLE " + tmp_reol_adr + "(ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR)"
                    + "AS SELECT A.ADRNR,A.REOL_ID,A.GKRETS_ID,SUBSTRING(A.GKRETS_ID,1,4)," + calcfld + "," + calcfld + "," + calcfld + " FROM " + KSPU_GDBUser + "." + tmp_reolmap_adr + " A";

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }


            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;

        }



        /// <summary>
        /// Updates the address reol table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAddressReolTable()
        {
            await Task.Run(() => { });
            try
            {
                string restpointGk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string KSPU_GDBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_GDBUser);
                string tmp_restpoints_gk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_restpoints_gk);

                string calcfld = "(select 1/count(*) from kspu_gdb." + restpointGk + ")";
                string insertQuery = "INSERT INTO " + tmp_reol_adr + "(ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR) " + "SELECT 0,A.REOL_ID,A.GKRETS_ID,SUBSTR(A.GKRETS_ID,1,4)," + calcfld + "," + calcfld + "," + calcfld + " FROM " + KSPU_GDBUser + "." + tmp_restpoints_gk + " A";

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }


            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the address reol postbox table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAddressReolPostboxTable()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_adr_postbox = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr_postbox);
                string input_boksanlegg = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_boksanlegg);
                string insertQuery = "CREATE TABLE " + tmp_reol_adr_postbox + " (ADRNR, REOL_ID,GKID,KOMMID,BEF,BEFGKFAKTOR, BEFREOLFAKTOR) " + "AS SELECT A.ADRNRGAB,A.REOL_ID,B.GKRETS_ID,SUBSTR(B.GKRETS_ID,1,4),B.BEF,B.BEF, B.BEF " + "FROM " + input_boksanlegg + " A, KSPU_GDB.GAB_ADR_P_BOLIG B " + "WHERE (B.ADRNR = A.ADRNRGAB) AND (A.REOL_ID NOT LIKE '% %' AND A.REOL_ID IS NOT NULL) ";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Inserts the address reol postbox table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertAddressReolPostboxTable()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_adr_postbox = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr_postbox);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string insertQuery = "INSERT INTO " + tmp_reol_adr + " SELECT * " + "FROM " + tmp_reol_adr_postbox;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the sum pop table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSumPopTable()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_sumbefgk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumbefgk);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string insertQuery = "CREATE TABLE " + tmp_sumbefgk + " AS SELECT GKID, SUM(BEF) AS SUMBEF FROM " + tmp_reol_adr + " GROUP BY GKID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the sum pop reol.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSumPopReol()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_sumbefreol = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumbefreol);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string insertQuery = "CREATE TABLE " + tmp_sumbefreol + " AS SELECT REOL_ID,SUM(BEF) SUMBEF " + "FROM " + tmp_reol_adr + " GROUP BY REOL_ID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Updates the addr reol tab with sum pop gk tab.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAddrReolTabWithSumPopGKTab()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string tmp_sumbefgk = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumbefgk);
                string insertQuery = "UPDATE " + tmp_reol_adr + " A " + "SET A.BEFGKFAKTOR = (SELECT (A.BEF/DECODE(B.SUMBEF,0,1,B.SUMBEF)) FROM " + tmp_sumbefgk + " B " + "WHERE B.GKID= A.GKID)";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Updates the addr reol tab with sum pop reol tab.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAddrReolTabWithSumPopReolTab()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string tmp_sumbefreol = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumbefreol);
                string updateQuery = "UPDATE " + tmp_reol_adr + " A " + "SET A.BEFREOLFAKTOR = (SELECT (A.BEF/DECODE(B.SUMBEF,0,1,B.SUMBEF)) FROM " + tmp_sumbefreol + " B " + "WHERE B.REOL_ID= A.REOL_ID)";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(updateQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the temporary input reolpunkter adr nr gab.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateTmpInputReolpunkterAdrNrGab()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_input_reolpunkter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_input_reolpunkter);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string adressenr_pas = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.adressenr_pas);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string input_reolpunkter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reolpunkter);

                string insertQuery = "create table " + tmp_input_reolpunkter + " as (select " + ReolIdFieldName + ", " + adressenr_pas + " from " + KSPU_DBUser + "." + input_reolpunkter + " where (to_number(x) = 0 and to_number(y) = 0) and to_number(" + adressenr_pas + ") <> 0 )";
                //string selectQuery =  "SELECT COUNT(*) ANTALL FROM " + tmp_input_reolpunkter >0;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the temporary input reolpunkter with valid xy.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateTmpInputReolpunkterWithValidXY()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_input_reolpunkter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_input_reolpunkter);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string input_reolpunkter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reolpunkter);
                string insertQuery = "create table " + tmp_input_reolpunkter + " as (select " + ReolIdFieldName + ", x, y from " + KSPU_DBUser + "." + input_reolpunkter + " where (cast(x as numeric) <> 0 and cast(y as numeric) <> 0))";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the temporary input boksanlegg adr nr gab.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateTmpInputBoksanleggAdrNrGab()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_input_boksanlegg = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_input_boksanlegg);
                string adressenr_pas = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.adressenr_pas);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string input_boksanlegg = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_boksanlegg);
                string insertQuery = "create table " + tmp_input_boksanlegg + " as (select " + ReolIdFieldName + ", " + adressenr_pas + " from " + KSPU_DBUser + "." + input_boksanlegg + " where (to_number(x) = 0 and to_number(y) = 0) and to_number(" + adressenr_pas + ") <> 0)";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
                string selectQuery = "SELECT COUNT(*) ANTALL FROM " + tmp_input_boksanlegg;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(selectQuery, CommandType.Text, null);
                }
                return Convert.ToInt32(selectQuery) > 0;

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }


        /// <summary>
        /// Creates the temporary input boksanlegg with valid xy.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateTmpInputBoksanleggWithValidXY()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_input_boksanlegg = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_input_boksanlegg);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string input_boksanlegg = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_boksanlegg);
                string insertQuery = "create table " + tmp_input_boksanlegg + " as (select " + ReolIdFieldName + ", X, Y from " + KSPU_DBUser + "." + input_boksanlegg + " where (to_number(x) <> 0 and to_number(y) <> 0))";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates the gk reol overforing.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateGKReolOverforing()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_gkreoloverforing = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_gkreoloverforing);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string insertQuery = "CREATE TABLE " + tmp_gkreoloverforing + " (REOL_ID, GKID, GKTOREOLFAKTOR,REOLTOGKFAKTOR) " + "AS SELECT REOL_ID, GKID,SUM(BEFGKFAKTOR),SUM(BEFREOLFAKTOR) FROM " + tmp_reol_adr + " GROUP BY REOL_ID, GKID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the index on GKGK reol overforing.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateIndexOnGKGKReolOverforing()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string idx_gkreoloverforing = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.idx_gkreoloverforing);
                string tmp_gkreoloverforing = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_gkreoloverforing);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string insertQuery = "CREATE INDEX " + idx_gkreoloverforing + " ON " + tmp_gkreoloverforing + "(" + ReolIdFieldName + ")";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the index on reol gk reol overforing.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateIndexOnReolGKReolOverforing()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string idx_reolgkreoloverforing = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.idx_reolgkreoloverforing);
                string tmp_gkreoloverforing = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_gkreoloverforing);
                string insertQuery = "CREATE INDEX " + idx_reolgkreoloverforing + " ON " + tmp_gkreoloverforing + "(GKID)";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Oppdaters the temporary reol adr med riktig reol identifier.
        /// </summary>
        /// <param name="pinyReolID">The piny reol identifier.</param>
        /// <returns></returns>
        public async Task<bool> Oppdater_tmp_reol_adrMedRiktigReolID(string pinyReolID)
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string tmp_reolid = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolid);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string updateQuery = "UPDATE " + tmp_reol_adr + " SET " + ReolIdFieldName + "='" + pinyReolID + "' WHERE ADRNR IN(SELECT reol_id from " + tmp_reolid + ")";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(updateQuery, CommandType.Text, null);
                }
                //RunSQL("COMMIT");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Antalls the adre punkter i tem reol adr.
        /// </summary>
        /// <param name="sReolID">The s reol identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error running SQL!</exception>
        public async Task<int> AntallAdrePunkterITem_reol_adr(string sReolID)
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetDBUserAsConnStr();
                //OracleCommand cmd = new OracleCommand("select antadr from " + Config.tmp_reolantadr + " where " + Config.ReolIdFieldName + " = '" + sReolID + "'");
                npgsqlParameters[0] = new NpgsqlParameter("p_sReolID", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = sReolID;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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


        /// <summary>
        /// Creates the temporary reolid.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateTMP_REOLID()
        {
            await Task.Run(() => { });
            SetDBUserAsConnStr();
            string tmp_reolid = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolid);
            string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
            string insertQuery = "CREATE TABLE " + tmp_reolid + " (" + ReolIdFieldName + " VARCHAR2(16 BYTE) NOT NULL ENABLE)";
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            return true;
        }


        /// <summary>
        /// Creates the reol ant adr table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateReolAntAdrTable()
        {
            await Task.Run(() => { });
            SetDBUserAsConnStr();
            string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
            string tmp_reolantadr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolantadr);
            string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
            string insertQuery = "CREATE TABLE " + tmp_reolantadr + " AS (SELECT " + ReolIdFieldName + ", COUNT(" + ReolIdFieldName + ") ANTADR FROM " + tmp_reol_adr + " GROUP BY " + ReolIdFieldName + ")";
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            return true;
        }


        /// <summary>
        /// Inserts the into temporary reolid.
        /// </summary>
        /// <param name="piReolID">The pi reol identifier.</param>
        /// <returns></returns>
        public async Task<bool> InsertInto_TMP_REOLID(string piReolID)
        {
            await Task.Run(() => { });
            SetDBUserAsConnStr();
            string tmp_reolid = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reolid);
            string insertQuery = "INSERT INTO " + tmp_reolid + " values('" + piReolID + "')";
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            //RunSQL("COMMIT");

            return true;
        }



        /// <summary>
        /// Creates the sum hh.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSumHH()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_sumhh_komm = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumhh_komm);
                string input_reoler = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reoler);
                string insertQuery = "CREATE TABLE " + tmp_sumhh_komm + " AS SELECT KOMMUNEID, SUM(HH) SUM_HH FROM " + input_reoler + " GROUP BY KOMMUNEID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the reol komm faktor.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateReolKommFaktor()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_reol_komm_faktor = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_komm_faktor);
                string tmp_sumhh_komm = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_sumhh_komm);
                string input_reoler = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reoler);
                string insertQuery = "CREATE TABLE " + tmp_reol_komm_faktor + " AS SELECT a.REOL_ID, a.HH/DECODE(b.SUM_HH,0,1,SUM_HH) ANDEL FROM " + input_reoler + " a" + " INNER JOIN " + tmp_sumhh_komm + " b ON a.KOMMUNEID=b.KOMMUNEID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the adjust reols.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAdjustReols()
        { 
            await Task.Run(() => { });
            try
            {
                
                SetDBUserAsConnStr();
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
                string input_reolpunkter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reolpunkter);
                string adressenr_pas = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.adressenr_pas);
                string KSPU_GDBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_GDBUser);
                ///'drop table tmp_reolxyadr;
                await DropTable("TMP_REOLXYADR");
                /// 'grupper reoler med samme x,y forekomster og tell opp hvor mange som har samme ID, x og y
                /// 'create table tmp_reolxyadr as (select reol_id, x, y, count(*) antall from input_reolpunkter where x <> 0 and y <> 0 group by reol_id, x, y);
                string insertQuery = "CREATE TABLE TMP_REOLXYADR AS (SELECT " + ReolIdFieldName + " , X, Y, COUNT(*) ANTALL FROM  " + input_reolpunkter +
                   " WHERE X <> 0 AND Y <> 0 GROUP BY " + ReolIdFieldName + ", X, Y";
                /// 'drop table tmp_reoladr;
                await DropTable("TMP_REOLADR");
                //// 'Av disse grupper opp alle forekomster for reol_id og tell opp hvor mange det er pr reol.
                /// 'create table tmp_reoladr as (select reol_id, count(Reol_ID) antall from tmp_reolxyadr group by reol_id);
                string insertQuery1 = "CREATE TABLE TMP_REOLADR AS (SELECT " + ReolIdFieldName + ", COUNT(" + ReolIdFieldName + ") ANTALL FROM TMP_REOLXYADR GROUP BY " + ReolIdFieldName;
                ///  'delete from tmp_reoladr where antall > 1;
                 deleteQuery  = "DELETE FROM TMP_REOLADR WHERE ANTALL > 1"; 
                ///  '-- Av disse grupper x,y og tell opp hvor mange ganger det forekommer
                /// 'drop table tmp_xyadr;
                await DropTable("TMP_XYADR");
                /// 'create table tmp_xyadr as (select x, y, count(*) antall from tmp_reolxyadr group by x, y);
                //string insertQuery2 = "CREATE TABLE TMP_XYADR AS (SELECT X, Y, COUNT(*) ANTALL FROM TMP_REOLXYADR GROUP BY X, Y)";
                /// '-- fjern de som forekommer 1 gang, de er unike.
                ///'delete from tmp_xyadr where antall = 1;
                deleteQuery1 = "DELETE FROM TMP_XYADR WHERE ANTALL = 1";
                ///'-- finn alle reoler som har xy som forekommer i flere reoler.
                ///'drop table adjust_reol;
                await DropTable("ADJUST_REOL");
                /// 'create table adjust_reol as (select reol_id from tmp_reolxyadr where x || ',' || y in (select x || ',' || y from tmp_xyadr) and reol_id in (select reol_id from tmp_reoladr)); 
                string insertQuery3 = "CREATE TABLE ADJUST_REOL AS (SELECT " + ReolIdFieldName + " FROM TMP_REOLXYADR WHERE X || ',' || Y IN (SELECT X || ',' || Y FROM TMP_XYADR) AND " +
                   ReolIdFieldName + " IN (SELECT " + ReolIdFieldName + " FROM TMP_REOLADR))";
                ///'Gjør det samme for ADBNRGAB
                ///'drop table tmp_reolgabadr;
                await DropTable("TMP_REOLGABADR");
                ///'create table tmp_reolgabadr as (select reol_id, adrnrgab, count(*) antall from input_reolpunkter where x=0 and y = 0 group by reol_id, adrnrgab);
                string insertQuery4 = "CREATE TABLE TMP_REOLGABADR AS (SELECT " + ReolIdFieldName + "," + adressenr_pas + ", COUNT(*) ANTALL FROM " + input_reolpunkter +
                   " WHERE X=0 AND Y = 0 GROUP BY " + ReolIdFieldName + "," + adressenr_pas + ")";
                ///'drop table tmp_reoladrgab;
                await DropTable("TMP_REOLADRGAB");
                ///'create table tmp_reoladrgab as (select reol_id, count(*) antall from tmp_reolgabadr group by reol_id);
                insertQuery4 = "CREATE TABLE TMP_REOLADRGAB AS (SELECT " + ReolIdFieldName + ", COUNT(*) ANTALL FROM TMP_REOLGABADR GROUP BY " + ReolIdFieldName + ")";
                ///'delete from tmp_reoladrgab where antall > 1;
                deleteQuery = "DELETE FROM TMP_REOLADRGAB WHERE ANTALL > 1";
                ///'drop table tmp_gab;
                await DropTable("TMP_GAB");
                ///'create table tmp_gab as (select adrnrgab, count(*) antall from tmp_reolgabadr group by adrnrgab);
                string insertQuery5 = "CREATE TABLE TMP_GAB AS (SELECT " + adressenr_pas + ", COUNT(*) ANTALL FROM TMP_REOLGABADR GROUP BY " + adressenr_pas + ")";
                ///'delete from tmp_gab where antall = 1;
                 deleteQuery2 = "DELETE FROM TMP_GAB WHERE ANTALL = 1";
                ///'insert into reol_adr (select reol_id from tmp_reolgabadr where reol_id in (select reol_id from tmp_reoladrgab) and adrnrgab in (select adrnrgab from tmp_gab));
                string insertQuery6 = ("INSERT INTO ADJUST_REOL (SELECT  " + ReolIdFieldName + " FROM TMP_REOLGABADR WHERE  " + ReolIdFieldName + " IN (SELECT  " + ReolIdFieldName +
                       " FROM TMP_REOLADRGAB) AND " + adressenr_pas + " IN (SELECT " + adressenr_pas + " FROM TMP_GAB))");
                 commit = ("COMMIT");
                await GrantSelect("ADJUST_REOL", KSPU_GDBUser);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the avisdekning.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAvisdekning()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                string tmp_avisdekning = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_avisdekning);
                string input_reoler = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reoler);
                string input_avisdekning = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_avisdekning);
                string tmp_reol_komm_faktor = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_komm_faktor);

                string insertQuery = "CREATE TABLE " + tmp_avisdekning + " AS SELECT a.REOL_ID, b.UTGAVE, b.EKSEMPLAR*c.ANDEL EKSEMPLAR,b.PROSENT*c.ANDEL PROSENT FROM " + input_reoler + " a INNER JOIN " + input_avisdekning + " b ON a.KOMMUNEID=b.KOMMUNEID" + " INNER JOIN " + tmp_reol_komm_faktor + " c ON c.REOL_ID=a.REOL_ID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the avis mapping table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> createAvisMappingTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Updates the avis mapping table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> updateAvisMappingTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Creates the post coverage table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> createPostCoverageTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Creates the newspaper table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> createNewspaperTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Inserts the data to post cover table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertDataToPostCoverTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Inserts the komm i ds to news paper table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertKommIDsToNewsPaperTable()
        {
            await Task.Run(() => { });
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


        /// <summary>
        /// Updates the news paper table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateNewsPaperTable()
        {
            await Task.Run(() => { });
            try
            {
                SetDBUserAsConnStr();
                int numNewspapers = await SelectCount("AVISFIELDMAPPING");
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


        /// <summary>
        /// Creates the segment temporary table 1.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSegmentTempTable_1()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_segment_1 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_1);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string tmp_reol_adr = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_adr);
                string insertQuery = "CREATE TABLE " + tmp_segment_1 + " AS SELECT REOL_ID,GKID,SUM(BEF) ANT_BEF FROM " + KSPU_DBUser + "." + tmp_reol_adr + " GROUP BY REOL_ID,GKID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the segment temporary table 2.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSegmentTempTable_2()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_segment_1 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_1);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string tmp_segment_2 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_2);
                string input_segmenter = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_segmenter);
                string insertQuery = "CREATE TABLE " + tmp_segment_2 + " AS SELECT a.REOL_ID, a.GKID,a.ANT_BEF,b.SEGMENT FROM " + tmp_segment_1 + " a," + KSPU_DBUser + "." + input_segmenter + " b" + " WHERE a.GKID=b.GKRETS_ID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the segment temporary table 3.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSegmentTempTable_3()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_segment_3 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_3);
                string tmp_segment_2 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_2);
                string insertQuery = "CREATE TABLE " + tmp_segment_3 + " AS SELECT REOL_ID, SEGMENT,SUM(ANT_BEF) SUM_BEF FROM " + tmp_segment_2 + " GROUP BY REOL_ID,SEGMENT";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        // Finds the segment value corresponding to each reol

        /// <summary>
        /// Creates the reol segment table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateReolSegmentTable()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_reol_segment = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_segment);
                string tmp_segment_3 = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_segment_3);
                string insertQuery = "CREATE TABLE " + tmp_reol_segment + " AS SELECT REOL_ID, MAX(SEGMENT) SEGMENT FROM " + "(SELECT REOL_ID,SEGMENT FROM " + tmp_segment_3 + " WHERE (REOL_ID,SUM_BEF) IN " + "(SELECT REOL_ID,MAX(SUM_BEF) SUM_BEF FROM " + tmp_segment_3 + " GROUP BY REOL_ID)) " + "GROUP BY REOL_ID";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }

        // Finds the segment value corresponding to each reol

        /// <summary>
        /// Creates the input reoler segment table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateInputReolerSegmentTable()
        {
            await Task.Run(() => { });
            try
            {
                SetGDBUserAsConnStr();
                string tmp_input_reoler_segment = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_input_reoler_segment);
                string KSPU_DBUser = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.KSPU_DBUser);
                string input_reoler = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.input_reoler);
                string tmp_reol_segment = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_reol_segment);
                string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);

                string insertQuery = "CREATE TABLE " + tmp_input_reoler_segment + " AS SELECT A.*,B.SEGMENT " + "FROM " + KSPU_DBUser + "." + input_reoler + " A left join " + tmp_reol_segment + " B " + "on (A." + ReolIdFieldName + " = B." + ReolIdFieldName + ")";
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the antall bef og reol tabell.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAntallBefOgReolTabell()
        {
            await Task.Run(() => { });
            SetGDBUserAsConnStr();
            string tmp_AntallBef_ReolTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_AntallBef_ReolTableName);
            string tmp_100mGrid_Reol = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_100mGrid_Reol);
            string ReolIdFieldName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.ReolIdFieldName);
            string insertQuery = "CREATE TABLE " + tmp_AntallBef_ReolTableName + " "
            + "as select " + ReolIdFieldName + ",sum(BEFOLKN) BEFOLKN,sum(HUSHOLDN) HUSHOLDN from " + tmp_100mGrid_Reol + " group by " + ReolIdFieldName;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            return true;
        }


        /// <summary>
        /// Deletes the antall bef reol tabell.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAntallBefReolTabell()
        {
            await Task.Run(() => { });
            SetGDBUserAsConnStr();
            string tmp_AntallBef_ReolTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.tmp_AntallBef_ReolTableName);
            try
            {
                string dropQuery = "DROP TABLE " + tmp_AntallBef_ReolTableName;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Creates the slutt rapport tabell.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateSluttRapportTabell()
        {
            await Task.Run(() => { });
            SetGDBUserAsConnStr();
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string insertQuery = "CREATE TABLE " + SluttRapportTableName + " (RAPPORTTEKST  VARCHAR2(2048) NOT NULL)";
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            return true;
        }


        /// <summary>
        /// Skrivs the til slutt rapport tabellen.
        /// </summary>
        /// <param name="piTekst">The pi tekst.</param>
        /// <returns></returns>
        public async Task<bool> SkrivTilSluttRapportTabellen(string piTekst)
        {
            await Task.Run(() => { });
            SetGDBUserAsConnStr();
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string insertQuery = "INSERT INTO " + SluttRapportTableName + " (RAPPORTTEKST) VALUES('" + piTekst + "')";
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(insertQuery, CommandType.Text, null);
            }
            return true;
        }


        /// <summary>
        /// Gets the rapport information.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRapportInfo()
        {
            await Task.Run(() => { });
            SetGDBUserAsConnStr();
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string selectQuery = "SELECT * FROM " + SluttRapportTableName;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(selectQuery, CommandType.Text, null);
            }
            return selectQuery;
        }


        /// <summary>
        /// Loggs the slutt rapport.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        public async Task<bool> LoggSluttRapport(string tablename)
        {
            await Task.Run(() => { });
            DataTable table;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            try
            {
                SetGDBUserAsConnStr();
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[0].Value = tablename;
                //OracleCommand cmd = new OracleCommand("select * from " + Config.SluttRapportTableName);
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    table = dbhelper.FillDataTable("kspu_db.LoggSluttRapport", CommandType.StoredProcedure, npgsqlParameters);
                }

                #region Commented code
                //string str = "";

                //foreach (DataRow row in table.Rows)
                //{
                //    //if (IsDBNull(row.ItemArray[0]))
                //    //{
                //    //    str = row.ItemArray[0];

                //    //    _logger.LogMessage(str);
                //    //}
                //} 
                #endregion

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return false;
            }
        }


        /// <summary>
        /// Sletts the slutt rapport tabell.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SlettSluttRapportTabell()
        {
            await Task.Run(() => { });
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string dropQuery = Convert.ToString(DropTable(SluttRapportTableName, true));
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(dropQuery, CommandType.Text, null);
            }
            //return DropTable(Config.SluttRapportTableName, true);
            return true;
        }



        public async Task<int> SelectCountAsDBUser(string piSQL)
        {
            await Task.Run(() => { });
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string dropQuery = Convert.ToString(DropTable(SluttRapportTableName, true));
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(dropQuery, CommandType.Text, null);
            }
            //return DropTable(Config.SluttRapportTableName, true);
            return 1;
        }


        public async Task<int> SelectCountAsGDBUser(string piSQL)
        {
            await Task.Run(() => { });
            string SluttRapportTableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SluttRapportTableName);
            string dropQuery = Convert.ToString(DropTable(SluttRapportTableName, true));
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery(dropQuery, CommandType.Text, null);
            }
            //return DropTable(Config.SluttRapportTableName, true);
            return 1;
        }

        /// <summary>
        /// Gets the GDBDB connection string.
        /// </summary>
        /// <value>
        /// The GDBDB connection string.
        /// </value>
        public string GDBDBConnectionString
        {
            get
            {
                return "Data Source=PUMAP;User Id=kspu_gdb;Password=kspu_gdb;Integrated Security= no";
            }
        }
        /// <summary>
        /// Gets the DBDB connection string.
        /// </summary>
        /// <value>
        /// The DBDB connection string.
        /// </value>
        public string DBDBConnectionString
        {
            get
            {
                return "Data Source=PUMA;User Id=kspu_db;Password=kspu_db;Integrated Security=no";
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Analyzes the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        public async Task<bool> AnalyzeTable(string tableName, bool gdbuser = false)
        {
            await Task.Run(() => { });

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            if (gdbuser)
                SetGDBUserAsConnStr();
            else
                SetDBUserAsConnStr();
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public async Task<DataTable> getData(string sql)
        {
            #region Vb Code
            //try
            //{
            //    return DABase.GetDataTable(new OracleCommand(sql));
            //}
            //catch (Exception exception)
            //{
            //    Logger.LogError(ex);
            //    return null/* TODO Change to default(_) if this is not a reference type */;
            //} 
            #endregion
            await Task.Run(() => { });

            return new DataTable();
        }

        /// <summary>
        /// Gets the alias table.
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> getAliasTable()
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetGDBUserAsConnStr();
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    table = dbhelper.FillDataTable("kspu_db.custom_getaliastable", CommandType.StoredProcedure, npgsqlParameters);
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


        /// <summary>
        /// Gets the dublikate reolkrets.
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetDublikateReolkrets()
        {
            await Task.Run(() => { });
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable table;
            try
            {
                SetGDBUserAsConnStr();

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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
        /// <summary>
        /// Sets the database connection string.
        /// </summary>
        /// <value>
        /// The database connection string.
        /// </value>
        protected string DBConnectionString
        {
            set
            {
                _DBConnectionString = value;
            }
        }

        /// <summary>
        /// Sets the GDB user as connection string.
        /// </summary>
        private void SetGDBUserAsConnStr()                                            // Connect as GDB
        {
            DBConnectionString = GDBDBConnectionString;

        }
        /// <summary>
        /// Sets the database user as connection string.
        /// </summary>
        private void SetDBUserAsConnStr()                                             // Connect as DB
        {
            DBConnectionString = DBDBConnectionString;
        }

        /// <summary>
        /// Selects the count.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        public async Task<int> SelectCount(string tablename)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Inside into SelectCount");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_gdb.custom_selectcount", CommandType.StoredProcedure, npgsqlParameters);
            }
            //return ExecuteScalar(new OracleCommand("Select Count(*) from " + tablename));

            if (result.ToString() == null) // | (result.ToString()) is DBNull)
                return 0;

            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from SelectCount");
            return System.Convert.ToInt32(result);
        }

        /// <summary>
        /// Gets the total field.
        /// </summary>
        /// <param name="totFieldString">The tot field string.</param>
        /// <param name="fieldname">The fieldname.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the total fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public async Task<string> getTotalFields(string fromTable, string prefix = "")
        {
            await Task.Run(() => { });

            SetGDBUserAsConnStr();
            DataTable table;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                table = dbhelper.FillDataTable("kspu_db.selectcount", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand("select column_name from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' " + "and column_name like 'TOT%' order by column_name");


            string str = "";
            foreach (DataRow row in table.Rows)
                str += prefix + row.ItemArray[0] + ",";
            return str.TrimEnd(',');
        }

        /// <summary>
        /// Gets the statistic fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="AdditionalIgnoreFields">The additional ignore fields.</param>
        /// <param name="additionSelect">The addition select.</param>
        /// <returns></returns>
        public async Task<string> GetStatisticFields(string fromTable, string prefix = "", string AdditionalIgnoreFields = "", string additionSelect = "")
        {
            await Task.Run(() => { });

            SetGDBUserAsConnStr();
            DataTable table;
            if (AdditionalIgnoreFields != "" & !AdditionalIgnoreFields.StartsWith(","))
                AdditionalIgnoreFields = "," + AdditionalIgnoreFields;
            //OracleCommand cmd = new OracleCommand("select column_name " + additionSelect + " from user_tab_columns where table_name = '" + fromTable.ToUpper() + "' " + "and column_name not like 'SEG%' AND column_name not " + "in (" + Config.IgnoreFields + AdditionalIgnoreFields + ") order by column_id");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromTable", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = fromTable;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
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
