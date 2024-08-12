using Npgsql;
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
using Puma.Shared;

namespace Puma.Infrastructure.Repository
{
    /// <summary>
    /// Repository for Address Point State
    /// </summary>
    public class AddressPointStateRepository : KsupDBGenericRepository<AddressPointsState>, IAddressPointStateRepository
    {
        private readonly ILogger<AddressPointStateRepository> _logger;
        public readonly string Connctionstring;

        public AddressPointStateRepository(KspuDBContext context, ILogger<AddressPointStateRepository> logger) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
        }

        #region Public Methods
        /// <summary>
        /// Save the address point. If exists then it iwll update otherwise adding new address point
        /// </summary>
        /// <param name="userId">Instance of user ID</param>
        /// <param name="addressPointList"> List of address point to be save</param>
        /// <returns>Number of row affecteds</returns>

        public async Task<int> SaveAdressPointsAPI(string userId, AddressPointList addressPointList)
        {
            try
            {
                _logger.LogDebug("Preparing the data for SaveAdressPointsAPI");
                await Task.Run(() => { });
                Exception exception = null;

                int result = 0;

                if (userId == null)
                {
                    exception = new Exception("userId can not be null for function SaveAdressPoints!");
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }
                if (addressPointList == null)
                {
                    exception = new Exception("addresspoints can not be null for function SaveAdressPoints!");
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }

                foreach (AddressPoint addressPoint in addressPointList)
                {
                    #region Parameter assignement
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];
                    npgsqlParameters[0] = new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[0].Value = (addressPoint.Name.Length < 255 ? addressPoint.Name : addressPoint.Name.Substring(0, 255));

                    npgsqlParameters[1] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[1].Value = userId;

                    npgsqlParameters[2] = new NpgsqlParameter("p_x", NpgsqlTypes.NpgsqlDbType.Double);
                    npgsqlParameters[2].Value = addressPoint.X;

                    npgsqlParameters[3] = new NpgsqlParameter("p_y", NpgsqlTypes.NpgsqlDbType.Double);
                    npgsqlParameters[3].Value = addressPoint.Y;

                    npgsqlParameters[4] = new NpgsqlParameter("p_timecreated", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                    npgsqlParameters[4].Value = DateTime.Now;

                    npgsqlParameters[5] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[5].Direction = ParameterDirection.Output;


                    #endregion

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        dbhelper.ExecuteNonQuery("kspu_db.InertUpdateAddressPointsState", CommandType.StoredProcedure, npgsqlParameters);

                        result = Convert.ToInt32(npgsqlParameters[5].Value);
                        _logger.LogInformation(string.Format("Number of row affected {0} for Userid {1}", result, userId));
                    }
                }

                _logger.LogDebug("Exiting from SaveAdressPointsAPI");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveAdressPointsAPI:", exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="userId">User ID to fetch list of address related to passed user</param>
        /// <returns>List of Address points</returns>

        public async Task<List<AddressPoint>> GetAddressPointsState(string userId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetAddressPointsState");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<AddressPoint> result = new List<AddressPoint>();

            npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = userId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteReader("kspu_db.getaddresspointsbyuserid", CommandType.StoredProcedure, (reader) => PopulateAddressPointList(reader, result), npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAddressPointsState:", exception.Message);
                throw;
            }
            _logger.LogInformation("Number of row returned {0}", result.Count);

            _logger.LogDebug("Exiting from GetAddressPointsState");

            return result;
        }


       
        #endregion 

        #region private Methods

        /// <summary>
        /// Populates a list of AddressPoint from the dataReader.
        /// </summary>
        /// <param name="reader">An instance of IDataReader</param>
        /// <param name="addressPointsList">The list of Address points to populate.</param>
        /// <returns>True if found records, false otherwise.</returns>
        private static bool PopulateAddressPointList(IDataReader reader, List<AddressPoint> addressPointsList)
        {
            var recordsFound = false;
            while (reader.Read())
            {
                addressPointsList.Add(
                    new AddressPoint()
                    {
                        Name = reader.GetValueOrDefault<string>("r_name"),
                        X = reader.GetValueOrDefault<double>("r_x"),
                        Y = reader.GetValueOrDefault<double>("r_y")
                    }
                );
                recordsFound = true;
            }
            return recordsFound;
        }

        #endregion
    }
}
