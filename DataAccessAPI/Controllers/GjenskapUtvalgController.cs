using DataAccessAPI.Extensions;
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GjenskapUtvalgController : ControllerBase
    {
        #region Variables
        private readonly ILogger<GjenskapUtvalgController> _logger;
        private readonly ConfigController configController;
        #endregion

        #region
        /// <summary>
        /// Initializes a new instance of the <see cref="GjenskapUtvalgController"/> class.
        /// </summary>
        /// <param name="logger">Instance of logger</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <exception cref="System.ArgumentNullException">logger</exception>
        public GjenskapUtvalgController(ILogger<GjenskapUtvalgController> logger, ILogger<ConfigController> loggerConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            configController = new ConfigController(loggerConfig);
        }
        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        public bool UtvalgRecreationProblemTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALGRECREATIONPROBLEM");
            }
        }

        public bool UtvalgRecreationRuntimeTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_RUNTIME");
            }
        }

        public bool UtvalgRecreationLogTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_LOG");
            }
        }

        public bool UtvalgRecreationWorstCasesTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_WORST_CASES");
            }
        }
        public bool UtvalgRecreationVerificationTableExists
        {
            get
            {
                return TableExistsforrecreateutvalg("KSPU_DB.UTVALG_RECREATION_VERIFICATION");
            }
        }

        [HttpGet("TableExistsforrecreateutvalg", Name = nameof(TableExistsforrecreateutvalg))]
        public bool TableExistsforrecreateutvalg(string tableName)
        {
            if (!tableName.Contains("."))
            {
                _logger.BeginScope("Inside into TableExistsforrecreateutvalg");
                object cnt;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;

                //npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Integer);
                //npgsqlParameters[0].Value = owner;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    cnt = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleCommand cmd = new OracleCommand(" SELECT COUNT(*) FROM ALL_TABLES WHERE TABLE_NAME = :tableName ");
                //AddParameterString(cmd, "tableName", tableName.ToUpper(), 30);
                //object cnt = ExecuteScalar(cmd);
                if (cnt is int)
                    return System.Convert.ToInt32(cnt) > 0;
                return false;
            }
            else
            {
                object cnt;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName.Split(".")[1];

                npgsqlParameters[1] = new NpgsqlParameter("p_schemaname", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = tableName.Split(".")[0];
                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    cnt = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists_owner", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleCommand cmd = new OracleCommand(" SELECT COUNT(*) FROM ALL_TABLES WHERE TABLE_NAME = :tableName AND OWNER = :owner ");
                //AddParameterString(cmd, "tableName", tableName.Split(".")(1).ToUpper(), 30);
                //AddParameterString(cmd, "owner", tableName.Split(".")(0).ToUpper(), 30);
                //object cnt = ExecuteScalar(cmd);
                if (cnt is decimal)
                    return System.Convert.ToInt32(cnt) > 0;
                return false;
            }
        }

        [HttpPost("CreateUtvalgRecreationProblemTable", Name = nameof(CreateUtvalgRecreationProblemTable))]
        public void CreateUtvalgRecreationProblemTable()
        {
            _logger.BeginScope("Inside into CreateUtvalgRecreationProblemTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];


            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationProblemTable", CommandType.StoredProcedure, npgsqlParameters);
            }
            GrantUtvalgRecreationProblemPermissions();
        }

        [HttpPost("ClearUtvalgRecreationProblemTable", Name = nameof(ClearUtvalgRecreationProblemTable))]
        public void ClearUtvalgRecreationProblemTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALGRECREATIONPROBLEM");
        }

        [HttpPost("CreateUtvalgRecreationRuntimeTable", Name = nameof(CreateUtvalgRecreationRuntimeTable))]
        public void CreateUtvalgRecreationRuntimeTable()
        {
            //StringBuilder sql = new StringBuilder();
            _logger.BeginScope("Inside into CreateUtvalgRecreationRuntimeTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];


            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationRuntimeTable", CommandType.StoredProcedure, npgsqlParameters);
            }
            GrantUtvalgRecreationRuntimePermissions();
        }

        [HttpPost("ClearUtvalgRecreationRuntimeTable", Name = nameof(ClearUtvalgRecreationRuntimeTable))]
        public void ClearUtvalgRecreationRuntimeTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_RUNTIME");
        }

        [HttpPost("CreateUtvalgRecreationLogTable", Name = nameof(CreateUtvalgRecreationLogTable))]
        public void CreateUtvalgRecreationLogTable()
        {
            //StringBuilder sql = new StringBuilder();

            _logger.BeginScope("Inside into CreateUtvalgRecreationLogTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];


            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationLogTable", CommandType.StoredProcedure, npgsqlParameters);
            }
            GrantUtvalgRecreationLogPermissions();
        }

        [HttpPost("ClearUtvalgRecerationLogTable", Name = nameof(ClearUtvalgRecerationLogTable))]
        public void ClearUtvalgRecerationLogTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_LOG");
        }

        [HttpPost("CreateUtvalgRecreationVerficationTable", Name = nameof(CreateUtvalgRecreationVerficationTable))]
        public void CreateUtvalgRecreationVerficationTable()
        {
            //StringBuilder sql = new StringBuilder();

            _logger.BeginScope("Inside into CreateUtvalgRecreationRuntimeTable");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationVerficationTable", CommandType.StoredProcedure, npgsqlParameters);
            }
            GrantUtvalgRecreationVerificationPermissions();
        }

        [HttpPost("CreateUtvalgRecreationWorstCases", Name = nameof(CreateUtvalgRecreationWorstCases))]
        public void CreateUtvalgRecreationWorstCases()
        {
            _logger.BeginScope("Inside into CreateUtvalgRecreationWorstCases");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.CreateUtvalgRecreationWorstCases", CommandType.StoredProcedure, npgsqlParameters);
            }
            GrantUtvalgRecreationWorstCasesPermissions();
        }

        [HttpPost("ClearUtvalgRecerationVerificationTable", Name = nameof(ClearUtvalgRecerationVerificationTable))]
        public void ClearUtvalgRecerationVerificationTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_VERIFICATION");
        }

        [HttpPost("ClearUtvalgRecerationWorstCasesTable", Name = nameof(ClearUtvalgRecerationWorstCasesTable))]
        public void ClearUtvalgRecerationWorstCasesTable()
        {
            DeleteAllRowsInTable("KSPU_DB.UTVALG_RECREATION_WORST_CASES");
        }

        [HttpGet("Get_Utvalg_ByID_SQL", Name = nameof(Get_Utvalg_ByID_SQL))]
        public string Get_Utvalg_ByID_SQL(Nullable<int> processId, string ID, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_Utvalg_ByID_SQL");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = ID;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
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
            return null;
        }

        [HttpPost("Get_Utvalg_Tilbud_Or_Bestilt_SQL", Name = nameof(Get_Utvalg_Tilbud_Or_Bestilt_SQL))]
        public string Get_Utvalg_Tilbud_Or_Bestilt_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_Utvalg_Tilbud_Or_Bestilt_SQL");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = maxNrOfUtvalg;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
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

            return sql.ToString();
        }

        [HttpPost("Get_UtvalgListID_Utvalg_In_List_SQL", Name = nameof(Get_UtvalgListID_Utvalg_In_List_SQL))]
        public string Get_UtvalgListID_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_UtvalgListID_Utvalg_In_List_SQL");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = maxNrOfUtvalg;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
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
            return null;
        }

        [HttpPost("Get_Utvalg_In_List_SQL", Name = nameof(Get_Utvalg_In_List_SQL))]
        public string Get_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_Utvalg_In_List_SQL");
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
            return null;
        }

        [HttpPost("Get_Andre_Utvalg_Recently_First_SQL", Name = nameof(Get_Andre_Utvalg_Recently_First_SQL))]
        public string Get_Andre_Utvalg_Recently_First_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_Andre_Utvalg_Recently_First_SQL");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_maxNrOfUtvalg", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = maxNrOfUtvalg;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
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
            return null;
        }

        /// <summary>
        ///     ''' Returns the sql-part which make sure the process only recreats the utvalgs it has locked
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        [HttpGet("GetLockingSQLPredicatePart", Name = nameof(GetLockingSQLPredicatePart))]
        public string GetLockingSQLPredicatePart(bool forLocking, Nullable<int> processId)
        {
            _logger.BeginScope("Inside into GetLockingSQLPredicatePart");
            //object result;
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //npgsqlParameters[0] = new NpgsqlParameter("p_processid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = processId;

            //using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.getlockingsqlpredicatepart", CommandType.StoredProcedure, npgsqlParameters);
            //}
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

            return result.ToString();
           // return Convert.ToString(result);
        }
        [HttpPost("GetSomeRowsInWorstCasesLogTable", Name = nameof(GetSomeRowsInWorstCasesLogTable))]
        public DataTable GetSomeRowsInWorstCasesLogTable(bool onlyOrder, DateTime fromDate, DateTime toDate, bool bWhereDistDate)
        {
            _logger.BeginScope("Inside into GetSomeRowsInWorstCasesLogTable");
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

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_db.GetSomeRowsInWorstCasesLogTable", CommandType.StoredProcedure, npgsqlParameters);
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
            return GetAllRowsInTableWhereAndOrderBy("KSPU_DB.UTVALG_RECREATION_WORST_CASES", string.Empty, " order by bestemorlisteid, morlisteid, utvalgid");
        }

        [HttpPost("DeleteFromOldUtvalgGeometryIfPresent", Name = nameof(DeleteFromOldUtvalgGeometryIfPresent))]
        public void DeleteFromOldUtvalgGeometryIfPresent(int utvalgId)
        {
            _logger.BeginScope("Inside into DeleteFromOldUtvalgGeometryIfPresent");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_db.deletefromoldutvalggeometryifpresent", CommandType.StoredProcedure, npgsqlParameters);
            }
            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from DeleteFromOldUtvalgGeometryIfPresent");

            // TODO: - Bruk ArcObjects
            // OracleCommand cmd = new OracleCommand("delete from kspu_gdb.oldutvalggeometry where utvalgid = :utvalgId");
            
        }

        [HttpPost("IsOldUtvalgGeometryPresent", Name = nameof(IsOldUtvalgGeometryPresent))]
        public bool IsOldUtvalgGeometryPresent(int utvalgId)
        {
            _logger.BeginScope("Inside into IsOldUtvalgGeometryPresent");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_gdb.isoldutvalggeometrypresent", CommandType.StoredProcedure, npgsqlParameters);
            }
            // TODO: - Bruk ArcObjects
            // OracleCommand cmd = new OracleCommand("Select utvalgid from kspu_gdb.oldutvalggeometry where utvalgid = :utvalgId");

            //return GetDataTable(cmd).Rows.Count > 0;
            return result.Rows.Count > 0;
        }

        [HttpPost("IsChangedBeforRecreate", Name = nameof(IsChangedBeforRecreate))]
        public bool IsChangedBeforRecreate(long piUtvalgID, DateTime LastModifiedDate)
        {
            _logger.BeginScope("Inside into IsChangedBeforRecreate");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_piutvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = piUtvalgID;

            npgsqlParameters[1] = new NpgsqlParameter("p_lastmodifieddate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = LastModifiedDate;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_db.ischangedbeforrecreate", CommandType.StoredProcedure, npgsqlParameters);
            }
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

        [HttpPost("MaxAntallUtvalgKandidaterForGjenskap", Name = nameof(MaxAntallUtvalgKandidaterForGjenskap))]
        public int MaxAntallUtvalgKandidaterForGjenskap(string sqlString)
        {
            _logger.BeginScope("Inside into MaxAntallUtvalgKandidaterForGjenskap");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_sqlstring", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = sqlString;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_db.MaxAntallUtvalgKandidaterForGjenskap", CommandType.StoredProcedure, npgsqlParameters);
            }
            //StringBuilder sql = new StringBuilder();

            //sql.AppendLine("SELECT count(*)");
            //sql.AppendLine("FROM KSPU_DB.Utvalg");
            //sql.AppendLine("WHERE NOT lower(ReolMapName) = lower(:CurrentReolMapName)");
            //// Fjerne alle kampanjer/arvingsutvalg
            //sql.AppendLine("AND (basedon=0 or basedon IS NULL) ");
            //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
            //sql.AppendLine("AND rownum <= " + Config.MaxNrOfUtvalgRecreationInOneProcess.ToString());
            //sql.AppendLine("AND antall > 0");
            //sql.AppendLine("AND UtvalgId NOT IN");
            //sql.AppendLine("(");
            //sql.AppendLine("SELECT UP.UtvalgId");
            //sql.AppendLine("FROM KSPU_DB.UTVALGRECREATIONPROBLEM UP");
            //sql.AppendLine(")");
            //sql.AppendLine(GetLockingSQLPredicatePart(true, 0));
            //sqlString = sql.ToString();

            //OracleCommand cmd = new OracleCommand(sql.ToString());
            //AddParameterString(cmd, "CurrentReolMapName", DAConfig.CurrentReolTableName, 1000);
            //AddParameterDate(cmd, "fromdate", DateTime.Today);
            //AddParameterDate(cmd, "todate", DateTime.Today.AddDays(Config.IgnoreNrOfDaysToDelivery));
            //object count = ExecuteScalar(cmd);
            //if (count == null | (count) is DBNull)
            //    return 0;
            //return System.Convert.ToInt32(count);
            return System.Convert.ToInt32(result);
        }

        [HttpPost("LockUtvalgRows", Name = nameof(LockUtvalgRows))]
        public void LockUtvalgRows(string insertSelect)
        {
            _logger.BeginScope("Inside into LockUtvalgRows");
            //StringBuilder sql = new StringBuilder();

            //sql.AppendLine("insert into kspu_db.utvalg_recreation_runtime");
            //sql.AppendLine("(UTVALGID, PROCESSID)");
            //sql.AppendLine(insertSelect);

            //OracleCommand cmd = new OracleCommand(sql.ToString().ToLower());

            //AddParameterString(cmd, "currentreolmapname", DAConfig.CurrentReolTableName, 1000);
            //if (sql.ToString().ToLower().IndexOf(":dato") > -1)
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
            //ExecuteNonQuery(cmd);

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_insertselect", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = insertSelect;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.lockutvalgrows", CommandType.StoredProcedure, npgsqlParameters);
            }

        }

        [HttpPost("RemoveUtvalgLocks", Name = nameof(RemoveUtvalgLocks))]
        public void RemoveUtvalgLocks(string utvalgIdSqlSelect)
        {
            _logger.BeginScope("Inside into RemoveUtvalgLocks");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgidsqlselect", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = utvalgIdSqlSelect;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteScalar<bool>("kspu_db.RemoveUtvalgLocks", CommandType.StoredProcedure, npgsqlParameters);
            }
            //StringBuilder sql = new StringBuilder();

            //sql.AppendLine("delete from kspu_db.utvalg_recreation_runtime");
            //sql.AppendLine("where utvalgId in");
            //sql.AppendLine("(");
            //sql.AppendLine(utvalgIdSqlSelect);
            //sql.AppendLine(")");

            //OracleCommand cmd = new OracleCommand(sql.ToString());

            //AddParameterString(cmd, "CurrentReolMapName", DAConfig.CurrentReolTableName, 1000);
            //ExecuteNonQuery(cmd);
        }

        [HttpPost("Get_Utvalg_In_List_ByID_SQL", Name = nameof(Get_Utvalg_In_List_ByID_SQL))]
        public string Get_Utvalg_In_List_ByID_SQL(Nullable<int> processId, string ID, bool forLocking)
        {
            _logger.BeginScope("Inside into Get_Utvalg_In_List_ByID_SQL");
            string selectPart = "SELECT UTVALGID";
            if (processId.HasValue)
                selectPart += ", " + processId.Value.ToString();

            //StringBuilder sql = new StringBuilder();
            //sql.AppendLine(selectPart);
            //sql.AppendLine("FROM KSPU_DB.Utvalg");
            //sql.AppendLine("WHERE NOT lower(ReolMapName) = lower(:CurrentReolMapName)");
            //// Fjerne alle kampanjer/arvingsutvalg
            //sql.AppendLine("AND (basedon=0 or basedon IS NULL) ");
            //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
            //// 'sql.AppendLine("AND UtvalgListID = " & ID)
            //sql.AppendLine("AND (UtvalgListID = " + ID + " OR UtvalgListID IN (SELECT UtvalgListID FROM KSPU_DB.UTVALGLIST WHERE PARENTUTVALGLISTID = " + ID + "))");
            //sql.AppendLine("AND antall > 0");
            //sql.AppendLine(GetLockingSQLPredicatePart(forLocking, processId));
            //sql.AppendLine("ORDER BY UTVALGID DESC");

            return selectPart.ToString();
        }

        [HttpPost("GetReols", Name = nameof(GetReols))]
        public DataTable GetReols(string sql)
        {
            _logger.BeginScope("Inside into GetReols");
            DataTable GetDataTable;
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
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            npgsqlParameters[0].Value = configController.CurrentReolTableName;

            if (sql.ToLower().IndexOf(":dato") > -1)
              npgsqlParameters[1] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
              npgsqlParameters[1].Value = DateTime.Today.AddMonths(0); //Config.NumberOfMonthsSinceDelivery

            if (sql.ToString().ToLower().IndexOf(":moddato") > -1)
            npgsqlParameters[2] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = DateTime.Today.AddMonths(0); //Config.NumberOfMonthsSinceModified

            if (sql.ToString().ToLower().IndexOf(":bruker") > -1)
            npgsqlParameters[3] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = "SystemUser";

            if (sql.ToString().ToLower().IndexOf(":fromdate") > -1 & sql.ToString().ToLower().IndexOf(":todate") > -1)
            {
                npgsqlParameters[4] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[4].Value = DateTime.Today;

                npgsqlParameters[5] = new NpgsqlParameter("p_tableName", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[5].Value = DateTime.Today.AddDays(3);
            }

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                GetDataTable = dbhelper.FillDataTable("kspu_db.getreols", CommandType.StoredProcedure, npgsqlParameters);
            }
            return GetDataTable;

        }
        //public bool MustStartNewRecreationProcess
        //{
        //    get
        //    {
        //        return NumberOfUtvalgToRecreate() > 0;
        //    }
        //}


        public int NumberOfUtvalgToRecreate
        {
            get
            {
                //StringBuilder sql = new StringBuilder();

                //sql.AppendLine("select count(*)");
                //sql.AppendLine("from kspu_db.utvalg");
                //sql.AppendLine("where not lower(ReolMapName) = lower(:CurrentReolMapName)");
                //// Fjerne alle kampanjer/arvingsutvalg
                //sql.AppendLine("and (basedon=0 or basedon is null) ");
                //sql.AppendLine("AND ((distribusjonsdato NOT BETWEEN :fromdate AND :todate) OR distribusjonsdato IS NULL)");
                //if (Config.OnlyRecreateTilbudAndOrdre)
                //    sql.AppendLine("and (lower(ordretype) = lower('T') or lower(ordretype) = lower('O'))");
                //sql.AppendLine("and antall > 0");
                //// Fjern alle ordre/tilbud hvor innleveringsdato > :dato
                //sql.AppendLine("and not utvalgId in");
                //sql.AppendLine("(");
                //sql.AppendLine("    select utvalgId");
                //sql.AppendLine("    from kspu_db.utvalg");
                //sql.AppendLine("    where (lower(ordretype) = lower('T') or lower(ordretype) = lower('O'))");
                //sql.AppendLine("    and innleveringsdato < :dato");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som er opptatt av andre prosesser
                //sql.AppendLine("and not utvalgId in");
                //sql.AppendLine("(");
                //sql.AppendLine("    select utvalgId");
                //sql.AppendLine("    from kspu_db.utvalg_recreation_runtime");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som har feilet og logget
                //sql.AppendLine("and not utvalgid in");
                //sql.AppendLine("(");
                //sql.AppendLine("    select utvalgId");
                //sql.AppendLine("    from kspu_db.utvalgrecreationproblem");
                //sql.AppendLine(")");
                //// Fjern alle utvalg som er eldre en angitt dato
                //sql.AppendLine("and utvalgid in");
                //sql.AppendLine("(");
                //sql.AppendLine("    select distinct utvalgid");
                //sql.AppendLine("    from kspu_db.utvalgmodification where modificationdate > :moddato and upper(userid) <> UPPER(:bruker)");
                //sql.AppendLine(")");

                //OracleCommand cmd = new OracleCommand(sql.ToString());
                //AddParameterDate(cmd, "dato", DateTime.Today.AddMonths(Config.NumberOfMonthsSinceDelivery));
                //AddParameterString(cmd, "CurrentReolMapName", DAConfig.CurrentReolTableName, 1000);
                //AddParameterDate(cmd, "moddato", DateTime.Today.AddMonths(Config.NumberOfMonthsSinceModified));
                //AddParameterString(cmd, "bruker", Config.SystemUserName, 50);
                //AddParameterDate(cmd, "fromdate", DateTime.Today);
                //AddParameterDate(cmd, "todate", DateTime.Today.AddDays(Config.IgnoreNrOfDaysToDelivery));

                //return System.Convert.ToInt32(ExecuteScalar(cmd));
                return 1;
            }
        }

        #endregion

        #region Private Methods
        private void GrantUtvalgRecreationLogPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_LOG");
        }

        private DataTable GetAllRowsInTableWhereAndOrderBy(string tableName, string strWhere, string orderBy)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_strwhere", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = strWhere;

            npgsqlParameters[2] = new NpgsqlParameter("p_orderby", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = orderBy;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_db.GetAllRowsInTableWhereAndOrderBy", CommandType.StoredProcedure, npgsqlParameters);
            }
            //return GetDataTable(new OracleCommand("select * from " + tableName + strWhere + orderBy));
            return result;
        }

        //private DataTable GetAllRowsInTable(string tableName)
        //{
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
        //    DataTable result;

        //    npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tableName;

        //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //    {
        //        result = dbhelper.FillDataTable("kspu_db.GetAllRowsInTable", CommandType.StoredProcedure, npgsqlParameters);
        //    }

        //    //return GetDataTable(new OracleCommand("select * from " + tableName));
        //    return result;
        //}

        private DataTable GetXnrOfRowsFromTable(string tableName, int nrOfRows)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_nrofrows", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = nrOfRows;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_db.GetXnrOfRowsFromTable", CommandType.StoredProcedure, npgsqlParameters);
            }

            // return GetDataTable(new OracleCommand("select * from " + tableName + " where rownum <= " + nrOfRows.ToString()));
            return result;
        }

        private void DeleteFromTableBasedOnUtvalgId(string tableName, DataRow dr)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            DataTable result;

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            npgsqlParameters[1] = new NpgsqlParameter("p_dr", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = dr;

            using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.FillDataTable("kspu_db.GetXnrOfRowsFromTable", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand("delete from " + tableName + " where UtvalgId = " + System.Convert.ToInt32(dr("UtvalgId")).ToString());
            //ExecuteNonQuery(cmd);
            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from DeleteCampaignList");
        }



        private void GrantUtvalgRecreationVerificationPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_VERIFICATION");
        }

        private void GrantUtvalgRecreationWorstCasesPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_WORST_CASES");
        }


        private void DeleteAllRowsInTable(string tableName)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = tableName;
            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.deleteallrowsintable", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand("delete from " + tableName);
            //ExecuteNonQuery(cmd);
        }

        private void GrantUtvalgRecreationRuntimePermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALG_RECREATION_RUNTIME");
        }



        private bool FailureOnUtvalgRegistred(int utvalgid)
        {
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object count;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                count = dbhelper.ExecuteNonQuery("kspu_db.FailureOnUtvalgRegistred", CommandType.StoredProcedure, npgsqlParameters);
            }
            //StringBuilder sql = new StringBuilder();

            //sql.AppendLine("select count(UTVALGID) from KSPU_DB.UTVALGRECREATIONPROBLEM WHERE UTVALGID = :UTVALGID");
            //OracleCommand cmd = new OracleCommand(sql.ToString());
            //AddParameterInteger(cmd, "UTVALGID", utvalgid);

            //object count = ExecuteScalar(cmd);
            if (count == null | (count) is DBNull)
                return false;
            return System.Convert.ToInt32(count) > 0;
        }

        private void UpdateRecreationFailure(int utvalgId, string errorText)
        {
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

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                count = dbhelper.ExecuteNonQuery("kspu_db.UpdateRecreationFailure", CommandType.StoredProcedure, npgsqlParameters);
            }
        }
        private void GrantUtvalgRecreationProblemPermissions()
        {
            GrantStandardPermissionsOnTable("KSPU_DB.UTVALGRECREATIONPROBLEM");
        }

        private void GrantStandardPermissionsOnTable(string tableName)
        {
            //ExecuteNonQuery(new OracleCommand("grant select on " + tableName + " to viewer"));
            //ExecuteNonQuery(new OracleCommand("grant select on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant update on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant insert on " + tableName + " to editor"));
            //ExecuteNonQuery(new OracleCommand("grant delete on " + tableName + " to editor"));

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName;

            using (PGDBHelper dbhelper = new Helper.PGDBHelper(ConfigSettings.GetConnectionString))
            {
                result = dbhelper.ExecuteNonQuery("kspu_db.grantstandardpermissionsontable", CommandType.StoredProcedure, npgsqlParameters);
            }

        }

        #endregion


    }
}
