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

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class ArbeidsListStateRepository : KsupDBGenericRepository<AddressPointsState>, IArbeidsListStateRepository
    {
        private readonly ILogger<ArbeidsListStateRepository> _logger;
        public readonly string Connctionstring;

        public ArbeidsListStateRepository(KspuDBContext context, ILogger<ArbeidsListStateRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods
        /// <summary>
        /// SAve the address point. If exists then it iwll update otherwise adding new address point
        /// </summary>
        /// <param name="arbeidsListState"> List of arbeidsListState to be saved</param>
        /// <returns>Number of row affecteds</returns>
        public async Task<int> SaveArbeidsListState(List<ArbeidsListEntryState> arbeidsListState)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveArbeidsListState");

            
            
            int result = 0;
            foreach (ArbeidsListEntryState entry in arbeidsListState)
            {
                #region Parameter assignement
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];
                npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[0].Value = entry.Id;

                npgsqlParameters[1] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[1].Value = entry.Type;

                npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Double, 50);
                npgsqlParameters[2].Value = entry.UserId;

                npgsqlParameters[3] = new NpgsqlParameter("p_active", NpgsqlTypes.NpgsqlDbType.Boolean);
                npgsqlParameters[3].Value = entry.Active;

                npgsqlParameters[4] = new NpgsqlParameter("p_timecreated", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                npgsqlParameters[4].Value = DateTime.Now;

                npgsqlParameters[5] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[5].Direction = ParameterDirection.Output;


                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery("kspu_db.insertupdatearbeidsliststate", CommandType.StoredProcedure, npgsqlParameters);

                    result = Convert.ToInt32(npgsqlParameters[5].Value);
                    _logger.LogInformation(string.Format("Result is: {0}", result));
                }
            }


            _logger.LogDebug("Exiting from SaveArbeidsListState");
            return result;
        }

        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="userId">User ID to fetch list of address related to passed user</param>
        /// <returns>List of Address points</returns>
        public async Task<List<ArbeidsListEntryState>> GetArbeidsListState(string userId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetArbeidsListState");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<ArbeidsListEntryState> result = new List<ArbeidsListEntryState>();
            DataTable dataTable;

            npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = userId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dataTable = dbhelper.FillDataTable("kspu_db.getarbeidsliststate", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow dataRow in dataTable.Rows)
                result.Add(CreateArbeidsListStateEntryFromRow(dataRow));

            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetArbeidsListState");

            return result;
        }

        #endregion

        #region private Methods

        /// <summary>
        /// Check the active entry in arbeidsListState
        /// </summary>
        /// <param name="arbeidsListState">List of arbeidsListState to check</param>
        /// <returns>True if found records, false otherwise.</returns>
        public async Task<bool> IsMaximumOneEntryActive(List<ArbeidsListEntryState> arbeidsListState)
        {
            await Task.Run(() => { });
            if (arbeidsListState.Where(c => c.Active == true).Any())
                return true;
            return false;
        }

        /// <summary>
        /// Populates a list of Address List entry state from the datarow.
        /// </summary>
        /// <param name="dataRow">An instance of IDataReader</param>
        /// <returns>ArbeidsListEntryState data</returns>
        private ArbeidsListEntryState CreateArbeidsListStateEntryFromRow(DataRow dataRow)
        {
            if (dataRow == null)
                throw new Exception("row can not be null for CreateArbeidsListStateEntryFromRow!");

            int id = Convert.ToInt32(dataRow["r_id"]);
            PumaEnum.ListType type = ArbeidsListEntryState.GetTypeValueFromChar(Convert.ToChar(dataRow["r_type"]));
            string userId = Convert.ToString(dataRow["r_userid"]);
            bool active = ArbeidsListEntryState.GetActiveValueFromChar(Convert.ToChar(dataRow["r_active"]));

            return new ArbeidsListEntryState(id, type, userId, active);
        }
        #endregion
    }
}
