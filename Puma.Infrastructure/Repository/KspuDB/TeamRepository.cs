using Npgsql;
using System.Collections.Generic;
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
    public class TeamRepository : KsupDBGenericRepository<AddressPointsState>, ITeamRepository
    {
        private readonly ILogger<TeamRepository> _logger;
        public readonly string Connctionstring;
        private readonly IConfigurationRepository _configurationRepository;
        public TeamRepository(KspuDBContext context, ILogger<TeamRepository> logger,
           IConfigurationRepository configurationRepository) : base(context)
        {
            _logger = logger;
            Connctionstring = _context.Database.GetConnectionString();
            _configurationRepository = configurationRepository;
        }
        #region Public Methods

        /// <summary>
        /// Search Team data on the basis of team name
        /// </summary>
        /// <param name="teamNavn">Team Name</param>
        /// <returns>TeamCollection data</returns>
        public async Task<TeamCollection> SearchTeam(string teamNavn)
        {
            _logger.LogDebug("Preparing the data for SearchTeam");
            DataTable teamCollection;
            TeamCollection result = new TeamCollection();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];



            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

            npgsqlParameters[1] = new NpgsqlParameter("p_teamnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 34);
            npgsqlParameters[1].Value = teamNavn;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                teamCollection = dbhelper.FillDataTable("kspu_gdb.custom_searchteam", CommandType.StoredProcedure, npgsqlParameters);
            }

            if (teamCollection?.Rows?.Count == 0)
                return null/* TODO Change to default(_) if this is not a reference type */;

            foreach (DataRow datarow in teamCollection?.Rows)
            {
                result.Add(new Team(datarow["r_teamnr"].ToString(), datarow["r_teamnavn"].ToString()));

            }

            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from SearchTeam");
            return result;
        }

        /// <summary>
        /// Get all team data
        /// </summary>
        /// <returns>TeamCollection </returns>
        public async Task<TeamCollection> GetAllTeam()
        {
            _logger.LogDebug("Preparing the data for GetAllTeam");
            DataTable teamCollection;
            TeamCollection result = new TeamCollection();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                teamCollection = dbhelper.FillDataTable("kspu_gdb.custom_getallteam", CommandType.StoredProcedure, npgsqlParameters);
            }
            if (teamCollection?.Rows?.Count == 0)
                throw new Exception("Fant ikke noe Team i databasen.");

            foreach (DataRow datarow in teamCollection?.Rows)
            {
                result.Add(new Team(datarow["r_teamnr"].ToString(), datarow["r_teamnavn"].ToString()));
            }

            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetAllTeam");
            return result;
        }

        /// <summary>
        /// Get all team Kommune keys
        /// </summary>
        /// <returns>TeamKommuneKey List</returns>
        
        public async Task<List<TeamKommuneKey>> GetAllTeamKommuneKeys()
        {
            _logger.LogDebug("Preparing the data for GetAllTeamKommuneKeys");
            DataTable teamKommuneKey;
            List<TeamKommuneKey> result = new List<TeamKommuneKey>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                teamKommuneKey = dbhelper.FillDataTable("kspu_gdb.custom_getallteamkommunekeys", CommandType.StoredProcedure, npgsqlParameters);
            }

            foreach (DataRow datarow in teamKommuneKey.Rows)
                result.Add(new TeamKommuneKey(System.Convert.ToString(datarow["r_teamnr"]), System.Convert.ToString(datarow["r_kommuneid"])));

            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetAllTeamKommuneKeys");
            return result;
        }

        #endregion
    }
}
