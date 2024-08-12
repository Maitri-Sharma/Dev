using Npgsql;
using System.Data;
using System.Threading.Tasks;
using Puma.DataLayer.DatabaseModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class ReolerKommuneRepository : KsupDBGenericRepository<AddressPointsState>, IReolerKommuneRepository
    {
        private readonly ILogger<ReolerKommuneRepository> _logger;
        public readonly string Connctionstring;

        public ReolerKommuneRepository(KspuDBContext context, ILogger<ReolerKommuneRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods

        /// <summary>
        /// To fetch data from Reoler Kommune table on the basis of Reol ID and Kommune ID
        /// </summary>
        /// <param name="ReolId">Reol ID to check in table</param>
        /// <param name="KommuneId">Kommune ID to check in table</param>
        /// <returns>ReolerKommune data</returns>
        public async Task<ReolerKommune> GetReolerKommune(long ReolId, string KommuneId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetReolerKommune");
            DataTable reolerKommune;
            Exception exception = null;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            ReolerKommuneKey reolerKommuneKey = new ReolerKommuneKey(ReolId, KommuneId);
            ReolerKommune result;


            npgsqlParameters[0] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = ReolId;

            npgsqlParameters[1] = new NpgsqlParameter("p_kommuneid", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            npgsqlParameters[1].Value = KommuneId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                reolerKommune = dbhelper.FillDataTable("kspu_gdb.custom_getreolerkommune", CommandType.StoredProcedure, npgsqlParameters);
            }

            if (reolerKommune?.Rows?.Count > 1)
            {
                exception = new Exception("Fant flere ReolerKommune i databasen med Reolid " + ReolId + " og KommuneId " + KommuneId + ".");
                throw exception;
            }
            else if (reolerKommune?.Rows?.Count == 0)
            {
                exception = new Exception("Fant ikke ReolerKommune hvor med Reolid " + ReolId + " og KommuneId " + KommuneId + ".");
                throw exception;
            }

            result = GetReolerKommuneFromDataRow(reolerKommune.Rows[0]);

            _logger.LogInformation("Number of row returned {0}", reolerKommune.Rows.Count);

            _logger.LogDebug("Exiting from GetReolerKommune");
            return result;
        }

        /// <summary>
        /// To fetch data from Reoler Kommune table on the basis of Kommune ID
        /// </summary>
        /// <param name="KommuneId">Kommune ID to fetch data from Reoler Kommune table</param>
        /// <returns>ReolerKommune data</returns>
        public async Task<ReolerKommuneCollection> GetReolerKommuneByKommuneId(string KommuneId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetReolerKommuneByKommuneId");
            DataTable reolerKommune;
            Exception exception = null;
            ReolerKommuneCollection result = new ReolerKommuneCollection();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];


            npgsqlParameters[0] = new NpgsqlParameter("p_kommuneid", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            npgsqlParameters[0].Value = KommuneId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                reolerKommune = dbhelper.FillDataTable("kspu_gdb.custom_getreolerkommunebykommuneid", CommandType.StoredProcedure, npgsqlParameters);
            }

            if (reolerKommune?.Rows?.Count == 0)
            {
                exception = new Exception("Fant ikke ReolerKommune i kommune med Kommuneid " + KommuneId + " i databasen.");
                throw exception;
            }

            foreach (DataRow dataRow in reolerKommune.Rows)
            {
                result.Add(GetReolerKommuneFromDataRow(dataRow));
            }



            _logger.LogInformation("Number of row returned {0}", reolerKommune.Rows.Count);

            _logger.LogDebug("Exiting from GetReolerKommuneByKommuneId");
            return result;
        }

        /// <summary>
        /// To fetch data from Reoler Kommune table
        /// </summary>
        /// <returns>ReolerKommune data</returns>
        public async Task<ReolerKommuneCollection> GetAllReolerKommune()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAllReolerKommune");
            DataTable reolerKommune;
            ReolerKommuneCollection result = new ReolerKommuneCollection();
            ReolerKommuneKey reolerKommuneKey;



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                reolerKommune = dbhelper.FillDataTable("kspu_gdb.custom_getallreolerkommune", CommandType.StoredProcedure, null);
            }

            foreach (DataRow dataRow in reolerKommune.Rows)
            {
                reolerKommuneKey = new ReolerKommuneKey((long)dataRow["r_reolid"], dataRow["r_kommuneid"].ToString());

                result.Add(GetReolerKommuneFromDataRow(dataRow));
            }

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAllReolerKommune");
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To Populate data in Reoler Kommune Object
        /// </summary>
        /// <param name="row">Data row thay has reoler kommune data</param>
        /// <returns>ReolerKommune data</returns>
        private static ReolerKommune GetReolerKommuneFromDataRow(DataRow row)
        {
            ReolerKommune r = new ReolerKommune();
            r.KommuneId = row["r_kommuneid"].ToString();
            r.ReolId = (long)row["r_reolid"];
            r.HH = (int)row["r_HH"];
            r.ER = (int)row["r_ER"];
            r.GB = (int)row["r_GB"];
            r.VH = (int)row["r_VH"];
            r.HH_RES = (int)row["r_HH_RES"];
            r.ER_RES = (int)row["r_ER_RES"];
            r.GB_RES = (int)row["r_GB_RES"];
            return r;
        }
        #endregion
    }
}
