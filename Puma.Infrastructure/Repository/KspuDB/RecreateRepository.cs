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

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class RecreateRepository : KsupDBGenericRepository<AddressPointsState>, IRecreateRepository
    {
        private readonly ILogger<RecreateRepository> _logger;
        public readonly string Connctionstring;

        public RecreateRepository(KspuDBContext context, ILogger<RecreateRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods
        /// <summary>
        ///  Create Table
        ///  </summary>
        /// <param name="tablename"></param>
        /// <param name="basedOn"></param>
        public async Task CreateTable(string tablename, string basedOn = "")
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for CreateTable");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            npgsqlParameters = null;
            object result = null;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteNonQuery("CREATE TABLE " + tablename + " " + basedOn + " ", CommandType.Text, npgsqlParameters);
            }

            _logger.LogInformation("Result is: {0}", result);

            _logger.LogDebug("Exiting from CreateTable");
        }

        /// <summary>
        ///  Drop Table
        ///  </summary>
        /// <param name="tablename"></param>
        public async Task<bool> DropTable(string tablename)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for DropTable");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                npgsqlParameters = null;
                object result = null;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery(" DROP TABLE " + tablename + " ", CommandType.Text, npgsqlParameters);
                }
                _logger.LogInformation("Result is: {0}", result);

                _logger.LogDebug("Exiting from DropTable");
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("ORA-00942"))
                    // Logger.LogWarning(tableName & " does not exist")
                    return true;
                else
                {
                    _logger.LogError(exception.ToString());
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
