using Npgsql;
using System.Data;
using System.Threading.Tasks;
using Puma.DataLayer.DatabaseModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Puma.Infrastructure.Interface.KsupDB;

namespace Puma.Infrastructure.Repository.KspuDB
{
    /// <summary>
    /// PostsoneReolMapperRepository
    /// </summary>
    public class PostsoneReolMapperRepository : KsupDBGenericRepository<AddressPointsState>, IPostsoneReolMapperRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PostsoneReolMapperRepository> _logger;
        /// <summary>
        /// The connctionstring
        /// </summary>
        public readonly string Connctionstring;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsoneReolMapperRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public PostsoneReolMapperRepository(KspuDBContext context, ILogger<PostsoneReolMapperRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        /// <summary>
        /// Gets the reol identifier postnummer mapping.
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetReolIdPostnummerMapping()
        {
            await Task.Run(() => { });
            DataTable _reolIdPostnummerMapping = null;
            _logger.LogDebug("Preparing the data for ReolIdPostnummerMapping");
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                _reolIdPostnummerMapping = dbhelper.FillDataTable("kspu_gdb.custom_reolidpostnummermapping", CommandType.StoredProcedure, null);
            }

            _logger.LogInformation("Number of row returned: ", _reolIdPostnummerMapping.Rows.Count);

            _logger.LogDebug("Exiting from ReolIdPostnummerMapping");

            return _reolIdPostnummerMapping;

        }


        /// <summary>
        /// Update Posten Reol Mapping Table
        /// </summary>
        public async Task UpdatePostsoneReolMappingTable()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdatePostsoneReolMappingTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Direction = ParameterDirection.Output;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.ExecuteNonQuery("kspu_gdb.updatepostsonereolmappingtable", CommandType.StoredProcedure, npgsqlParameters);
            }
            result = Convert.ToInt32(npgsqlParameters[1].Value);
            _logger.LogInformation(string.Format("Result is: {0} ", result));

            _logger.LogDebug("Exiting from UpdatePostsoneReolMappingTable");
        }
    }
}
