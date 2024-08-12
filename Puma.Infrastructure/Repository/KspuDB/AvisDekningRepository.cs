using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface;
using Puma.Infrastructure.SQLDbHelper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.DataLayer.DatabaseModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System.Text;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class AvisDekningRepository : KsupDBGenericRepository<AddressPointsState>, IAvisDekningRepository
    {
        private readonly ILogger<AvisDekningRepository> _logger;
        public readonly string Connctionstring;

        public AvisDekningRepository(KspuDBContext context, ILogger<AvisDekningRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }
        #region Public Methods
        /// <summary>
        /// To check if Avis exists
        /// </summary>
        /// <param name="utgave">utgave to fetch data from AvisDekning table</param>
        /// <returns>True or false</returns>
        public async Task<bool> AvisExists(string utgave)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for AvisExists");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utgave", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = utgave;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_db.avisexists", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Is AvisExists: ", result);

            _logger.LogDebug("Exiting from AvisExists");

            if (result == null | (result) is DBNull)
                return false;

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// To fetch data from AvisDekning table
        /// </summary>
        /// <returns>List of Utgave</returns>
        public async  Task<List<string>> GetAllUtgaver()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllUtgaver");
            DataTable aviser = null;
            //TODO: For cache first will check data here if null then will call mediator and will return result

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                aviser = dbhelper.FillDataTable("kspu_db.getallutgaver", CommandType.StoredProcedure, null);
            }

            List<string> result = new List<string>();
            foreach (DataRow dataRow in aviser.Rows)
                result.Add(Convert.ToString(dataRow["r_utgave"]));


            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetAllUtgaver");


            return new List<string>(result);
        }

        /// <summary>
        /// To fetch data from AVISFIELDMAPPING table
        /// </summary>
        /// <returns>Avis List</returns>
        public async Task<List<Avis>> GetPaperList()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetPaperList");
            DataTable papers = null;


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                papers = dbhelper.FillDataTable("kspu_db.getpaperlist", CommandType.StoredProcedure, null);
            }
            List<Avis> result = new List<Avis>();
            foreach (DataRow row in papers.Rows)
                result.Add(new Avis(row["r_utgave"].ToString(), row["r_feltnavn"].ToString()));


            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetPaperList");

            return result;
        }

        /// <summary>
        /// To fetch Coverage List
        /// </summary>
        /// <returns>Coverage List</returns>
        public async Task<DataTable> GetCoverageList(string[] feltnavn)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetCoverageList");
            DataTable coverage = null;
            if (feltnavn != null && feltnavn.Length > 0)
            {
                StringBuilder feltnavnSql = new StringBuilder();
                foreach (string item in feltnavn)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        feltnavnSql.Append(",nvl(adk.");
                        feltnavnSql.Append(item);
                        feltnavnSql.Append(",0) as ");
                        feltnavnSql.Append(item);
                    }
                }

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    coverage = dbhelper.FillDataTable((string.Format("kspu_db.getcoveragelist", feltnavnSql.ToString())), CommandType.StoredProcedure, null);
                }
            }

            _logger.LogInformation("Number of row returned {0}", coverage.Rows.Count);

            _logger.LogDebug("Exiting from GetCoverageList");
            return coverage;
        }

        /// <summary>
        /// To fetch data from AvisDekning table on the basis of Reol ID
        /// </summary>
        /// <returns>Avis Dekning Collection</returns>
        public async Task<AvisDekningCollection> GetAvisDekning(long reolId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAvisDekning");
            DataTable avisDekning = null;
            AvisDekningCollection result = new AvisDekningCollection();

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            npgsqlParameters[0].Value = Convert.ToString(reolId);

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                avisDekning = dbhelper.FillDataTable("kspu_db.getavisdekning", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow dataRow in avisDekning.Rows)
            {
                result.Add(GetAvisDekningFromDataRow(dataRow));
            }

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAvisDekning");

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To fetch data from AvisDekning table
        /// </summary>
        /// <returns>Avis List</returns>
        private AvisDekning GetAvisDekningFromDataRow(DataRow dataRow)
        {
            AvisDekning avisDekning = new AvisDekning();
            avisDekning.ReolId = long.Parse(dataRow["r_reolid"].ToString());
            avisDekning.Utgave = dataRow["r_utgave"].ToString();
            // r.Eksemplar = commonRepository.GetOracleNumberFromRow(row, "EKSEMPLAR");
            //r.Prosent = commonRepository.GetOracleNumberFromRow(row, "PROSENT");
            return avisDekning;
        }

        #endregion

    }
}
