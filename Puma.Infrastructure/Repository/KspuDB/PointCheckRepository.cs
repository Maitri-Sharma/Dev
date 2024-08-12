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
    /// <summary>
    /// PointCheckRepository
    /// </summary>
    public class PointCheckRepository : KsupDBGenericRepository<AddressPointsState>, IPointCheckRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PointCheckRepository> _logger;
        /// <summary>
        /// The connctionstring
        /// </summary>
        public readonly string Connctionstring;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCheckRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public PointCheckRepository(KspuDBContext context, ILogger<PointCheckRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        /// <summary>
        /// To get GAB Error
        /// </summary>
        /// <param name="adrnr">Kommune name to check in table</param>
        /// <returns>
        /// hits
        /// </returns>
        public async Task<DataTable> GetGABError(int adrnr)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetGABError");
            DataTable hits;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_adrnr1", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = adrnr;

            npgsqlParameters[1] = new NpgsqlParameter("p_adrnr1duplicate", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = adrnr;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {

                hits = dbhelper.FillDataTable("kspu_gdb.custom_getgaberror", CommandType.StoredProcedure, npgsqlParameters);

            }

            _logger.LogInformation("Number of row returned {0}", hits.Rows.Count);

            _logger.LogDebug("Exiting from GetGABError");
            return hits;
        }


        /// <summary>
        /// Get kommune no. from Postnummer table
        /// </summary>
        /// <param name="postNo">Post No.to fetch data from Postnummer table</param>
        /// <returns>
        /// Komm No.
        /// </returns>
        public async Task<string> GetKommNo(int postNo)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetKommNo");
            DataTable kommNoTable;
            string kommNo = "";
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_postno", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = Convert.ToString(postNo);

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {

                kommNoTable = dbhelper.FillDataTable("kspu_gdb.custom_getkommno", CommandType.StoredProcedure, npgsqlParameters);

            }
            if (kommNoTable != null && kommNoTable.Rows.Count > 0)
            {
                if (kommNoTable.Rows[0]["getkommno"] != DBNull.Value)
                    kommNo = System.Convert.ToString(kommNoTable.Rows[0]["getkommno"]);
            }

            _logger.LogInformation("Number of row returned {0}", kommNo);

            _logger.LogDebug("Exiting from GetKommNo");
            return kommNo;
        }

        /// <summary>
        /// To check correct Municipality No.
        /// </summary>
        /// <param name="adrNo">adr no to check in table</param>
        /// <param name="kommId">Kommune ID to check in table</param>
        /// <returns>
        /// hits
        /// </returns>
        public async Task<DataTable> HasGABCorrectMunicipalityNo(int adrNo, int kommId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for HasGABCorrectMunicipalityNo");
            DataTable hits;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_adrno", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = adrNo;

            npgsqlParameters[1] = new NpgsqlParameter("p_kommid", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = kommId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                hits = dbhelper.FillDataTable("kspu_gdb.hasGABcorrectmunicipalityno", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Number of row returned {0}", hits.Rows.Count);

            _logger.LogDebug("Exiting from HasGABCorrectMunicipalityNo");
            return hits;
        }
    }
}
