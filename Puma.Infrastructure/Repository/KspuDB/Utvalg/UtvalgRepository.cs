using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.DataLayer.Helper;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
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
using Microsoft.Extensions.DependencyInjection;
using PostgreSQLCopyHelper;
using Puma.DataLayer.BusinessEntity.Utvalg;
using System.Text.RegularExpressions;

namespace Puma.Infrastructure.Repository.KspuDB.Utvalg
{
    public class UtvalgRepository : KsupDBGenericRepository<utvalg>, IUtvalgRepository
    {
        private readonly IReolRepository _reolRepository;
        private readonly IConfigurationRepository _configurationRepository;

        private IUtvalgListRepository _utvalgListRepository;


        private readonly IGjenskapUtvalgRepository _gjenskapUtvalgRepository;


        private readonly ILogger<UtvalgRepository> _logger;
        public readonly string Connctionstring;
        IServiceProvider _services;
        private List<(ReolCollection reolData, string tableName)> reolData;

        public UtvalgRepository(KspuDBContext context, IReolRepository reolRepository, ILogger<UtvalgRepository> logger,
            IConfigurationRepository configurationRepository, IGjenskapUtvalgRepository gjenskapUtvalgRepository
            , IServiceProvider services) : base(context)
        {
            _reolRepository = reolRepository;

            _logger = logger;
            _configurationRepository = configurationRepository;
            _services = services;
            // _utvalgListRepository = services.GetService<IUtvalgListRepository>();

            _gjenskapUtvalgRepository = gjenskapUtvalgRepository;

            Connctionstring = _context.Database.GetConnectionString();
            reolData = new List<(ReolCollection reolData, string tableName)>();

        }

        #region Public Methods



        /// <summary>
        /// Get the address points based on user id
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        public async Task<bool> IsUtvalgSkrivebeskyttetInDB(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsUtvalgSkrivebeskyttetInDB");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgskrivebeskyttetindb", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsUtvalgSkrivebeskyttetInDB:", exception.Message);
                throw;
            }
            if (result == DBNull.Value)
            {
                _logger.LogInformation("Is Utvalg Skrivebeskyttet In DB: False");
                _logger.LogDebug("Exiting from IsUtvalgSkrivebeskyttetInDB");
                return false;
            }
            _logger.LogInformation("Is Utvalg Skrivebeskyttet In DB: ", (bool)result);
            _logger.LogDebug("Exiting from IsUtvalgSkrivebeskyttetInDB");
            return (bool)result;
        }

        /// <summary>
        /// GetUtvalg With All References
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>Utvalg data</returns>

        public async Task<Puma.Shared.Utvalg> GetUtvalgWithAllReferences(int utvalgId)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgWithAllReferences");

            //if (utvalgListController.IsUtvalgConnectedToParentList(utvalgId))
            //{
            //    return utvalgListController.GetRootUtvalgListWithAllReferences(utvalgId).GetUtvalgDescendant(utvalgId);

            //}
            //else
            //{
            _logger.LogDebug("Exiting from GetUtvalgWithAllReferences");
            return await GetUtvalg(utvalgId);
            //}

        }

        /// <summary>
        /// Used by ErgoInterfaces.UsagePatternMethods
        /// Fetches a datatable containing usage patterns for the specified period.
        /// </summary>
        /// <param name="fromDateInclusive"></param>
        /// <param name="toDateInclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<DataTable> GetUsagePattern(DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Preparing the data for GetUsagePattern");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_fromdateinclusive", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = fromDateInclusive.Date;

            npgsqlParameters[1] = new NpgsqlParameter("p_todateinclusive", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = toDateInclusive.Date.AddDays(1);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getusagepattern", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUsagePattern:", exception.Message);
                throw;
            }
            _logger.LogInformation("Number of row returned: ", utvData.Rows.Count);
            _logger.LogDebug("Exiting from GetUsagePattern");
            return utvData;


        }





        /// <summary>
        /// Get Utvalg Data
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch data from Utvalg table</param>
        /// <returns>Utvalg data</returns>

        public async Task<Puma.Shared.Utvalg> GetUtvalg(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalg");
            DataTable utvData;
            //Exception exception = null;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalg:", exception.Message);
                throw;
            }
            if (utvData.Rows.Count != 1)
            {
                //exception = new Exception("Fant ikke unikt utvalg med utvalgsid " + utvalgId + " i databasen.");
                _logger.LogError("Fant ikke unikt utvalg med utvalgsid " + utvalgId + " i databasen.");
                //throw exception;
            }

            _logger.LogInformation("Number of row returned: ", utvData.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalg");

            if (utvData.Rows.Count == 1)
                return GetUtvalgFromDataRow(utvData.Rows[0]);
            return null;
        }


        public async Task<DataSet> GetUtvalgAllData(int utvalgId, int basedOn)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Get all utvalg details");
            //Exception exception = null;
            DataSet dsUtvalgDetails = null;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[8];


            npgsqlParameters[0] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[0].Value = "utvalgModification";

            npgsqlParameters[1] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[1].Value = "utvalgReceiver";

            npgsqlParameters[2] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[2].Value = "utvalgKommune";

            npgsqlParameters[3] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[3].Value = "utvalgDistrict";

            npgsqlParameters[4] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[4].Value = "utvalgPostalZone";

            npgsqlParameters[5] = new NpgsqlParameter("refcursor", NpgsqlTypes.NpgsqlDbType.Refcursor);
            npgsqlParameters[5].Value = "utvalgCriteria";

            npgsqlParameters[6] = new NpgsqlParameter("utvalgId", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[6].Value = utvalgId;

            npgsqlParameters[7] = new NpgsqlParameter("p_basedOn", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[7].Value = basedOn;

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dsUtvalgDetails = dbhelper.GetDataSets("kspu_db.getutvalgalldetailsbyid", npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalg:", exception.Message);
                throw;
            }

            return dsUtvalgDetails;
        }


        // }


        /// <summary>
        ///  Return username for user that last saved a utvalg
        ///  </summary>
        ///  <param name="utvalgId"></param>
        ///  <returns></returns>

        public async Task<string> LastSavedBy(int utvalgId)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for LastSavedBy");
                object result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<string>("kspu_db.lastsavedby", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogInformation("Result is: ", result);

                _logger.LogDebug("Exiting from LastSavedBy");
                if ((result) is DBNull)
                    return null;
                else
                    return System.Convert.ToString(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in LastSavedBy:", exception.Message);
                throw;
            }

        }

        /// <summary>
        /// Does a utvalg with a spesific name exist in the database?
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <returns>boolean</returns>
        public async Task<bool> UtvalgNameExists(string utvalgNavn)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UtvalgNameExists");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = utvalgNavn;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalgnameexists", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UtvalgNameExists:", exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from UtvalgNameExists");
            if (result == null | (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Does a utvalg with a spesific name exist in the database?
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <returns>boolean</returns>
        public async Task<bool> UtvalgsNameExists(List<string> utvalgNavns)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UtvalgNameExists");
            DataTable result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = "'" + String.Join("', '", utvalgNavns) + "'";
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.utvalgsnameexists", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UtvalgNameExists:", exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from UtvalgNameExists");
            if (result == null)
                return false;
            return System.Convert.ToInt32(result.Rows.Count) > 0;
        }


        /// <summary>
        /// Search Utvalg Simple
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <param name="searchMethod"></param>
        /// <returns>UtvalgSearchCollection</returns>
        public async Task<UtvalgSearchCollection> SearchUtvalgSimple(string utvalgNavn, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgSimple");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgSearchCollection result = new UtvalgSearchCollection();


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            npgsqlParameters[0].Value = utvalgNavn;
            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[1].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgSimple:", exception.Message);
                throw;
            }
            foreach (DataRow datarow in utvData.Rows)
                result.Add(GetUtvalgSearchResultFromDataRow(datarow));

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgSimple");
            return result;
        }

        /// <summary>
        ///  Search Utvalg by Utvalg ID
        ///  </summary>
        ///  <param name="userID"></param>
        ///  <param name="searchMethod"></param>
        ///   <returns>UtvalgSearchCollection</returns>
        public async Task<UtvalgSearchCollection> SearchUtvalgByUserID1(string userID, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByUserID1");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgSearchCollection result = new UtvalgSearchCollection();


            npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = userID;
            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuserid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUserID1:", exception.Message);
                throw;
            }
            foreach (DataRow row in utvData.Rows)
                result.Add(GetUtvalgSearchResultFromDataRow(row));

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUserID1");
            return result;
        }

        /// <summary>
        /// Search by user id and/or customer number and agreement number, and you may only select Basisutvalg
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="customerNos"></param>
        /// <param name="agreementNos"></param>
        /// <param name="forceCustomerAndAgreementCheck"></param>
        /// <param name="onlyBasisUtvalg"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<UtvalgCollection> SearchUtvalgByUserIDAndCustNo(string userID, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, int onlyBasisUtvalg)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByUserIDAndCustNo");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            UtvalgCollection result = new UtvalgCollection();


            npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = "%" + userID + "%";

            npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";

            npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";

            npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";

            try
            {
                if (forceCustomerAndAgreementCheck)
                {
                    if (utils.CanSearch(customerNos, agreementNos))
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuseridcustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
                        }
                    }
                    else
                        // no search
                        return result;
                }
                else
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyuseridcustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }

                foreach (DataRow row in utvData.Rows)
                    result.Add(GetUtvalgSearchFromDataRow(row));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUserIDAndCustNo:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUserIDAndCustNo");
            return result;


        }



        public async Task<int> GetSequenceNextVal(string sequenceName)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetSequenceNextVal");
            int nextVal;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_sequencename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = sequenceName;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                nextVal = dbhelper.ExecuteScalar<int>("kspu_db.GetSequenceNextVal", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" SELECT " + sequenceName + ".NEXTVAL FROM dual ", trans.Connection, trans);
            //int nextVal = System.Convert.ToInt32(ExecuteScalar(cmd));
            _logger.LogInformation("Next Value returned: ", nextVal);
            _logger.LogDebug("Exiting from SearchUtvalgByUserIDAndCustNo");
            return nextVal;
        }




        /// <summary>
        ///      Search by user name and/or customer number and agreement number, and you may only select Basisutvalg
        ///      </summary>
        ///      <param name="utvalgNavn"></param>
        ///      <param name="customerNos"></param>
        ///      <param name="agreementNos"></param>
        ///      <param name="forceCustomerAndAgreementCheck"></param>
        ///      <param name="extendedInfo"></param>
        ///      <param name="onlyBasisUtvalg"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>


        public async Task<UtvalgCollection> SearchUtvalgByUtvalgName(string utvalgNavn, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisUtvalg)
        {
            var jsonstring = new StringBuilder();
            _logger.LogDebug("Preparing the data for SearchUtvalgByUtvalgName");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            UtvalgCollection result = new UtvalgCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar);
            if (utvalgNavn != null)
            {
                npgsqlParameters[0].Value = "%" + utvalgNavn.Replace("_", "\\_") + "%";
            }
            else
            {
                npgsqlParameters[0].Value = "";
            }



            npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";

            npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";

            npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";

            try
            {
                if (forceCustomerAndAgreementCheck)
                {
                    if (utils.CanSearch(customerNos, agreementNos))
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgnamecustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
                        }

                    }
                    else
                        // no search
                        return result;
                }
                else
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgnamecustomernoagreementno", CommandType.StoredProcedure, npgsqlParameters);
                    }

                }




                foreach (DataRow row in utvData.Rows)
                {
                    Puma.Shared.Utvalg u = GetUtvalgSearchFromDataRow(row);

                    // add last saved by user if extendedInfo= true
                    if (extendedInfo)
                    {
                        await GetUtvalgModifications(u, false);

                        await GetUtvalgReoler(u);
                        await GetUtvalgReceiver(u);
                        await GetUtvalgCriteria(u);
                        await GetUtvalgOldReoler(u, Convert.ToInt32(row["r_utvalgid"]));


                    }

                    result.Add(u);


                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUtvalgName:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUtvalgName");
            return result;


        }

        /// <summary>
        /// Search Utvalg by Utvalg Name
        /// </summary>
        /// <param name="utvalgNavn"></param>
        /// <param name="searchMethod"></param>
        /// <param name="includeReoler"></param>
        /// <returns>UtvalgSearchCollection</returns>

        public async Task<UtvalgCollection> SearchUtvalg(string utvalgNavn, SearchMethod searchMethod, bool includeReoler = true)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalg");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgCollection result = new UtvalgCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgnavn", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = utvalgNavn;
            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    result.Add(GetUtvalgFromDataRow(row, includeReoler));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalg:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalg");
            return result;
        }

        /// <summary>
        /// Search Utvalg by Utvalg ID
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <param name="includeReols"></param>
        /// <returns>UtvalgSearchCollection</returns>

        public async Task<UtvalgCollection> SearchUtvalgByUtvalgId(int utvalgId, bool includeReols = true)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByUtvalgId");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgCollection result = new UtvalgCollection();


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgid", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    result.Add(GetUtvalgFromDataRow(row, includeReols));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUtvalgId:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUtvalgId");
            return result;
        }

        /// <summary>
        /// Get Utvalg Campaigns
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns>CampaignDescription</returns>

        public async Task<List<CampaignDescription>> GetUtvalgCampaigns(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgCampaigns");
            DataTable utvData;
            DataTable utvData2;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<CampaignDescription> utvColl = new List<CampaignDescription>();


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalgcampaignsbybasedon", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    utvColl.Add(GetCampaignDescriptionFromUtvalgDataRow(row, false));

                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
                npgsqlParameters1[0].Value = utvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData2 = dbhelper.FillDataTable("kspu_db.getutvalgcampaignsbywasbasedon", CommandType.StoredProcedure, npgsqlParameters1);
                }
                foreach (DataRow row in utvData2.Rows)
                    utvColl.Add(GetCampaignDescriptionFromUtvalgDataRow(row, true));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgCampaigns:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", utvColl.Count);
            _logger.LogDebug("Exiting from GetUtvalgCampaigns");
            return utvColl;
        }


        /// <summary>
        /// Get Utvalg Name
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns>Utvalg name</returns>

        public async Task<string> GetUtvalgName(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgName");
            string result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<string>("kspu_db.getutvalgname", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgName:", exception.Message);
                throw;
            }
            if (result == null) // | result == DBNull.Value)
                return null;
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from GetUtvalgName");
            return result;
        }

        /// <summary>
        /// Search by Utvalg ID and customer number
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <param name="agreementNos"></param>
        /// <param name="customerNos"></param>
        /// <param name="includeReols"></param>
        /// <param name="forceCustomerAndAgreementCheck"></param>
        /// <param name="extendedInfo"></param>
        /// <param name="onlyBasisUtvalg"></param>    
        /// <returns></returns>
        /// <remarks></remarks>


        public async Task<UtvalgCollection> SearchUtvalgByUtvalgIdAndCustmerNo(int utvalgId, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool includeReols, bool extendedInfo, int onlyBasisUtvalg)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByUtvalgIdAndCustmerNo");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            UtvalgCollection result = new UtvalgCollection();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;

            npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";

            npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";


            npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = onlyBasisUtvalg == 0 ? "0,1" : "1";

            try
            {
                if (forceCustomerAndAgreementCheck)
                {
                    if (utils.CanSearch(customerNos, agreementNos))
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgidandcustmernoandagreementno", CommandType.StoredProcedure, npgsqlParameters);
                        }
                    }
                    else
                        // no search
                        return result;
                }
                else
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvalgidandcustmernoandagreementno", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }

                foreach (DataRow row in utvData.Rows)
                {
                    Puma.Shared.Utvalg u = GetUtvalgFromDataRow(row, includeReols);
                    // add last saved by user if extendedInfo= true
                    if (extendedInfo)
                        await GetUtvalgModifications(u, false);
                    result.Add(u);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUtvalgIdAndCustmerNo:", exception.Message);
            }

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUtvalgIdAndCustmerNo");
            return result;


        }

        /// <summary>
        /// Search Utvalg by KundeNummer
        /// </summary>
        /// <param name="KundeNummer"></param>
        /// <param name="searchMethod"></param>
        /// <param name="includeReols"></param>
        /// <returns>UtvalgSearchCollection</returns>

        public async Task<UtvalgCollection> SearchUtvalgByKundeNr(string KundeNummer, SearchMethod searchMethod, bool includeReols = true
            , bool extendedInfo = true)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByKundeNr");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgCollection result = new UtvalgCollection();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = KundeNummer;
            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbykundenr", CommandType.StoredProcedure, npgsqlParameters);
                }


                //List<decimal> lstUtvalgIds = utvData.Rows.OfType<DataRow>()
                //  .Select(dr => dr.Field<decimal?>("r_utvalgid"))
                //  .Where(i => i.HasValue)
                //  .Select(i => i.Value)
                //  .ToList();

                //List<Puma.Shared.Utvalg> ut = await GetUtvalgDetails(lstUtvalgIds);

                foreach (DataRow row in utvData.Rows)
                    result.Add(GetUtvalgFromDataRow(row, includeReols, extendedInfo));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByKundeNr:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from SearchUtvalgByKundeNr");
            return result;
        }

        /// <summary>
        /// Search Utvalg by Utvalg list ID
        /// </summary>
        /// <param name="utvalgListId"></param>
        /// <param name="includeReols"></param>
        /// <returns>UtvalgSearchCollection</returns>

        public async Task<UtvalgCollection> SearchUtvalgByUtvalListId(Int64 utvalgListId, bool includeReols = true)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByUtvalListId");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgCollection result = new UtvalgCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyutvallistid", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow dataRow in utvData.Rows)
                    result.Add(GetUtvalgFromDataRow(dataRow, includeReols));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByUtvalListId:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByUtvalListId");
            return result;
        }

        /// <summary>
        /// Search Utvalg by Utvalg list ID
        /// </summary>
        /// <param name="OrdreReferanse"></param>
        /// <param name="includeReols"></param>
        /// <param name="OrdreType"></param>
        /// <param name="searchMethod"></param>
        /// <returns>UtvalgSearchCollection</returns>


        public async Task<UtvalgCollection> SearchUtvalgByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod, bool includeReols = true)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgByOrdreReferanse");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            UtvalgCollection result = new UtvalgCollection();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = OrdreReferanse;
            npgsqlParameters[1] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            npgsqlParameters[1].Value = OrdreType;
            npgsqlParameters[2] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 10);
            npgsqlParameters[2].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgbyordrereferanse", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    result.Add(GetUtvalgFromDataRow(row, includeReols));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgByOrdreReferanse:", exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgByOrdreReferanse");
            return result;
        }

        /// <summary>
        /// Is Ordered selection, ready for update request
        /// </summary>
        /// <param name="deliveryDate">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        public async Task<bool> IsOrderedUtvalgAndListsReadyForUpdateRequest(DateTime deliveryDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsOrderedUtvalgAndListsReadyForUpdateRequest");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_deliverydate", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = deliveryDate;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isorderedutvalgandlistsreadyforupdaterequest", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsOrderedUtvalgAndListsReadyForUpdateRequest:", exception.Message);
                throw;
            }

            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from IsOrderedUtvalgAndListsReadyForUpdateRequest");
            if (result is decimal)
                return System.Convert.ToInt32(result) == 0;
            return System.Convert.ToInt32(result) == 1;
        }

        /// <summary>
        /// Is utvalg list ordered
        /// </summary>
        /// <param name="ID">Utvalg List ID</param>
        /// <returns>True or false</returns>

        public async Task<bool> IsUtvalgListOrdered(long ID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsUtvalgListOrdered");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = ID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalglistordered", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsUtvalgListOrdered:", exception.Message);
                throw;
            }

            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from IsUtvalgListOrdered");
            if (result is decimal)
                return System.Convert.ToInt32(result) > 0;
            return System.Convert.ToInt32(result) < 0;
        }

        /// <summary>
        /// Is utvalg ordered
        /// </summary>
        /// <param name="ID">Utvalg ID</param>
        /// <returns>True or false</returns>

        public async Task<bool> IsUtvalgOrdered(long ID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsUtvalgOrdered");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = ID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgordered", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsUtvalgOrdered:", exception.Message);
                throw;
            }

            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from IsUtvalgOrdered");
            if (result is decimal)
                return System.Convert.ToInt32(result) > 0;
            return System.Convert.ToInt32(result) < 0;
        }

        /// <summary>
        ///  Get selection for update request
        ///  </summary>
        ///  <param name="deliveryDate"></param>
        ///  <returns>AutoUpdateMessage list</returns>

        public async Task<List<AutoUpdateMessage>> GetOrderedUtvalgAndListsForUpdateRequest(DateTime deliveryDate)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetOrderedUtvalgAndListsForUpdateRequest");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<AutoUpdateMessage> result = new List<AutoUpdateMessage>();

            npgsqlParameters[0] = new NpgsqlParameter("p_dato", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[0].Value = deliveryDate;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                utvData = dbhelper.FillDataTable("kspu_db.getorderedutvalgandlistsforupdaterequest", CommandType.StoredProcedure, npgsqlParameters);
            }
            foreach (DataRow row in utvData.Rows)
            {
                try
                {
                    AutoUpdateMessage id = new AutoUpdateMessage();
                    id.Id = (int)row["utvalgsid"];
                    id.IsList = (bool)Interaction.IIf(row["utvalgtype"].ToString().ToLower().Equals("liste"), true, false);
                    if ((utvData.Columns.Contains("ordrereferanse")) && (!row.IsNull("ordrereferanse")))
                        id.Ordrereferanse = row["ordrereferanse"].ToString();
                    result.Add(id);
                }

                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                    _logger.LogError("Feil i parsing av oppdateringslista for utvalg og lister. Hoppet over " + row["utvalgsid"] + ".");
                }
            }

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetOrderedUtvalgAndListsForUpdateRequest");
            return result;
        }

        /// <summary>
        /// For integration: Get all distinct PRS from reoler in Utvalg.
        /// Uses current ReolMap, independent of recreation done, needed or not.
        /// </summary>
        /// <param name="utvalg"></param>
        /// <returns>PRS list</returns>

        public async Task<List<string>> GetDistinctPRS(int utvalgid)
        {
            _logger.LogDebug("Preparing the data for GetDistinctPRS");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            List<string> result = new List<string>();
            //var prsField = "prsnr";
            //int utvalgId = utvalg.UtvalgId;
            string table = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgid;

            npgsqlParameters[1] = new NpgsqlParameter("p_tablename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = table;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getdistinctprs", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                {
                    string s = row[0].ToString();
                    result.Add(s);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetDistinctPRS:", exception.Message);
            }

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetDistinctPRS");
            return result;
        }

        /// <summary>
        ///     ''' Get Utvalg Reoler Count
        ///     ''' </summary>
        ///     ''' <param name="utvalgID"></param>
        ///     ''' <returns>Reoler count</returns>

        public async Task<int> GetUtvalgReolerCount(long utvalgID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgReolerCount");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            int result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.getutvalgreolercount", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgReolerCount:", exception.Message);
                throw;
            }
            if (result.ToString() == null) // | (result.ToString()) is DBNull)
                return 0;

            _logger.LogInformation("Result is : ", result);
            _logger.LogDebug("Exiting from GetUtvalgReolerCount");
            return System.Convert.ToInt32(result);
        }

        /// <summary>
        ///  Get Demography Index Info for Reol
        ///  </summary>
        ///  <param name="utv"></param>
        ///  <param name="demographyOpts"></param>
        ///   <param name="tablename1"></param>
        ///    <param name="tablename2"></param>
        ///  <returns>PRS list</returns>
        //[HttpGet("GetDemographyIndexInfoForReol", Name = nameof(GetDemographyIndexInfoForReol))]
        //public DataTable GetDemographyIndexInfoForReol(Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        //{
        //    _logger.LogDebug("Inside into GetDemographyIndexInfoForReol");
        //    DataTable utvData;
        //    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
        //    DemographyValuesCollection result = new DemographyValuesCollection();

        //    string table = configController.CurrentReolTableName;
        //    //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
        //    //npgsqlParameters[0].Value = utv.UtvalgId;

        //    npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[0].Value = tablename1;

        //    npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
        //    npgsqlParameters[1].Value = tablename2;

        //    using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
        //    {
        //        utvData = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
        //    }
        //    utvData.Columns.Add("r_budrute"); 
        //    // result.Columns("Budrute").SetOrdinal(1) 'Move column to position 1
        //    utvData.Columns.Add("r_antall", typeof(int));
        //    utvData.Columns.Add("r_fylke", typeof(string));
        //    utvData.Columns.Add("r_kommune", typeof(string));
        //    utvData.Columns.Add("r_team", typeof(string));

        //    ReolCollection sortedReols = new ReolCollection(utv.Reoler);
        //    // sortedReols.SortByReolNumber()

        //    foreach (Reol r in sortedReols)
        //    {
        //        foreach (DataRow row in utvData.Rows)
        //        {
        //            if (r.ReolId.ToString() == row[0].ToString())
        //            {
        //                row["r_budrute"] = r.Name;
        //                row["r_antall"] = r.Antall.GetTotalAntall(utv.Receivers);
        //                row["r_fylke"] = r.Fylke;
        //                row["r_kommune"] = r.Kommune;
        //                row["r_team"] = r.TeamName;
        //            }
        //        }
        //    }

        //    if (utvData.Columns.Contains("r_fylke") && utvData.Columns.Contains("r_kommune") && utvData.Columns.Contains("r_team") && utvData.Columns.Contains("r_reolnr"))
        //        utvData.DefaultView.Sort = "r_fylke, r_kommune, r_team, r_reolnr";

        //    _logger.LogInformation("Number of row returned: ", utvData);
        //    _logger.LogDebug("Exiting from GetDemographyIndexInfoForReol");
        //    return utvData;
        //}



        public DataTable GetDemographyIndexInfoForReol(Puma.Shared.Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        {
            _logger.LogDebug("Preparing the data for GetDemographyIndexInfoForReol");
            DataTable result;

            int startPos = demographyOpts.SQLOrderby.IndexOf(tablename1);
            if (startPos == -1)
                startPos = demographyOpts.SQLOrderby.IndexOf(tablename1);
            int endPos = demographyOpts.SQLOrderby.IndexOf(tablename2);

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename1;

            npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = tablename2;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetDemographyIndexInfoForReol:", exception.Message);
                throw;
            }
            if (utv.Reoler.Count > 0)
            {

                int maxINCounter = 0;
                foreach (Puma.Shared.Reol r in utv.Reoler)
                {
                    maxINCounter = maxINCounter + 1;
                    if (maxINCounter == 1000)
                    {
                        maxINCounter = 0;
                    }
                }

            }

            result.Columns.Add("Budrute");

            result.Columns.Add("Antall", typeof(int));
            result.Columns.Add("Fylke", typeof(string));
            result.Columns.Add("Kommune", typeof(string));
            result.Columns.Add("Team", typeof(string));


            // sortedReols.SortByReolNumber()

            foreach (Puma.Shared.Reol r in new ReolCollection(utv.Reoler))
            {
                foreach (DataRow row in result.Rows)
                {
                    if (r.ReolId.ToString() == row.ItemArray[0].ToString())
                    {
                        row["r_budrute"] = r.Name;
                        row["r_antall"] = r.Antall.GetTotalAntall(utv.Receivers);
                        row["r_fylke"] = r.Fylke;
                        row["r_kommune"] = r.Kommune;
                        row["r_team"] = r.TeamName;
                    }
                }
            }

            if (result.Columns.Contains("Fylke") && result.Columns.Contains("Kommune") && result.Columns.Contains("Team") && result.Columns.Contains("Reolnr"))
                result.DefaultView.Sort = "Fylke, Kommune, Team, Reolnr";

            _logger.LogDebug("Exiting from GetUtvalgReolerCount");
            return result;
        }



        /// <summary>
        /// Gets the demography index information.
        /// </summary>
        /// <param name="utv">The utv.</param>
        /// <param name="demographyOpts">The demography opts.</param>
        /// <param name="tablename1">The tablename1.</param>
        /// <param name="tablename2">The tablename2.</param>
        /// <returns></returns>
        public async Task<DemographyValuesCollection> GetDemographyIndexInfo(Puma.Shared.Utvalg utv, DemographyOptions demographyOpts, string tablename1, string tablename2)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetDemographyIndexInfo");
            DataTable minmaxValues;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            DemographyValuesCollection result = new DemographyValuesCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_tablename1", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = tablename1;

            npgsqlParameters[1] = new NpgsqlParameter("p_tablename2", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = tablename2;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    minmaxValues = dbhelper.FillDataTable("kspu_db.getdemographyindexinfoforreol", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetDemographyIndexInfo:", exception.Message);
                throw;
            }
            if (minmaxValues != null)
            {
                //Do code here
            }
            return result;
        }


        /// <summary>
        ///  Get Number Of Budruter In Team By TeamNR
        ///  </summary>
        ///  <param name="teamnr"></param>
        /// <returns>No. of Budruter</returns>


        public async Task<string> GetNumberOfBudruterInTeamByTeamNR(string teamnr)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetNumberOfBudruterInTeamByTeamNR");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_teamnr", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = teamnr;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_gdb.custom_getnumberofbudruterinteam", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetNumberOfBudruterInTeamByTeamNR:", exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from GetNumberOfBudruterInTeamByTeamNR");
            if (result == null | (result) is DBNull)
                return System.Convert.ToString(0);
            return System.Convert.ToString(result);
        }

        /// <summary>
        ///  Get Number Of Budruter In Team
        ///  </summary>
        /// <returns>DataTabler</returns>


        public async Task<DataTable> GetNumberOfBudruterInTeam()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetNumberOfBudruterInTeam");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            DataTable result;
            npgsqlParameters = null;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_gdb.custom_getnumberofbudruterinteam", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetNumberOfBudruterInTeam:", exception.Message);
                throw;
            }
            //dt = new DataTable();
            //dt = result;
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            _logger.LogInformation("Result is: ", result);

            return result;
        }

        /// <summary>
        /// Get Probable Next UtvalgId
        ///  </summary>
        /// <returns>Utvalg ID</returns>


        public async Task<int> GetProbableNextUtvalgId()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetProbableNextUtvalgId");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters = null;
            int result = -1;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.getprobablenextutvalgid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetProbableNextUtvalgId:", exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from GetProbableNextUtvalgId");

            return result;
        }


        /// <summary>
        /// Updates the logo.
        /// </summary>
        /// <param name="utvalgId">The utvalg identifier.</param>
        /// <param name="Llogo">The llogo.</param>
        /// <param name="username">The username.</param>
        public async Task UpdateLogo(Int64 utvalgId, string logo, string username)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for UpdateLogo");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                int result;
                //string modificationInfo = "Update Logo ";

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = utvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[1].Value = !string.IsNullOrWhiteSpace(logo) ? logo : string.Empty;

                //npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                //modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                //npgsqlParameters[2].Value = modificationInfo;

                npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[2].Value = username;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updatelogoforutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                Puma.Shared.Utvalg utvalg = new Shared.Utvalg()
                {
                    UtvalgId = Convert.ToInt32(utvalgId)
                };
                _logger.LogDebug("Calling the SaveUtvalgModifications");
                await SaveUtvalgModifications(utvalg, username, "Update Logo ");

                _logger.LogDebug("Exiting from UpdateLogo");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateLogo: " + exception.Message);
            }
        }

        /// <summary>
        /// UpdateUtvalgForIntegration
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        /// <param name="username"></param>

        public async Task UpdateUtvalgForIntegration(Puma.Shared.Utvalg utv, string username)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for UpdateUtvalgForIntegration");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[8];
                int result;
                //string modificationInfo = "UpdateUtvalgForIntegration - ";

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
                if (!string.IsNullOrWhiteSpace(utv.OrdreReferanse))
                    npgsqlParameters[1].Value = utv.OrdreReferanse;
                else
                    npgsqlParameters[1].Value = DBNull.Value;

                //npgsqlParameters[1].Value = !string.IsNullOrWhiteSpace(utv.OrdreReferanse) == utv.OrdreReferanse; ? utv.OrdreReferanse;

                //npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                //modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                //npgsqlParameters[2].Value = modificationInfo;

                npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = (int)utv.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utv.OrdreType);

                npgsqlParameters[3] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[3].Value = (int)utv.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utv.OrdreStatus);

                npgsqlParameters[4] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                npgsqlParameters[4].Value = (utv.InnleveringsDato != DateTime.MinValue ? utv.InnleveringsDato : DateTime.Now);

                npgsqlParameters[5] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[5].Value = utv.Avtalenummer;

                npgsqlParameters[6] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                npgsqlParameters[6].Value = utv.DistributionDate;

                npgsqlParameters[7] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[7].Value = (int)utv.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utv.DistributionType);

                //npgsqlParameters[9] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                //npgsqlParameters[9].Value = username;
                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalgforintegration", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utv.UtvalgId);
                await SaveUtvalgModifications(utv, username, "UpdateUtvalgForIntegration - ");

                _logger.LogDebug("Exiting from UpdateUtvalgForIntegration");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateUtvalgForIntegration: " + exception.Message);
            }
        }

        /// <summary>
        /// Metoden laster reolene for å kunne kjøre forretningslaget i frikobling av lister etterpå
        ///  </summary>
        /// <param name="utv"></param>
        ///  <remarks></remarks>

        public async Task GetUtvalgReolerForIntegration(Puma.Shared.Utvalg utv)
        {
            await GetUtvalgReoler(utv);
        }

        /// <summary>
        ///  Metoden oppdaterer reolmapnavn under frikobling et utvalg
        ///  Kalles fra UpdateDisconnectUtvalgForIntegration og BasisUtvalgManager.cs sin DisconnectUtvalg
        ///  </summary>
        ///  <param name="utv"></param>
        ///  <param name="username"></param>
        ///  <remarks></remarks>


        public async Task UpdateReolMapnameForDisconnectedUtvalg(Puma.Shared.Utvalg utv, string username)
        {
            try
            {
                if (utv.WasBasedOn > 0 | utv.BasedOn > 0)
                {
                    _logger.LogDebug("Preparing the data for UpdateReolMapnameForDisconnectedUtvalg");

                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                    int result;
                    //string modificationInfo = "UpdateReolMapnameForDisconnectedUtvalg - ";

                    #region Parameter assignement

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Integer);
                    if (utv.WasBasedOn > 0)
                        npgsqlParameters[1].Value = utv.WasBasedOn;
                    else
                        npgsqlParameters[1].Value = utv.BasedOn;

                    //npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                    //modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                    //npgsqlParameters[2].Value = modificationInfo;

                    //npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                    //npgsqlParameters[3].Value = username;

                    #endregion

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.updatereolmapnamefordisconnectedutvalg", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    if (result == 0)
                        _logger.LogWarning("Oppdaterer av reolmapnavn under frikobling av utvalg endret ingen rader.Utvalgsid: " + utv.UtvalgId);

                    await SaveUtvalgModifications(utv, username, "UpdateReolMapnameForDisconnectedUtvalg - ");


                    _logger.LogDebug("Exiting from UpdateReolMapnameForDisconnectedUtvalg");

                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateReolMapnameForDisconnectedUtvalg: " + exception.Message);
            }
        }


        /// <summary>
        ///  Metoden oppdaterer utvalget med skrivebeskyttelse.
        ///  </summary>
        ///  <param name="utv"></param>
        ///  <param name="username"></param>
        ///  <remarks></remarks>

        public async Task UpdateWriteprotectUtvalg(Puma.Shared.Utvalg utv, string username)
        {
            try
            {
                _logger.LogDebug("Preparing the data for UpdateWriteprotectUtvalg");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
                int result;
                string modificationInfo = "UpdateWriteprotectUtvalg - ";

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Integer);
                //npgsqlParameters[1].Value = IIf(utv.Skrivebeskyttet, 1, 0);

                npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                npgsqlParameters[2].Value = modificationInfo;

                npgsqlParameters[3] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[3].Value = username;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updatewriteprotectutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Oppdatering av skrivebeskyttelse forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utv.UtvalgId);

                await SaveUtvalgModifications(utv, username, "UpdateReolMapnameForDisconnectedUtvalg - ");

                _logger.LogDebug("Exiting from UpdateWriteprotectUtvalg");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateWriteprotectUtvalg: " + exception.Message);
            }
        }

        /// <summary>
        ///  Delete Utvalg data
        ///  </summary>
        ///  <param name="utvalgId"> Utvalg ID</param>
        ///  <param name="username"></param>
        ///  <remarks></remarks>

        public async Task DeleteUtvalg(int utvalgId, string username)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for DeleteUtvalg");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                int result;
                // int utvalgListId = await GetUtvalgListId(utvalgId);

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = utvalgId;

                //npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
                //npgsqlParameters[1].Value = utvalgListId;

                //npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                //npgsqlParameters[2].Value = username;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }

                _logger.LogDebug("Exiting from DeleteUtvalg");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteUtvalg: " + exception.Message);
            }

        }

        /// <summary>
        ///  Delete old Utvalg data
        ///  </summary>
        ///  <param name="utvalgId"> Utvalg ID</param>
        ///  <remarks></remarks>

        public async Task DeleteFromUtvalgOldReol(int utvalgId)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for DeleteFromUtvalgOldReol");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utvalgId;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deletefromutvalgoldreol", CommandType.StoredProcedure, npgsqlParameters);
                }

                _logger.LogDebug("Exiting from DeleteFromUtvalgOldReol");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteFromUtvalgOldReol: " + exception.Message);
            }
        }

        /// <summary>
        /// GetUtvalgListId
        /// Metoden returnerer utvalgslisteid til ett utvalg med en gitt utvalgId.
        /// Dersom utvalget ikke har noen utvalgslisteid returneres verdien -1
        /// </summary>
        /// <param name="utvalgId"></param>
        /// <returns></returns>

        public async Task<int> GetUtvalgListId(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListId");
            DataTable utvData;
            DataRow row;
            int utvalgListId = -1;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListId: " + exception.Message);
                throw;
            }
            if (utvData.Rows.Count > 0)
            {
                row = utvData.Rows[0];
                if (!Convert.IsDBNull(row[0]))
                    utvalgListId = Convert.ToInt32(row["r_utvalglistid"]);
            }
            _logger.LogInformation("Result is: ", utvalgListId);

            _logger.LogDebug("Exiting from GetUtvalgListId");
            return utvalgListId;

        }

        /// <summary>
        ///     ''' Metoden hent elste ID uten flagg 
        ///     ''' </summary>
        ///     ''' <remarks></remarks>

        public async Task<List<UtvalgBasisFordeling>> FindBasisUtvalgFordelingToSend()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for FindBasisUtvalgFordelingToSend");
            DataTable utvData;
            List<UtvalgBasisFordeling> result = new List<UtvalgBasisFordeling>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            npgsqlParameters = null;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.findbasisutvalgfordelingtosend", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FindBasisUtvalgFordelingToSend: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvData.Rows)
            {
                UtvalgBasisFordeling ubf = new UtvalgBasisFordeling();
                ubf.ID = (int)row["r_utvalgid"];
                ubf.Utvalgtype = row["r_utvalgtype"].ToString();
                ubf.Dato = (DateTime)row["r_dato"];
                result.Add(ubf);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from FindBasisUtvalgFordelingToSend");
            return result;
        }

        /// <summary>
        ///     ''' Metoden oppdaterer BasisUtvalgFordelings tabellen med sendt til OEBS info
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>

        public async Task UpdateBasisUtvalgFordelingOppdatering(UtvalgBasisFordeling ubf)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateBasisUtvalgFordelingOppdatering");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            int result;

            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("p_isoebs", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = IIf(ubf.IsSendtOEBS, 1, 0);

            npgsqlParameters[1] = new NpgsqlParameter("p_datooebs", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[1].Value = DateTime.Now;

            npgsqlParameters[2] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[2].Value = ubf.ID;

            npgsqlParameters[3] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            npgsqlParameters[3].Value = ubf.Utvalgtype;

            npgsqlParameters[4] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[4].Direction = ParameterDirection.Output;

            #endregion
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updatebasisutvalgfordelingoppdatering", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FindBasisUtvalgFordelingToSend: " + exception.Message);
                throw;
            }
            if (result == 0)
                _logger.LogWarning("Oppdatering av Basisutvalgsfordeling forsøkte å oppdatere et linje som ikke eksisterer i KSPU.Utvalgsid: " + ubf.ID + " Type: " + ubf.Utvalgtype + " Dato: " + ubf.Dato);

            result = Convert.ToInt32(npgsqlParameters[5].Value);
            _logger.LogInformation(string.Format("Number of row affected {0} for Userid {1}", result));

            _logger.LogDebug("Exiting from UpdateBasisUtvalgFordelingOppdatering");

        }


        public async Task<string[]> UtvalgNamesExists(string[] utvalgNames)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UtvalgNamesExists");
            List<string> existingNames = new List<string>();
            //int i = 0;
            //int j = 0;
            StringBuilder nameSql = new StringBuilder();

            //foreach (string name in utvalgNames)
            //{
            //    nameSql.Append("upper(name) = ");
            //    nameSql.Append(":Name"); nameSql.Append(i.ToString());
            //    nameSql.Append(" OR ");
            //    AddParameterString(cmd, "Name" + i.ToString(), name.ToUpper(), 100);
            //    i += 1;
            //    j += 1;
            //    if (i == 10 || j == utvalgNames.Length)
            //    {
            //        i = 0;
            //        nameSql.Remove(nameSql.Length - 4, 3); // remove last 'OR '
            //        cmd.CommandText = " SELECT NAME FROM KSPU_DB.Utvalg WHERE " + nameSql.ToString();
            //        DataTable result = GetDataTable(cmd);
            //        foreach (DataRow row in result.Rows)
            //            existingNames.Add(GetStringFromRow(row, "name"));

            //        nameSql.Remove(0, nameSql.Length);
            //        if (!cmd.Parameters == null)
            //            cmd.Parameters.Clear();
            //        cmd.CommandText = "";
            //    }
            //}


            return existingNames.ToArray();
        }


        /// <summary>
        ///     ''' Metoden oppretter BasisUtvalgFordelings tabellen med ider som skal legges i køen
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>


        public async Task CreateBasisUtvalgFordelingOppdatering(UtvalgBasisFordeling ubf)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for CreateBasisUtvalgFordelingOppdatering");
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_dato", NpgsqlTypes.NpgsqlDbType.Date);
                npgsqlParameters[0].Value = ubf.Dato;

                npgsqlParameters[1] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = ubf.ID;

                npgsqlParameters[2] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = ubf.Utvalgtype;
                #endregion Parameter assignement
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.createbasisutvalgfordelingoppdatering", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CreateBasisUtvalgFordelingOppdatering: " + exception.Message);
            }


            _logger.LogDebug("Exiting from UpdateBasisUtvalgFordelingOppdatering");
        }

        /// <summary>
        ///     Metoden sjekker om utvalgid og type allerede finnes i køen
        ///     </summary>
        ///      <param name="ubf"></param>
        ///     <remarks></remarks>

        public async Task<bool> BasisUtvalgFordelingExistsOnQue(UtvalgBasisFordeling ubf)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Preparing the data for BasisUtvalgFordelingExistsOnQue");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            object count;
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = ubf.ID;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            npgsqlParameters[1].Value = ubf.Utvalgtype;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    count = dbhelper.ExecuteScalar<int>("kspu_db.basisutvalgfordelingexistsonque", CommandType.StoredProcedure, npgsqlParameters);
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in BasisUtvalgFordelingExistsOnQue: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from BasisUtvalgFordelingExistsOnQue");
            //object count = ExecuteScalar(cmd);
            if (count == null | (count) is DBNull)
                return false;
            return System.Convert.ToInt32(count) > 0;

        }

        /// <summary>
        ///     ''' Metoden oppretter BasisUtvalgFordelings tabellen med ider som skal legges i køen, dersom id og type ikke allerede ligger på køen klar til å sendes.
        ///     ''' </summary>
        ///     ''' <param name="ubf"></param>
        ///     ''' <remarks></remarks>

        public async Task SendBasisUtvalgFordelingToQue(UtvalgBasisFordeling ubf)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SendBasisUtvalgFordelingToQue");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            int result;

            #region Parameter assignement

            npgsqlParameters[0] = new NpgsqlParameter("p_insertdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
            npgsqlParameters[0].Value = ubf.Dato;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = ubf.ID;

            npgsqlParameters[2] = new NpgsqlParameter("p_utvalgtype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            npgsqlParameters[2].Value = ubf.Utvalgtype;

            npgsqlParameters[3] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[3].Direction = ParameterDirection.Output;

            #endregion
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.sendbasisutvalgfordelingtoque", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SendBasisUtvalgFordelingToQue: " + exception.Message);
                throw;
            }
            if (result == 0)
                _logger.LogWarning("Oppdatering av Basisutvalgsfordeling forsøkte å oppdatere et linje som ikke eksisterer i KSPU.Utvalgsid: " + ubf.ID + " Type: " + ubf.Utvalgtype + " Dato: " + ubf.Dato);

            result = Convert.ToInt32(npgsqlParameters[3].Value);
            _logger.LogInformation(string.Format("Number of row affected {0}: ", result));

            _logger.LogDebug("Exiting from SendBasisUtvalgFordelingToQue");
        }

        /// <summary>
        ///     ''' Checks if distribution is in the next x days ( X = Config.IgnoreNrOfDaysToDelivery ) 
        ///     ''' </summary>
        ///     ''' <param name="idsU"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks> Will also check if any contents of the list has distribution in the next x days</remarks>

        public async Task<List<int>> CheckIfUtvalgListsDistributionIsToClose(int[] idsU)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for CheckIfUtvalgListsDistributionIsToClose");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            foreach (string idList in CreateInClauses(idsU, 1000))
            {
                {
                    npgsqlParameters[0] = new NpgsqlParameter("p_idlist", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[0].Value = idList;

                    npgsqlParameters[1] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[1].Value = DateTime.Today;

                    npgsqlParameters[2] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Varchar);
                    // npgsqlParameters[2].Value = DateTime.Today.AddDays(Config.IgnoreNrOfDaysToDelivery);
                    try
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.checkifutvalglistsdistributionistoclose", CommandType.StoredProcedure, npgsqlParameters);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error in CheckIfUtvalgListsDistributionIsToClose: " + exception.Message);
                        throw;
                    }
                    foreach (DataRow row in utvData.Rows)
                    {
                        int id = (int)row["r_id"];
                        resultDict[id] = id;
                    }
                }
            }
            _logger.LogInformation("Number of row returned: ", resultDict.Count);

            _logger.LogDebug("Exiting from CheckIfUtvalgListsDistributionIsToClose");
            return new List<int>(resultDict.Values);
        }

        /// <summary>
        ///     ''' Check If UtvalgLists Need On The Fly Update
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks> Will also check if any contents of the list has distribution in the next x days</remarks>

        public async Task<List<int>> CheckIfUtvalgListsNeedOnTheFlyUpdate(IEnumerable<int> utvalgListIDs)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for CheckIfUtvalgListsNeedOnTheFlyUpdate");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            utvalgListIDs = ReplaceCampaignListIDsWithBasisListIDs(utvalgListIDs);
            foreach (int id in CheckIfParentUtvalgListsNeedOnTheFlyUpdate(utvalgListIDs))
                resultDict[id] = id;
            foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            {
                {
                    npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[0].Value = str;
                    try
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.CheckIfUtvalgListsNeedOnTheFlyUpdate", CommandType.StoredProcedure, npgsqlParameters);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error in CheckIfParentUtvalgListsNeedOnTheFlyUpdate: " + exception.Message);
                        throw;
                    }
                    foreach (DataRow row in utvData.Rows)
                    {
                        int id = (int)row["utvalglistid"];
                        resultDict[id] = id;
                    }
                }
            }
            _logger.LogInformation("Number of row returned: ", resultDict.Count);

            _logger.LogDebug("Exiting from CheckIfUtvalgListsDistributionIsToClose");
            return new List<int>(resultDict.Values);
        }

        /// <summary>
        ///     ''' Find Expired Utvalg IDs
        ///     ''' </summary>

        public async Task<List<int>> FindExpiredUtvalgIDs()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for FindExpiredUtvalgIDs");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];
            List<int> result = new List<int>();

            string table = _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result;
            npgsqlParameters[0] = new NpgsqlParameter("p_dato", NpgsqlTypes.NpgsqlDbType.Timestamp);
            npgsqlParameters[0].Value = DateTime.Today.AddMonths(-24);

            npgsqlParameters[1] = new NpgsqlParameter("p_dato2", NpgsqlTypes.NpgsqlDbType.Timestamp);
            npgsqlParameters[1].Value = DateTime.Today.AddMonths(-2);

            npgsqlParameters[2] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[2].Value = "kundeweb_u";

            npgsqlParameters[3] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Boolean);
            npgsqlParameters[3].Value = true;

            npgsqlParameters[4] = new NpgsqlParameter("p_systemuser", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[4].Value = "SystemUser";
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.findexpiredutvalgids", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FindExpiredUtvalgIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvData.Rows)
                result.Add((int)row["r_utvalgid"]);

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from FindExpiredUtvalgIDs");
            return result;
        }

        /// <summary>
        ///      Gets a list of the reol ids in the UtvalgReol table for a given utvalg.
        ///      </summary>
        ///      <param name="utvalgID"></param>
        ///      <returns></returns>

        public async Task<List<long>> GetUtvalgReolIDs(int utvalgID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgReolIDs");
            DataTable reolID;
            List<long> result = new List<long>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    reolID = dbhelper.FillDataTable("kspu_db.getutvalgreolids", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgReolIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reolID.Rows)
                result.Add((long)(Convert.ToDouble(row["r_reolid"])));
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetUtvalgReolIDs");
            return result;
        }

        public async Task<List<long>> GetUtvalgReolIDsWithCurrentReolTable(int utvalgID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgReolIDs");
            DataTable reolID;
            List<long> result = new List<long>();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    reolID = dbhelper.FillDataTable("kspu_db.getutvalgreolidsWithCurrentTable", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgReolIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reolID.Rows)
                result.Add((long)(Convert.ToDouble(row["r_reolid"])));
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetUtvalgReolIDs");
            return result;
        }

        public async Task<long> SaveUtvalgData(Puma.Shared.Utvalg utvData, string userName, bool saveOldReoler = false, bool skipHistory = false, int forceUtvalgListId = 0)
        {
            utvData.Name = utvData.Name.Trim();
            if (utvData.Name.Trim().Length < 3)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgName);
            if (utvData.Name.Trim().IndexOf(">") > -1 | utvData.Name.IndexOf("<") > -1)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgIllegalCharsUtv);
            if (utvData.Name.Trim().IndexOf(" ", 0, 1) > -1 | utvData.Name.Trim().IndexOf(" ", utvData.Name.Length - 1, 1) > -1)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgNameWithSpaces);
            if (utvData.TotalAntall == 0)
                throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgHasNoReceivers);

            _logger.LogDebug("Preparing the data for SaveUtvalgDataRepository");
            _utvalgListRepository = _services.GetService<IUtvalgListRepository>();
            int UtvalgListId;
            if (forceUtvalgListId > 0)
                UtvalgListId = forceUtvalgListId;
            else
                UtvalgListId = await GetUtvalgListId(utvData.UtvalgId);

            // HACK: Remove duplicate receivers
            ArrayList foundIds = new ArrayList();
            foreach (UtvalgReceiver r in new ArrayList(utvData.Receivers))
            {
                if (foundIds.Contains(r.ReceiverId))
                    utvData.Receivers.Remove(r);
                else
                    foundIds.Add(r.ReceiverId);
            }
            // HACK: Remove duplicate reoler
            ArrayList foundReolIds = new ArrayList();
            foreach (Puma.Shared.Reol r in new ArrayList(utvData.Reoler))
            {
                if (foundReolIds.Contains(r.ReolId))
                    utvData.Reoler.Remove(r);
                else
                    foundReolIds.Add(r.ReolId);
            }

            utvData.ReolMapName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
            if (utvData.ReolMapName == null | utvData.ReolMapName == "")
                utvData.ReolMapName = "UNKNOWN_REOLMAP";
            // HACK: Make sure we always have receivers and criteria
            // If utv.Receivers.Count = 0 Then utv.Receivers.IncludeReceiver(ReceiverType.Households, True) 'Kjetil: Not needed any more as a new utvalg automatically gets default receivers + if user has chosen not to include any users this will overwrite it when saving the utvalg. 
            if (utvData.Criterias.Count == 0)
            {
                UtvalgCriteria uc = new UtvalgCriteria();
                uc.CriteriaType = CriteriaType.SelectedInMap;
                uc.Criteria = "";
                utvData.Criterias.Add(uc);
            }
            utvData.AntallWhenLastSaved = utvData.CalculateTotalAntall();
            int updateResult = 0;

            if (utvData.UtvalgId != 0)
            {
                _logger.LogDebug("Calling the UpdateUtvalg");
                //Update utvalg in db
                updateResult = await UpdateUtvalg(utvData);
            }

            if (utvData.UtvalgId == 0)
                utvData.UtvalgId = await GetSequenceNextVal("KSPU_DB.UtvalgId_Seq");

            // If reserved are included, set utvalg to write protected
            if (utvData.Receivers?.Any() == true)
            {
                utvData.Skrivebeskyttet = utvData.Receivers.Where(x => x.ReceiverId == ReceiverType.FarmersReserved || x.ReceiverId == ReceiverType.HouseholdsReserved || x.ReceiverId == ReceiverType.HousesReserved)?.Any() == true;
                #region Old Code
                //foreach (UtvalgReceiver uRec in utvData.Receivers)
                //{
                //    if (uRec.ReceiverId == ReceiverType.FarmersReserved || uRec.ReceiverId == ReceiverType.HouseholdsReserved || uRec.ReceiverId == ReceiverType.HousesReserved)
                //    {
                //        utvData.Skrivebeskyttet = true;
                //    }
                //} 
                #endregion
            }
            //if utvalg id is there but still data update returns 0 then consider it as insert
            if (updateResult == 0)
            {
                _logger.LogDebug("Calling the SaveUtvalg");
                _ = await SaveUtvalg(utvData);
            }

            if (!(utvData.BasedOn > 0))
            {
                _logger.LogDebug("Calling the SaveUtvalgReoler");
                await SaveUtvalgReolerBulk(utvData);
            }
            if (!skipHistory)
            {
                _logger.LogDebug("Calling the SaveUtvalgModifications");
                await SaveUtvalgModifications(utvData, userName, "SaveUtvalg - ");

                // Save utvalglistmodification if utvalg connected to a list
                if (UtvalgListId > 0)
                {
                    var utvalgList = await _utvalgListRepository.GetUtvalgList(UtvalgListId);
                    await _utvalgListRepository.SaveUtvalgListData(utvalgList, userName);
                }

            }

            await SaveUtvalgKommuner(utvData);
            if (!(utvData.BasedOn > 0))
                await SaveUtvalgReceiver(utvData);
            await SaveUtvalgDistrict(utvData);
            await SaveUtvalgPostalZone(utvData);
            await SaveUtvalgCriteria(utvData);
            if (utvData.IsBasis)
            {
                await SaveUtvalgAntallToInheritors(utvData);
                // DONE: Update antall in list for utvalglists containing inheritors
                // Not necessary, because inheritors can not be part of an utvalg list. (They can only be based on a basisutvalg which can be part of a basislist)
            }

            if (saveOldReoler)
            {
                // Save reolerbeforerecreation to UtvalgOldReol table if that table is currently empty.
                if (!await UtvalgHasRecreatedBefore(utvData.UtvalgId))
                {
                    await SaveUtvalgPreviousReoler(utvData);
                    await UpdateOldReolMapName(utvData);
                }
            }
            else
            {
                await DeleteFromUtvalgOldReol(utvData.UtvalgId);
                utvData.OldReolMapName = "";
                utvData.ReolerBeforeRecreation = null;
                await UpdateOldReolMapName(utvData);
            }

            if (UtvalgListId > 0)
            {
                await _utvalgListRepository.UpdateAntallInList(UtvalgListId);
            }

            //TODO : We have to check this as in pervious code we are using utvalg list object which is commented for now.
            if (!string.IsNullOrEmpty(utvData.ListId))
            {
                await _utvalgListRepository.UpdateAntallInList(Convert.ToInt32(utvData.ListId));
            }

            if ((userName ?? "") != (await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SystemUserName) ?? ""))
            {
                await _gjenskapUtvalgRepository.DeleteFromOldUtvalgGeometryIfPresent(utvData.UtvalgId);
            }

            return utvData.UtvalgId;
        }
        public async Task<int> SaveUtvalg(Puma.Shared.Utvalg utvalgData)
        {
            await Task.Run(() => { });
            //return 1;
            _logger.LogDebug("Preparing the data for SaveUtvalg");
            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[22];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalgData.UtvalgId;

            npgsqlParameters[1] = new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = utvalgData.Name;

            npgsqlParameters[2] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = utvalgData.Logo;

            npgsqlParameters[3] = new NpgsqlParameter("p_reolmapname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = utvalgData.ReolMapName;

            npgsqlParameters[4] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[4].Value = Int32.TryParse(utvalgData.ListId, out int numValue) ? numValue : DBNull.Value;

            npgsqlParameters[5] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[5].Value = utvalgData.TotalAntall;

            npgsqlParameters[6] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = (int)utvalgData.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalgData.OrdreType);

            npgsqlParameters[7] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[7].Value = !string.IsNullOrWhiteSpace(utvalgData.OrdreReferanse) ? utvalgData.OrdreReferanse : DBNull.Value;

            npgsqlParameters[8] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[8].Value = (int)utvalgData.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalgData.OrdreStatus);

            npgsqlParameters[9] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = utvalgData.KundeNummer;

            npgsqlParameters[10] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[10].Value = (utvalgData.InnleveringsDato != DateTime.MinValue ? utvalgData.InnleveringsDato : DateTime.Now.Date);

            npgsqlParameters[11] = new NpgsqlParameter("p_oldreolmapname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[11].Value = !string.IsNullOrWhiteSpace(utvalgData.OldReolMapName) ? utvalgData.OldReolMapName : DBNull.Value;


            npgsqlParameters[12] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[12].Value = utvalgData.Skrivebeskyttet == true ? 1 : 0;

            npgsqlParameters[13] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[13].Value = utvalgData.Avtalenummer;

            npgsqlParameters[14] = new NpgsqlParameter("p_arealavvik", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[14].Value = utvalgData.ArealAvvik;

            npgsqlParameters[15] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[15].Value = utvalgData.IsBasis == true ? 1 : 0;

            npgsqlParameters[16] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[16].Value = utvalgData.BasedOn;

            npgsqlParameters[17] = new NpgsqlParameter("p_wasbasedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[17].Value = utvalgData.WasBasedOn;

            npgsqlParameters[18] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[18].Value = utvalgData.Weight;

            npgsqlParameters[19] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.Date);
            npgsqlParameters[19].Value = utvalgData.DistributionDate;

            npgsqlParameters[20] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[20].Value = (int)utvalgData.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalgData.DistributionType);

            npgsqlParameters[21] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[21].Value = utvalgData.Thickness;

            //npgsqlParameters[22] = new NpgsqlParameter("p_username", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[22].Value = 'o';

            //npgsqlParameters[23] = new NpgsqlParameter("p_forceUtvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[23].Value = 0;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in SaveUtvalg: " + exception.Message);
                    throw;
                }
            }

            _logger.LogDebug("Exiting from SaveUtvalg");

            return Convert.ToInt32(result);
        }

        public async Task<int> UpdateUtvalg(Puma.Shared.Utvalg utvalgData)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Preparing the data for Update Utvalg");
            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[22];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalgData.UtvalgId;

            npgsqlParameters[1] = new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = utvalgData.Name;

            npgsqlParameters[2] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = utvalgData.Logo;

            npgsqlParameters[3] = new NpgsqlParameter("p_reolmapname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = utvalgData.ReolMapName;

            npgsqlParameters[4] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[4].Value = Int32.TryParse(utvalgData.ListId, out int numValue) ? numValue : DBNull.Value;

            npgsqlParameters[5] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[5].Value = utvalgData.TotalAntall;

            npgsqlParameters[6] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = (int)utvalgData.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalgData.OrdreType);

            npgsqlParameters[7] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[7].Value = !string.IsNullOrWhiteSpace(utvalgData.OrdreReferanse) ? utvalgData.OrdreReferanse : DBNull.Value;

            npgsqlParameters[8] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[8].Value = (int)utvalgData.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalgData.OrdreStatus);

            npgsqlParameters[9] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = !string.IsNullOrWhiteSpace(utvalgData.KundeNummer) ? utvalgData.KundeNummer : DBNull.Value;

            npgsqlParameters[10] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[10].Value = (utvalgData.InnleveringsDato != DateTime.MinValue ? utvalgData.InnleveringsDato : DateTime.Now.Date);

            npgsqlParameters[11] = new NpgsqlParameter("p_oldreolmapname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[11].Value = utvalgData.OldReolMapName;


            npgsqlParameters[12] = new NpgsqlParameter("p_skrivebeskyttet", NpgsqlTypes.NpgsqlDbType.Smallint);
            npgsqlParameters[12].Value = utvalgData.Skrivebeskyttet == true ? 1 : 0;

            npgsqlParameters[13] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[13].Value = utvalgData.Avtalenummer;

            npgsqlParameters[14] = new NpgsqlParameter("p_arealavvk", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[14].Value = utvalgData.ArealAvvik;

            npgsqlParameters[15] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Smallint);
            npgsqlParameters[15].Value = utvalgData.IsBasis == true ? 1 : 0;

            npgsqlParameters[16] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[16].Value = utvalgData.BasedOn;

            npgsqlParameters[17] = new NpgsqlParameter("p_wasbasedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[17].Value = utvalgData.WasBasedOn;

            npgsqlParameters[18] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[18].Value = utvalgData.Weight;

            npgsqlParameters[19] = new NpgsqlParameter("p_distributjondato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[19].Value = utvalgData.DistributionDate;

            npgsqlParameters[20] = new NpgsqlParameter("p_distributjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[20].Value = (int)utvalgData.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalgData.DistributionType);

            npgsqlParameters[21] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[21].Value = utvalgData.Thickness;

            //npgsqlParameters[22] = new NpgsqlParameter("p_username", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[22].Value = 'o';

            //npgsqlParameters[23] = new NpgsqlParameter("p_forceUtvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[23].Value = 0;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in UpdateUtvalg: " + exception.Message);
                    throw;
                }
            }

            _logger.LogDebug("Exiting from UpdateUtvalg");

            return Convert.ToInt32(result);


        }

        public async Task SaveUtvalgReoler(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Preparing the data for SaveUtvalgReoler");

            NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

            npgsqlParameters1[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters1[0].Value = utv.UtvalgId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgreol", CommandType.StoredProcedure, npgsqlParameters1);
            }

            foreach (Puma.Shared.Reol r in utv.Reoler)
            {
                try
                {
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                    int result;

                    #region Parameter assignement

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[1].Value = r.ReolId;

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgreoler", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    #endregion
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
                }


            }

            _logger.LogDebug("Exiting from SaveUtvalgReoler");
        }




        public async Task SaveMultipleUtvalgData(List<Puma.Shared.Utvalg> utvsData, string userName, bool saveOldReoler = false, bool skipHistory = false, int forceUtvalgListId = 0)
        {

            List<UtvalgReceiverEntity> lstReceivers = new List<UtvalgReceiverEntity>();
            List<UtvalgKommuneEntity> lstKomunes = new List<UtvalgKommuneEntity>();
            List<UtvalgDistrictEntity> lstdistrict = new List<UtvalgDistrictEntity>();
            List<UtvalgPostalZoneEntity> lstPostalZone = new List<UtvalgPostalZoneEntity>();
            List<UtvalgCriteriaEntity> lstCriteria = new List<UtvalgCriteriaEntity>();
            List<UtvalgModificationEntity> utvalgModifications = new List<UtvalgModificationEntity>();
            List<ReolSave> lstReolSave = new List<ReolSave>();

            List<UtvalgEntity> lstUtvalg = new List<UtvalgEntity>();

            foreach (var utvData in utvsData)
            {
                utvData.Name = utvData.Name.Trim();
                if (utvData.Name.Trim().Length < 3)
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgName);
                if (utvData.Name.Trim().IndexOf(">") > -1 | utvData.Name.IndexOf("<") > -1)
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgIllegalCharsUtv);
                if (utvData.Name.Trim().IndexOf(" ", 0, 1) > -1 | utvData.Name.Trim().IndexOf(" ", utvData.Name.Length - 1, 1) > -1)
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgNameWithSpaces);
                if (utvData.TotalAntall == 0)
                    throw new Exception(Puma.DataLayer.BusinessEntity.Constants.errMsgUtvalgHasNoReceivers);

                _logger.LogDebug("Preparing the data for SaveUtvalgDataRepository");

                //_utvalgListRepository = _services.GetService<IUtvalgListRepository>();

                int UtvalgListId = Convert.ToInt32(utvData.ListId);

                // HACK: Remove duplicate receiers
                ArrayList foundIds = new ArrayList();
                foreach (UtvalgReceiver r in new ArrayList(utvData.Receivers))
                {
                    if (foundIds.Contains(r.ReceiverId))
                        utvData.Receivers.Remove(r);
                    else
                        foundIds.Add(r.ReceiverId);
                }
                // HACK: Remove duplicate reoler
                ArrayList foundReolIds = new ArrayList();
                foreach (Puma.Shared.Reol r in new ArrayList(utvData.Reoler))
                {
                    if (foundReolIds.Contains(r.ReolId))
                        utvData.Reoler.Remove(r);
                    else
                        foundReolIds.Add(r.ReolId);
                }

                utvData.ReolMapName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);
                if (utvData.ReolMapName == null | utvData.ReolMapName == "")
                    utvData.ReolMapName = "UNKNOWN_REOLMAP";
                // HACK: Make sure we always have receivers and criteria
                // If utv.Receivers.Count = 0 Then utv.Receivers.IncludeReceiver(ReceiverType.Households, True) 'Kjetil: Not needed any more as a new utvalg automatically gets default receivers + if user has chosen not to include any users this will overwrite it when saving the utvalg. 
                if (utvData.Criterias.Count == 0)
                {
                    UtvalgCriteria uc = new UtvalgCriteria();
                    uc.CriteriaType = CriteriaType.SelectedInMap;
                    uc.Criteria = "";
                    utvData.Criterias.Add(uc);
                }
                utvData.AntallWhenLastSaved = utvData.CalculateTotalAntall();

                //if (utvData.UtvalgId == 0)
                //    utvData.UtvalgId = await GetSequenceNextVal("KSPU_DB.UtvalgId_Seq");

                // If reserved are included, set utvalg to write protected
                if (utvData.Receivers?.Any() == true)
                {
                    utvData.Skrivebeskyttet = utvData.Receivers.Where(x => x.ReceiverId == ReceiverType.FarmersReserved || x.ReceiverId == ReceiverType.HouseholdsReserved || x.ReceiverId == ReceiverType.HousesReserved)?.Any() == true;
                }
                //if utvalg id is there but still data update returns 0 then consider it as insert
                //if (updateResult == 0)
                //{
                //    _logger.LogDebug("Calling the SaveUtvalg");
                //    _ = await SaveUtvalg(utvData);
                //}



                if (!skipHistory)
                {
                    _logger.LogDebug("Calling the SaveUtvalgModifications");
                    //await SaveUtvalgModifications(utvData, userName, "SaveUtvalg - ");

                    utvalgModifications.Add(new UtvalgModificationEntity()
                    {
                        ModificationId = await GetSequenceNextVal("KSPU_DB.UtvalgModificationId_Seq"),
                        ModificationTime = DateTime.Now,
                        UserId = userName,
                        UtvalgId = utvData.UtvalgId,
                        info = "SaveUtvalg -  Utvalgets antall ved sist lagring: " + utvData.AntallWhenLastSaved.ToString()
                    });

                    // Save utvalglistmodification if utvalg connected to a list
                    //if (UtvalgListId > 0)
                    //{
                    //    var utvalgList = await _utvalgListRepository.GetUtvalgList(UtvalgListId);
                    //    await _utvalgListRepository.SaveUtvalgListData(utvalgList, userName);
                    //}

                }

                //await SaveUtvalgKommuner(utvData);
                lstKomunes.AddRange(utvData.Kommuner?.Select(x => new UtvalgKommuneEntity() { KommuneId = x.KommuneId, KommuneMapName = x.KommuneMapName, UtvalgId = utvData.UtvalgId }));


                if (!(utvData.BasedOn > 0))
                {
                    //await SaveUtvalgReceiver(utvData);
                    lstReceivers.AddRange(utvData.Receivers.Where(x => x.ReceiverId > 0)?.Select(x => new UtvalgReceiverEntity() { ReceiverId = (int)x.ReceiverId, Selected = x.Selected ? 1 : 0, UtvalgId = utvData.UtvalgId }));
                }


                //await SaveUtvalgDistrict(utvData);
                lstdistrict.AddRange(utvData.Districts.Select(x => new UtvalgDistrictEntity() { DistrictId = x.DistrictId, DistrictMapName = x.DistrictMapName, UtvalgId = utvData.UtvalgId }));

                //await SaveUtvalgPostalZone(utvData);
                lstPostalZone.AddRange(utvData.PostalZones.Select(x => new UtvalgPostalZoneEntity() { PostalZone = x.PostalZone, PostalZoneMapName = x.PostalZoneMapName, UtvalgId = utvData.UtvalgId }));


                //await SaveUtvalgCriteria(utvData);
                foreach (var itemCriteria in utvData.Criterias)
                {
                    lstCriteria.Add(new UtvalgCriteriaEntity()
                    {
                        CriteriaId = await GetSequenceNextVal("KSPU_DB.CriteriaId_Seq"),
                        Criteria = itemCriteria.Criteria,
                        CriteriaType = (int)itemCriteria.CriteriaType,
                        UtvalgId = utvData.UtvalgId
                    });
                }



                //NOTE : commented as considering its new selections so it won't have and based on
                //if (utvData.IsBasis)
                //{
                //    await SaveUtvalgAntallToInheritors(utvData);
                //    // DONE: Update antall in list for utvalglists containing inheritors
                //    // Not necessary, because inheritors can not be part of an utvalg list. (They can only be based on a basisutvalg which can be part of a basislist)
                //}


                //Note : As we are saving new selection there is no need to delete old reol map data as there will be none
                //await DeleteFromUtvalgOldReol(utvData.UtvalgId);
                utvData.OldReolMapName = "";
                utvData.ReolerBeforeRecreation = null;


                if (UtvalgListId > 0)
                {
                    //TODO : Write this code once all the selections are saved 
                    //await _utvalgListRepository.UpdateAntallInList(UtvalgListId);
                }

                //TODO : We have to check this as in pervious code we are using utvalg list object which is commented for now.
                //if (!string.IsNullOrEmpty(utvData.ListId))
                //{
                //    await _utvalgListRepository.UpdateAntallInList(Convert.ToInt32(utvData.ListId));
                //}

                //if ((userName ?? "") != (await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.SystemUserName) ?? ""))
                //{
                //    await _gjenskapUtvalgRepository.DeleteFromOldUtvalgGeometryIfPresent(utvData.UtvalgId);
                //}

                if (!(utvData.BasedOn > 0))
                {
                    lstReolSave.AddRange(utvData.Reoler.Select(x => new ReolSave() { ReolId = x.ReolId, UtvalgId = utvData.UtvalgId }));
                }

                UtvalgEntity utvalgEntity = new UtvalgEntity()
                {
                    Name = utvData.Name,
                    Logo = utvData.Logo,
                    UtvalgId = utvData.UtvalgId,
                    ReolMapName = utvData.ReolMapName,
                    AntallWhenLastSaved = utvData.AntallWhenLastSaved,
                    ArealAvvik = utvData.ArealAvvik,
                    Avtalenummer = utvData.Avtalenummer,
                    InnleveringsDato = utvData.InnleveringsDato != DateTime.MinValue ? utvData.InnleveringsDato : DateTime.Now,
                    BasedOn = utvData.BasedOn,
                    IsBasis = utvData.IsBasis ? 1 : 0,
                    KundeNummer = utvData.KundeNummer,
                    ListId = utvData.ListId,
                    ModificationDate = utvData.ModificationDate,
                    OldReolMapName = utvData.OldReolMapName,
                    Skrivebeskyttet = utvData.Skrivebeskyttet ? 1 : 0,
                    Thickness = utvData.Thickness,
                    TotalAntall = utvData.TotalAntall,
                    WasBasedOn = utvData.WasBasedOn,
                    Weight = utvData.Weight,
                    DistributionDate = utvData.DistributionDate,
                    DistributionType = (int)utvData.DistributionType == 0 ? null : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvData.DistributionType),
                    OrdreReferanse = !string.IsNullOrWhiteSpace(utvData.OrdreReferanse) ? utvData.OrdreReferanse : null,
                    OrdreStatus = (int)utvData.OrdreStatus == 0 ? null : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvData.OrdreStatus),
                    OrdreType = (int)utvData.OrdreType == 0 ? null : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvData.OrdreType)
                };

                lstUtvalg.Add(utvalgEntity);

            }

            //Once All data set call methods to save Data in Db
            await SaveUtvalgBulk(lstUtvalg);

            await SaveUtvalgReolerDataBulk(lstReolSave);

            await SaveUtvalgModificationsBulk(utvalgModifications);

            await SaveUtvalgKommunerBulk(lstKomunes);

            await SaveUtvalgReceiverBulk(lstReceivers);

            await SaveUtvalgDistrictBulk(lstdistrict);

            await SaveUtvalgPostalZoneBulk(lstPostalZone);

            await SaveUtvalgCriteriaBulk(lstCriteria);

        }


        public async Task SaveUtvalgBulk(List<UtvalgEntity> utvsData)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgEntity>("kspu_db", "utvalg")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapVarchar("name", x => x.Name)
                .MapVarchar("logo", x => x.Logo)
                .MapVarchar("reolmapname", x => x.ReolMapName)
                .MapNumeric("utvalglistid", x => Convert.ToInt64(x.ListId))
                .MapNumeric("antall", x => x.TotalAntall)
                 .MapVarchar("ordretype", x => x.OrdreType)
                .MapVarchar("ordrereferanse", x => x.OrdreReferanse)
                 .MapVarchar("ordrestatus", x => x.OrdreStatus)
                .MapVarchar("kundenummer", x => x.KundeNummer)
                .MapTimeStamp("innleveringsdato", x => x.InnleveringsDato)
                .MapVarchar("oldreolmapname", x => x.OldReolMapName)
                .MapSmallInt("skrivebeskyttet", x => Convert.ToInt16(x.Skrivebeskyttet))
                .MapBigInt("avtalenummer", x => x.Avtalenummer)
                .MapNumeric("arealavvik", x => Convert.ToDecimal(x.ArealAvvik))
                .MapSmallInt("isbasis", x => Convert.ToInt16(x.IsBasis))
                .MapNumeric("basedon", x => x.BasedOn)
                .MapNumeric("wasbasedon", x => x.WasBasedOn)
                .MapInteger("vekt", x => x.Weight)
                 .MapTimeStamp("distribusjonsdato", x => x.DistributionDate)
                  .MapVarchar("distribusjonstype", x => x.DistributionType)
                 .MapDouble("thickness", x => x.Thickness);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgEntity>(copyHelper, utvsData);
            }



            _logger.LogDebug("Exiting from SaveUtvalgBulk");

        }

        public async Task SaveUtvalgReolerBulk(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });

            _logger.LogDebug("Preparing the data for SaveUtvalgReoler");

            NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

            npgsqlParameters1[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters1[0].Value = utv.UtvalgId;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgreol", CommandType.StoredProcedure, npgsqlParameters1);
            }

            var copyHelper = new PostgreSQLCopyHelper<ReolSave>("kspu_db", "utvalgreol")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapNumeric("reolid", x => x.ReolId);

            List<ReolSave> lstReolSave = new List<ReolSave>();
            foreach (Puma.Shared.Reol r in utv.Reoler)
            {
                lstReolSave.Add(new ReolSave() { ReolId = r.ReolId, UtvalgId = utv.UtvalgId });


            }

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<ReolSave>(copyHelper, lstReolSave);
            }



            _logger.LogDebug("Exiting from SaveUtvalgReoler");
        }


        public async Task SaveUtvalgModificationsBulk(List<UtvalgModificationEntity> lstModifications)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgModificationEntity>("kspu_db", "utvalgmodification")

                .MapNumeric("utvalgmodificationid", x => x.ModificationId)
                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapTimeStamp("modificationdate", x => x.ModificationTime)
                .MapCharacter("userid", x => x.UserId)
                .MapVarchar("info", x => x.info);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgModificationEntity>(copyHelper, lstModifications);
            }

            _logger.LogDebug("Exiting from SaveUtvalgModificationBulk");
        }

        public async Task SaveUtvalgKommunerBulk(List<UtvalgKommuneEntity> lstUtvalgKomune)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgKommuneEntity>("kspu_db", "utvalgkommune")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapVarchar("kommuneid", x => x.KommuneId)
                .MapVarchar("kommunemapname", x => x.KommuneMapName);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgKommuneEntity>(copyHelper, lstUtvalgKomune);
            }

            _logger.LogDebug("Exiting from SaveUtvalgKommunerBulk");
        }


        public async Task SaveUtvalgReceiverBulk(List<UtvalgReceiverEntity> lstUtvalgReceiver)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgReceiverEntity>("kspu_db", "utvalgreceiver")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapNumeric("receiverid", x => x.ReceiverId)
                .MapSmallInt("selected", x => Convert.ToInt16(x.Selected));



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgReceiverEntity>(copyHelper, lstUtvalgReceiver);
            }

            _logger.LogDebug("Exiting from SaveUtvalgReceiverBulk");
        }

        public async Task SaveUtvalgDistrictBulk(List<UtvalgDistrictEntity> lstUtvalgDistrict)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgDistrictEntity>("kspu_db", "utvalgdistrict")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapVarchar("districtid", x => x.DistrictId)
                .MapVarchar("districtmapname", x => x.DistrictMapName);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgDistrictEntity>(copyHelper, lstUtvalgDistrict);
            }

            _logger.LogDebug("Exiting from SaveUtvalgReceiverBulk");
        }

        public async Task SaveUtvalgPostalZoneBulk(List<UtvalgPostalZoneEntity> lstUtvalgPostalZone)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgPostalZoneEntity>("kspu_db", "utvalgpostalzone")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapNumeric("postalzone", x => x.PostalZone)
                .MapVarchar("postalzonemapname", x => x.PostalZoneMapName);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgPostalZoneEntity>(copyHelper, lstUtvalgPostalZone);
            }

            _logger.LogDebug("Exiting from SaveUtvalgPostalZoneBulk");
        }

        public async Task SaveUtvalgCriteriaBulk(List<UtvalgCriteriaEntity> lstUtvalgCriteria)
        {
            await Task.Run(() => { });
            var copyHelper = new PostgreSQLCopyHelper<UtvalgCriteriaEntity>("kspu_db", "utvalgcriteria")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapNumeric("criteriaid", x => x.CriteriaId)
                .MapVarchar("criteria", x => x.Criteria)
                .MapNumeric("criteriatype", x => x.CriteriaType);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<UtvalgCriteriaEntity>(copyHelper, lstUtvalgCriteria);
            }

            _logger.LogDebug("Exiting from SaveUtvalgCriteriaBulk");
        }

        public async Task SaveUtvalgReolerDataBulk(List<ReolSave> lstReolSave)
        {
            await Task.Run(() => { });

            var copyHelper = new PostgreSQLCopyHelper<ReolSave>("kspu_db", "utvalgreol")

                .MapNumeric("utvalgid", x => x.UtvalgId)
                .MapNumeric("reolid", x => x.ReolId);


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            {
                dbhelper.WriteToDatabase<ReolSave>(copyHelper, lstReolSave);
            }



            _logger.LogDebug("Exiting from SaveUtvalgReolerDataBulk");
        }

        public class ReolSave
        {
            public int UtvalgId { get; set; }

            public long ReolId { get; set; }
        }

        /// <summary>
        ///     ''' Metoden frikobler et utvalget brukt fra integrasjon mot Ordre2, Normal forretningslagskall til KSPU.BusinessLayer.BasisUtvalgManager.DisconnectUtvalg fungerte ikke i integrasjon.
        ///     ''' (do NOT chanhe to anchestor)
        ///     ''' </summary>
        ///     ''' <param name="utv"></param>
        ///     ''' <param name="username"></param>
        ///     ''' <remarks></remarks>




        #endregion
        #region Private Methods
        private Puma.Shared.Utvalg GetUtvalgSearchFromDataRow(DataRow row)
        {
            Puma.Shared.Utvalg utv = new Puma.Shared.Utvalg();
            Utils utils = new Utils();
            utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);
            utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
            utv.Name = Convert.ToString(row["r_name"]);
            utv.Logo = row["r_logo"].ToString();
            utv.OrdreReferanse = Convert.ToString(row["r_ordrereferanse"]);
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), "Null");
            utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), "Null");
            utv.InnleveringsDato = Convert.ToDateTime(Convert.IsDBNull(row["r_innleveringsdato"]) ? null : (DateTime?)(row["r_innleveringsdato"]));
            utv.ReolMapName = Convert.ToString(row["r_reolmapname"]);
            utv.AntallWhenLastSaved = (long)Convert.ToDouble(row["r_antall"]);
            utv.OldReolMapName = Convert.ToString(row["r_oldreolmapname"]);
            utv.Skrivebeskyttet = Convert.ToInt32(row["r_skrivebeskyttet"]) > 0;
            utv.Weight = Convert.ToInt32(row["r_vekt"]);
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));
            utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), "Null");
            utv.ArealAvvik = Convert.ToInt32(row["r_arealAvvik"]);
            utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;
            utv.BasedOn = Convert.ToInt32(row["r_basedon"]);
            utv.WasBasedOn = Convert.ToInt32(row["r_wasbasedon"]);
            utv.Thickness = Convert.ToDouble(Convert.IsDBNull(row["r_thickness"]) ? null : (Double?)(row["r_thickness"]));
            //if (row["r_utvalglistid"] != null)
            //{
            //    utv.List = new UtvalgList();
            //    utv.List.ListId = Convert.ToInt32(Convert.IsDBNull(row["r_utvalglistid"]) ? null : (Decimal?)(row["r_utvalglistid"]));
            //}
            utv.ListId = Convert.ToString(row["r_utvalglistid"]);
            utv.TotalAntall = Convert.ToInt32(utv.AntallWhenLastSaved);
            if (row.Table.Columns.Contains("r_modificationDate"))
            {
                utv.ModificationDate = Convert.ToDateTime(Convert.IsDBNull(row["r_modificationDate"]) ? null : (DateTime?)(row["r_modificationDate"]));
            }
            if (!string.Equals(utv.ReolMapName, _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result, StringComparison.OrdinalIgnoreCase) && utv.BasedOn <= 0)
            {
                utv.IsRecreated = false;
            }
            return utv;
        }

        private UtvalgSearchResult GetUtvalgSearchResultFromDataRow(DataRow row)
        {
            UtvalgSearchResult utv = new UtvalgSearchResult();
            Utils utils = new Utils();
            utv.UtvalgName = Convert.ToString(row["r_name"]);
            utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);
            utv.Antall = Convert.ToInt32(row["r_antall"]);
            utv.ReolCount = GetUtvalgReolerCount(utv.UtvalgId).Result;
            utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;
            utv.BasedOn = Convert.ToInt32(row["r_basedOn"]);
            if (row["r_utvalgListId"] != null)
                //utv.List = DAUtvalgList.GetUtvalgListSimple(GetIntFromRow(row, "UtvalgListId"));
                utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), "Null");
            return utv;
        }

        private Puma.Shared.Utvalg GetUtvalgFromDataRow(DataRow row, bool includeReols = true, bool extendedInfo = true)
        {
            Puma.Shared.Utvalg utv = new Puma.Shared.Utvalg();
            Utils utils = new Utils();
            utv.UtvalgId = Convert.ToInt32(row["r_utvalgid"]);



            utv.KundeNummer = Convert.ToString(row["r_kundenummer"]);
            utv.Name = Convert.ToString(row["r_name"]);
            utv.Logo = Convert.ToString(row["r_logo"]);
            utv.OrdreReferanse = Convert.ToString(row["r_ordrereferanse"]);
            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null))))
                utv.OrdreType = 0;
            else
                utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null);
            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null))))
                utv.OrdreStatus = 0;
            else
                utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null);

            utv.InnleveringsDato = Convert.ToDateTime(Convert.IsDBNull(row["r_innleveringsdato"]) ? null : (DateTime?)(row["r_innleveringsdato"]));
            utv.ReolMapName = Convert.ToString(row["r_reolMapName"]);
            utv.AntallWhenLastSaved = (long)Convert.ToDouble(row["r_antall"]);
            utv.ArealAvvik = Convert.ToInt32(row["r_arealavvik"]);
            utv.Weight = Convert.ToInt32(row["r_vekt"]);
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));

            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null))))
                utv.DistributionType = 0;
            else
                utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null);
            utv.Skrivebeskyttet = Convert.ToInt32(row["r_skrivebeskyttet"]) > 0;
            //if (Convert.ToString(row["r_utvalglistid"]) != null)

            //{
            //utv.List = new UtvalgList();
            //utv.List.ListId = Convert.ToInt32(Convert.IsDBNull(row["r_utvalglistid"]) ? null : (Decimal?)(row["r_utvalglistid"]));
            //utv.List =  _utvalgListRepository.GetUtvalgList((int)row["UtvalgListId"]).Result;
            //if (!utv.List.MemberUtvalgs.Contains(utv))
            //    utv.List.MemberUtvalgs.Add(utv);


            utv.ListId = Convert.ToString(row["r_utvalglistid"]);
            utv.OldReolMapName = Convert.ToString(row["r_oldreolmapname"]);
            //utv.Avtalenummer = Convert.ToInt32(row["r_avtalenummer"]);
            if (String.IsNullOrWhiteSpace(Convert.ToString((row, "r_avtalenummer"))))
                utv.Avtalenummer = 0;
            else
                utv.Avtalenummer = Convert.ToInt32(Convert.IsDBNull(row["r_avtalenummer"]));
            //utv.Avtalenummer = (Convert.IsDBNull(row["r_avtalenummer"]) ? null : (int?)(row["r_avtalenummer"]));
            utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;

            utv.BasedOn = Convert.ToInt32(row["r_basedOn"]);
            utv.WasBasedOn = Convert.ToInt32(row["r_wasBasedOn"]);
            utv.Thickness = Convert.ToDouble(Convert.IsDBNull(row["r_thickness"]) ? null : (Double?)(row["r_thickness"]));

            if (row.Table.Columns.Contains("r_ListName"))
            {
                utv.ListName = Convert.ToString(Convert.IsDBNull(row["r_ListName"]) ? null : (string)(row["r_ListName"]));
            }

            if (utv.BasedOn > 0)
            {
                utv.BasedOnName = SetBasedOnWasBasedOnName(utv.BasedOn).Result;
            }

            if (utv.WasBasedOn > 0)
            {
                utv.WasBasedOnName = SetBasedOnWasBasedOnName(utv.WasBasedOn).Result;
            }

            if (includeReols)
            {
                GetUtvalgReoler(utv).Wait();
            }
            if (extendedInfo)
            {
                DataSet dsUtvalgInfo = GetUtvalgAllData(utv.UtvalgId, utv.BasedOn).Result;
                if (dsUtvalgInfo != null && dsUtvalgInfo.Tables.Count > 0)
                {
                    if (dsUtvalgInfo.Tables[0] != null)
                    {
                        GetUtvalgModificationsWithTable(utv, true, dsUtvalgInfo.Tables[0]).Wait();
                    }
                    if (dsUtvalgInfo.Tables[1] != null)
                    {
                        GetUtvalgReceiverWithTable(utv, dsUtvalgInfo.Tables[1]).Wait();
                    }
                    if (dsUtvalgInfo.Tables[2] != null)
                    {
                        GetUtvalgKommune(utv, dsUtvalgInfo.Tables[2]).Wait();
                    }
                    if (dsUtvalgInfo.Tables[3] != null)
                    {
                        GetUtvalgDistrict(utv, dsUtvalgInfo.Tables[3]).Wait();
                    }
                    if (dsUtvalgInfo.Tables[4] != null)
                    {
                        GetUtvalgPostalZone(utv, dsUtvalgInfo.Tables[4]).Wait();
                    }
                    if (dsUtvalgInfo.Tables[5] != null)
                    {
                        GetUtvalgCriteriaWithTable(utv, dsUtvalgInfo.Tables[5]).Wait();
                    }
                }
            }
            utv.SetInitialData();
            utv.TotalAntall = Convert.ToInt32(utv.AntallWhenLastSaved);
            if (!string.Equals(utv.ReolMapName, _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result, StringComparison.OrdinalIgnoreCase) && utv.BasedOn <= 0)
            {
                utv.IsRecreated = false;
            }
            return utv;
        }
        public async Task GetUtvalgReceiver(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgReceiver");
            DataTable reciever;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            UtvalgReceiver utvalgReceiver = null;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            if (utv.BasedOn > 0)
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.BasedOn;
            }
            else
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;
            }
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    reciever = dbhelper.FillDataTable("kspu_db.getutvalgreceiver", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgReceiver: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reciever.Rows)
            {
                utvalgReceiver = new UtvalgReceiver();
                int receiverUd = Convert.ToInt32(row["r_receiverid"]);
                utvalgReceiver.ReceiverId = (ReceiverType)System.Enum.ToObject(typeof(ReceiverType), receiverUd);
                utvalgReceiver.Selected = Convert.ToInt16(row["r_receiverid"]) != 0;// utils.GetBooleanFromRow(row, "r_selected");
                utv.Receivers.Add(utvalgReceiver);
            }
            _logger.LogInformation("Number of rows returned:", reciever.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgReceiver");
        }

        public async Task GetUtvalgReceiverWithTable(Puma.Shared.Utvalg utv, DataTable reciever)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgReceiver");
            //DataTable reciever;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            UtvalgReceiver utvalgReceiver = null;
            #region Old Code to get data from DB
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            //if (utv.BasedOn > 0)
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[0].Value = utv.BasedOn;
            //}
            //else
            //{
            //    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //    npgsqlParameters[0].Value = utv.UtvalgId;
            //}
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        reciever = dbhelper.FillDataTable("kspu_db.getutvalgreceiver", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgReceiver: " + exception.Message);
            //    throw;
            //} 
            #endregion

            foreach (DataRow row in reciever.Rows)
            {
                utvalgReceiver = new UtvalgReceiver();
                int receiverUd = Convert.ToInt32(row["r_receiverid"]);
                utvalgReceiver.ReceiverId = (ReceiverType)System.Enum.ToObject(typeof(ReceiverType), receiverUd);
                utvalgReceiver.Selected = Convert.ToInt16(row["r_receiverid"]) != 0;// utils.GetBooleanFromRow(row, "r_selected");
                utv.Receivers.Add(utvalgReceiver);
            }
            _logger.LogInformation("Number of rows returned:", reciever.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgReceiver");
        }


        public async Task GetUtvalgKommuneOld(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgKommune");
            DataTable komm;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    komm = dbhelper.FillDataTable("kspu_db.getutvalgkommune", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgKommune: " + exception.Message);
                throw;
            }

            foreach (DataRow row in komm.Rows)
            {
                UtvalgKommune k = new UtvalgKommune();
                k.KommuneId = utils.GetStringFromRow(row, "r_kommuneid");
                k.KommuneMapName = utils.GetStringFromRow(row, "r_kommuneMapname");
                utv.Kommuner.Add(k);
            }
            _logger.LogInformation("Number of row returned: ", komm.Rows.Count);

            _logger.LogDebug("Exiting from GetUtvalgKommune");
        }

        public async Task GetUtvalgKommune(Puma.Shared.Utvalg utv, DataTable komm)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgKommune");
            //DataTable komm;
            Utils utils = new Utils();
            #region Old Code for get data from DB
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utv.UtvalgId;
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        komm = dbhelper.FillDataTable("kspu_db.getutvalgkommune", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgKommune: " + exception.Message);
            //    throw;
            //} 
            #endregion

            foreach (DataRow row in komm.Rows)
            {
                UtvalgKommune k = new UtvalgKommune();
                k.KommuneId = utils.GetStringFromRow(row, "r_kommuneid");
                k.KommuneMapName = utils.GetStringFromRow(row, "r_kommuneMapname");
                utv.Kommuner.Add(k);
            }
            _logger.LogInformation("Number of row returned: ", komm.Rows.Count);

            _logger.LogDebug("Exiting from GetUtvalgKommune");
        }


        public async Task GetUtvalgDistrictOld(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgDistrict");
            DataTable dis;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dis = dbhelper.FillDataTable("kspu_db.getutvalgdistrict", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgDistrict: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dis.Rows)
            {
                UtvalgDistrict d = new UtvalgDistrict();
                d.DistrictId = utils.GetStringFromRow(row, "r_districtid");
                d.DistrictMapName = utils.GetStringFromRow(row, "r_districtmapname");
                utv.Districts.Add(d);
            }
            _logger.LogInformation("Number of row returned: ", dis.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgDistrict");
        }

        public async Task GetUtvalgDistrict(Puma.Shared.Utvalg utv, DataTable dis)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgDistrict");
            // DataTable dis;
            Utils utils = new Utils();
            #region Old Code to get data from DB
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utv.UtvalgId;
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        dis = dbhelper.FillDataTable("kspu_db.getutvalgdistrict", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgDistrict: " + exception.Message);
            //    throw;
            //} 
            #endregion
            foreach (DataRow row in dis.Rows)
            {
                UtvalgDistrict d = new UtvalgDistrict();
                d.DistrictId = utils.GetStringFromRow(row, "r_districtid");
                d.DistrictMapName = utils.GetStringFromRow(row, "r_districtmapname");
                utv.Districts.Add(d);
            }
            _logger.LogInformation("Number of row returned: ", dis.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgDistrict");
        }



        public async Task GetUtvalgPostalZoneOld(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgPostalZone");
            DataTable pos;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    pos = dbhelper.FillDataTable("kspu_db.getutvalgpostalzone", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgPostalZone: " + exception.Message);
                throw;
            }
            foreach (DataRow row in pos.Rows)
            {
                UtvalgPostalZone p = new UtvalgPostalZone();
                p.PostalZone = utils.GetIntFromRow(row, "r_postalzone");
                p.PostalZoneMapName = utils.GetStringFromRow(row, "r_postalZonemapname");
                utv.PostalZones.Add(p);
            }
            _logger.LogInformation("Number of row returned: ", pos.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgPostalZone");
        }

        public async Task GetUtvalgPostalZone(Puma.Shared.Utvalg utv, DataTable pos)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgPostalZone");
            // DataTable pos;
            Utils utils = new Utils();
            #region Old Code to get data from DB
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utv.UtvalgId;
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        pos = dbhelper.FillDataTable("kspu_db.getutvalgpostalzone", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgPostalZone: " + exception.Message);
            //    throw;
            //} 
            #endregion

            foreach (DataRow row in pos.Rows)
            {
                UtvalgPostalZone p = new UtvalgPostalZone();
                p.PostalZone = utils.GetIntFromRow(row, "r_postalzone");
                p.PostalZoneMapName = utils.GetStringFromRow(row, "r_postalZonemapname");
                utv.PostalZones.Add(p);
            }
            _logger.LogInformation("Number of row returned: ", pos.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgPostalZone");
        }

        public async Task GetUtvalgCriteria(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgCriteria");
            DataTable cri;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    cri = dbhelper.FillDataTable("kspu_db.getutvalgcriteria", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgCriteria: " + exception.Message);
                throw;
            }
            foreach (DataRow row in cri.Rows)
            {
                UtvalgCriteria c = new UtvalgCriteria();
                c.CriteriaId = Convert.ToInt32(row["r_criteriaid"]);
                c.Criteria = utils.GetStringFromRow(row, "r_criteria");
                if (Convert.ToInt32(row["r_criteriatype"]) > 0)
                {
                    c.CriteriaType = (CriteriaType)utils.GetEnumFromRow(row, "r_criteriatype", typeof(CriteriaType));
                }

                utv.Criterias.Add(c);
            }
            _logger.LogInformation("Number of row returned: ", cri.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgPostalZone");
        }


        public async Task GetUtvalgCriteriaWithTable(Puma.Shared.Utvalg utv, DataTable cri)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgCriteria");
            //DataTable cri;
            Utils utils = new Utils();
            #region Old Code to fetch data from DB
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utv.UtvalgId;
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        cri = dbhelper.FillDataTable("kspu_db.getutvalgcriteria", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgCriteria: " + exception.Message);
            //    throw;
            //} 
            #endregion
            foreach (DataRow row in cri.Rows)
            {
                UtvalgCriteria c = new UtvalgCriteria();
                c.CriteriaId = Convert.ToInt32(row["r_criteriaid"]);
                c.Criteria = utils.GetStringFromRow(row, "r_criteria");
                if (Convert.ToInt32(row["r_criteriatype"]) > 0)
                {
                    c.CriteriaType = (CriteriaType)utils.GetEnumFromRow(row, "r_criteriatype", typeof(CriteriaType));
                }

                utv.Criterias.Add(c);
            }
            _logger.LogInformation("Number of row returned: ", cri.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgPostalZone");
        }



        public async Task GetUtvalgModifications(Puma.Shared.Utvalg utv, bool bAsc)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgModifications");
            DataTable modification;
            Utils utils = new Utils();
            //string SortOrdrer = "ASC";
            //if (!bAsc)
            //    SortOrdrer = "DESC";
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    modification = dbhelper.FillDataTable("kspu_db.getutvalgmodifications", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgModifications: " + exception.Message);
                throw;
            }
            foreach (DataRow row in modification.Rows)
            {
                UtvalgModification m = new UtvalgModification();
                m.UserId = utils.GetStringFromRow(row, "UserId");
                m.ModificationId = Convert.ToInt32(utils.GetLongFromRow(row, "UtvalgModificationId"));
                m.ModificationTime = utils.GetTimestampFromRow(row, "ModificationDate");
                utv.Modifications.Add(m);
            }
            _logger.LogInformation("Number of row returned: ", modification.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgModifications");
        }

        public async Task GetUtvalgModificationsWithTable(Puma.Shared.Utvalg utv, bool bAsc, DataTable modification)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgModifications");
            //DataTable modification;
            Utils utils = new Utils();
            ////string SortOrdrer = "ASC";
            ////if (!bAsc)
            ////    SortOrdrer = "DESC";
            //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            //npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            //npgsqlParameters[0].Value = utv.UtvalgId;
            //try
            //{
            //    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
            //    {
            //        modification = dbhelper.FillDataTable("kspu_db.getutvalgmodifications", CommandType.StoredProcedure, npgsqlParameters);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _logger.LogError(exception, "Error in GetUtvalgModifications: " + exception.Message);
            //    throw;
            //}
            foreach (DataRow row in modification.Rows)
            {
                UtvalgModification m = new UtvalgModification();
                m.UserId = utils.GetStringFromRow(row, "UserId");
                m.ModificationId = Convert.ToInt32(utils.GetLongFromRow(row, "UtvalgModificationId"));
                m.ModificationTime = utils.GetTimestampFromRow(row, "ModificationDate");
                utv.Modifications.Add(m);
            }
            _logger.LogInformation("Number of row returned: ", modification.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgModifications");
        }


        public async Task SaveUtvalgKommuner(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgKommuner");
            try
            {
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgkommune", CommandType.StoredProcedure, npgsqlParameters1);
                }

                int res;
                UtvalgKommune utvalgKommune = new UtvalgKommune();

                foreach (var itemKommuner in utv.Kommuner)
                {
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_kommuneid", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[1].Value = itemKommuner.KommuneId;

                    npgsqlParameters[2] = new NpgsqlParameter("p_kommunemapname", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[2].Value = itemKommuner.KommuneMapName;


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        res = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgkommuner", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgKommuner: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from SaveUtvalgKommuner");

        }


        public async Task SaveUtvalgReceiver(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            try
            {

                _logger.LogDebug("Preparing the data for SaveUtvalgReceiver");
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgreceiver", CommandType.StoredProcedure, npgsqlParameters1);

                }

                int res;
                foreach (var itemreceivers in utv.Receivers)
                {
                    if ((int)itemreceivers.ReceiverId > 0)
                    {
                        NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                        npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                        npgsqlParameters[0].Value = utv.UtvalgId;

                        npgsqlParameters[1] = new NpgsqlParameter("p_receiverid", NpgsqlTypes.NpgsqlDbType.Integer);
                        npgsqlParameters[1].Value = (int)itemreceivers.ReceiverId;
                        npgsqlParameters[2] = new NpgsqlParameter("p_selected", NpgsqlTypes.NpgsqlDbType.Smallint);
                        npgsqlParameters[2].Value = itemreceivers.Selected == true ? 1 : 0;

                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                        {
                            res = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgreceiver", CommandType.StoredProcedure, npgsqlParameters);
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReceiver: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from SaveUtvalgReceiver");
        }


        public async Task SaveUtvalgAntallToInheritors(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgAntallToInheritors");
            int res;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utv.UtvalgId;

            npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = utv.AntallWhenLastSaved;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    res = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgantalltoinheritors", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgAntallToInheritors: " + exception.Message);

            }
            _logger.LogDebug("Exiting from SaveUtvalgAntallToInheritors");
            //OracleCommand updateCmd = new OracleCommand(" UPDATE KSPU_DB.Utvalg SET Antall = :Antall WHERE BasedOn = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(updateCmd, "UtvalgID", utv.UtvalgId);
            //AddParameterLong(updateCmd, "Antall", utv.AntallWhenLastSaved);
            //ExecuteNonQuery(updateCmd);
        }


        public async Task SaveUtvalgDistrict(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgDistrict");
            try
            {
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgdistrict", CommandType.StoredProcedure, npgsqlParameters1);
                }

                int district;

                foreach (var itemdistrict in utv.Districts)
                {
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_districtid", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[1].Value = itemdistrict.DistrictId;

                    npgsqlParameters[2] = new NpgsqlParameter("p_districtmapname", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[2].Value = itemdistrict.DistrictMapName;


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        district = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgdistrict", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgDistrict: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from SaveUtvalgDistrict");
            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgDistrict WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgDistrict (UtvalgId, DistrictId, DistrictMapName) VALUES (:UtvalgId, :DistrictId, :DistrictMapName) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterString(insertCmd, "DistrictId", "", 50);
            //AddParameterString(insertCmd, "DistrictMapName", "", 50);

        }

        public async Task SaveUtvalgPostalZone(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgPostalZone");
            try
            {
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgpostalzone", CommandType.StoredProcedure, npgsqlParameters1);
                }

                int postalzone;

                foreach (var itempostelzones in utv.PostalZones)
                {
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_postalzone", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[1].Value = itempostelzones.PostalZone;

                    npgsqlParameters[2] = new NpgsqlParameter("p_postalzonemapname", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[2].Value = itempostelzones.PostalZoneMapName;



                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        postalzone = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgpostalzone", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgPostalZone: " + exception.Message);

            }

            //OracleCommand cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgPostalZone WHERE UtvalgID = :UtvalgID ", trans.Connection, trans);
            //AddParameterInteger(cmd, "UtvalgID", utv.UtvalgId);
            //ExecuteNonQuery(cmd);
            //OracleCommand insertCmd = new OracleCommand(" INSERT INTO KSPU_DB.UtvalgPostalZone (UtvalgId, PostalZone, PostalZoneMapName) VALUES (:UtvalgId, :PostalZone, :PostalZoneMapName) ", trans.Connection, trans);
            //AddParameterInteger(insertCmd, "UtvalgId", utv.UtvalgId);
            //AddParameterInteger(insertCmd, "PostalZone", 0);
            //AddParameterString(insertCmd, "PostalZoneMapName", "", 50);
            _logger.LogDebug("Exiting from SaveUtvalgPostalZone");
        }


        public async Task SaveUtvalgCriteria(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for SaveUtvalgCriteria");
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgcriteria", CommandType.StoredProcedure, npgsqlParameters1);
                }

                int Criteria;



                foreach (var itemutvalgcriteria in utv.Criterias?.Where(x => !string.IsNullOrWhiteSpace(x.Criteria)))
                {


                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];

                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = utv.UtvalgId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_criteriaid", NpgsqlTypes.NpgsqlDbType.Integer);


                    npgsqlParameters[2] = new NpgsqlParameter("p_criteria", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[2].Value = itemutvalgcriteria.Criteria;


                    npgsqlParameters[3] = new NpgsqlParameter("p_criteriatype", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[3].Value = (int)itemutvalgcriteria.CriteriaType;

                    //if (itemutvalgcriteria.CriteriaId == 0)
                    //{
                    itemutvalgcriteria.CriteriaId = await GetSequenceNextVal("KSPU_DB.CriteriaId_Seq");
                    //}


                    npgsqlParameters[1].Value = itemutvalgcriteria.CriteriaId;


                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        Criteria = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgcriteria", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgCriteria: " + exception.Message);

            }
            _logger.LogDebug("Exiting from SaveUtvalgCriteria");
            //foreach (UtvalgCriteria c in utv.Criterias)
            //{
            //    if (c.CriteriaId == 0)
            //        //  c.CriteriaId = GetSequenceNextVal("KSPU_DB.CriteriaId_Seq", trans);
            //        // insertCmd.Parameters.Item("CriteriaId").Value = c.CriteriaId;
            //        if (c.Criteria == null)
            //            c.Criteria = "";
            //    //insertCmd.Parameters.Item("Criteria").Value = c.Criteria;
            //    //insertCmd.Parameters.Item("CriteriaType").Value = System.Convert.ToInt32(c.CriteriaType);
            //    //ExecuteNonQuery(insertCmd);
            //}
        }


        /// <summary>
        ///     ''' Get Utvalg Reoler
        ///     ''' </summary>
        ///     ''' <param name="utv"></param>
        ///     ''' <returns>Reoler count</returns>
        public async Task GetUtvalgReoler(Puma.Shared.Utvalg utv)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgReoler");
            int utvalgId = utv.UtvalgId;
            ReolCollection reols = null;
            if ((utv.BasedOn > 0))
            {
                // Fetch reoler from the Basis Utvalg that this utvalg is based on
                utvalgId = utv.BasedOn;
                GetUtvalgReolMapNames(utv, utvalgId);
            }
            if (utv.OldReolMapName != "")
                // Utvalg was recreated by batch job, read ReolerBeforeRecreation from table UtvalgOldReol
                await GetUtvalgOldReoler(utv, utvalgId);
            if (!string.Equals(utv.ReolMapName, _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result, StringComparison.OrdinalIgnoreCase))
            {
                if (utv.ReolerBeforeRecreation == null)
                {
                    // Recreate utvalg on the fly based on ids
                    utv.ReolerBeforeRecreation = new ReolCollection();
                    reols = await GetReolData(utv.ReolMapName);
                    if (reols != null && reols.Any())
                    {
                        foreach (long reolid in GetUtvalgReolIDs(utvalgId).Result)
                        {
                            // Supportsak #621937 - ignore missing reolids, even if it should not happen... 
                            Puma.Shared.Reol Oldr = await _reolRepository.GetReolFromReolCollectionByReolId(reolid, reols, NotFoundAction.ReturnNothing, false);
                            if (Oldr != null)
                                utv.ReolerBeforeRecreation.Add(Oldr);
                        }
                    }
                    else
                        utv.OldReolMapMissing = true;
                }
                // Current recreation strategy: Get new reoler with same reolids, ignore missing reolids.
                List<long> reolIDsToUse;
                if (utv.OldReolMapName != "")
                    reolIDsToUse = await GetUtvalgOldReolIDsWithCurrentReol(utvalgId);
                else
                    reolIDsToUse = await GetUtvalgReolIDsWithCurrentReolTable(utvalgId);

                string tableName = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName);

                reols = await GetReolData(tableName);


                foreach (long reolid in reolIDsToUse)
                {
                    Puma.Shared.Reol r = await _reolRepository.GetReolFromReolCollectionByReolId(reolid, reols, NotFoundAction.ReturnNothing);
                    if (r != null)
                        utv.Reoler.Add(r);
                }
            }
            else
            {
                // No recreation is necessary
                reols = await _reolRepository.GetAllReolsFromTable(_configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CurrentReolTableName).Result);
                foreach (long reolid in await GetUtvalgReolIDsWithCurrentReolTable(utvalgId))
                    utv.Reoler.Add(await _reolRepository.GetReolFromReolCollectionByReolId(reolid, reols));
            }

        }

        public async Task<List<long>> GetUtvalgOldReolIDs(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgOldReolIDs");
            DataTable reolID;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    reolID = dbhelper.FillDataTable("kspu_db.getutvalgoldreolids", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgOldReolIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reolID.Rows)
                // result.Add((long)row["r_reolid"]);
                result.Add(utils.GetLongFromRow(row, "r_reolid"));
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetUtvalgOldReolIDs");
            return result;
        }

        public async Task<List<long>> GetUtvalgOldReolIDsWithCurrentReol(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgOldReolIDs");
            DataTable reolID;
            List<long> result = new List<long>();
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    reolID = dbhelper.FillDataTable("kspu_db.getutvalgoldreolidswithcurrentreol", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgOldReolIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reolID.Rows)
                // result.Add((long)row["r_reolid"]);
                result.Add(utils.GetLongFromRow(row, "r_reolid"));
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from GetUtvalgOldReolIDs");
            return result;
        }


        private void GetUtvalgReolMapNames(Puma.Shared.Utvalg utv, int utvalgId)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgReolMapNames");
            DataTable result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.FillDataTable("kspu_db.getutvalgreolmapnames", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgReolMapNames: " + exception.Message);
                throw;
            }
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                utv.ReolMapName = row["r_reolmapname"].ToString();
                utv.OldReolMapName = row["r_oldreolmapname"].ToString();
            }
            _logger.LogInformation("Number of row returned: ", result.Rows.Count);

            _logger.LogDebug("Exiting from GetUtvalgReolMapNames");
        }

        private CampaignDescription GetCampaignDescriptionFromUtvalgDataRow(DataRow row, bool isDisconnected)
        {
            CampaignDescription utv = new CampaignDescription();
            Utils utils = new Utils();
            utv.ID = Convert.ToInt64(row["r_utvalgid"]);// utils.GetIntFromRow(row, "r_utvalgid");
            utv.Name = utils.GetStringFromRow(row, "r_name");
            utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_OrdreType", typeof(OrdreType), "Null");
            utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_OrdreStatus", typeof(OrdreStatus), "Null");
            //utv.DistributionDate = (DateTime)row["r_Distribusjonsdato"];
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));
            utv.IsDisconnected = isDisconnected;
            return utv;
        }

        /// <summary>
        ///     ''' Get Utvalg old Reoler
        ///     ''' </summary>
        ///     ''' <param name="utv"></param>
        ///     ''' <param name="utvalgId"></param>     
        ///     ''' <returns>Reoler count</returns>
        public async Task GetUtvalgOldReoler(Puma.Shared.Utvalg utv, int utvalgId)
        {
            utv.ReolerBeforeRecreation = new ReolCollection();
            if (!string.IsNullOrWhiteSpace(utv.OldReolMapName))
            {
                ReolCollection reols = await GetReolData(utv.OldReolMapName);
                if (reols != null && reols.Any())
                {
                    foreach (long reolID in await GetUtvalgOldReolIDs(utvalgId))
                    {
                        Puma.Shared.Reol reolDetail = await _reolRepository.GetReolFromReolCollectionByReolId(reolID, reols, NotFoundAction.ReturnNothing, false);

                        if (reolDetail != null)
                            utv.ReolerBeforeRecreation.Add(reolDetail);
                    }
                }
                else
                {
                    utv.OldReolMapMissing = true;
                }
            }
            else
                utv.OldReolMapMissing = true;


        }

        /// <summary>
        /// Gets the reol data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public async Task<ReolCollection> GetReolData(string tableName)
        {
            return await _reolRepository.GetOrCreateReolCache(tableName);
        }

        /// <summary>
        /// Save Utvalg Previous Reoler
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        public async Task SaveUtvalgPreviousReoler(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for SaveUtvalgPreviousReoler");

                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvagid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = utv.UtvalgId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    _ = dbhelper.ExecuteNonQuery("kspu_db.deletefromutvalgoldreol", CommandType.StoredProcedure, npgsqlParameters1);
                }

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_reolid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[1].Value = 0;
                #endregion

                foreach (Puma.Shared.Reol r in utv.ReolerBeforeRecreation)
                {
                    npgsqlParameters[1].Value = r.ReolId;

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgpreviousreoler", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }

                _logger.LogDebug("Exiting from SaveUtvalgPreviousReoler");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }

        /// <summary>
        /// SaveUtvalgModifications
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        /// <param name="userName"></param>
        /// <param name="modificationInfo"></param>

        public async Task SaveUtvalgModifications(Puma.Shared.Utvalg utv, string userName, string modificationInfo = "")
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for SaveUtvalgModifications");

                UtvalgModification utvalgModification = new UtvalgModification();
                utvalgModification.ModificationTime = DateTime.Now;
                utvalgModification.UserId = userName;
                //utvalgModification.ModificationId =  await GetSequenceNextVal("KSPU_DB.UtvalgModificationId_Seq");
                //utv.Modifications.Add(utvalgModification);
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                int result;

                #region Parameter assignement
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[1].Value = utvalgModification.UserId;

                //npgsqlParameters[2] = new NpgsqlParameter("p_info", NpgsqlTypes.NpgsqlDbType.Varchar);
                //npgsqlParameters[2].Value = utvalgModification.mo;

                npgsqlParameters[2] = new NpgsqlParameter("p_info", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                modificationInfo = modificationInfo + " Utvalgets antall ved sist lagring: " + utv.AntallWhenLastSaved.ToString();
                npgsqlParameters[2].Value = modificationInfo;
                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalgmodifications", CommandType.StoredProcedure, npgsqlParameters);
                }
                _logger.LogInformation("Number of row returned: ", result);

                _logger.LogDebug("Exiting from SaveUtvalgModifications");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgModifications: " + exception.Message);
            }
        }




        /// <summary>
        /// Save Utvalg Previous Reoler
        /// </summary>
        /// <param name="utv">Utvalg Object</param>
        public async Task UpdateOldReolMapName(Puma.Shared.Utvalg utv)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for UpdateOldReolMapName");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = utv.UtvalgId;

                npgsqlParameters[1] = new NpgsqlParameter("p_oldreolmapname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                if (utv.OldReolMapName.Length != 0)
                    npgsqlParameters[1].Value = utv.OldReolMapName;
                else
                    npgsqlParameters[1].Value = DBNull.Value;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateoldreolmapname", CommandType.StoredProcedure, npgsqlParameters);
                }

                _logger.LogDebug("Exiting from UpdateOldReolMapName");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgReoler: " + exception.Message);
            }
        }

        /// <summary>
        /// Utvalg Has Recreated Before
        /// </summary>
        /// <param name="utvalgId">Utvalg ID to fetch list of address related to passed user</param>
        /// <returns>True or false</returns>
        public async Task<bool> UtvalgHasRecreatedBefore(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UtvalgHasRecreatedBefore");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalghasrecreatedbefore", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UtvalgHasRecreatedBefore: " + exception.Message);
                throw;
            }

            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from UtvalgHasRecreatedBefore");
            if (result == DBNull.Value)
                return false;
            return System.Convert.ToInt32(result) > 0;
        }


        /// <summary>
        ///     ''' Checks if distribution is in the next x days ( X = Config.IgnoreNrOfDaysToDelivery ) 
        ///     ''' </summary>
        ///     ''' <param name="idsU"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>


        public List<int> CheckIfUtvalgsDistributionIsToClose(int[] idsU)
        {
            _logger.LogDebug("Preparing the data for CheckIfUtvalgsDistributionIsToClose");
            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            Utils utils = new Utils();
            DataTable utvData;
            foreach (string idList in CreateInClauses(idsU, 1000))
            {
                StringBuilder sql = new StringBuilder();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                //object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_idsu", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = Convert.ToInt32(idList);

                npgsqlParameters[1] = new NpgsqlParameter("p_fromdate", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = idList;

                npgsqlParameters[2] = new NpgsqlParameter("p_todate", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[2].Value = idList;
                try
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.checkifutvalgsdistributionistoclose", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in CheckIfUtvalgsDistributionIsToClose: " + exception.Message);
                    throw;
                }
                foreach (DataRow row in utvData.Rows)
                {
                    int id = utils.GetIntFromRow(row, "r_utvalgid");
                    resultDict[id] = id;
                }
            }
            return new List<int>(resultDict.Values);
        }


        /// <summary>
        ///    Checks if on the fly is needed for a set of utvalg ids
        ///      </summary>
        ///      <param name="idsU"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>


        public List<int> CheckIfUtvalgsNeedOnTheFlyUpdate(int[] idsU)
        {
            _logger.LogDebug("Preparing the data for CheckIfUtvalgsNeedOnTheFlyUpdate");
            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            Utils utils = new Utils();
            DataTable utvData;
            foreach (string idList in CreateInClauses(idsU, 1000))
            {
                StringBuilder sql = new StringBuilder();
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                //object result;

                npgsqlParameters[0] = new NpgsqlParameter("p_idsu", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = Convert.ToInt32(idList);
                try
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.checkifutvalgsneedontheflyupdate", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in CheckIfUtvalgsNeedOnTheFlyUpdate: " + exception.Message);
                    throw;
                }
                foreach (DataRow row in utvData.Rows)
                {
                    int id = utils.GetIntFromRow(row, "r_utvalgid");
                    resultDict[id] = id;
                }
            }
            return new List<int>(resultDict.Values);
        }


        private List<string> CreateInClauses(IEnumerable<int> ids, int maxLength)
        {
            List<string> bolker = new List<string>();
            StringBuilder idsULsb = new StringBuilder();
            foreach (int id in ids)
            {
                if (id > 0)
                {
                    idsULsb.Append(id.ToString());
                    if (idsULsb.Length > maxLength)
                    {
                        bolker.Add(idsULsb.ToString());
                        idsULsb.Remove(0, idsULsb.Length);
                    }
                    else
                        idsULsb.Append(",");
                }
            }
            if (idsULsb.Length > 0)
            {
                idsULsb.Remove(idsULsb.Length - 1, 1);
                bolker.Add(idsULsb.ToString());
            }
            return bolker;
        }

        /// <summary>
        ///     ''' GetUtvalgListId
        ///     ''' Metoden returnerer utvalgslisteid til ett utvalg med en gitt utvalgId.
        ///     ''' Dersom utvalget ikke har noen utvalgslisteid returneres verdien -1
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>

        private List<int> CheckIfParentUtvalgListsNeedOnTheFlyUpdate(IEnumerable<int> utvalgListIDs)
        {
            _logger.LogDebug("Preparing the data for CheckIfParentUtvalgListsNeedOnTheFlyUpdate");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            List<int> result = new List<int>();
            foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = str;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.checkifparentutvalglistsneedontheflyupdate", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                    result.Add((int)row["r_utvalglistid"]);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);

            _logger.LogDebug("Exiting from CheckIfParentUtvalgListsNeedOnTheFlyUpdate");
            return result;

        }

        /// <summary>
        ///     ''' Returns a new list of utvalglistids containing the same ids as the input, except campaign list ids have been replaced with their basis list id.
        ///     ''' </summary>
        ///     ''' <param name="utvalgListIDs"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private List<int> ReplaceCampaignListIDsWithBasisListIDs(IEnumerable<int> utvalgListIDs)
        {
            _logger.LogDebug("Preparing the data for ReplaceCampaignListIDsWithBasisListIDs");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            Dictionary<int, bool> resultDict = new Dictionary<int, bool>();
            foreach (string str in CreateInClauses(utvalgListIDs, 4000))
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_str", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[0].Value = str;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.replacecampaignlistidswithbasislistids", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in utvData.Rows)
                {
                    int oldId = (int)row["r_utvalglistid"];
                    int newId = (int)row["r_basedon"];
                    if ((newId > 0))
                        resultDict[newId] = true;
                    else
                        resultDict[oldId] = true;
                }
            }
            _logger.LogInformation("Number of row returned: ", resultDict.Count);

            _logger.LogDebug("Exiting from ReplaceCampaignListIDsWithBasisListIDs");
            return new List<int>(resultDict.Keys);

        }

        #endregion

        public async Task UpdateDisconnectUtvalgForIntegration(Puma.Shared.Utvalg utv, string username)
        {
            await Task.Run(() => { });

        }

        public async Task<List<Puma.Shared.Utvalg>> GetUtvalgDetails(List<decimal> utvalgId)
        {
            var utvalgData = await (from utvalg in _context.utvalg
                                    where utvalgId.Contains(utvalg.utvalgid)
                                    select new Puma.Shared.Utvalg()
                                    {
                                        UtvalgId = Convert.ToInt32(utvalg.utvalgid),
                                        KundeNummer = utvalg.kundenummer,
                                        Name = utvalg.name,
                                        Logo = utvalg.logo,
                                        OrdreReferanse = utvalg.ordrereferanse,
                                        //OrdreType = !string.IsNullOrWhiteSpace(utvalg.ordretype) ?
                                        //(PumaEnum.OrdreType)Enum.Parse(typeof(PumaEnum.OrdreType), utvalg.ordretype)
                                        //: 0,

                                        InnleveringsDato = utvalg.innleveringsdato.HasValue ? utvalg.innleveringsdato.Value : DateTime.MinValue,
                                        ReolMapName = utvalg.reolmapname,
                                        AntallWhenLastSaved = Convert.ToInt64(utvalg.antall),
                                        ArealAvvik = Convert.ToInt32(utvalg.arealavvik),
                                        Weight = Convert.ToInt32(utvalg.vekt),
                                        DistributionDate = utvalg.distribusjonsdato.HasValue ? utvalg.distribusjonsdato.Value : DateTime.MinValue,
                                        Modifications = (from utvMod in _context.UtvalgModifications
                                                         where utvalg.utvalgid == utvMod.UtvalgId
                                                         select new Puma.Shared.UtvalgModification
                                                         {
                                                             ModificationId = Convert.ToInt32(utvMod.UtvalgModificationId)
                                                             // ModificationTime = ut
                                                         }).Take(5).ToList()

                                        //OrdreStatus = (OrdreStatus)Enum.Parse(typeof(OrdreStatus), utvalglists.ordrestatus),
                                        //OrdreType = (OrdreType)Enum.Parse(typeof(OrdreType), utvalglists.ordretype),
                                        //DistributionType = (DistributionType)Enum.Parse(typeof(DistributionType), utvalglists.distributiontype)
                                    }).ToListAsync();

            //List<UtvalgModifications> utvalgModifications = (from utvMod in _context.UtvalgModifications
            //                                                 where utvalgId.Contains(utvMod.UtvalgId)
            //                                                 select utvMod).ToList();

            var utvalgModifications = _context.UtvalgModifications.AsEnumerable().GroupBy(x => x.UtvalgId, (key, g) => g.OrderByDescending(e => e.ModificationDate).Take(5)).ToList();

            foreach (var itemUtvaDat in utvalgData)
            {
                itemUtvaDat.Modifications = new List<UtvalgModification>();
                //itemUtvaDat.Modifications.Add(utvalgModifications.SelectMany(x=>x.))
            }

            return utvalgData;

        }

        public async Task<string> SetBasedOnWasBasedOnName(int id)
        {
            string basedonWasbasedonName = await GetUtvalgName(id);

            //Note : As discussed in based on and wasbased on will always have selection id
            //if (string.IsNullOrWhiteSpace(basedonWasbasedonName))
            //{
            //    _utvalgListRepository = _services.GetService<IUtvalgListRepository>();
            //    basedonWasbasedonName = await _utvalgListRepository.GetUtvalgListName(id);
            //}

            return basedonWasbasedonName;
        }

        public async Task<List<UtvalgIdWrapper>> GetUtvalgsToRefreshDueToUpdate(Puma.Shared.Utvalg basisUtvalg)
        {
            List<UtvalgIdWrapper> ids = new List<UtvalgIdWrapper>();
            if (basisUtvalg.IsBasis && basisUtvalg.UtvalgId > 0)
            {
                await GetUtvalgCampaigns(basisUtvalg.UtvalgId);
                if (basisUtvalg?.UtvalgsBasedOnMe?.Any() == true)
                {
                    foreach (var c in basisUtvalg?.UtvalgsBasedOnMe)
                    {
                        if (c.OrdreType == OrdreType.T && c.IsDisconnected == false)
                        {
                            ids.Add(new UtvalgIdWrapper((int)c.ID, UtvalgType.Utvalg));
                        }
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Distributions the details.
        /// </summary>
        /// <param name="dayDetails">The day details.</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public List<int> distributionDetails(string dayDetails, bool showHouseholds, bool showBusiness, Puma.Shared.Reol data)
        {
            int VHD1 = 0;
            int VHD2 = 0;
            int HHD1 = 0;
            int HHD2 = 0;

            switch (dayDetails)
            {
                case "A-uke, tidliguke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved;
                            HHD2 = data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "A-uke, midtuke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = 0;
                            HHD2 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "B-uke, tidliguke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved;
                            HHD2 = data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "B-uke, midtuke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = 0;
                            HHD2 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
            }

            return new List<int>() { VHD1, VHD2, HHD1, HHD2 };

        }
        /// <summary>
        /// Gets the house holds count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reoler">The reoler.</param>
        /// <param name="item">The item.</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="showHouseholdsReserved">if set to <c>true</c> [show households reserved].</param>
        /// <returns></returns>
        private T GetHouseHoldsCount<T>(Puma.Shared.Reol reoler, T item, string strDayDetails, bool showHouseholds, bool showBusiness, bool showHouseholdsReserved) where T : BasicDetail, new()
        {
            List<int> DistrDetails = distributionDetails(strDayDetails, showHouseholds, showBusiness, reoler);

            item.house = showHouseholds ? reoler.Antall.Households : 0;
            item.householdsReserved = showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0;
            item.businesses = showBusiness ? reoler.Antall.Businesses : 0;
            item.HHD1 = DistrDetails[2];
            item.HHD2 = DistrDetails[3];
            item.VHD1 = DistrDetails[0];
            item.VHD2 = DistrDetails[1];
            item.zone0 = (reoler.PrisSone == 0 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.zone1 = (reoler.PrisSone == 1 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.zone2 = (reoler.PrisSone == 2 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H0 = (reoler.PrisSone == 0 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H1 = (reoler.PrisSone == 1 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H2 = (reoler.PrisSone == 2 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.V0 = (reoler.PrisSone == 0 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.V1 = (reoler.PrisSone == 1 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.V2 = (reoler.PrisSone == 2 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.total = ((showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0));

            return item;
        }

        /// <summary>
        /// Fills the recursive.
        /// </summary>
        /// <param name="flatObjects">The flat objects.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        private static List<BasicDetail> FillRecursive(List<BasicDetail> flatObjects, int? parentId = null)
        {
            List<BasicDetail> basicDetails = new List<BasicDetail>();
            for (int i = 0; i < flatObjects.Count; i++)
            {
                if (flatObjects[i] != null)
                {
                    if (flatObjects[i].pkey == null)
                    {
                        int index = basicDetails.FindIndex(x => x.key == flatObjects[i].key);

                        if (index < 0)
                        {
                            basicDetails.Add(flatObjects[i]);
                        }
                        else
                        {
                            basicDetails[index].house += flatObjects[i].house;
                            basicDetails[index].businesses += flatObjects[i].businesses;
                            basicDetails[index].householdsReserved += flatObjects[i].householdsReserved;
                            basicDetails[index].HHD1 += flatObjects[i].HHD1;
                            basicDetails[index].HHD2 += flatObjects[i].HHD2;
                            basicDetails[index].VHD1 += flatObjects[i].VHD1;
                            basicDetails[index].VHD2 += flatObjects[i].VHD2;
                            basicDetails[index].H0 += flatObjects[i].H0;
                            basicDetails[index].H1 += flatObjects[i].H1;
                            basicDetails[index].H2 += flatObjects[i].H2;
                            basicDetails[index].V0 += flatObjects[i].V0;
                            basicDetails[index].V1 += flatObjects[i].V1;
                            basicDetails[index].V2 += flatObjects[i].V2;
                            basicDetails[index].zone0 += flatObjects[i].zone0;
                            basicDetails[index].zone1 += flatObjects[i].zone1;
                            basicDetails[index].zone2 += flatObjects[i].zone2;
                            basicDetails[index].total += flatObjects[i].total;
                        }
                    }
                    else
                    {
                        int parentIndex = flatObjects.FindIndex(x => x.key == flatObjects[i].pkey);
                        int childIndex = -1;

                        if (flatObjects[parentIndex].children is not null)
                        {
                            childIndex = flatObjects[parentIndex].children.FindIndex(y => y.key == flatObjects[i].key);
                        }
                        else
                        {
                            flatObjects[parentIndex].children = new List<BasicDetail>();
                        }

                        if (childIndex < 0)
                        {
                            flatObjects[parentIndex].children.Add(flatObjects[i]);
                        }
                        else
                        {
                            flatObjects[parentIndex].children[childIndex].house += flatObjects[i].house;
                            flatObjects[parentIndex].children[childIndex].businesses += flatObjects[i].businesses;
                            flatObjects[parentIndex].children[childIndex].householdsReserved += flatObjects[i].householdsReserved;
                            flatObjects[parentIndex].children[childIndex].HHD1 += flatObjects[i].HHD1;
                            flatObjects[parentIndex].children[childIndex].HHD2 += flatObjects[i].HHD2;
                            flatObjects[parentIndex].children[childIndex].VHD1 += flatObjects[i].VHD1;
                            flatObjects[parentIndex].children[childIndex].VHD2 += flatObjects[i].VHD2;
                            flatObjects[parentIndex].children[childIndex].H0 += flatObjects[i].H0;
                            flatObjects[parentIndex].children[childIndex].H1 += flatObjects[i].H1;
                            flatObjects[parentIndex].children[childIndex].H2 += flatObjects[i].H2;
                            flatObjects[parentIndex].children[childIndex].V0 += flatObjects[i].V0;
                            flatObjects[parentIndex].children[childIndex].V1 += flatObjects[i].V1;
                            flatObjects[parentIndex].children[childIndex].V2 += flatObjects[i].V2;
                            flatObjects[parentIndex].children[childIndex].zone0 += flatObjects[i].zone0;
                            flatObjects[parentIndex].children[childIndex].zone1 += flatObjects[i].zone1;
                            flatObjects[parentIndex].children[childIndex].zone2 += flatObjects[i].zone2;
                            flatObjects[parentIndex].children[childIndex].total += flatObjects[i].total;

                        }
                    }
                }
            }

            return basicDetails;
        }

        /// <summary>
        /// Formats the report data.
        /// </summary>
        /// <param name="utvalg">The utvalg.</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showHouseholdsReserved">if set to <c>true</c> [show households reserved].</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="level">The level.</param>
        /// <param name="uptolevel">The uptolevel.</param>
        /// <param name="reportType">The Report Type.</param>
        /// <returns></returns>
        public Task<List<BasicDetail>> formatReportData(Puma.Shared.Utvalg utvalg, bool showBusiness, bool showHouseholds, bool showHouseholdsReserved, string strDayDetails, int level, int uptolevel, string reportType)
        {


            List<BasicDetail> routesData = new List<BasicDetail>();

            foreach (var reoler in utvalg.Reoler)
            {
                if (level <= 0)
                {
                    BasicDetail flk = new BasicDetail()
                    {
                        name = reoler.Fylke,
                        cat = "flyke",
                        prisZone = reoler.PrisSone,
                        key = reoler.FylkeId,
                        pkey = null,
                        CssClass = reportType.ToLower() == "pdf" ? "flykerow" : "5"
                    };
                    _ = GetHouseHoldsCount<BasicDetail>(reoler, flk, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);

                    routesData.Add(flk);
                }

                if (uptolevel <= 0) continue;

                if (level <= 1)
                {
                    BasicDetail kommune = new BasicDetail()
                    {
                        name = reoler.Kommune.ToString(),
                        cat = "kommune",
                        prisZone = reoler.PrisSone,
                        key = reoler.KommuneId,
                        pkey = level == 1 ? null : reoler.FylkeId,
                        CssClass = reportType.ToLower() == "pdf" ? "kommunerow" : "6"
                    };
                    var kommuneobj = GetHouseHoldsCount<BasicDetail>(reoler, kommune, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(kommune);
                }
                if (uptolevel <= 1) continue;

                if (level <= 2)
                {
                    BasicDetail team = new BasicDetail()
                    {
                        name = reoler.TeamName.ToString(),
                        cat = "team",
                        prisZone = reoler.PrisSone,
                        key = reoler.KommuneId + reoler.TeamName,
                        pkey = level == 2 ? null : reoler.KommuneId,
                        CssClass = reportType.ToLower() == "pdf" ? "teamrow" : "7"
                    };
                    _ = GetHouseHoldsCount<BasicDetail>(reoler, team, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(team);
                }
                if (uptolevel <= 2) continue;

                if (level == 3 && uptolevel == 4)
                {
                    BasicDetail postalData = new BasicDetail()
                    {
                        name = reoler.PostalZone,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = reoler.PostalZone.ToString(),
                        pkey = null,
                        CssClass = reportType.ToLower() == "pdf" ? "postalrow" : "8"
                    };

                    _ = GetHouseHoldsCount<BasicDetail>(reoler, postalData, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(postalData);

                    BasicDetail route = new BasicDetail()
                    {
                        name = reoler.Name,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = reoler.ReolId.ToString(),
                        pkey = reoler.PostalZone.ToString(),
                        CssClass = reportType.ToLower() == "pdf" ? "ruterow" : "8"
                    };

                    _ = GetHouseHoldsCount<BasicDetail>(reoler, route, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(route);
                }
                else
                {
                    BasicDetail route = new BasicDetail()
                    {
                        name = level == 3 ? reoler.PostalZone : reoler.Name,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = level == 3 ? reoler.PostalZone.ToString() : reoler.ReolId.ToString(),
                        pkey = level >= 3 ? null : reoler.KommuneId + reoler.TeamName,
                        CssClass = reportType.ToLower() == "pdf" ? "ruterow" : "8"
                    };

                    _ = GetHouseHoldsCount<BasicDetail>(reoler, route, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(route);
                }


            }

            List<BasicDetail> formattedList = FillRecursive(routesData, null);

            return Task.FromResult(formattedList);
        }
    }
}
