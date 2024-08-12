using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.DataLayer.Helper;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Interface.MemoryCache;
using Puma.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Repository.KspuDB.Reol
{
    public class ReolRepository : KsupDBGenericRepository<utvalg>, IReolRepository
    {

        private static Dictionary<string, ReolCollection> _ReolCache;
        private static Dictionary<string, Dictionary<long, Puma.Shared.Reol>> _ReolDictionaryCache;
        public string _ReolTableName;
        private readonly ILogger<ReolRepository> _logger;
        public readonly string connctionstring;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IManageCache _manageCache;
        public ReolRepository(KspuDBContext context, ILogger<ReolRepository> logger,
            IConfigurationRepository configurationRepository, IManageCache manageCache) : base(context)
        {
            _logger = logger;
            _configurationRepository = configurationRepository;
            _manageCache = manageCache;

            connctionstring = _context.Database.GetConnectionString();


        }
        public Task<bool> TableExists(string tableName)
        {
            _logger.LogDebug("Preparing the data for TableExists");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object result;
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tableName.Split(".")[1];

            npgsqlParameters[1] = new NpgsqlParameter("p_schemaname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = tableName.Split(".")[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.tableexists_owner", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in TableExists:", exception.Message);
                throw;
            }
            if (result == DBNull.Value)
                return Task.FromResult(false);
            _logger.LogInformation("Is TableExists : ", result);
            _logger.LogDebug("Exiting from TableExists");
            return Task.FromResult(System.Convert.ToInt32(result) > 0);
        }

        public Task<Puma.Shared.Reol> GetReol(long reolId, PumaEnum.NotFoundAction actionIfNotFound = PumaEnum.NotFoundAction.ThrowException, bool getAvisDekning = true, string reolTableName = "")
        {
            _logger.LogDebug("Preparing the data for GetReol");
            if (!string.IsNullOrWhiteSpace(reolTableName))
                _ReolTableName = reolTableName;
            else
                _ReolTableName = _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result;

            Dictionary<long, Puma.Shared.Reol> allReoler = GetReolDictionary();
            Puma.Shared.Reol result = null;
            if (allReoler.ContainsKey(reolId))
            {
                result = allReoler[reolId];
                return Task.FromResult(result);
            }
            else
            {
                if (actionIfNotFound == PumaEnum.NotFoundAction.ThrowException)
                    throw new Exception("Fant ikke reolen med reolid " + reolId + " i tabellen " + reolTableName + ".");
                return Task.FromResult(result);/* TODO Change to default(_) if this is not a reference type */


            }
        }

        private Dictionary<long, Puma.Shared.Reol> GetReolDictionary()
        {
            GetAllReols(); // Makes sure that dictionary has been cached
            return _ReolDictionaryCache[_ReolTableName];
        }

        public ReolCollection GetAllReols()
        {
            if (_ReolCache == null)
                _ReolCache = new Dictionary<string, ReolCollection>();
            if (_ReolDictionaryCache == null)
                _ReolDictionaryCache = new Dictionary<string, Dictionary<long, Puma.Shared.Reol>>();
            _ReolTableName = _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result;
            if (!_ReolCache.ContainsKey(_ReolTableName))
            {
                _ReolCache[_ReolTableName] = GetAllReolsFromTable(_ReolTableName).Result;
                _ReolDictionaryCache[_ReolTableName] = CreateReolDictionary(_ReolCache[_ReolTableName]);
            }
            return _ReolCache[_ReolTableName];
        }

        public async Task<ReolCollection> GetOrCreateReolCache(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

            var reolData = await _manageCache.GetFromCache<ReolCollection>(tableName);
            if (reolData != null)
                return reolData;

            // Key not in cache, so get data.
            reolData = await GetAllReolsFromTable(tableName);

            await _manageCache.SetCache(tableName, reolData);

            return reolData;
        }

        public async Task<ReolCollection> GetAllReolsFromTable(string tableName)
        {
            _logger.LogDebug("Preparing the data for GetAllReolsFromTable");
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

            var reolData = await _manageCache.GetFromCache<ReolCollection>(tableName);

            // Check data exist in cache 
            if (reolData != null)
            {
                return reolData;
            }
            // if not then first check whether the table exist and if not then retun null or else get data from table and set in cache
            else if (await TableExists(tableName))
            {
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                ReolCollection result = new ReolCollection();
                DataTable reols;

                npgsqlParameters[0] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = tableName;
                try
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        reols = dbhelper.FillDataTable("kspu_gdb.custom_getallreolsfromtable", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in GetAllReolsFromTable:", exception.Message);
                    throw;
                }
                foreach (DataRow datarow in reols.Rows)
                {
                    Puma.Shared.Reol reol = GetReolFromDataRow(datarow);
                    result.Add(reol);
                }

                _logger.LogInformation("Result is : ", result);
                _logger.LogDebug("Exiting from GetAllReolsFromTable");

                await _manageCache.SetCache(tableName, result);
                return result;
            }
            else
            {
                _logger.LogInformation("Table not exist : ", tableName);
                _logger.LogDebug("Exiting from GetAllReolsFromTable");
                return null;
            }
        }

        private static Dictionary<long, Puma.Shared.Reol> CreateReolDictionary(ReolCollection reoler)
        {
            Dictionary<long, Puma.Shared.Reol> dict = new Dictionary<long, Puma.Shared.Reol>();
            foreach (Puma.Shared.Reol reol in reoler)
                dict[reol.ReolId] = reol;
            return dict;
        }


        private Puma.Shared.Reol GetReolFromDataRow(DataRow row)
        {
            Utils utils = new Utils();
            Puma.Shared.Reol r = new Puma.Shared.Reol();
            // TODO: Add field names to configuration file
            r.ReolId = long.Parse(row["r_reol_id"].ToString());
            // // Ref: POB 5340 - Tilbakestilling av mappinglogikk Postnr og poststed skal nå hentes fra reolene 
            r.PostalZone = utils.GetStringFromRow(row, "r_postnr");
            r.PostalArea = utils.GetStringFromRow(row, "r_poststed");
            r.Name = utils.GetStringFromRow(row, "r_reolnavn");
            r.ReolNumber = utils.GetStringFromRow(row, "r_reolnr");
            r.Comment = utils.GetStringFromRow(row, "r_kommentar");
            r.KommuneId = utils.GetStringFromRow(row, "r_kommuneid");
            r.Kommune = utils.GetStringFromRow(row, "r_kommune");
            r.FylkeId = utils.GetStringFromRow(row, "r_fylkeid");
            r.Fylke = utils.GetStringFromRow(row, "r_fylke");
            r.TeamNumber = utils.GetStringFromRow(row, "r_teamnr");
            r.TeamName = utils.GetStringFromRow(row, "r_teamnavn");
            r.PrisSone = Convert.ToInt32(row["r_prissone"]);
            r.Antall = GetAntallFromDataRow(row).Result;
            // TODO: Uncomment line below before deployment, so that avisdekning is read for reoler. Commented out now to speed up development.
            // If getAvisDekning Then r.AvisDeknings = DAAvisDekning.GetAvisDekning(r.ReolId)
            r.SegmentId = utils.GetStringFromRow(row, "r_segment");

            r.Description = utils.GetStringFromRow(row, "r_beskrivelse");
            r.RuteType = utils.GetStringFromRow(row, "r_reoltype");
            r.PostkontorNavn = utils.GetStringFromRow(row, "r_pbkontnavn");
            r.PrsEnhetsId = utils.GetStringFromRow(row, "r_prsnr");
            r.PrsName = utils.GetStringFromRow(row, "r_prsnavn");
            r.PrsDescription = utils.GetStringFromRow(row, "r_prsbeskrivelse");
            // Added for RDF
            r.Frequency = utils.GetStringFromRow(row, "r_rutedistfreq");
            r.SondagFlag = utils.GetStringFromRow(row, "r_sondagflag");
            if (string.IsNullOrWhiteSpace(r.Description))
                r.Description = utils.GetStringFromRow(row, "r_pbkontnavn", "", true);
            if (string.IsNullOrWhiteSpace(r.Description))
                r.Description = utils.GetStringFromRow(row, "r_prsbeskrivelse", "", true);
            // 08.08.2006 - Reolnavn skal brukes dersom den har verdi, ellers får den beskrivelse verdien
            if (string.IsNullOrWhiteSpace(r.Name))
                r.Name = r.Description;
            return r;
        }

        public async Task<AntallInformation> GetAntallFromDataRow(DataRow row)
        {
            Utils utils = new Utils();
            await Task.Run(() => { });
            AntallInformation antallInfo = new AntallInformation();
            antallInfo.Businesses = Convert.ToInt32(row["r_vh"]);
            antallInfo.FarmersReserved = Convert.ToInt32(row["r_gb_res"]);
            antallInfo.Households = Convert.ToInt32(row["r_hh"]);
            antallInfo.HouseholdsReserved = Convert.ToInt32(row["r_hh_res"]);
            antallInfo.Houses = Convert.ToInt32(row["r_er"]);
            antallInfo.HousesReserved = Convert.ToInt32(row["r_er_res"]);
            antallInfo.PriorityHouseholdsReserved = Convert.ToInt32(utils.GetLongFromRow(row, "r_p_hh_u_res"));
            antallInfo.NonPriorityHouseholdsReserved = Convert.ToInt32(utils.GetLongFromRow(row, "r_np_hh_u_res"));
            antallInfo.PriorityBusinessReserved = Convert.ToInt32(utils.GetLongFromRow(row, "r_p_vh_u_res"));
            antallInfo.NonPriorityBusinessReserved = Convert.ToInt32(utils.GetLongFromRow(row, "r_np_vh_u_res"));

            return antallInfo;
        }


        #region Public Methods

        /// <summary>
        /// Rebuild Reol Cache
        /// </summary>

        public async Task RebuildReolCache()
        {
            _logger.LogDebug("Preparing the data for RebuildReolCache");

            Dictionary<string, ReolCollection> newCache = new Dictionary<string, ReolCollection>();

            // Cache reoler for current and previous reol table. (Previous is to speed up recreation)
            string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
            string previousTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.PreviousReolTableName);
            newCache[currentTable] = await GetAllReolsFromTable(currentTable);
            newCache[previousTable] = await GetAllReolsFromTable(previousTable);

            Dictionary<string, Dictionary<long, Puma.Shared.Reol>> newDictionaryCache = new Dictionary<string, Dictionary<long, Puma.Shared.Reol>>();
            newDictionaryCache[currentTable] = CreateReolDictionary(newCache[currentTable]);
            newDictionaryCache[previousTable] = CreateReolDictionary(newCache[previousTable]);

            _ReolCache = newCache;
            _ReolDictionaryCache = newDictionaryCache;

            _logger.LogInformation("Number of row returned _ReolCache: ", _ReolCache.Count, "and _ReolDictionaryCache: ", _ReolDictionaryCache.Count);

            _logger.LogDebug("Exiting from RebuildReolCache");
        }


        public async Task<List<long>> GetAllReolIds()
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetAllReolIds");
                DataTable reolIds;
                List<long> result = new List<long>();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolIds = dbhelper.FillDataTable("kspu_gdb.custom_getallreolids", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow row in reolIds.Rows)
                    result.Add(long.Parse(row["r_reol_id"].ToString()));

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetAllReolIds");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllReolIds: " + exception.Message);
                return null;
            }
        }


        /// <summary>
        /// Get All Reol IDs as Object IDs
        /// </summary>
        /// <param name="WhereClause"></param>
        /// <returns>Reol ID List</returns>

        public async Task<List<int>> GetAllReolIDSASObjectIds(string WhereClause)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for GetAllReolIDSASObjectIds");
                DataTable reolIds;
                List<int> result = new List<int>();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_whereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = WhereClause;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolIds = dbhelper.FillDataTable("kspu_gdb.custom_getallreolidsasobjectids", CommandType.StoredProcedure, npgsqlParameters);
                }

                foreach (DataRow datarow in reolIds.Rows)
                    result.Add(Convert.ToInt32(long.Parse(datarow["r_objectid"].ToString())));

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetAllReolIDSASObjectIds");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetAllReolIDSASObjectIds: " + exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Reol in Fylke
        /// </summary>
        /// <param name="fylkeId"></param>
        /// <returns>Reol ID</returns>
        public async Task<ReolCollection> GetReolsInFylke(string fylkeId)
        {
            _logger.LogDebug("Preparing the data for GetReolsInFylke");
            ReolCollection reolCollection = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));
            _logger.LogInformation("Number of row returned: ", reolCollection.Count);
            _logger.LogDebug("Exiting from GetReolsInFylke");
            return reolCollection.GetReolsInFylke(fylkeId);
        }

        /// <summary>
        /// Get Reol in Kommune
        /// </summary>
        /// <param name="kommuneId"></param>
        /// <returns>Reol ID</returns>
        public async Task<ReolCollection> GetReolsInKommune(string kommuneId)
        {
            _logger.LogDebug("Preparing the data for GetReolsInKommune");
            ReolCollection reolCollection = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));
            _logger.LogInformation("Number of row returned: ", reolCollection.Count);
            _logger.LogDebug("Exiting from GetReolsInKommune");
            return reolCollection.GetReolsInKommune(kommuneId);
        }

        /// <summary>
        /// Get Reol by team no
        /// </summary>
        /// <param name="teamNr"></param>
        /// <returns>Reol ID</returns>
        public async Task<ReolCollection> GetReolsByTeamNr(string teamNr)
        {
            var data = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));

            return data.GetReolsByTeamNr(teamNr);
        }

        /// <summary>
        /// Get Reol by team name
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns>Reol ID</returns>
        public async Task<ReolCollection> GetReolsInTeam(List<string> teamName)
        {
            _logger.LogDebug("Preparing the data for GetReolsInKommune");
            ReolCollection reolCollection = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));
            _logger.LogInformation("Number of row returned: ", reolCollection.Count);
            _logger.LogDebug("Exiting from GetReolsInKommune");
            ReolCollection reol = new ReolCollection();
            foreach (var itemName in teamName)
            {
                reol.AddRange(reolCollection.GetReolsInTeam(itemName));
            }
            return reol;
        }


        public async Task<ReolCollection> GetReolsInPostNr(string postnummer)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for GetReolsInPostNr");
                DataTable reolIds;
                ReolCollection result = new ReolCollection();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_postnummer", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = postnummer;

                string qryFetchReol = "select reol_id as r_reol_id from kspu_gdb.norway_reol where  cast(POSTNR as numeric)  in (" + postnummer + ")";

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    //reolIds = dbhelper.FillDataTable("kspu_gdb.custom_getreolsinpostnr", CommandType.StoredProcedure, npgsqlParameters);

                    reolIds = dbhelper.FillDataTable(qryFetchReol, CommandType.Text, null);
                }

                foreach (DataRow datarow in reolIds.Rows)
                {
                    foreach (Puma.Shared.Reol reol in GetAllReols())
                    {
                        if (!datarow.IsNull("r_reol_id"))
                        {
                            if (reol.ReolId == long.Parse(datarow["r_reol_id"].ToString()))
                                result.Add(reol);
                        }
                    }
                }

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetReolsInPostNr");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolsInPostNr: " + exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Reol data from Reol ID
        /// </summary>
        /// <param name="reolID"></param>
        /// <param name="getAvisDekning"></param> 
        /// <returns>Reol ID List</returns>

        public async Task<Puma.Shared.Reol> GetReolFromReolID(long reolID, bool getAvisDekning = true)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolFromReolID");
                DataTable reolData;
                DataRow datarow;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                npgsqlParameters[0] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = reolID.ToString();

                npgsqlParameters[1] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                npgsqlParameters[2] = new NpgsqlParameter("p_getavisdekning", NpgsqlTypes.NpgsqlDbType.Boolean);
                npgsqlParameters[2].Value = getAvisDekning;


                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolfromreolid", CommandType.StoredProcedure, npgsqlParameters);
                }

                datarow = reolData.Rows[0];

                _logger.LogInformation("Number of row returned: ", datarow.Table.Rows.Count);

                _logger.LogDebug("Exiting from GetReolFromReolID");

                return GetReolFromDataRow(datarow);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolFromReolID: " + exception.Message);
                return null;
            }
        }


        public async Task<ReolCollection> GetReolsFromReolIDs(long[] ReolIDs)
        {
            _logger.LogDebug("Preparing the data for GetReolsFromReolIDs");
            ReolCollection result = new ReolCollection();
            foreach (long reolId in ReolIDs)
                result.Add(await this.GetReol(reolId));

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetReolsFromReolIDs");
            return result;
        }


        public async Task<ReolCollection> SearchReolByReolName(string reolName)
        {
            var reolData = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));
            return reolData.GetReolsByNameSearch(reolName);
        }

        public async Task<ReolCollection> GetReolsByNameSearch(string reolName)
        {
            var reolData = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));
            return reolData.GetReolsByNameSearch(reolName);
        }

        public async Task<ReolCollection> SearchReolPostboksByReolName(string reolName, string kommuneName)
        {
            var reolData = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));

            return reolData.GetReolsPostboksByNameSearch(reolName, kommuneName);
        }

        public async Task<ReolCollection> GetReolsPostboksByNameSearch(string reolName, string kommuneName)
        {
            var reolData = await GetAllReolsFromTable(await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName));

            return reolData.GetReolsPostboksByNameSearch(reolName, kommuneName);
        }

        /// <summary>
        /// Get Reol data from Reol ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>ReolCollection</returns>
        public async Task<ReolCollection> GetReolsFromReolIDString(string ids)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolsFromReolIDString");
                DataTable reolData;
                ReolCollection result = new ReolCollection();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

                npgsqlParameters[0] = new NpgsqlParameter("p_reoltablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                npgsqlParameters[1] = new NpgsqlParameter("p_ids", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = ids;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolsfromreolidstring", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow datarow in reolData.Rows)
                {
                    Puma.Shared.Reol reol = GetReolFromDataRow(datarow);
                    result.Add(reol);
                }

                _logger.LogInformation("Number of row returned: ", result.Count);

                _logger.LogDebug("Exiting from GetReolsFromReolIDString");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolsFromReolIDString: " + exception.Message);
                return null;
            }
        }


        public async Task<Puma.Shared.Utvalg> GetReolerBySegmenterSearch(DemographyOptions options)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerBySegmenterSearch");
                DataTable reolData;
                Puma.Shared.Utvalg utvalg = new Puma.Shared.Utvalg();
                Utils utils = new Utils();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];


                npgsqlParameters[0] = new NpgsqlParameter("p_sqlwhereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = options.SQLWhereClause;

                npgsqlParameters[1] = new NpgsqlParameter("p_sqlwhereclausegeography", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = options.SQLWhereClauseGeography;

                npgsqlParameters[2] = new NpgsqlParameter("p_sqlorderby", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = options.SQLOrderby;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_testsegment", CommandType.StoredProcedure, npgsqlParameters);
                }


                utvalg.Reoler = new ReolCollection();

                foreach (DataRow row in reolData.Rows)
                {
                    long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "r_reol_id"));
                    Puma.Shared.Reol r = await GetReol(reolid, NotFoundAction.ReturnNothing, false);
                    utvalg.Reoler.Add(r);

                }
                _logger.LogInformation("Number of row returned: ", utvalg);
                _logger.LogDebug("Exiting from GetReolsFromReolIDString");
                return utvalg;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerBySegmenterSearch: " + exception.Message);
                return null;
            }
        }


        public async Task<ReolCollection> GetReolerByKommuneSearch(string kummuneIder)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByKommuneSearch");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                DataTable reolData;
                string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
                //OracleCommand cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + kummuneIder);
                // AddParameterString(cmd, "reolid", ReolID, 10)
                npgsqlParameters[0] = new NpgsqlParameter("p_kummuneider", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = kummuneIder;

                npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = currentTable;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolerbykommunesearch", CommandType.StoredProcedure, npgsqlParameters);
                }
                //DataTable reolData = GetDataTable(cmd);
                ReolCollection result = new ReolCollection();
                foreach (DataRow row in reolData.Rows)
                {
                    Puma.Shared.Reol r = GetReolFromDataRow(row);
                    result.Add(r);
                }
                _logger.LogInformation("Number of row returned: ", result.Count);
                _logger.LogDebug("Exiting from GetReolerByKommuneSearch");
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByKommuneSearch: " + exception.Message);
                return null;
            }
        }

        public async Task<ReolCollection> GetReolerByFylkeSearch(string fylkeIder)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByFylkeSearch");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                DataTable reolData;
                string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                //if (Strings.InStr(Strings.UCase(fylkeIder), "FYLKEID") != 0)
                //{
                npgsqlParameters[0] = new NpgsqlParameter("p_fylkeid", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = fylkeIder;
                if (fylkeIder.ToUpper().IndexOf("FYLKEID") < 0)
                {
                    string value = string.Join(",", fylkeIder.Split(',').Select(x => string.Format("'{0}'", x)).ToList());

                    npgsqlParameters[0].Value = value;
                }

                npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = currentTable;


                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolerbyfylkesearch", CommandType.StoredProcedure, npgsqlParameters);
                }
                //}
                //// cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + fylkeIder);
                //else
                //{
                //    npgsqlParameters[0] = new NpgsqlParameter("p_postboks", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[0].Value = fylkeIder;

                //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[1].Value = currentTable;

                //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                //    {
                //        reolData = dbhelper.FillDataTable("kspu_gdb.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
                //    }
                //}
                //cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE FylkeID IN(" + fylkeIder + ")");
                // AddParameterString(cmd, "reolid", ReolID, 10)
                //DataTable reolData = GetDataTable(cmd);
                ReolCollection result = new ReolCollection();
                foreach (DataRow row in reolData.Rows)
                {
                    Puma.Shared.Reol r = GetReolFromDataRow(row);
                    result.Add(r);
                }
                _logger.LogInformation("Number of row returned: ", result.Count);
                _logger.LogDebug("Exiting from GetReolerByFylkeSearch");
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByFylkeSearch:", exception.Message);
                throw;
            }
        }

        public async Task<ReolCollection> GetReolerByTeamSearch(string teamNames)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByTeamSearch");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                DataTable reolData;
                string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                //if (Strings.InStr(Strings.UCase(teamNames), "TEAMNAME") != 0)
                //{
                npgsqlParameters[0] = new NpgsqlParameter("p_teamnames", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = teamNames;

                if (teamNames.ToUpper().IndexOf("TEAMNAME") < 0)
                {
                    string value = string.Join(",", teamNames.Split(',').Select(x => string.Format("'{0}'", x)).ToList());

                    npgsqlParameters[0].Value = value;
                }

                npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = currentTable;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolerbyteamsearch", CommandType.StoredProcedure, npgsqlParameters);
                }
                //}
                ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + teamNames);
                //else
                //{
                //    npgsqlParameters[0] = new NpgsqlParameter("p_teamNames", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[0].Value = teamNames;

                //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[1].Value = currentTable;


                //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                //    {
                //        reolData = dbhelper.FillDataTable("kspu_gdb.GetReolerByTeamSearch", CommandType.StoredProcedure, npgsqlParameters);
                //    }
                //}
                // cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE TeamName= '" + teamNames + "'");
                // 
                //DataTable reolData = GetDataTable(cmd);
                ReolCollection result = new ReolCollection();
                foreach (DataRow row in reolData.Rows)
                {
                    Puma.Shared.Reol r = GetReolFromDataRow(row);
                    result.Add(r);
                }
                _logger.LogInformation("Number of row returned: ", result.Count);
                _logger.LogDebug("Exiting from GetReolerByTeamSearch");
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByTeamSearch:", exception.Message);
                throw;
            }
        }


        public async Task<ReolCollection> GetReolerByPostalZoneSearch(string postalZone)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByPostalZoneSearch");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                DataTable reolData;
                string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
                //if (Strings.InStr(Strings.UCase(postalZone), "POSTNR") != 0)
                //{
                npgsqlParameters[0] = new NpgsqlParameter("p_postalzone", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = postalZone;

                npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = currentTable;


                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolerbypostalzonesearch", CommandType.StoredProcedure, npgsqlParameters);
                }
                //}
                ////cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + postalZone);
                //else
                //{
                //    npgsqlParameters[0] = new NpgsqlParameter("p_postalZone", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[0].Value = postalZone;

                //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
                //    npgsqlParameters[1].Value = currentTable;

                //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                //    {
                //        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostalZoneSearch", CommandType.StoredProcedure, npgsqlParameters);
                //    }
                //}
                //cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE Postnr= '" + postalZone + "'");
                // Postnr in (2019, 2050)
                //DataTable reolData = GetDataTable(cmd);
                ReolCollection result = new ReolCollection();
                foreach (DataRow row in reolData.Rows)
                {
                    Puma.Shared.Reol r = GetReolFromDataRow(row);
                    result.Add(r);
                }
                _logger.LogInformation("Number of row returned: ", result.Count);
                _logger.LogDebug("Exiting from GetReolerByPostalZoneSearch");
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByPostalZoneSearch:", exception.Message);
                throw;
            }
        }

        public async Task<ReolCollection> GetReolerByPostboksSearch(string postboks)
        {
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByPostboksSearch");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                DataTable reolData;
                string currentTable = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                if (Strings.InStr(Strings.UCase(postboks), "POSTNR") != 0)
                {

                    npgsqlParameters[0] = new NpgsqlParameter("p_postboks", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[0].Value = postboks;


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }

                //cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE " + postboks);
                else
                {
                    npgsqlParameters[0] = new NpgsqlParameter("p_postboks", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[0].Value = postboks;


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        reolData = dbhelper.FillDataTable("kspu_db.GetReolerByPostboksSearch", CommandType.StoredProcedure, npgsqlParameters);
                    }

                }
                // cmd = new OracleCommand(" SELECT * FROM " + DAConfig.CurrentReolTableName + " WHERE Postnr= '" + postboks + "'");
                // Postnr in (2019, 2050)
                // DataTable reolData = GetDataTable(cmd);
                ReolCollection result = new ReolCollection();
                foreach (DataRow row in reolData.Rows)
                {
                    Puma.Shared.Reol r = GetReolFromDataRow(row);
                    result.Add(r);
                }
                _logger.LogInformation("Number of row returned: ", result.Count);
                _logger.LogDebug("Exiting from GetReolerByPostboksSearch");
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByPostboksSearch:", exception.Message);
                throw;
            }
        }

        public async Task<List<long>> ValidateAndGetNewPIBs(long oldReolID)
        {
            await Task.Run(() => { });
            return null;
        }

        public async Task<Puma.Shared.Utvalg> GetReolerByDemographySearch(DemographyOptions options, Puma.Shared.Utvalg utvalg,
            bool isFromKundeweb = false)
        {
            await Task.Run(() => { });

          //  await GetReolerByIndexOrg_DemographySearch(utvalg, options);
            try
            {
                _logger.LogDebug("Preparing the data for GetReolerByDemographySearch");
                DataTable reolData;
                Utils utils = new Utils();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];


                npgsqlParameters[0] = new NpgsqlParameter("p_sqlwhereclause", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = options.SQLWhereClause;

                npgsqlParameters[1] = new NpgsqlParameter("p_sqlwhereclausegeography", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = options.SQLWhereClauseGeography;

                npgsqlParameters[2] = new NpgsqlParameter("p_sqlorderby", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = options.SQLOrderby;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolData = dbhelper.FillDataTable("kspu_gdb.custom_getreolerbydemographysearch", CommandType.StoredProcedure, npgsqlParameters);
                }
                UtvalgReceiverList urLst = new UtvalgReceiverList();
                //new UtvalgReceiverList();
                List<UtvalgReceiverList> result = new List<UtvalgReceiverList>();

                if ((utvalg == null))
                    utvalg = Shared.Utvalg.CreateNewUtvalg(Puma.DataLayer.BusinessEntity.Constants.NewUtvalgName, null);
                else
                    utvalg.Reoler = new ReolCollection();

                long sum = 0;
                //DAReolTable daReol = new DAReolTable(DAConfig.CurrentReolTableName);
                ReolCollection reols = new ReolCollection();
                if (reolData != null && reolData.Rows.Count > 0)
                {
                    reols = await GetOrCreateReolCache("");
                }
                List<Puma.Shared.Reol> lstReols = new List<Puma.Shared.Reol>();
                if (isFromKundeweb && options.MaxAntall == 9999999)
                {
                    Puma.Shared.Utvalg utvalgTemp = Shared.Utvalg.CreateNewUtvalg(Puma.DataLayer.BusinessEntity.Constants.NewUtvalgName, null);

                    foreach (DataRow row in reolData.Rows)
                    {
                        long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "r_reol_id"));
                        Puma.Shared.Reol r = await GetReolFromReolCollectionByReolId(reolid, reols);
                        lstReols.Add(r);
                        utvalgTemp.Reoler.Add(r);
                    }

                    options.MaxAntall = (utvalgTemp.TotalAntall * 20) / 100;
                }

                foreach (DataRow row in reolData.Rows)
                {
                    long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "r_reol_id"));
                    Puma.Shared.Reol r = null;
                    if (isFromKundeweb && options.MaxAntall == 9999999)
                        r = lstReols.Find(x => x.ReolId == reolid);
                    else
                        r = await GetReolFromReolCollectionByReolId(reolid, reols);
                    utvalg.Reoler.Add(r);

                    // tester mot maxantall
                    if (options.MaxAntall > 0)
                    {
                        sum += r.Antall.GetTotalAntall(utvalg.Receivers);
                        if (sum > options.MaxAntall)
                            break;// hopp ut av løkka
                    }
                }
                _logger.LogInformation("Result is : ", result);
                _logger.LogDebug("Exiting from GetReolerByDemographySearch");
                return utvalg;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetReolerByDemographySearch:", exception.Message);
                throw;
            }
        }
        public async Task<Puma.Shared.Utvalg> GetReolerByIndex_DemographySearch(Puma.Shared.Utvalg utvalg, DemographyOptions options)
        {
            await Task.Run(() => { });
            return null;
        }
        public async Task<Puma.Shared.Reol> GetReolFromReolCollectionByReolId(long reolid, ReolCollection reolData, PumaEnum.NotFoundAction actionIfNotFound = PumaEnum.NotFoundAction.ThrowException, bool getAvisDekning = true)
        {
            await Task.Run(() => { });

            Puma.Shared.Reol result = null;

            if (reolData?.Any() == true)
            {
                result = reolData.Where(x => x.ReolId == reolid)?.FirstOrDefault();
            }

            if (result == null && actionIfNotFound == PumaEnum.NotFoundAction.ThrowException)
                throw new Exception("Fant ikke reolen med reolid " + reolid + "");

            return result;/* TODO Change to default(_) if this is not a reference type */

        }

        //public async Task<Puma.Shared.Utvalg> GetReolerByIndexOrg_DemographySearch(Puma.Shared.Utvalg utvalg, DemographyOptions options)
        //{
        //    await Task.Run(() => { });
        //    return null;
        //}



        public async Task<Puma.Shared.Utvalg> GetReolerByIndexOrg_DemographySearch(Puma.Shared.Utvalg utvalg, DemographyOptions options)
        {
            await Task.Run(() => { });
            // TODO: Dersom denne tas ibruk igjen må Biltype select skrives litt om. Feiler pga manglende presisjon i databasen.
            // TODO: Skal spørringa inneholde mottakerkriterier? Får vist reoler med ingen mottakere..Men mottakere kan endres etter at søket gjort!


            ArrayList pIndexFields;
            System.Collections.Hashtable pIndexCollNamed = new System.Collections.Hashtable();
            System.Collections.Hashtable pIndexCollAntall = new System.Collections.Hashtable();
            System.Collections.Hashtable pIndexFieldsSnittIndex = new System.Collections.Hashtable();
            
            string SQLFields = "";
            pIndexFields = options.IndexFieldSelected;

            string index = "";

            foreach (var indexField in pIndexFields)
                SQLFields = SQLFields + ", " + indexField;




            _logger.LogDebug("Preparing the data for GetReolerByDemographySearch");
            DataTable reolData;
            Utils utils = new Utils();

            string query = "select main.reol_id  " + SQLFields + " from kspu_gdb.norway_reol_index main,kspu_gdb.tbl_norway_reol_Index indeks where main.reol_id = indeks.reol_id AND " + options.SQLWhereClause + " " + options.SQLWhereClauseGeography + " " + options.SQLOrderby + "";

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                reolData = dbhelper.FillDataTable(query, CommandType.Text, null);
            }

            // debug
            // Logger.LogMessage("Demografi SQL = " + SQL)



           
            int nRecords = 0;

            ReolCollection reols = new ReolCollection();
            if (utvalg == null && reolData != null && reolData.Rows.Count > 0)
            {
                reols = await GetOrCreateReolCache("");
            }
            else {
                foreach (var itemReol in utvalg.Reoler)
                {
                    reols.Add(itemReol);
                }
                utvalg.Reoler = new ReolCollection();
            }

            // fjern eksisterende reoler fra utvalget
            if ((utvalg == null))
                utvalg = Shared.Utvalg.CreateNewUtvalg(Puma.DataLayer.BusinessEntity.Constants.NewUtvalgName, null);
            //else
            //    utvalg.Reoler = new ReolCollection();

            foreach (DataRow row in reolData.Rows)
            {
                nRecords = nRecords + 1;
                long nAntall;
                long reolid = Convert.ToInt64(utils.GetStringFromRow(row, "reol_id"));
                Puma.Shared.Reol r = await GetReolFromReolCollectionByReolId(reolid, reols,NotFoundAction.ReturnNothing);
                if (r == null)
                    continue;
                nAntall = r.Antall.GetTotalAntall(utvalg.Receivers);

                if (nAntall > 0)
                {
                    r.IndexData = new Dictionary<string, double>();
                    foreach (string indexField in pIndexFields)
                    {
                        if (indexField.Contains("."))
                            index = indexField.Split(".")[1];

                        double indexValue = Math.Round(Convert.ToDouble(row[index].ToString()));

                        r.IndexData.Add(index, indexValue);

                        if (pIndexCollNamed.ContainsKey(index))
                        {
                            Hashtable TempArr;
                            TempArr = (Hashtable)pIndexCollNamed[index];

                            double pTempIndex = 0;
                            long ptempantall = 0;

                            // pTempIndex = CType(TempArr.Item("Sum"), Double)
                            //need to check this code
                            pTempIndex = Convert.ToInt64(TempArr["Sum"]);
                            pTempIndex = pTempIndex + (indexValue * nAntall);
                            TempArr["Sum"] = pTempIndex;

                            // ptempantall = CType(TempArr.Item("Antall"), Integer)
                            ptempantall = Convert.ToInt64(TempArr["Antall"]);
                            ptempantall = ptempantall + nAntall;
                            TempArr["Antall"] = ptempantall;

                            pIndexCollNamed[index] = TempArr;
                        }
                        else
                        {
                            Hashtable TempArr = new Hashtable();
                            TempArr.Add("Sum", indexValue * nAntall);
                            TempArr.Add("Antall", nAntall);
                            pIndexCollNamed.Add(index, TempArr);
                        }
                    }
                }

                //reols.Add(r);
            }

            foreach (var indexColName in pIndexCollNamed.Keys)
            {
                double pTempSum;
                double pSnitt;
                double pTempAntall;
                Hashtable TempArr;
                TempArr = (Hashtable)pIndexCollNamed[indexColName];

                pTempSum = System.Convert.ToDouble(TempArr["Sum"]);
                pTempAntall = System.Convert.ToDouble(TempArr["Antall"]);

                if (pTempAntall > 0)
                {
                    // pSnitt = pTempSum / (nRecords * pTempAntall)
                    pSnitt = pTempSum / pTempAntall;
                    pIndexFieldsSnittIndex.Add(indexColName, pSnitt);
                }
            }

            long sum = 0;
            foreach (DataRow row in reolData.Rows)
            {
                long reolid = Convert.ToInt64(row["reol_id"]);
                Puma.Shared.Reol r = reols.Where(x => x.ReolId == reolid)?.FirstOrDefault();
                
                if (r == null)
                    continue;

                if (r.Antall.GetTotalAntall(utvalg.Receivers) > 0)
                    {


                        utvalg.Reoler.Add(r);
                        // tester mot maxantall
                        if (options.MaxAntall > 0)
                            sum += r.Antall.GetTotalAntall(utvalg.Receivers);

                        if (sum > options.MaxAntall)
                            break;// hopp ut av løkka
                    }
            }



           

            return utvalg;
        }


        #endregion



        #region Code not in use
        //#region Private Methods
        //private string GetAllReolsJSONFromTable(string tableName)
        //{
        //    _logger.LogDebug("Inside into GetAllReolsJSONFromTable");

        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
        //    StringBuilder result = new StringBuilder();
        //    DataTable reols;

        //    npgsqlParameters[0] = new NpgsqlParameter("_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tableName;

        //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
        //    {
        //        reols = dbhelper.FillDataTable("kspu_gdb.custom_getallreolinjsonformat", CommandType.StoredProcedure, npgsqlParameters);
        //    }
        //    int count = 1;
        //    foreach (DataRow row in reols.Rows)
        //    {
        //        if (count == 1)
        //        {
        //            result.Append("[");
        //        }
        //        result.Append(row.ItemArray[0].ToString());
        //        result.Append(count < reols.Rows.Count ? "," : "]");
        //        count += 1;
        //    }
        //    _logger.LogInformation("Number of row returned {0}", result);
        //    _logger.LogDebug("Exiting from GetAllReolsJsonFromTable");

        //    return result.ToString();
        //}


        //#endregion 
        #endregion
    }
}
