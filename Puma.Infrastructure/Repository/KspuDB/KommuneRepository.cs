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
    public class KommuneRepository : KsupDBGenericRepository<AddressPointsState>, IKommuneRepository
    {
        private readonly ILogger<KommuneRepository> _logger;
        private readonly IConfigurationRepository _configurationRepository;
        public readonly string Connctionstring;
        public const string ConfigKey = "CurrentReolTableName";
        public KommuneRepository(KspuDBContext context, ILogger<KommuneRepository> logger, IConfigurationRepository configurationRepository) : base(context)
        {
            _logger = logger;
            _configurationRepository = configurationRepository;
            Connctionstring = _context.Database.GetConnectionString();
        }


        #region Public Methods
        /// <summary>
        /// To get all Kommune data
        /// </summary>
        /// <returns>KommuneCollection</returns>

        public async Task<KommuneCollection> GetAllKommunes()
        {
            
            _logger.LogDebug("Preparing the data for GetAllKommunes");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            KommuneCollection result = new KommuneCollection();
            string table = await _configurationRepository.GetConfigValue(ConfigKey);
            DataTable kommuner;


            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = table;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                kommuner = dbhelper.FillDataTable("kspu_gdb.custom_getallkommunes", CommandType.StoredProcedure, npgsqlParameters);

                foreach (DataRow row in kommuner.Rows)
                    result.Add(GetKommuneFromDataRow(row));

            }

            _logger.LogDebug("Exiting from GetAllKommunes");

            return result;
        }

        /// <summary>
        /// Check if Kommune Exists
        /// </summary>
        /// <param name="Kommunenavn">Kommune name to check in table</param>
        /// <returns>Kommune Exist or not</returns>
        public async Task<bool> KommuneExists(string Kommunenavn)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for KommuneExists");

            string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);


            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_kommunenavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = Kommunenavn;

            npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = currentTable;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                result = dbhelper.ExecuteScalar<int>("kspu_gdb.kommuneexists", CommandType.StoredProcedure, npgsqlParameters);
            }

            _logger.LogInformation("Is KommuneExists: {0}", result);

            _logger.LogDebug("Exiting from KommuneExists");

            return (Convert.ToInt32(result) > 0);
        }

        /// <summary>
        /// To fetch Kommune ID from Current Reol Table
        /// </summary>
        /// <param name="Kommunenavn">Kommune name to check in table</param>
        /// <param name="FylkeNavn">Flyke name to check in table</param>
        /// <returns>Kommune ID</returns>
        public async Task<string> GetKommuneID(string Kommunenavn, string FylkeNavn)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetKommuneID");
            DataTable kommuneID;
            string kID;
            Exception exception = null;
            string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);


            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_kommunenavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = Kommunenavn.ToUpper();

            npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = currentTable;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                kommuneID = dbhelper.FillDataTable("kspu_gdb.kommuneexists", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (kommuneID.Rows.Count == 0)
            {
                exception = new Exception("Fant ikke kommunen med Kommunenavn " + Kommunenavn + " i databasen.");
                _logger.LogError(exception, exception.Message);
                throw exception;
            }

            if (kommuneID.Rows.Count > 1)
            {
                if (FylkeNavn == null)
                {
                    exception = new Exception("Fant flere kommuner i databasen med Kommunenavn " + Kommunenavn + ".");
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }
                else
                {
                }
                // retrive the first
                kID = kommuneID.Rows[0].ToString(); //GetStringFromRow(kommuneID.Rows[0], "KommuneID");
            }
            else
                kID = kommuneID.Rows[0].ToString();

            _logger.LogInformation("Number of row returned {0}", kID);

            _logger.LogDebug("Exiting from KommuneExists");
            return kID;
        }

        /// <summary>
        /// To set Kommune property
        /// </summary>
        /// <param name="kommunes">Kommune name to check in table</param>

        public async Task SetIsKommuneUniqueProperty(KommuneCollection kommunes)
        {
            foreach (Kommune k in kommunes)
                k.IsKommuneNameUnique = await IsKommuneNameUnique(k);

        }

        /// <summary>
        /// To fetch Kommune data from Current Reol Table
        /// </summary>
        /// <param name="KommuneId">Kommune ID to check in table</param>
        /// <returns>Kommune Data</returns>
        public async Task<Kommune> GetKommune(string KommuneId)
        {
            Exception exception = null;
            var allKomunes = await GetAllKommunes();
            Kommune k = allKomunes.GetKommuneById(KommuneId);
            if (k == null)
            {
                exception = new Exception("Fant ikke kommunen med Kommuneid " + KommuneId + " i databasen.");
                _logger.LogError(exception, exception.Message);
                throw exception;
            }
            return k;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To check Kommune name is unique 
        /// </summary>
        /// <param name="komm">Kommune ID to check in table</param>
        /// <returns>True if unique else false</returns>
        private async Task<bool> IsKommuneNameUnique(Kommune komm)
        {
            var allKommunes = await GetAllKommunes();
            foreach (Kommune k in allKommunes)
            {
                if (k.KommuneID != komm.KommuneID)
                {
                    if (k.KommuneName.ToLower().Trim() == komm.KommuneName.Trim().ToLower())
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fill Kommune object
        /// </summary>
        /// <param name="row">Object of Datarow</param>
        /// <returns>Kommune data</returns>
        private Kommune GetKommuneFromDataRow(DataRow row)
        {
            Kommune k = new Kommune();
            k.KommuneID = row["r_kommuneid"].ToString();
            k.KommuneName = row["r_kommune"].ToString();
            k.FylkeID = row["r_fylkeid"].ToString();
            k.FylkeName = row["r_fylke"].ToString();
            return k;
        }

        #endregion

    }
}
