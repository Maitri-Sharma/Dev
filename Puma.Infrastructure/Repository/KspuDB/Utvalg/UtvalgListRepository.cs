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
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;
using Microsoft.Extensions.DependencyInjection;
using Puma.DataLayer.BusinessEntity.UtvalgList;

namespace Puma.Infrastructure.Repository.KspuDB.Utvalg
{
    public class UtvalgListRepository : KsupDBGenericRepository<utvalglist>, IUtvalgListRepository
    {

        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IConfigurationRepository _configurationRepository;
        //private readonly IUtvalgListModificationRepository _utvalgListModificationRepository;

        private readonly ILogger<UtvalgListRepository> _logger;
        //private readonly ILogger<UtvalgListRepository> logger1;
        private string ConfigKey = "CurrentReolTableName";
        public readonly string connctionstring;
        public readonly string errMsgListName = "Listenavnet må ha minst 3 tegn.";
        public readonly string errMsgIllegalCharsLst = "Listenavnet inneholder ulovlige tegn. Fjern tegnene '<' og '>' dersom eksisterer i navnet.";
        public readonly string errMsgListNameWithSpaces = "Listenavnet kan ikke ha mellomrom i begynnelsen eller slutten av navnet. Fjern mellomrom og prøv på nytt.";
        public UtvalgListRepository(KspuDBContext context, IReolRepository reolRepository, ILogger<UtvalgListRepository> logger,
             IServiceProvider services, IConfigurationRepository configurationRepository) : base(context)
        {
            _logger = logger;
            // _utvalgRepository = utvalgRepository;
            _configurationRepository = configurationRepository;
            //_utvalgListModificationRepository = utvalgListModificationRepository;
            _utvalgRepository = services.GetService<IUtvalgRepository>();

            connctionstring = _context.Database.GetConnectionString();

        }

        public Task<UtvalgList> GetUtvalgListSimple(int listId)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgListSimple");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            DataRow dataRow;


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListSimple: " + exception.Message);
                throw;
            }
            if (utvData.Rows.Count != 1)
                throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            dataRow = utvData.Rows[0];
            _logger.LogInformation("Number of row returned: ", utvData.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListSimple");

            return Task.FromResult(CreateListFromRow(dataRow));
        }

        private UtvalgList CreateListFromRow(DataRow row)
        {
            UtvalgList utv = new UtvalgList();
            Utils utils = new Utils();
            utv.ListId = Convert.ToInt32(row["r_utvalglistid"]);// utils.GetIntFromRow(row, "r_utvalglistid");
            utv.KundeNummer = utils.GetStringFromRow(row, "r_kundenummer");
            utv.Name = utils.GetStringFromRow(row, "r_utvalglistname");
            utv.Logo = utils.GetStringFromRow(row, "r_logo");
            utv.OrdreReferanse = utils.GetStringFromRow(row, "r_ordrereferanse");
            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null))))
                utv.OrdreType = 0;
            else
                utv.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordretype", typeof(OrdreType), null);

            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null))))
                utv.OrdreStatus = 0;
            else
                utv.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_ordrestatus", typeof(OrdreStatus), null);

            utv.InnleveringsDato = Convert.ToDateTime(Convert.IsDBNull(row["r_innleveringsdato"]) ? null : (DateTime?)(row["r_innleveringsdato"]));

            utv.Avtalenummer = Convert.ToInt32(Convert.IsDBNull(row["r_avtalenummer"]) ? null : Convert.ToInt32(row["r_avtalenummer"]));// utils.GetIntFromRow(row, "r_avtalenummer");

            utv.BasedOn = Convert.ToInt32(Convert.IsDBNull(row["r_basedon"]) ? null : Convert.ToInt32(row["r_basedon"]));
            int utvalgCount = GetMemberUtlvagCount(utv.ListId).Result;
            if (utvalgCount == 0 && utv.BasedOn <= 0)
            {
                utv.AntallWhenLastSaved = 0;
            }
            else
                utv.AntallWhenLastSaved = (long)Convert.ToDouble(row["r_antall"]);
            utv.Weight = Convert.ToInt32(Convert.IsDBNull(row["r_vekt"]) ? null : (int?)(row["r_vekt"]));
            //utv.Weight = Convert.ToInt32(row["r_vekt"]);
            utv.DistributionDate = Convert.ToDateTime(Convert.IsDBNull(row["r_distribusjonsdato"]) ? null : (DateTime?)(row["r_distribusjonsdato"]));

            if (String.IsNullOrWhiteSpace(Convert.ToString(utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null))))
                utv.DistributionType = 0;
            else
                utv.DistributionType = (DistributionType)utils.GetEnumFromNameFromRow(row, "r_distribusjonstype", typeof(DistributionType), null);

            //utv.IsBasis = Convert.ToDouble(Convert.IsDBNull(row["r_isbasis"]) ? null : (double?)(row["r_isbasis"]))>0;
            //utv.IsBasis = Convert.ToInt32(row["r_isbasis"]) > 0;// utils.GetIntFromRow(row, "r_isbasis") > 0;
            utv.IsBasis = Convert.ToInt32(Convert.IsDBNull(row["r_isbasis"]) ? null : Convert.ToInt32(row["r_isbasis"])) > 0;
            //utv.BasedOn = Convert.ToInt32(row["r_basedon"]);// utils.GetIntFromRow(row, "r_basedon");
            utv.WasBasedOn = Convert.ToInt32(Convert.IsDBNull(row["r_wasbasedon"]) ? null : Convert.ToInt32(row["r_wasbasedon"]));
            //utv.Thickness = Convert.ToDouble(row["r_thickness"]);// utils.GetDoubleFromRow(row, "r_thickness");
            //utv.AllowDouble = Convert.ToInt32(row["r_allowdouble"]) > 0; //utils.GetIntFromRow(row, "r_allowdouble") > 0;
            //utv.WasBasedOn = Convert.ToInt32(row["r_wasbasedon"]);// utils.GetIntFromRow(row, "r_wasbasedon");
            utv.Thickness = Convert.ToDouble(Convert.IsDBNull(row["r_thickness"]) ? null : Convert.ToDouble(row["r_thickness"]));
            utv.AllowDouble = Convert.ToInt32(Convert.IsDBNull(row["r_allowdouble"]) ? null : Convert.ToInt32(row["r_allowdouble"])) > 0;

            if (utv.BasedOn > 0)
            {
                utv.BasedOnName = SetBasedOnWasBasedOnName(utv.BasedOn).Result;
            }

            if (utv.WasBasedOn > 0)
            {
                utv.WasBasedOnName = SetBasedOnWasBasedOnName(utv.WasBasedOn).Result;
            }

            if (row.Table.Columns.Contains("r_modificationDate"))
            {
                utv.ModificationDate = Convert.ToDateTime(Convert.IsDBNull(row["r_modificationDate"]) ? null : (DateTime?)(row["r_modificationDate"]));
            }
            if (row.Table.Columns.Contains("r_selectionCount"))
            {
                long recreatedCount = Convert.ToInt64((row["r_selectionCount"]));

                if (recreatedCount > 0)
                    utv.IsRecreated = false;
                else
                    utv.IsRecreated = true;
            }
            else
                utv.IsRecreated = true;

            if (row.Table.Columns.Contains("r_memberListCount"))
            {
                long memberListCount = Convert.ToInt64((row["r_memberListCount"]));

                if (memberListCount > 0)
                    utv.hasMemberList = true;
                else
                    utv.hasMemberList = false;
            }
            else
                utv.hasMemberList = false;

            if (row.Table.Columns.Contains("r_parentutvalglistid"))
            {
                utv.ParentListId = Convert.ToInt64(Convert.IsDBNull(row["r_parentutvalglistid"]) ? 0 : Convert.ToInt64(row["r_parentutvalglistid"]));
            }

            return utv;


        }

        public Task<int> GetMemberUtlvagCount(int ListId)
        {
            _logger.LogDebug("Preparing the data for GetMemberUtlvagCount");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result; ;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = ListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.memberutvalgcount", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetMemberUtlvagCount: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from GetMemberUtlvagCount");

            if (result == DBNull.Value)
                return Task.FromResult(0);
            return Task.FromResult(System.Convert.ToInt32(result));
        }
        public async Task<long> SaveUtvalgListData(Puma.Shared.UtvalgList utvalglistData, string userName)
        {
            _logger.LogDebug("Inside into SaveUtvalgListData");
            int ParentUtvalgListId = await GetParentUtvalgListId(utvalglistData.ListId);


            int updateResult = 0;
            if (utvalglistData.ListId > 0)
            {
                updateResult = await UpdateUtvalgList(utvalglistData);
            }

            if (utvalglistData.ListId == 0)
            {
                utvalglistData.ListId = await _utvalgRepository.GetSequenceNextVal("KSPU_DB.UtvalgListId_Seq");
            }

            if (updateResult == 0)
            {
                _ = await SaveUtvalgList(utvalglistData);
            }

            await SaveUtvalgListModifications(utvalglistData.ListId, "SaveUtvalgList - ", userName);

            // Check if list was connected to parent list, if so - update modification table
            if (ParentUtvalgListId != 0)
                await SaveUtvalgListModifications(ParentUtvalgListId, "SaveUtvalgList Parent- ", userName);

            if ((ParentUtvalgListId > 0))
                await UpdateAntallInList(ParentUtvalgListId);

            if ((utvalglistData.ParentList != null && utvalglistData.ParentList.ListId != ParentUtvalgListId))
                await UpdateAntallInList(utvalglistData.ParentList.ListId);

            return utvalglistData.ListId;
        }

        /// <summary>
        /// Update Utvalg list distribution data info
        /// </summary>
        /// <param name="utvalglistData"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<long> UpdateUtvalgListDistributionInfo(Puma.Shared.UtvalgList utvalglistData, string userName)
        {
            _logger.LogDebug("Inside into UpdateUtvalgListDistributionData");

            if (utvalglistData.ListId > 0)
            {
                _ = await UpdateUtvalgListDistributionData(utvalglistData);
            }


            await SaveUtvalgListModifications(utvalglistData.ListId, "SaveUtvalgList - ", userName);


            return utvalglistData.ListId;
        }

        public async Task<int> SaveUtvalgList(Puma.Shared.UtvalgList utvalglistData)
        {



            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgList");
            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[19];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalglistData.ListId;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = utvalglistData.Name;

            npgsqlParameters[2] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[2].Value = utvalglistData.ParentList == null ? 0 : utvalglistData.ParentList.ListId;

            npgsqlParameters[3] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[3].Value = utvalglistData.Antall;

            npgsqlParameters[4] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = (int)utvalglistData.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalglistData.OrdreType);

            npgsqlParameters[5] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[5].Value = string.IsNullOrWhiteSpace(utvalglistData.OrdreReferanse) ? string.Empty : Convert.ToString(utvalglistData.OrdreReferanse);

            npgsqlParameters[6] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = (int)utvalglistData.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalglistData.OrdreStatus);

            npgsqlParameters[7] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[7].Value = !string.IsNullOrWhiteSpace(utvalglistData.KundeNummer) ? utvalglistData.KundeNummer : DBNull.Value;

            npgsqlParameters[8] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[8].Value = (utvalglistData.InnleveringsDato != DateTime.MinValue ? utvalglistData.InnleveringsDato.Date : DateTime.Now);

            npgsqlParameters[9] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = !string.IsNullOrWhiteSpace(utvalglistData.Logo) ? utvalglistData.Logo : DBNull.Value;

            npgsqlParameters[10] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[10].Value = utvalglistData.Avtalenummer;

            npgsqlParameters[11] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[11].Value = utvalglistData.IsBasis ? 1 : 0;

            npgsqlParameters[12] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[12].Value = utvalglistData.BasedOn;

            npgsqlParameters[13] = new NpgsqlParameter("p_wasbasedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[13].Value = utvalglistData.WasBasedOn;

            npgsqlParameters[14] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[14].Value = utvalglistData.Weight;

            npgsqlParameters[15] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[15].Value = utvalglistData.DistributionDate;

            npgsqlParameters[16] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[16].Value = (int)utvalglistData.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalglistData.DistributionType);

            npgsqlParameters[17] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[17].Value = utvalglistData.AllowDouble ? 1 : 0;

            npgsqlParameters[18] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[18].Value = utvalglistData.Thickness;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglist", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in SaveUtvalgList: " + exception.Message);
                    throw;
                }
            }


            _logger.LogDebug("Exiting from saveutvalglist");

            return Convert.ToInt32(result);
        }


        /// <summary>
        /// Update utvalg list distribution data in DB
        /// </summary>
        /// <param name="utvalglistData"></param>
        /// <returns></returns>
        public async Task<int> UpdateUtvalgListDistributionData(Puma.Shared.UtvalgList utvalglistData)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateUtvalgListDistributionData");
            object result;


            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[5];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalglistData.ListId;

            npgsqlParameters[1] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = utvalglistData.Weight;

            npgsqlParameters[2] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[2].Value = utvalglistData.DistributionDate;

            npgsqlParameters[3] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = (int)utvalglistData.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalglistData.DistributionType);



            npgsqlParameters[4] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[4].Value = utvalglistData.Thickness;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalglistdistributiondata", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in UpdateUtvalgListDistributionData: " + exception.Message);
                    throw;
                }
            }


            _logger.LogDebug("Exiting from UpdateUtvalgListDistributionData");

            return Convert.ToInt32(result);
        }

        public async Task<int> UpdateUtvalgList(Puma.Shared.UtvalgList utvalglistData)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateUtvalgList");
            object result;


            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[19];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalglistData.ListId;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = utvalglistData.Name;

            npgsqlParameters[2] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[2].Value = utvalglistData.ParentList == null ? 0 : utvalglistData.ParentList.ListId;

            npgsqlParameters[3] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[3].Value = utvalglistData.Antall;

            npgsqlParameters[4] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = (int)utvalglistData.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalglistData.OrdreType);

            npgsqlParameters[5] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[5].Value = !string.IsNullOrWhiteSpace(utvalglistData.OrdreReferanse) ? utvalglistData.OrdreReferanse : DBNull.Value;

            npgsqlParameters[6] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[6].Value = (int)utvalglistData.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalglistData.OrdreStatus);

            npgsqlParameters[7] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[7].Value = !string.IsNullOrWhiteSpace(utvalglistData.KundeNummer) ? utvalglistData.KundeNummer : DBNull.Value;

            npgsqlParameters[8] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[8].Value = (utvalglistData.InnleveringsDato != DateTime.MinValue ? utvalglistData.InnleveringsDato : DateTime.Now);

            npgsqlParameters[9] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[9].Value = !string.IsNullOrWhiteSpace(utvalglistData.Logo) ? utvalglistData.Logo : DBNull.Value;

            npgsqlParameters[10] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[10].Value = utvalglistData.Avtalenummer;

            npgsqlParameters[11] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[11].Value = utvalglistData.IsBasis ? 1 : 0;


            npgsqlParameters[12] = new NpgsqlParameter("p_basedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[12].Value = utvalglistData.BasedOn;

            npgsqlParameters[13] = new NpgsqlParameter("p_wasbasedon", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[13].Value = utvalglistData.WasBasedOn;

            npgsqlParameters[14] = new NpgsqlParameter("p_vekt", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[14].Value = utvalglistData.Weight;

            npgsqlParameters[15] = new NpgsqlParameter("p_distribusjonsdato", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[15].Value = utvalglistData.DistributionDate;

            npgsqlParameters[16] = new NpgsqlParameter("p_distribusjonstype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[16].Value = (int)utvalglistData.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalglistData.DistributionType);

            npgsqlParameters[17] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[17].Value = utvalglistData.AllowDouble ? 1 : 0;

            npgsqlParameters[18] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
            npgsqlParameters[18].Value = utvalglistData.Thickness;

            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalgList", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in UpdateUtvalgList: " + exception.Message);
                    throw;
                }
            }


            _logger.LogDebug("Exiting from saveutvalglist");

            return Convert.ToInt32(result);
        }


        public async Task<int> ChangeParentOfList(ChangeOfParentList changeOfParentList)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for ChangeParentOfList");
            object result;


            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[7];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistidtobechange", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = changeOfParentList.ListId;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistidparent", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = changeOfParentList.ParentListId;

            npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = (int)changeOfParentList.OrderType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)changeOfParentList.OrderType);

            npgsqlParameters[3] = new NpgsqlParameter("p_ordretypenew", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = (int)changeOfParentList.NewOrderType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)changeOfParentList.NewOrderType);

            npgsqlParameters[4] = new NpgsqlParameter("p_ordrereferansenew", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[4].Value = !string.IsNullOrWhiteSpace(changeOfParentList.NewOrderReference) ? changeOfParentList.NewOrderReference : DBNull.Value;

            npgsqlParameters[5] = new NpgsqlParameter("p_ordrestatusnew", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[5].Value = (int)changeOfParentList.NewOrderStattus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)changeOfParentList.NewOrderStattus);

            npgsqlParameters[6] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[6].Value = changeOfParentList.Antall;



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.changeparentlistoflist", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in ChangeParentListOfList: " + exception.Message);
                    throw;
                }
            }


            _logger.LogDebug("Exiting from ChangeParentListOfList");

            return Convert.ToInt32(result);
        }


        public async Task<int> AcceptAllChangesForList(int listId, string userName)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for AcceptAllChangesForList");
            object result;


            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;

            npgsqlParameters[1] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = userName;


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.AcceptAllListChanges", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in AcceptAllListChanges: " + exception.Message);
                    throw;
                }
            }


            _logger.LogDebug("Exiting from AcceptAllChangesForList");

            return Convert.ToInt32(result);
        }

        //NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
        //    int result;
        //    if (list.Name.Trim().Length < 3)
        //        throw new Exception(errMsgListName);
        //    if ((list.Name.IndexOf(">") > -1 | list.Name.IndexOf("<") > -1))
        //        throw new Exception(errMsgIllegalCharsLst);
        //    if ((list.Name.IndexOf(" ", 0, 1) > -1 | list.Name.IndexOf(" ", list.Name.Length - 1, 1) > -1))
        //        throw new Exception(errMsgListNameWithSpaces);
        //    // If Not list.KundeNummer Is Nothing AndAlso list.KundeNummer.Trim() = "" Then Throw New Exception("Customer number '" & list.KundeNummer & "' is valid when saving an utvalgsliste!")
        //    try
        //    {
        //        // Finner evt parentlistid
        //        int ParentUtvalgListId = await GetParentUtvalgListId(list.ListId);

        //        if (list.ListId == 0)
        //            // list.ListId = GetSequenceNextVal("KSPU_DB.UtvalgListId_Seq", trans);
        //            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
        //        npgsqlParameters[0].Value = list.ListId;

        //        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
        //        {
        //            result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglist", CommandType.StoredProcedure, npgsqlParameters);
        //        }

        //        // _logger.LogInformation("Number of row returned {0}", Convert.ToInt32(result));

        //        if (result == 0)
        //        {
        //            // Update affected no rows, we must do an insert
        //            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
        //            {
        //                result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglist", CommandType.StoredProcedure, npgsqlParameters);
        //            }

        //            //cmd.CommandText = " INSERT INTO KSPU_DB.UtvalgList (UtvalgListID, KundeNummer, Logo, OrdreReferanse, OrdreType, OrdreStatus, InnleveringsDato, Antall, UtvalgListName, ParentUtvalgListId, Vekt, Distribusjonsdato, Distribusjonstype, IsBasis, BasedOn, WasBasedOn, AllowDouble, Thickness) "
        //            //    + " VALUES (:UtvalgListId, :KundeNummer, :Logo, :OrdreReferanse, :OrdreType, :OrdreStatus, :InnleveringsDato, :Antall, :UtvalgListName, :ParentUtvalgListId, :Vekt, :Distribusjonsdato, :Distribusjonstype, :IsBasis, :BasedOn, :WasBasedOn, :AllowDouble, :Thickness) ";
        //            //ExecuteNonQuery(cmd);
        //        }

        //        SaveUtvalgListModifications(list.ListId, userName, "SaveUtvalgList - ", list.Antall);

        //        // Check if list was connected to parent list, if so - update modification table
        //        if (list.ParentList != null)
        //            SaveUtvalgListModifications(list.ParentList.ListId, userName, "SaveUtvalgList Parent- ", list.Antall);

        //        if ((ParentUtvalgListId > 0))
        //            await UpdateAntallInList(ParentUtvalgListId);

        //        if ((list.ParentList != null && list.ParentList.ListId != ParentUtvalgListId))
        //            await UpdateAntallInList(list.ParentList.ListId);
        //        list.AntallWhenLastSaved = list.Antall;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public Task<int> GetParentUtvalgListId(int utvalgListId)
        {
            _logger.LogDebug("Preparing the data for GetParentUtvalgListId");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            object result; ;

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.getparentutvalglistid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetParentUtvalgListId: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from GetParentUtvalgListId");

            if (result == DBNull.Value)
                return Task.FromResult(-1);
            return Task.FromResult(System.Convert.ToInt32(result));

        }

        public async Task SaveUtvalgListModifications(int listid, string info, string userName)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SaveUtvalgListModifications");
            int result;
            //int seq = GetSequenceNextVal("KSPU_DB.GD_UTVALGLISTMOD_SEQ");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listid;

            npgsqlParameters[1] = new NpgsqlParameter("p_username", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = userName;

            npgsqlParameters[2] = new NpgsqlParameter("p_modificationinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[2].Value = info;

            npgsqlParameters[3] = new NpgsqlParameter("p_modificationinfoantall", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[3].Value = 0;

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.saveutvalglistmodifications", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SaveUtvalgListModifications: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from SaveUtvalgListModifications");

        }


        public async Task UpdateAntallInList(int UtvalgListId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateAntallInList");
            //DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            int totalsum = 0;
            try
            {
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid1", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = UtvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid2", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[1].Value = UtvalgListId;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    totalsum = dbhelper.ExecuteScalar<int>("kspu_db.updateantallinlist", CommandType.StoredProcedure, npgsqlParameters);
                    //utvData = dbhelper.FillDataTable("kspu_db.updateantallinlist", CommandType.StoredProcedure, npgsqlParameters);
                }

                //OracleCommand cmd = new OracleCommand();
                //string sql = " select sum(antall) from KSPU_DB.utvalg where utvalglistid in (select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :UtvalgListId1 or utvalglistid = :UtvalgListId2)";
                //cmd = new OracleCommand(sql);
                //AddParameterInteger(cmd, "UtvalgListId1", UtvalgListId);
                //AddParameterInteger(cmd, "UtvalgListId2", UtvalgListId);



                // Oppdater lista med nytt antall
                int result;
                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = UtvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[1].Value = totalsum;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {

                    result = dbhelper.ExecuteNonQuery("kspu_db.updateantallinlistforantall", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" UPDATE KSPU_DB.UtvalgList SET Antall = :Antall WHERE UtvalgListID = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //AddParameterInteger(cmd, "Antall", totalsum);
                //ExecuteNonQuery(cmd);

                // Oppdater arvet liste med nytt antall
                if (UtvalgListId > 0)
                {
                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[0].Value = UtvalgListId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[1].Value = totalsum;
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.updateantallinlistforantallforbasedon", CommandType.StoredProcedure, npgsqlParameters);
                    }
                }
                //cmd = new OracleCommand(" UPDATE KSPU_DB.UtvalgList SET Antall = :Antall WHERE BasedOn = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //AddParameterInteger(cmd, "Antall", totalsum);
                //ExecuteNonQuery(cmd);

                // Oppdatering av evt parent list etter endring av childlist
                // Finn parent list
                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];
                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = UtvalgListId;

                int ParentUtvalgListId = 0;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    ParentUtvalgListId = dbhelper.ExecuteScalar<int>("kspu_db.getparentutvalglistid", CommandType.StoredProcedure, npgsqlParameters1);
                }

                // Oppdater liste
                if (ParentUtvalgListId > 0)
                    await UpdateAntallInList(ParentUtvalgListId);
            }
            catch (Exception exception)
            {
                throw new Exception("Fikk ikke oppdatert antall på lister" + exception.Message + "for utvalg med Id=" + UtvalgListId);

            }
            _logger.LogDebug("Exiting from UpdateAntallInList");
        }


        /// <summary>
        /// Update antall data in utvalg List
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="antall"></param>
        /// <returns></returns>
        public async Task UpdateAntallForList(int listId, double antall)
        {

            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateAntallForList");
            //DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            int result;
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;

            npgsqlParameters[1] = new NpgsqlParameter("p_antall", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = antall;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {

                result = dbhelper.ExecuteNonQuery("kspu_db.updateantallinlistforantall", CommandType.StoredProcedure, npgsqlParameters);
            }
        }
        public async Task<UtvalgsListCollection> SearchUtvalgListWithChildrenById(int utvalglistId, bool includeReols = true)
        {
            _logger.LogDebug("Preparing the data for SearchUtvalgListWithChildrenById");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgsListCollection utvLColl;


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalglistId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithchildrenbyid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListWithChildrenById: " + exception.Message);
                throw;
            }
            utvLColl = GetAllListContent(utvData, includeReols, false);
            await AddSistOppdatertToLists(utvLColl);
            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListWithChildrenById");

            return utvLColl;
        }


        public async Task AddSistOppdatertToLists(UtvalgsListCollection utvalgsListCollection)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for AddSistOppdatertToLists");
            string sistEndretAv = "";
            foreach (UtvalgList list in utvalgsListCollection)
            {

                // sjekker listens egne datoer også
                DateTime sistListOppdatert = new DateTime(1, 1, 1);
                if (list.Modifications == null)
                {
                    list.Modifications = new List<UtvalgModification>();
                    GetUtvalgListModifications(list);
                }
                if (list.Modifications != null)
                {
                    foreach (UtvalgModification modification in list.Modifications)
                    {
                        if (modification.ModificationTime.CompareTo(sistListOppdatert) > 0)
                        {
                            sistListOppdatert = modification.ModificationTime;
                            sistEndretAv = modification.UserId;
                        }
                    }
                }
                list.SistOppdatert = sistListOppdatert;
                list.SistEndretAv = sistEndretAv;

                // sjekker listens utvalg
                DateTime sistOppdatert = new DateTime(1, 1, 1);
                foreach (Puma.Shared.Utvalg utvalg in list.MemberUtvalgs)
                {
                    if (utvalg.Modifications != null)
                    {
                        foreach (UtvalgModification modification in utvalg.Modifications)
                        {
                            if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                            {
                                sistOppdatert = modification.ModificationTime;
                                sistEndretAv = modification.UserId;
                            }
                        }
                    }
                }
                if (sistOppdatert.CompareTo(list.SistOppdatert) > 0)
                {
                    list.SistOppdatert = sistOppdatert;
                    list.SistEndretAv = sistEndretAv;
                }

                // childlist
                foreach (UtvalgList childlist in list.MemberLists)
                {
                    sistOppdatert = new DateTime(1, 1, 1);
                    if (childlist.MemberUtvalgs.Count == 0)
                        childlist.MemberUtvalgs = _utvalgRepository.SearchUtvalgByUtvalListId(childlist.ListId, false).Result;
                    foreach (Puma.Shared.Utvalg utvalg in childlist.MemberUtvalgs)
                    {
                        if (utvalg.Modifications != null)
                        {
                            foreach (UtvalgModification modification in utvalg.Modifications)
                            {
                                if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                                {
                                    sistOppdatert = modification.ModificationTime;
                                    sistEndretAv = modification.UserId;
                                }
                            }
                        }
                    }

                    // sjekk underlistens egen dato også
                    if (childlist.Modifications == null)
                    {
                        childlist.Modifications = new List<UtvalgModification>();
                        GetUtvalgListModifications(childlist);
                    }
                    if (childlist.Modifications != null)
                    {
                        foreach (UtvalgModification modification in childlist.Modifications)
                        {
                            if (modification.ModificationTime.CompareTo(sistOppdatert) > 0)
                            {
                                sistOppdatert = modification.ModificationTime;
                                sistEndretAv = modification.UserId;
                            }
                        }
                    }

                    childlist.SistOppdatert = sistOppdatert;
                    childlist.SistEndretAv = sistEndretAv;
                    if (childlist.SistOppdatert.CompareTo(list.SistOppdatert) > 0)
                    {
                        list.SistOppdatert = childlist.SistOppdatert;
                        list.SistEndretAv = childlist.SistEndretAv;
                    }
                }
            }
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med KundeNummer.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste.
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="kundeNummer"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <param name="includeReols"></param>
        ///     ''' <returns>UtvalgsListCollection</returns>
        ///     ''' <remarks></remarks>

        public async Task<UtvalgsListCollection> SearchUtvalgListWithChildrenByKundeNummer(string kundeNummer, SearchMethod searchMethod, bool includeReols = true)
        {
            _logger.LogDebug("Preparing the data for SearchUtvalgListWithChildrenByKundeNummer");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgsListCollection utvLColl;
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = kundeNummer;

            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistwithchildrenbykundeNummer", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListWithChildrenByKundeNummer: " + exception.Message);
                throw;
            }
            utvLColl = GetAllListContent(utvData, includeReols, false);
            await AddSistOppdatertToLists(utvLColl);
            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListWithChildrenByKundeNummer");

            return utvLColl;
        }

        /// <summary>
        ///     ''' Gets a list of the reol ids in the UtvalgReoltable for a given utvalglist.
        ///     ''' Includes all childslists utvalg
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<List<long>> GetUtvalgListReolIDs(int listId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListReolIDs");
            DataTable reolIds;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<long> result = new List<long>();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    reolIds = dbhelper.FillDataTable("kspu_db.getutvalglistreolids", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListReolIDs: " + exception.Message);
                throw;
            }
            foreach (DataRow row in reolIds.Rows)
                //result.Add((long)row["r_reolid"]);
                result.Add(Convert.ToInt64(utils.GetLongFromRow(row, "r_reolid")));

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetUtvalgListReolIDs");

            return result;

        }

        /// <summary>
        ///  To check if utvalg List exists
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <returns>True or False</returns>
        ///     ''' <remarks></remarks>


        public async Task<bool> UtvalgListNameExists(string utvalglistname)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UtvalgListNameExists");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = utvalglistname.ToUpper();
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.utvalglistnameexists", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UtvalgListNameExists: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from UtvalgListNameExists");
            if (result == null | (result) is DBNull)
                return false;
            return System.Convert.ToInt32(result) > 0;

        }

        /// <summary>
        ///     ''' Checks if a list can be connected to the specified parent list. Returns nothing if both:
        ///     ''' * The list has no member lists.
        ///     ''' * The parent list has no parent list.
        ///     ''' 
        ///     ''' This is to avoid structures with more than two levels of lists.
        ///     ''' 
        ///     ''' Otherwise returns an error message.
        ///     ''' </summary>
        ///     ''' <param name="listID"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        public async Task<bool> ListHasMemberLists(int listID)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for ListHasMemberLists");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listID;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.ListHasMemberLists", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in ListHasMemberLists: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);

            _logger.LogDebug("Exiting from ListHasMemberLists");
            if ((result) is decimal)
            {
                if (System.Convert.ToInt32(result) > 0)
                    return true;
            }
            return false;

        }



        /// <summary>
        ///  To check if list has Parent list or not
        ///     ''' </summary>
        ///     ''' <param name="utvalgListId"></param>
        ///     ''' <returns>True or False</returns>
        ///     ''' <remarks></remarks>
        public async Task<bool> ListHasParentList(int utvalgListId)
        {
            return await GetParentUtvalgListId(utvalgListId) > 0;
        }

        /// <summary>
        ///     ''' Search Utvalg List by user ID
        ///     ''' </summary>
        ///     ''' <param name="userID"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns>UtvalgSearchCollection</returns>
        public async Task<UtvalgsListCollection> SearchUtvalgListByUserID(string userID, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListByUserID");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();


            npgsqlParameters[0] = new NpgsqlParameter("p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utils.CreateSearchString(userID, searchMethod);
            npgsqlParameters[0].Value = userID;
            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistbyuserid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListByUserID: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvLData.Rows)
                utvLColl.Add(CreateListFromRow(row));

            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListByUserID");
            return utvLColl;
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med navn.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        public async Task<UtvalgsListCollection> SearchUtvalgListSimple(string utvalglistname, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListSimple");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utils.CreateSearchString(utvalglistname, searchMethod);
            npgsqlParameters[0].Value = utvalglistname;

            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListSimple: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvLData.Rows)
                utvLColl.Add(CreateListFromRow(row));

            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimple");
            return utvLColl;
        }

        /// Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - parentlisten
        public async Task<UtvalgList> GetUtvalgList(Int64 listId, bool getParentList = true, bool getMemberUtvalg = false)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data fo GetUtvalgList");
            DataTable utvLData;
            UtvalgList list;
            DataRow row;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];


            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglist", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgList: " + exception.Message);
                throw;
            }

            if (utvLData.Rows.Count != 1)
            {
                _logger.LogError("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
                return null;
            }
            row = utvLData.Rows[0];
            list = CreateListFromRow(row);

            if (!Convert.IsDBNull(row["r_parentutvalglistid"]))
            {
                if (getParentList)
                {
                    if (list.BasedOn <= 0)
                    {
                        list.ParentList = await GetUtvalgList(Convert.ToInt32(row["r_parentutvalglistid"]));
                        if (list != null && list.ParentList != null && !list.ParentList.MemberLists.Contains(list))
                            list.ParentList.MemberLists.Add(list);
                    }
                }
            }

            // tilhørende utvalg
            if (getMemberUtvalg)
            {
                UtvalgCollection uc = await _utvalgRepository.SearchUtvalgByUtvalListId(list.ListId);
                foreach (Puma.Shared.Utvalg u in uc.ToArray())
                    list.MemberUtvalgs.Add(u);
                //u.List = list;
            }

            _logger.LogInformation("Result is: ", list);
            _logger.LogDebug("Exiting from GetUtvalgList");
            return list;

        }

        /// <summary>
        ///     ''' Used to get listinfo for a 'arvingliste'. Does get the actual list, not swapping by arvingliste!
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgList> GetUtvalgListNoChild(int listId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListNoChild");
            DataTable utvLData;
            UtvalgList list;
            DataRow row;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistnochild", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListNoChild: " + exception.Message);
                throw;
            }
            if (utvLData.Rows.Count != 1)
                throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            row = utvLData.Rows[0];
            list = CreateListFromRow(row);
            _logger.LogInformation("Result is: ", list);
            _logger.LogDebug("Exiting from GetUtvalgListNoChild");
            return list;

        }

        /// <summary>
        ///     ''' For integration: Get all distinct PRS from reoler in Utvalg from selected Utvalglist and its childlist.
        ///     ''' Uses current ReolMap, independent of recreation done, needed or not.
        ///     ''' </summary>
        ///     ''' <param name="utvalglistId"></param>
        ///     ''' <remarks></remarks>
        public async Task<List<string>> GetDistinctPRSByListID(long utvalglistId)
        {
            _logger.LogDebug("Preparing the data for GetDistinctPRSByListID");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];
            List<string> result = new List<string>();
            string table = await _configurationRepository.GetConfigValue(ConfigKey);
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalglistId;

            npgsqlParameters[1] = new NpgsqlParameter("p_table", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = table;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getdistinctprsbylistid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetDistinctPRSByListID: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvData.Rows)
            {
                string s = row[0].ToString();
                result.Add(s);
            }

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetDistinctPRSByListID");
            return result;
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="utvalglistid"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgsListCollection> SearchUtvalgListSimpleById(int utvalglistid)
        {
            _logger.LogDebug("Preparing the data for SearchUtvalgListSimpleById");
            DataTable utvLData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalglistid;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListSimpleById: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvLData.Rows)
                utvLColl.Add(CreateListFromRow(row));
            await AddSistOppdatertToLists(utvLColl);
            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimpleById");
            return utvLColl;
        }


        /// <summary>
        ///     ''' Search by utvalg list id and/or customer number and agreement number, and you may only select Basisutvalg
        ///     ''' </summary>
        ///     ''' <param name="utvalglistid"></param>
        ///     ''' <param name="customerNos"></param>
        ///     ''' <param name="agreementNos"></param>
        ///     ''' <param name="forceCustomerAndAgreementCheck"></param>
        ///     ''' <param name="extendedInfo"></param>
        ///     ''' <param name="onlyBasisLists"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgsListCollection> SearchUtvalgListSimpleByIdAndCustNoAgreeNo(int utvalglistid, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg = false)
        {
            _logger.LogDebug("Preparing the data for SearchUtvalgListSimpleByIdAndCustNoAgreeNo");
            DataTable utvData;
            //bool abortSearch = false;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            UtvalgsListCollection result = new UtvalgsListCollection();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[0].Value = Convert.ToInt32(utvalglistid);

            npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";
            //npgsqlParameters[1].Value = utils.GetCustomerNoSearchString(customerNos, searchMethodCusomerNos);

            npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";
            //npgsqlParameters[2].Value = utils.GetAgreementNoSearchString(agreementNos);

            npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = onlyBasisLists == 0 ? "0,1" : "1";

            try
            {
                if (forceCustomerAndAgreementCheck)
                {
                    if (utils.CanSearch(customerNos, agreementNos))
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyidandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
                        }
                        foreach (DataRow row in utvData.Rows)
                        {
                            UtvalgList l = CreateListFromRow(row);
                            // add last saved by user if extendedInfo= true
                            if (extendedInfo)
                            {
                                l.Modifications = new List<UtvalgModification>();
                                GetUtvalgListModifications(l);
                            }
                            if (includeChildrenUtvalg)
                            {
                                // tilhørende utvalg til lista
                                UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
                                foreach (Puma.Shared.Utvalg u in uc)
                                    l.MemberUtvalgs.Add(u);
                            }
                            result.Add(l);
                        }
                    }
                    //else
                    //{
                    //    abortSearch = true;
                    //    return result;
                    //}
                }
                else
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyidandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    foreach (DataRow row in utvData.Rows)
                    {
                        UtvalgList l = CreateListFromRow(row);
                        // add last saved by user if extendedInfo= true
                        if (extendedInfo)
                        {
                            l.Modifications = new List<UtvalgModification>();
                            GetUtvalgListModifications(l);
                        }
                        if (includeChildrenUtvalg)
                        {
                            // tilhørende utvalg til lista
                            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
                            foreach (Puma.Shared.Utvalg u in uc)
                                l.MemberUtvalgs.Add(u);
                        }
                        result.Add(l);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListSimpleByIdAndCustNoAgreeNo: " + exception.Message);
            }
            await AddSistOppdatertToLists(result);

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimpleByIdAndCustNoAgreeNo");
            return result;


        }

        /// <summary>
        ///     ''' Search by utvalg list name and/or customer number and agreement number, and you may only select Basisutvalg
        ///     ''' </summary>
        ///     ''' <param name="utvalglistname"></param>
        ///     ''' <param name="customerNos"></param>
        ///     ''' <param name="agreementNos"></param>
        ///     ''' <param name="forceCustomerAndAgreementCheck"></param>
        ///     ''' <param name="extendedInfo"></param>
        ///     ''' <param name="onlyBasisLists"></param>
        ///     ''' <param name="includeChildrenUtvalg"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgsListCollection> SearchUtvalgListSimpleByIDAndCustomerNo(string utvalglistname, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg)
        {
            await Task.Run(() => { });
            //await _utvalgListModificationRepository.GetUtvalgListModifications(
            //    utvalglistname, customerNos, agreementNos, forceCustomerAndAgreementCheck, extendedInfo, onlyBasisLists, includeChildrenUtvalg
            //    );

            //need to check
            _logger.LogDebug("Preparing the data for SearchUtvalgListSimpleByIDAndCustomerNo");
            DataTable utvData;
            //bool abortSearch = false;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[4];
            UtvalgsListCollection result = new UtvalgsListCollection();
            Utils utils = new Utils();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = "%" + utvalglistname.Replace("_", "\\_") + "%";
            //npgsqlParameters[0].Value = utils.CreateSearchString(utvalglistname, searchMethodUtvalgList);

            npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[1].Value = customerNos.Length > 0 ? "'" + String.Join("', '", customerNos) + "'" : "";
            //npgsqlParameters[1].Value = utils.GetCustomerNoSearchString(customerNos, searchMethodCusomerNos);

            npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = agreementNos.Length > 0 ? "'" + String.Join("', '", agreementNos) + "'" : "";

            //npgsqlParameters[1] = new NpgsqlParameter("p_customernos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[1].Value = "'" + String.Join("', '", customerNos) + "'";
            ////npgsqlParameters[1].Value = utils.GetCustomerNoSearchString(customerNos, searchMethodCusomerNos);

            //npgsqlParameters[2] = new NpgsqlParameter("p_agreementnos", NpgsqlTypes.NpgsqlDbType.Varchar);
            //npgsqlParameters[2].Value = "'" + String.Join("', '", agreementNos) + "'";
            //npgsqlParameters[2].Value = utils.GetAgreementNoSearchString(agreementNos);

            npgsqlParameters[3] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = onlyBasisLists == 0 ? "0,1" : "1";

            try
            {
                if (forceCustomerAndAgreementCheck)
                {
                    if (utils.CanSearch(customerNos, agreementNos))
                    {
                        using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                        {
                            utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyutvalgnameandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
                        }
                        foreach (DataRow row in utvData.Rows)
                        {
                            UtvalgList l = CreateListFromRow(row);
                            // add last saved by user if extendedInfo= true
                            if (extendedInfo)
                            {
                                l.Modifications = new List<UtvalgModification>();
                                GetUtvalgListModifications(l);
                            }

                            if (includeChildrenUtvalg)
                            {
                                // tilhørende utvalg til lista
                                UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
                                foreach (Puma.Shared.Utvalg u in uc)
                                    l.MemberUtvalgs.Add(u);
                            }
                            result.Add(l);
                        }


                    }
                    //else
                    //{
                    //    abortSearch = true;
                    //    // return result;
                    //}
                }
                else
                {
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyutvalgnameandcustomernosandagreementnos", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    foreach (DataRow row in utvData.Rows)
                    {
                        UtvalgList l = CreateListFromRow(row);
                        // add last saved by user if extendedInfo= true
                        if (extendedInfo)
                        {
                            l.Modifications = new List<UtvalgModification>();
                            GetUtvalgListModifications(l);
                        }

                        if (includeChildrenUtvalg)
                        {
                            // tilhørende utvalg til lista
                            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(l.ListId, false).Result;
                            foreach (Puma.Shared.Utvalg u in uc)
                                l.MemberUtvalgs.Add(u);
                        }
                        result.Add(l);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListSimpleByIDAndCustomerNo: " + exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimpleByIDAndCustomerNo");
            string utvalgId = string.Join(",", result.Select(x => x.ListId));
            return result;


        }




        public async Task<UtvalgsListCollection> SearchUtvalgListByIsBasis(string utvalglistname, int onlyBasisLists, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListByIsBasis");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utils.CreateSearchString(utvalglistname, searchMethod);
            npgsqlParameters[0].Value = utvalglistname;

            npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);


            npgsqlParameters[2] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = onlyBasisLists == 0 ? "0" : "1";

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistByIsBasis", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListByIsBasis: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvLData.Rows)
                utvLColl.Add(CreateListFromRow(row));

            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimple");
            return utvLColl;


        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med OrdreReferanse og OEBSType.
        ///     '''  Metoden returnerer:
        ///     '''  - alle lister etter søkekriteriet 
        ///     ''' </summary>
        ///     ''' <param name="OrdreReferanse"></param>
        ///     ''' <param name="OrdreType"></param>
        ///     ''' <returns>UtvalgsListCollection</returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgsListCollection> SearchUtvalgListSimpleByOrdreReferanse(string OrdreReferanse, string OrdreType, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListSimpleByOrdreReferanse");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            //npgsqlParameters[0].Value = utils.CreateSearchString(OrdreReferanse, searchMethod);
            npgsqlParameters[0].Value = Convert.ToString(OrdreReferanse);

            npgsqlParameters[1] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, 1);
            //npgsqlParameters[0].Value = utils.CreateSearchString(OrdreReferanse, searchMethod);
            npgsqlParameters[1].Value = Convert.ToString(OrdreType);

            npgsqlParameters[2] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer, 10);
            npgsqlParameters[2].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalglistsimplebyordrereferanse", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListSimpleByOrdreReferanse: " + exception.Message);
                throw;
            }
            foreach (DataRow dataRow in utvLData.Rows)
                utvLColl.Add(CreateListFromRow(dataRow));

            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListSimpleByOrdreReferanse");
            return utvLColl;
        }


        /// <summary>
        ///     ''' Get unique Receivers for a list. 
        ///     ''' Gets only distinct 'ReceiverId'. Field 'Selected' not returned and is always set to False(I think its not in use at all..)
        ///     ''' Checking all chils list and utvalg
        ///     ''' </summary>
        ///     ''' <param name="listid"></param>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgReceiverList> GetUtvalgListReceivers(int listid)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListReceivers");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgReceiverList result = new UtvalgReceiverList();
            UtvalgReceiver utvalgReceiver = new UtvalgReceiver();

            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Bigint);
            npgsqlParameters[0].Value = listid;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistreceivers", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListReceivers: " + exception.Message);
                throw;
            }
            foreach (DataRow dataRow in utvLData.Rows)
            {
                utvalgReceiver.ReceiverId = (ReceiverType)utils.GetEnumFromRow(dataRow, "r_receiverid", typeof(ReceiverType));
                utvalgReceiver.Selected = false;
                result.Add(utvalgReceiver);
            }

            _logger.LogInformation("Number of row returned: ", result.Count);
            _logger.LogDebug("Exiting from GetUtvalgListReceivers");
            return result;

        }


        /// <summary>
        ///     ''' Metoden oppdaterer utvalglista i henhold til krav i integrasjon mot Ordre2
        ///     ''' </summary>
        ///     ''' <param name="utvalgList"></param>
        ///     ''' <param name="username"></param>
        public async Task UpdateUtvalgListForIntegration(UtvalgList utvalgList, string username)
        {
            await Task.Run(() => { });
            try
            {
                _logger.LogDebug("Preparing the data for UpdateUtvalgListForIntegration");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[11];
                int result;
                int result2;
                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = utvalgList.ListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
                npgsqlParameters[1].Value = !string.IsNullOrWhiteSpace(utvalgList.OrdreReferanse) ? utvalgList.OrdreReferanse : string.Empty;

                npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
                npgsqlParameters[2].Value = (int)utvalgList.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalgList.OrdreType);

                npgsqlParameters[3] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreType.Null);
                npgsqlParameters[3].Value = (int)utvalgList.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalgList.OrdreStatus); // Add parameter in Enu type

                npgsqlParameters[4] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
                npgsqlParameters[4].Value = (utvalgList.InnleveringsDato != DateTime.MinValue ? utvalgList.InnleveringsDato : DateTime.Now);

                npgsqlParameters[5] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[5].Value = utvalgList.Weight;

                npgsqlParameters[6] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Timestamp);
                npgsqlParameters[6].Value = utvalgList.DistributionDate;

                npgsqlParameters[7] = new NpgsqlParameter("p_distributiontype", NpgsqlTypes.NpgsqlDbType.Char, (int)DistributionType.Null);
                npgsqlParameters[7].Value = (int)utvalgList.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalgList.DistributionType);

                npgsqlParameters[8] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
                npgsqlParameters[8].Value = utvalgList.Thickness;

                npgsqlParameters[9] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Bigint);
                npgsqlParameters[9].Value = utvalgList.Avtalenummer;

                npgsqlParameters[10] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[10].Direction = ParameterDirection.Output;
                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateutvalglistforintegration", CommandType.StoredProcedure, npgsqlParameters);
                }
                result = Convert.ToInt32(npgsqlParameters[10].Value);
                if (result == 0)
                    _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere et utvalg som ikke eksisterer i KSPU. Utvalgsid: " + utvalgList.ListId);
                if ((utvalgList.BasedOn == 0 && !utvalgList.IsBasis))
                {
                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[0].Value = utvalgList.ListId;

                    npgsqlParameters[1] = new NpgsqlParameter("p_ordrereferanse", NpgsqlTypes.NpgsqlDbType.Varchar, 30);
                    npgsqlParameters[1].Value = !string.IsNullOrWhiteSpace(utvalgList.OrdreReferanse) ? utvalgList.OrdreReferanse : string.Empty;

                    npgsqlParameters[2] = new NpgsqlParameter("p_ordretype", NpgsqlTypes.NpgsqlDbType.Varchar);
                    npgsqlParameters[2].Value = (int)utvalgList.OrdreType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreType)(int)utvalgList.OrdreType);


                    npgsqlParameters[3] = new NpgsqlParameter("p_ordrestatus", NpgsqlTypes.NpgsqlDbType.Varchar, (int)OrdreStatus.Null);
                    npgsqlParameters[3].Value = (int)utvalgList.OrdreStatus == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.OrdreStatus)(int)utvalgList.OrdreStatus);

                    npgsqlParameters[4] = new NpgsqlParameter("p_innleveringsdato", NpgsqlTypes.NpgsqlDbType.Timestamp);
                    npgsqlParameters[4].Value = (utvalgList.InnleveringsDato != DateTime.MinValue ? utvalgList.InnleveringsDato : DateTime.Now);

                    npgsqlParameters[5] = new NpgsqlParameter("p_weight", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[5].Value = utvalgList.Weight;

                    npgsqlParameters[6] = new NpgsqlParameter("p_distributiondate", NpgsqlTypes.NpgsqlDbType.Timestamp);
                    npgsqlParameters[6].Value = utvalgList.DistributionDate;

                    npgsqlParameters[7] = new NpgsqlParameter("p_distributiontype", NpgsqlTypes.NpgsqlDbType.Varchar, (int)DistributionType.Null);
                    npgsqlParameters[7].Value = (int)utvalgList.DistributionType == 0 ? DBNull.Value : EnumDescription.GetEnumDescription((PumaEnum.DistributionType)(int)utvalgList.DistributionType);

                    npgsqlParameters[8] = new NpgsqlParameter("p_thickness", NpgsqlTypes.NpgsqlDbType.Double);
                    npgsqlParameters[8].Value = utvalgList.Thickness;

                    npgsqlParameters[9] = new NpgsqlParameter("p_avtalenummer", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[9].Value = utvalgList.Avtalenummer;

                    npgsqlParameters[10] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[10].Direction = ParameterDirection.Output;

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result2 = dbhelper.ExecuteNonQuery("kspu_db.updateutvalglistforintegrationfromutvalg", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    result2 = Convert.ToInt32(npgsqlParameters[10].Value);
                    if (result2 == 0)
                        // Update affected no rows, list does not exist
                        _logger.LogWarning("Integrasjon gjennom Webservice metode 'Ordrestatus' forsøkte å oppdatere en liste som ikke eksisterer i KSPU. Utvalgsid: " + utvalgList.ListId);
                    // update modificated-by for utvalg. Need loop since utvalg
                    // mainUtvalg
                    if (utvalgList.MemberUtvalgs != null)
                    {
                        foreach (Puma.Shared.Utvalg u in utvalgList.MemberUtvalgs)
                            await _utvalgRepository.SaveUtvalgModifications(u, username, "UpdateUtvalgListForIntegration - MedlemsUtvalg: ");
                    }
                    // utvalg in childList
                    if (utvalgList.MemberLists != null)
                    {
                        foreach (UtvalgList lChild in utvalgList.MemberLists)
                        {
                            if (lChild.MemberUtvalgs != null)
                            {
                                foreach (Puma.Shared.Utvalg uChild in lChild.MemberUtvalgs)
                                    await _utvalgRepository.SaveUtvalgModifications(uChild, username, "UpdateUtvalgListForIntegration - utvalg i underlister:");
                            }
                        }
                    }
                }

                _logger.LogInformation(string.Format("Number of row affected {0} ", result));

                _logger.LogDebug("Exiting from UpdateUtvalgListForIntegration");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateUtvalgListForIntegration: " + exception.Message);
            }

        }


        /// <summary>
        ///      Oppdaterer AllowDouble på lista og alle underlister og alle tilknyttede kampanjer
        ///      </summary>
        ///      <param name="list"></param>
        ///      <remarks></remarks>
        public async Task UpdateAllowDouble(long listId, bool allowDouble)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for UpdateAllowDouble");
            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_allowdouble", NpgsqlTypes.NpgsqlDbType.Smallint);
            npgsqlParameters[0].Value = allowDouble == true ? 1 : 0;

            npgsqlParameters[1] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[1].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateallowdouble", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateAllowDouble: " + exception.Message);
                throw;
            }
            if (result == 0)
                _logger.LogWarning("Oppdatering av AllowDouble feilet for Utvalglistid: " + listId);

            _logger.LogDebug("Exiting from UpdateWriteprotectUtvalg");

        }


        /// <summary>
        /// Updates the is basis.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="isBasis">if set to <c>true</c> [is basis].</param>
        /// <param name="basedOn">The based on.</param>
        /// <exception cref="System.Exception">Lista er basert på en annen liste og kan ikke være basisliste.</exception>
        public async Task UpdateIsBasis(long listId, bool isBasis, int basedOn)
        {
            await Task.Run(() => { });
            if (basedOn > 0)
                throw new Exception("Lista er basert på en annen liste og kan ikke være basisliste.");
            try
            {
                //Update Lister
                _logger.LogDebug("Preparing the data for GetUtvalgListReceivers");
                int result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

                npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = listId;

                npgsqlParameters[1] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Smallint);
                npgsqlParameters[1].Value = isBasis == true ? 1 : 0;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateisbasis", CommandType.StoredProcedure, npgsqlParameters);
                }
                if (result == 0)
                    _logger.LogWarning("Oppdatering av IsBasis feilet for Utvalglistid: " + listId);

                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[2];

                npgsqlParameters1[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters1[0].Value = listId;

                npgsqlParameters1[1] = new NpgsqlParameter("p_isbasis", NpgsqlTypes.NpgsqlDbType.Smallint);
                npgsqlParameters1[1].Value = isBasis == true ? 1 : 0;
                //Update Utvalg
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateisbasisfromutvalg", CommandType.StoredProcedure, npgsqlParameters1);
                }
                if (result == 0)
                    _logger.LogWarning("Oppdatering av IsBasis på utvalg feilet for Utvalglistid: " + listId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in UpdateIsBasis: " + exception.Message);
            }
        }

        public async Task<UtvalgsListCollection> GetUtvalgListSimpleByKundeNr(string KundeNummer)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgListSimpleByKundeNr");
            DataTable utvData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();

            npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            //npgsqlParameters[0].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            npgsqlParameters[0].Value = KundeNummer;
            //npgsqlParameters[1] = new NpgsqlParameter("p_searchmethod", NpgsqlTypes.NpgsqlDbType.Integer);
            ////npgsqlParameters[1].Value = utils.CreateSearchString(utvalgNavn, searchMethod);
            //npgsqlParameters[1].Value = Convert.ToInt32(searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistsimplebykundenr", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListSimpleByKundeNr: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvData.Rows)
                utvLColl.Add(CreateListFromRow(row));
            await AddSistOppdatertToLists(utvLColl);
            return utvLColl;
        }

        /// <summary>
        ///      Returns a list of all campaigns based on a basislist's parent list.
        ///      </summary>
        ///      <param name="listId"></param>
        ///      <returns></returns>
        ///      <remarks></remarks>

        public async Task<List<CampaignDescription>> GetUtvalgListParentCampaigns(int listId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListParentCampaigns");
            DataTable listData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<CampaignDescription> cdColl = new List<CampaignDescription>();
            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);

            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    listData = dbhelper.FillDataTable("kspu_db.getutvalglistparentcampaigns", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListParentCampaigns: " + exception.Message);
                throw;
            }
            foreach (DataRow row in listData.Rows)
                cdColl.Add(GetCampaignDescriptionFromListDataRow(row, false));
            _logger.LogInformation("Number of row returned: ", cdColl.Count);
            _logger.LogDebug("Exiting from GetUtvalgListParentCampaigns");

            return cdColl;
        }
        ///// <summary>
        /////      Checks if there are campaigns based on basisutvalgs or basislists that are part of a top level basislist. Used to verify that the top level basislist can be deleted.
        /////      </summary>
        /////      <param name="listId"></param>
        /////      <returns></returns>
        /////      <remarks></remarks>
        //public  bool IsBasisListDeletable(int listId)
        //{
        //    // TODO. Currently verification is done in the GUI, but should ideally be done here by querying the database.
        //    // Dim cmd As New OracleCommand(" SELECT COUNT(*) FROM KSPU_DB.Utvalglist WHERE ... ")
        //    return true;
        //}

        /// <summary>
        ///     ''' Returns a list of all campaigns and disconnected campaigns based on a basislist.
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        public async Task<List<CampaignDescription>> GetUtvalgListCampaigns(int listId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListCampaigns");
            DataTable listData;
            DataTable utvData2;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            List<CampaignDescription> cdColl = new List<CampaignDescription>();
            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    listData = dbhelper.FillDataTable("kspu_db.getutvalglistcampaignsbybasedon", CommandType.StoredProcedure, npgsqlParameters);
                }
                foreach (DataRow row in listData.Rows)
                    cdColl.Add(GetCampaignDescriptionFromListDataRow(row, false));

                NpgsqlParameter[] npgsqlParameters1 = new NpgsqlParameter[1];
                npgsqlParameters1[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
                npgsqlParameters1[0].Value = listId;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData2 = dbhelper.FillDataTable("kspu_db.getutvalglistcampaignsbywasbasedon", CommandType.StoredProcedure, npgsqlParameters1);
                }
                foreach (DataRow row in utvData2.Rows)
                    cdColl.Add(GetCampaignDescriptionFromListDataRow(row, true));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListCampaigns: " + exception.Message);
            }
            _logger.LogInformation("Number of row returned: ", cdColl.Count);
            _logger.LogDebug("Exiting from GetUtvalgListCampaigns");
            return cdColl;
        }

        public async Task<UtvalgsListCollection> SearchUtvalgListWithoutReferences(string Utvalglistname, SearchMethod searchMethod, string customerNumber, bool canHaveEmptyCustomerNumber)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListWithoutReferences");
            Utils utils = new Utils();
            DataTable utvLData;
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[2];

            npgsqlParameters[0] = new NpgsqlParameter("p_Kundenummer", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[0].Value = customerNumber;

            npgsqlParameters[1] = new NpgsqlParameter("p_Utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[1].Value = utils.CreateSearchString(Utvalglistname, searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListWithoutReferences: " + exception.Message);
                throw;
            }
            foreach (DataRow row in utvLData.Rows)
            {
                UtvalgList list = CreateListFromRow(row);
                utvLColl.Add(list);
            }
            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListWithoutReferences");
            return utvLColl;
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med navn.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister eller evt parentlist
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste.
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="Utvalglistname"></param>
        ///     ''' <param name="searchMethod"></param>
        ///     ''' <returns>UtvalgsListCollection</returns>
        ///     ''' <remarks></remarks>
        public async Task<UtvalgsListCollection> SearchUtvalgListWithChildren(string Utvalglistname, SearchMethod searchMethod)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for SearchUtvalgListWithChildren");
            Utils utils = new Utils();
            DataTable utvLData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[1] = new NpgsqlParameter("p_Utvalglistname", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            npgsqlParameters[1].Value = utils.CreateSearchString(Utvalglistname, searchMethod);
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvLData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SearchUtvalgListWithChildren: " + exception.Message);
                throw;
            }

            UtvalgsListCollection utvLColl = GetAllListContent(utvLData, false, true);
            _logger.LogInformation("Number of row returned: ", utvLColl.Count);
            _logger.LogDebug("Exiting from SearchUtvalgListWithChildren");
            return utvLColl;
        }


        public async Task<UtvalgList> GetUtvalgListWithAllReferences(int UtvalglistId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListWithAllReferences");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            Utils utils = new Utils();
            DataTable utvLData;
            // swap if list is basedOn
            UtvalgList shallowList = await GetUtvalgListNoChild(UtvalglistId);
            if (shallowList.BasedOn > 0)
            {
                UtvalgList basisList = await GetUtvalgListWithAllReferences(shallowList.BasedOn);
                return AddBasedOnValuesToList(basisList, shallowList);
            }

            var parentListId = await GetParentUtvalgListId(UtvalglistId);
            if (parentListId > 0)
            {
                Puma.Shared.UtvalgList result = await GetUtvalgListWithAllReferences(parentListId);
                return result.GetUtvalgListDescendant(UtvalglistId);
            }

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer, 50);
            npgsqlParameters[0].Value = UtvalglistId;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                utvLData = dbhelper.FillDataTable("kspu_db.getutvalglist", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE UtvalglistId = :UtvalglistId ");
            //AddParameterInteger(cmd, "UtvalglistId", UtvalglistId);
            //DataTable utvLData = GetDataTable(cmd);
            if (utvLData.Rows.Count > 1)
                throw new Exception("found more than one utvalgList with same utvalglistid!");
            if (utvLData.Rows.Count == 0)
                return null/* TODO Change to default(_) if this is not a reference type */;

            DataRow row = utvLData.Rows[0];

            UtvalgList list = CreateListFromRow(row);

            // tilhøtrende utvalg
            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(list.ListId, true).Result;
            foreach (Puma.Shared.Utvalg u in uc)
            {   //u.List = list;
                u.ListId = Convert.ToString(list.ListId);
                u.ListName = list.Name;
                list.MemberUtvalgs.Add(u);   //Added commented line
            }
            if (!Convert.IsDBNull(row["r_parentutvalglistid"]) && Convert.ToInt32(row["r_parentutvalglistid"]) > 0)
            {
                if (list.BasedOn <= 0)
                {
                    list.ParentList = await GetUtvalgList(utils.GetIntFromRow(row, "r_parentutvalglistid"));
                    if (!list.ParentList.MemberLists.Contains(list))
                        list.ParentList.MemberLists.Add(list);
                }
            }
            else
            {
                DataTable utvData;
                NpgsqlParameter[] npgsqlParam = new NpgsqlParameter[1];
                // Children lister? - Kun dersom det ikke finnes en parent liste.
                npgsqlParam[0] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParam[0].Value = list.ListId;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistwithallreferences", CommandType.StoredProcedure, npgsqlParam);
                }
                //cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :ParentUtvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
                //AddParameterInteger(cmd, "ParentUtvalgListId", list.ListId);

                int i = 0;
                foreach (DataRow r in utvData.Rows)
                {
                    UtvalgList childList = await GetUtvalgList(Convert.ToInt32(r["r_utvalglistid"]), false);
                    //list.MemberLists.Add(childList); //Addedd commented line
                    childList.ParentList = list;
                    // Children utvalg
                    uc = _utvalgRepository.SearchUtvalgByUtvalListId(Convert.ToInt32(r["r_utvalglistid"]), true).Result;
                    foreach (Puma.Shared.Utvalg u in uc.ToArray())
                    {   //u.List = childList;
                        u.ListId = Convert.ToString(childList.ListId);
                        u.ListName = list.Name;
                        //u.Reoler = uc[i].Reoler.Add(u);
                        //list.MemberLists[i].MemberUtvalgs[i].Reoler.Add(u);
                        list.MemberLists[i].MemberUtvalgs.Add(u); //Addedd commented line
                    }
                    i += 1;
                }
            }

            UtvalgsListCollection dummyColl = new UtvalgsListCollection();
            dummyColl.Add(list);
            await AddSistOppdatertToLists(dummyColl);

            _logger.LogDebug("Exiting from GetUtvalgListWithAllReferences");
            return list;
        }

        /// <summary>
        ///     '''  Søker etter utvalgslister med id.
        ///     '''  Metoden returnerer:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister
        ///     '''  - alle utvalg som tilhører childlisten
        ///     ''' 
        ///     '''  En liste kan inneholde en liste. Max 2 nivåer med lister. Liste i liste. 
        ///     '''  Dvs enten har lista ingen parent eller children, eller den har enten en parent eller children..
        ///     ''' </summary>
        ///     ''' <param name="listId"></param>
        ///     ''' <param name="getParentListMemberUtvalg"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        public async Task<UtvalgList> GetUtvalgListWithChildren(int listId, bool getParentListMemberUtvalg = false)
        {

            _logger.LogDebug("Preparing the data for GetUtvalgListWithChildren");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistwithchildren", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE UtvalgListId = :utvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
            //AddParameterInteger(cmd, "utvalgListId", listId);

            if (utvLData.Rows.Count != 1)
                throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            DataRow row = utvLData.Rows[0];
            UtvalgList list = CreateListFromRow(row);

            // TODO: Refactor code below to use GetAllListContent(..)

            // swap if list is basedOn
            if (list.BasedOn > 0)
                listId = list.BasedOn;

            // tilhørende utvalg
            _logger.LogDebug("Calling the SearchUtvalgByUtvalListId from utvalgRepository");
            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(listId).Result;
            foreach (Puma.Shared.Utvalg u in uc)
            {
                //if (u.List != list)
                if (Convert.ToInt32(u.ListId) != list.ListId)
                {

                    //if (u.List != list)
                    //    u.List = list;
                    u.ListId = list.ListId.ToString();
                }
            }

            if (!Convert.IsDBNull(row["r_parentutvalglistid"]) && Convert.ToInt64(row["r_parentutvalglistid"]) > 0)
            {
                if (list.BasedOn <= 0)
                {
                    list.ParentList = await GetUtvalgList(Convert.ToInt64(row["r_parentutvalglistid"]), false, getParentListMemberUtvalg);
                    if (list.ParentList != null && list.ParentList.MemberLists != null && !list.ParentList.MemberLists.Contains(list))
                        list.ParentList.MemberLists.Add(list);
                }
            }
            else
            {
                // Children lister? - Kun dersom det ikke finnes en parent liste.
                DataTable utvData;
                npgsqlParameters[0] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = listId;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistwithchildrenbyparentlist", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :ParentUtvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
                //AddParameterInteger(cmd, "ParentUtvalgListId", listId);

                foreach (DataRow r in utvData.Rows)
                {
                    _logger.LogDebug("Calling the GetUtvalgList");
                    UtvalgList childList = await GetUtvalgList(utils.GetIntFromRow(r, "r_utvalglistid"), false,true);
                    
                    if (childList.BasedOn <= 0)
                    {
                        list.MemberLists.Add(childList);
                        childList.ParentList = list;
                        //// Children utvalg
                        //_logger.LogDebug("Calling the SearchUtvalgByUtvalListId");
                        //uc = _utvalgRepository.SearchUtvalgByUtvalListId(utils.GetIntFromRow(r, "r_utvalglistid")).Result;
                        //if (uc?.Any() == true)
                        //{
                            
                        //    foreach (Puma.Shared.Utvalg u in uc)
                        //        //  list.MemberLists.Item(i).MemberUtvalgs.Add(u);
                        //        i += 1;
                        //}
                    }
                }
            }

            if (list.BasedOn > 0)
            {
                _logger.LogDebug("Calling the GetUtvalgListNoChild");
                UtvalgList checkIfBasedOnList = await GetUtvalgListNoChild(list.BasedOn);
                list = AddBasedOnValuesToList(list, checkIfBasedOnList);
            }
            _logger.LogDebug("Exiting from GetUtvalgListWithChildren");
            return list;
        }

        /// <summary>
        /// GetUtvalgListWithChildrenData
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="getParentListMemberUtvalg"></param>
        /// <returns></returns>
        public async Task<UtvalgList> GetUtvalgListWithChildrenData(int listId, bool getParentListMemberUtvalg = false)
        {

            _logger.LogDebug("Preparing the data for GetUtvalgListWithChildren");
            DataTable utvLData;
            Utils utils = new Utils();
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;
            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                utvLData = dbhelper.FillDataTable("kspu_db.getutvalglistwithchildren", CommandType.StoredProcedure, npgsqlParameters);
            }
            //OracleCommand cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE UtvalgListId = :utvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
            //AddParameterInteger(cmd, "utvalgListId", listId);

            if (utvLData.Rows.Count != 1)
                throw new Exception("Fant ikke unikt utvalgsliste med utvalgslisteid " + listId + " i databasen.");
            DataRow row = utvLData.Rows[0];
            UtvalgList list = CreateListFromRow(row);

            // TODO: Refactor code below to use GetAllListContent(..)

            // swap if list is basedOn
            if (list.BasedOn > 0)
                listId = list.BasedOn;

            // tilhørende utvalg
            _logger.LogDebug("Calling the SearchUtvalgByUtvalListId from utvalgRepository");
            UtvalgCollection uc = _utvalgRepository.SearchUtvalgByUtvalListId(listId).Result;
            foreach (Puma.Shared.Utvalg u in uc)
            {
                list.MemberUtvalgs.Add(u);
                //if (u.List != list)
                if (Convert.ToInt32(u.ListId) != list.ListId)
                {

                    //if (u.List != list)
                    //    u.List = list;
                    u.ListId = list.ListId.ToString();
                }
            }

            if (!Convert.IsDBNull(row["r_parentutvalglistid"]) && Convert.ToInt64(row["r_parentutvalglistid"]) > 0)
            {
                if (list.BasedOn <= 0)
                {
                    list.ParentList = await GetUtvalgList(Convert.ToInt64(row["r_parentutvalglistid"]), false, getParentListMemberUtvalg);
                    if (list.ParentList != null && list.ParentList.MemberLists != null && !list.ParentList.MemberLists.Contains(list))
                        list.ParentList.MemberLists.Add(list);
                }
            }
            else
            {
                // Children lister? - Kun dersom det ikke finnes en parent liste.
                DataTable utvData;
                npgsqlParameters[0] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = listId;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistwithchildrenbyparentlist", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" SELECT * FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :ParentUtvalgListId ORDER BY UPPER(UTVALGLISTNAME)");
                //AddParameterInteger(cmd, "ParentUtvalgListId", listId);

                foreach (DataRow r in utvData.Rows)
                {
                    _logger.LogDebug("Calling the GetUtvalgList");
                    UtvalgList childList = await GetUtvalgList(utils.GetIntFromRow(r, "r_utvalglistid"), false, true);

                    if (childList.BasedOn <= 0)
                    {

                        // Children utvalg
                        var ucList = _utvalgRepository.SearchUtvalgByUtvalListId(utils.GetIntFromRow(r, "r_utvalglistid")).Result;
                        foreach (Puma.Shared.Utvalg u in ucList)
                            childList.MemberUtvalgs.Add(u);
                        list.MemberLists.Add(childList);
                        childList.ParentList = list;

                        //list.MemberLists.Add(childList);
                        //childList.ParentList = list;
                        //// Children utvalg
                        //_logger.LogDebug("Calling the SearchUtvalgByUtvalListId");
                        //uc = _utvalgRepository.SearchUtvalgByUtvalListId(utils.GetIntFromRow(r, "r_utvalglistid")).Result;
                        //if (uc?.Any() == true)
                        //{

                        //    foreach (Puma.Shared.Utvalg u in uc)
                        //        //  list.MemberLists.Item(i).MemberUtvalgs.Add(u);
                        //        i += 1;
                        //}
                    }
                }
            }

            if (list.BasedOn > 0)
            {
                _logger.LogDebug("Calling the GetUtvalgListNoChild");
                UtvalgList checkIfBasedOnList = await GetUtvalgListNoChild(list.BasedOn);
                list = AddBasedOnValuesToList(list, checkIfBasedOnList);
            }
            _logger.LogDebug("Exiting from GetUtvalgListWithChildren");
            return list;
        }


        /// <summary>
        /// Deletes the campaign list.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="BasedOn">The based on.</param>
        /// <exception cref="System.Exception">Kan ikke slette liste, da listen ikke er en kampanje.</exception>
        public async Task DeleteCampaignList(int listId, int BasedOn)
        {
            await Task.Run(() => { });
            if (BasedOn == 0)
                throw new Exception("Kan ikke slette liste, da listen ikke er en kampanje.");
            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;
            // Delete Modification. Slett for hovedliste
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteCampaignList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteCampaignList: " + exception.Message);
                throw;
            }
            // Delete Liste
            //using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            //{
            //    result = dbhelper.ExecuteNonQuery("kspu_db.DeleteCampaignList", CommandType.StoredProcedure, npgsqlParameters);
            //}
            _logger.LogInformation("Number of row returned: ", result);
            _logger.LogDebug("Exiting from DeleteCampaignList");

        }

        /// <summary>
        ///     ''' Metoden sletter utvalgslisten. Man kan velge å slette kun listen eller listen med alle underliggende elementer.
        ///     ''' Dersom sletting med underliggende elementer er metoden rekursiv.
        ///     ''' Sletter man kun listen settes alle nøkler til 'null' i relaterte utvalg og lister.
        ///     ''' Hvis parentlista blir tom som et resultat av slettingen, slettes også denne (med mindre den har kampanjer), og metoden returnerer true. Ellers returneres false.
        ///     ''' </summary>
        ///     ''' <param name="UtvalgListId"></param>
        ///     ''' <param name="withChildren"></param>
        ///     ''' <param name="userName"></param>
        ///     ''' <remarks></remarks>

        public async Task<bool> DeleteUtvalgList(int UtvalgListId, bool withChildren, string userName)
        {
            await Task.Run(() => { });
            // TODO: Skriv om til å bruke transaksjoner
            int result;
            UtvalgList ul = await GetUtvalgListWithAllReferences(UtvalgListId);
            if (ul.BasedOn > 0)
                throw new Exception("Kan ikke slette kampanjeliste i metoden DeleteUtvalgList, bruk metoden DeleteCampaignList.");
            try
            {
                // Finner evt parentlistid
                int ParentUtvalgListId = await GetParentUtvalgListId(UtvalgListId);

                if (withChildren)
                {
                    // Vi sletter alle utvalg
                    if (ul.MemberUtvalgs.Count > 0)
                    {
                        foreach (Puma.Shared.Utvalg utvalg in ul.MemberUtvalgs)
                            await _utvalgRepository.DeleteUtvalg(utvalg.UtvalgId, userName);
                    }
                    // Vi sletter alle child-lister med deres utvalg, dersom det finnes lister i listen
                    if (ul.MemberLists.Count > 0)
                    {
                        foreach (UtvalgList utvList in ul.MemberLists)
                            await DeleteUtvalgList(utvList.ListId, true, userName);
                    }

                    // Delete Modification. Slett for lista
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[0].Value = UtvalgListId;

                    // Delete Modification. Slett for hovedliste
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgmodificationlist", CommandType.StoredProcedure, npgsqlParameters);
                    }

                    // Delete Liste
                    NpgsqlParameter[] npgsqlParameterlist = new NpgsqlParameter[1];
                    npgsqlParameterlist[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameterlist[0].Value = UtvalgListId;

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalglist", CommandType.StoredProcedure, npgsqlParameterlist);
                    }

                }
                else
                {
                    // Vi fjerner listeid'en i alle utvalg
                    if (ul.MemberUtvalgs.Count > 0)
                    {
                        foreach (Puma.Shared.Utvalg utvalg in ul.MemberUtvalgs.ToArray())
                        {
                            //utvalg.List = null;
                            utvalg.ListId = null;
                            // utvalgController.SaveUtvalg(utvalg, userName);

                        }
                    }
                    // Vi setter alle parentlistid i alle child-lister til ingenting
                    if (ul.MemberLists.Count > 0)
                    {
                        foreach (UtvalgList utvList in ul.MemberLists.ToArray())
                        {
                            utvList.ParentList = null;
                            await SaveUtvalgList(utvList);
                        }
                    }

                    // Delete Modification. Slett for lista
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                    npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameters[0].Value = UtvalgListId;

                    // Delete Modification. Slett for hovedliste
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalgmodificationlist", CommandType.StoredProcedure, npgsqlParameters);
                    }

                    // Delete Liste
                    NpgsqlParameter[] npgsqlParameterlist = new NpgsqlParameter[1];
                    npgsqlParameterlist[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
                    npgsqlParameterlist[0].Value = UtvalgListId;

                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        result = dbhelper.ExecuteNonQuery("kspu_db.deleteutvalglist", CommandType.StoredProcedure, npgsqlParameterlist);
                    }
                }

                // oppdater evt listeantall
                if (ParentUtvalgListId > 0)
                {
                    await UpdateAntallInList(ParentUtvalgListId);

                    try
                    {
                        await SaveUtvalgListModifications(ParentUtvalgListId, "DeleteUtvalgList - " + UtvalgListId, userName);
                        // CommitTransaction(trans);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                        // RollbackTransaction(trans);
                        throw;
                    }
                    // Sletter parent listen hvis den er tom
                    return await CheckAndDeleteUtvalgListIfEmpty(ParentUtvalgListId, userName);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteUtvalgList: " + exception.Message);
            }
            _logger.LogDebug("Exiting from DeleteUtvalgList");
            return false;
        }


        /// <summary>
        ///     ''' Metoden kalles etter at man sletter et utvalg.
        ///     ''' Metoden sletter listen utvalget lå i, dersom listen ble tom pga. sletting.
        ///     ''' Inneholder listen andre lister slettes den ikke, men den slettes selv om den ligger som barn av en annen liste.
        ///     ''' </summary>
        ///     ''' <param name="UtvalgListId"></param>
        ///     ''' <param name="userName"></param>
        ///     ''' <remarks></remarks>
        public async Task<bool> CheckAndDeleteUtvalgListIfEmpty(int UtvalgListId, string userName)
        {
            try
            {
                _logger.LogDebug("Preparing the data for CheckAndDeleteUtvalgListIfEmpty");
                DataTable utvData;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[1] = new NpgsqlParameter("p_UtvalgListId", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[1].Value = UtvalgListId;
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleCommand cmd;
                //cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.UtvalgList WHERE ParentUtvalgListId = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //DataTable utvData = GetDataTable(cmd);
                DataRow row;
                row = utvData.Rows[0];
                int count1 = (int)row[0];
                if (count1 > 0)
                    return false;

                // En tom basis listen skal ikke kunne slettes om den har kampanjer knyttet til seg. 
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }

                //cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.UtvalgList WHERE basedon = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //utvData = GetDataTable(cmd);
                row = utvData.Rows[0];
                int countK = (int)row[0];
                if (countK > 0)
                    return false;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.searchutvalgsimple", CommandType.StoredProcedure, npgsqlParameters);
                }

                //cmd = new OracleCommand(" SELECT count(*) FROM KSPU_DB.Utvalg WHERE UtvalgListId = :UtvalgListId ");
                //AddParameterInteger(cmd, "UtvalgListId", UtvalgListId);
                //utvData = GetDataTable(cmd);
                row = utvData.Rows[0];
                int count = (int)row[0];
                if (count == 0)
                {
                    await DeleteUtvalgList(UtvalgListId, false, userName);
                    return true;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in CheckAndDeleteUtvalgListIfEmpty: " + exception.Message);
            }
            _logger.LogDebug("Exiting from CheckAndDeleteUtvalgListIfEmpty");
            return false;
        }

        /// <summary>
        ///     ''' Metoden sletter alle utvalgslister som er tomme og deretter de gjenstående tomme liste i liste forekomstene. 
        ///     ''' </summary>
        ///     ''' <remarks></remarks>

        public async Task DeleteEmptyUtvalgLists()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for DeleteEmptyUtvalgLists");
            try
            {
                int result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleteemptyutvalglistsfromutvalglist", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand(" DELETE FROM KSPU_DB.UtvalgList WHERE BasedOn=0 AND UtvalgListId IN (SELECT utvalglistid FROM KSPU_DB.utvalglist MINUS " + " SELECT parentutvalglistid FROM KSPU_DB.utvalglist WHERE parentutvalglistid IS NOT NULL MINUS SELECT utvalglistid FROM KSPU_DB.utvalg)");
                // Slett liste i liste.
                //while (result > 0)
                //dbhelper.exc("commit");


                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.deleteemptyutvalglistsfromutvalglistmodification", CommandType.StoredProcedure, npgsqlParameters);
                }
                //cmd = new OracleCommand("DELETE FROM KSPU_DB.UTVALGLISTMODIFICATION WHERE UTVALGLISTID IN (SELECT UTVALGLISTID FROM KSPU_DB.UTVALGLISTMODIFICATION " + "MINUS SELECT UTVALGLISTID FROM KSPU_DB.UTVALGLIST)");

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteEmptyUtvalgLists: " + exception.Message);
            }
            _logger.LogDebug("Exiting from DeleteEmptyUtvalgLists");
        }

        //[HttpPost("UpdateLogo", Name = nameof(UpdateLogo))]
        public async Task UpdateListLogo(long utvalgListId, string username, string logo)
        {
            try
            {
                int result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[0].Value = utvalgListId;

                npgsqlParameters[1] = new NpgsqlParameter("p_username", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[1].Value = username;


                npgsqlParameters[2] = new NpgsqlParameter("p_logo", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[2].Value = logo;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.UpdateLogo", CommandType.StoredProcedure, npgsqlParameters);
                }

                Puma.Shared.UtvalgList list = new Shared.UtvalgList()
                {
                    ListId = Convert.ToInt32(utvalgListId)
                };

                await SaveUtvalgListModifications(list.ListId, "Oppdatert logo til " + list.Logo, username);


            }
            catch
            {
                throw;
            }
        }



        public async Task<int> GetNrOfDoubleCoverageReolsForList(int listId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetNrOfDoubleCoverageReolsForList");
            //UtvalgList checkIfBasedOnList = await GetUtvalgListNoChild(listId);
            //if (checkIfBasedOnList.BasedOn > 0)
            //    listId = checkIfBasedOnList.BasedOn;

            //System.Text.StringBuilder query = new System.Text.StringBuilder();
            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<int>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetNrOfDoubleCoverageReolsForList: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from GetNrOfDoubleCoverageReolsForList");
            return System.Convert.ToInt32(result);

        }


        public async Task<UtvalgsListCollection> FindUtvalgListsWhithCustomerNumberRestrictions(string listName, string customerNumber)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for FindUtvalgListsWhithCustomerNumberRestrictions");
            DataTable dtUtvalgList;
            if (customerNumber == null)
                throw new Exception("customerNumber can not be null for FindUtvalgListsWhithCustomerNumberRestrictions");
            UtvalgsListCollection result = new UtvalgsListCollection();

            //System.Text.StringBuilder query = new System.Text.StringBuilder();
            //query.AppendLine("select distinct uvl1.*");
            //query.AppendLine("from KSPU_DB.utvalglist uvl1");
            //query.AppendLine("where (uvl1.KUNDENUMMER is null or uvl1.KUNDENUMMER = :Kundenummer)");
            //query.AppendLine(@"and UPPER(uvl1.utvalglistname) like :ListName escape'\'");
            //query.AppendLine("and not uvl1.UtvalgListId in");
            //query.AppendLine("  (");
            //query.AppendLine("  select utv2.utvalglistId");
            //query.AppendLine("	from KSPU_DB.utvalg utv2");
            //query.AppendLine("	where utv2.UTVALGLISTID is not null");
            //query.AppendLine("	and utv2.kundenummer is not null");
            //query.AppendLine("	and utv2.kundenummer <> :Kundenummer");
            //query.AppendLine("  )");

            //OracleCommand cmd = new OracleCommand(query.ToString());
            //AddParameterString(cmd, "ListName", DAUtvalg.CreateSearchString(listName, SearchMethod.ContainsIgnoreCase), 50);
            //AddParameterString(cmd, "Kundenummer", customerNumber, 30);
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            npgsqlParameters[0] = new NpgsqlParameter("p_listname", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = listName;

            npgsqlParameters[0] = new NpgsqlParameter("p_kundenummer", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = customerNumber;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dtUtvalgList = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FindUtvalgListsWhithCustomerNumberRestrictions: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dtUtvalgList.Rows)
                result.Add(CreateListFromRow(row));
            _logger.LogDebug("Exiting from FindUtvalgListsWhithCustomerNumberRestrictions");
            return result;
        }

        // private List<int> _utvalgListIdsWithIllegalCustomerComposition = null;
        //[HttpGet("UtvalgListIdsWithIllegalCustomerComposition", Name = nameof(UtvalgListIdsWithIllegalCustomerComposition))]
        public List<int> UtvalgListIdsWithIllegalCustomerComposition
        {
            get
            {
                if (_utvalgListIdsWithIllegalCustomerComposition != null)
                    return _utvalgListIdsWithIllegalCustomerComposition;

                List<int> result = new List<int>();

                foreach (int id in GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer().Result)
                {
                    if (!result.Contains(id))
                        result.Add(id);
                }

                foreach (int id in GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg().Result)
                {
                    if (!result.Contains(id))
                        result.Add(id);
                }

                foreach (int id in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList().Result)
                {
                    if (!result.Contains(id))
                        result.Add(id);
                }

                foreach (int id in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg().Result)
                {
                    if (!result.Contains(id))
                        result.Add(id);
                }

                _utvalgListIdsWithIllegalCustomerComposition = result;

                return _utvalgListIdsWithIllegalCustomerComposition;
            }
        }


        public async Task<List<int>> GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg");
            List<int> result = new List<int>();
            DataTable dt;
            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select distinct uvl1.utvalglistid");
            //query.AppendLine("from KSPU_DB.utvalglist uvl1");
            //query.AppendLine("inner join KSPU_DB.utvalglist uvl2");
            //query.AppendLine("on uvl1.PARENTUTVALGLISTID = uvl2.UTVALGLISTID");
            //query.AppendLine("inner join KSPU_DB.utvalg utv1");
            //query.AppendLine("on utv1.UtvalgListId = uvl1.utvalglistid");
            //query.AppendLine("inner join KSPU_DB.utvalg utv2");
            //query.AppendLine("on utv2.UtvalgListId = uvl2.utvalglistid");
            //query.AppendLine("where utv1.Kundenummer is not null");
            //query.AppendLine("and utv2.Kundenummer is not null");
            //query.AppendLine("and utv1.Kundenummer <> utv2.Kundenummer");
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg: " + exception.Message);
                throw;
            }

            foreach (DataRow row in dt.Rows)
                result.Add(System.Convert.ToInt32(row["utvalglistid"]));
            _logger.LogInformation("Number of rows returned: ", dt.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanDirectUtvalg");
            return result;
        }


        public async Task<List<int>> GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList");
            List<int> result = new List<int>();
            DataTable dt;
            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select distinct uvl1.utvalglistid");
            //query.AppendLine("from KSPU_DB.utvalglist uvl1");
            //query.AppendLine("inner join KSPU_DB.utvalglist uvl2");
            //query.AppendLine("on uvl1.PARENTUTVALGLISTID = uvl2.UTVALGLISTID");
            //query.AppendLine("inner join KSPU_DB.utvalg utv1");
            //query.AppendLine("on utv1.UtvalgListId = uvl1.utvalglistid");
            //query.AppendLine("where utv1.Kundenummer is not null");
            //query.AppendLine("and uvl2.Kundenummer is not null");
            //query.AppendLine("and utv1.Kundenummer <> uvl2.Kundenummer");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dt.Rows)
                result.Add(System.Convert.ToInt32(row["utvalglistid"]));
            _logger.LogInformation("Number of rows returned: ", dt.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList");
            return result;
        }


        public async Task<List<int>> GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer");
            List<int> result = new List<int>();

            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select distinct utv1.UtvalgListId");
            //query.AppendLine("from KSPU_DB.utvalg utv1, KSPU_DB.utvalg utv2");
            //query.AppendLine("where utv1.UtvalgListId is not null");
            //query.AppendLine("and utv1.UtvalgListId = utv2.UtvalgListId");
            //query.AppendLine("and utv1.UtvalgId <> utv2.UtvalgId");
            //query.AppendLine("and utv1.Kundenummer is not null");
            //query.AppendLine("and utv2.Kundenummer is not null");
            //query.AppendLine("and (utv1.Kundenummer <> utv2.Kundenummer)");


            DataTable dt;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dt = dbhelper.FillDataTable("kspu_db.DeleteUtvalgList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dt.Rows)
                result.Add(System.Convert.ToInt32(row["UtvalgListId"]));
            _logger.LogInformation("Number of rows returned: ", dt.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListIdsWhereMemberUtvalgHasDifferentKundenummer");
            return result;
        }


        public async Task<List<int>> GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg()
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg");
            List<int> result = new List<int>();

            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select distinct uvl.UtvalgListid");
            //query.AppendLine("from KSPU_DB.UtvalgList uvl");
            //query.AppendLine("inner join KSPU_DB.Utvalg utv");
            //query.AppendLine("on uvl.UTVALGLISTID = utv.UTVALGlISTID");
            //query.AppendLine("where uvl.Kundenummer is not null");
            //query.AppendLine("and utv.Kundenummer is not null");
            //query.AppendLine("and uvl.Kundenummer <> utv.Kundenummer");


            DataTable dt;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dt = dbhelper.FillDataTable("kspu_db.getutvalglistidswhereutvalglisthasdifferentkundenummerthanmemberutvalg", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dt.Rows)
                result.Add(System.Convert.ToInt32(row["UtvalgListid"]));
            _logger.LogInformation("Number of rows returned: ", dt.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg");
            return result;
        }


        public async Task<bool> IsUtvalgConnectedToList(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsUtvalgConnectedToList");
            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select count(*)");
            //query.AppendLine("from KSPU_DB.utvalg");
            //query.AppendLine("where utvalgId = :UtvalgId");
            //query.AppendLine("and utvalglistId is not null");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgconnectedtolist", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsUtvalgConnectedToList: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from IsUtvalgConnectedToList");
            //OracleCommand cmdListCheck = new OracleCommand(query.ToString());
            //AddParameterInteger(cmdListCheck, "UtvalgId", utvalgId);

            //return System.Convert.ToInt32(ExecuteScalar(cmdListCheck)) == 1;
            return System.Convert.ToInt32(result) == 1;
        }


        public async Task<bool> IsUtvalgConnectedToParentList(int utvalgId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for IsUtvalgConnectedToParentList");
            if (!await IsUtvalgConnectedToList(utvalgId))
                return false;

            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select count(*)");
            //query.AppendLine("from KSPU_DB.utvalgList");
            //query.AppendLine("where utvalgListId = ");
            //query.AppendLine("(");
            //query.AppendLine("  select utvalgListId");
            //query.AppendLine("  from KSPU_DB.utvalg");
            //query.AppendLine("  where utvalgId = :UtvalgId");
            //query.AppendLine(")");
            //query.AppendLine("and parentUtvalgListId is not null");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.isutvalgconnectedtoparentlist", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in IsUtvalgConnectedToParentList: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from IsUtvalgConnectedToParentList");
            return System.Convert.ToInt32(result) == 1;
        }


        public async Task<UtvalgList> GetRootUtvalgListWithAllReferences(int utvalgId)
        {
            _logger.LogDebug("Preparing the data for GetRootUtvalgListWithAllReferences");
            if (await IsUtvalgConnectedToParentList(utvalgId))
                return GetConnectedParentListWithAllReferences(utvalgId);
            if (await IsUtvalgConnectedToList(utvalgId))
                return GetConnectedListWithAllReferences(utvalgId);

            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public async Task<List<long>> GetListsToRefreshDueToUpdateToBasisList(Puma.Shared.UtvalgList utvalgList)
        {
            _logger.LogDebug("Preparing the data for GetListsToRefreshDueToUpdateToBasisList");
            if (utvalgList.IsBasis && utvalgList.ListId > 0)
            {
                //Get List CampainData
                var listCampainData = await GetUtvalgListCampaigns(utvalgList.ListId);

                return listCampainData?.Where(k => k.OrdreType == Puma.Shared.PumaEnum.OrdreType.T).Select(x => x.ID).ToList();
            }
            return new List<long>();
        }

        public async Task<List<long>> GetListsToRefreshDueToUpdateToBasisListChild(UtvalgList utvalgList)
        {
            _logger.LogDebug("Preparing the data for GetListsToRefreshDueToUpdateToBasisListChild");
            if (utvalgList.IsBasis && utvalgList.ListId > 0)
            {
                var parentListCampainData = await GetUtvalgListParentCampaigns(utvalgList.ListId);
                return parentListCampainData?.Where(k => k.OrdreType == Puma.Shared.PumaEnum.OrdreType.T).Select(x => x.ID).ToList();


            }
            return new List<long>();
        }

        public async Task<bool> HasListDemSegUtvalgDescendant(int utvalgListId)
        {
            _logger.LogDebug("Preparing the data for HasListDemSegUtvalgDescendant");
            UtvalgList checkIfBasedOnList = await GetUtvalgListNoChild(utvalgListId);
            if (checkIfBasedOnList.BasedOn > 0)
                utvalgListId = checkIfBasedOnList.BasedOn;

            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select");
            //query.AppendLine("(select count(distinct u.utvalgId)");
            //query.AppendLine("from kspu_db.utvalg u");
            //query.AppendLine("inner join kspu_db.utvalgCriteria c");
            //query.AppendLine("on u.utvalgId = c.utvalgId");
            //query.AppendLine("where u.utvalgListId = :ListId");
            //query.AppendLine("and c.CriteriaType in (2, 12))");
            //query.AppendLine("+");
            //query.AppendLine("(select count(distinct u.utvalgId)");
            //query.AppendLine("from kspu_db.utvalg u");
            //query.AppendLine("inner join kspu_db.utvalgList ul");
            //query.AppendLine("on ul.UtvalgListId = u.UtvalgListId");
            //query.AppendLine("inner join kspu_db.utvalgCriteria c");
            //query.AppendLine("on u.utvalgId = c.utvalgId");
            //query.AppendLine("where ul.ParentUtvalgListId = :ListId");
            //query.AppendLine("and c.CriteriaType in (2, 12))");
            //query.AppendLine("from dual");
            object result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = utvalgListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<bool>("kspu_db.haslistdemsegutvalgdescendant", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in HasListDemSegUtvalgDescendant: " + exception.Message);
                throw;
            }
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from HasListDemSegUtvalgDescendant");
            return System.Convert.ToInt32((result)) > 0;
        }


        public DateTime FindLastModifiedDateForList(int utvalgListId)
        {
            _logger.LogDebug("Preparing the data for FindLastModifiedDateForList");
            // Supportsak # 621854 PUMA, utskrift av utvalgsliste viser ikke endring i forhandlerpåtrykk
            // Deactivated original code
            // Dim cmd As New OracleCommand("select modificationdate from KSPU_DB.utvalglistmodification where utvalglistid = :utvalgListId order by modificationdate desc")
            // AddParameterInteger(cmd, "utvalgListId", utvalgListId)
            // Dim obj As Object = ExecuteScalar(cmd)

            // If obj Is DBNull.Value Then Return DateTime.MinValue
            // Return obj

            // Supportsak # 621854 PUMA, utskrift av utvalgsliste viser ikke endring i forhandlerpåtrykk - må ha siste dato på listen og hele dens innhold

            // Find last date for all changes in this list including all its childlist
            object objL;
            object objU;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    objL = dbhelper.ExecuteScalar<DateTime>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
                }
                //OracleCommand cmdL = new OracleCommand("select * from KSPU_DB.utvalglistmodification where utvalglistid in( select utvalglistId from KSPU_DB.utvalglist where utvalglistid in ( select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :utvalgListId) or utvalglistid = :utvalgListId) order by modificationdate desc");
                //AddParameterInteger(cmdL, "utvalgListId", utvalgListId);


                // Find last date for all utvalg in this list and all its childlist
                //OracleCommand cmdU = new OracleCommand("select * from KSPU_DB.utvalgmodification where utvalgid in (select utvalgid from KSPU_DB.utvalg where utvalglistid in ( select utvalglistId from KSPU_DB.utvalglist where utvalglistid in ( select utvalglistid from KSPU_DB.utvalglist where parentutvalglistid = :utvalgListId) or utvalglistid = :utvalgListId)) order by modificationdate desc");
                //AddParameterInteger(cmdU, "utvalgListId", utvalgListId);
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    objU = dbhelper.ExecuteScalar<DateTime>("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in FindLastModifiedDateForList: " + exception.Message);
                throw;
            }


            DateTime sistListOppdatert = new DateTime(1, 1, 1);
            try
            {
                if (objL != DBNull.Value)
                {
                    if (sistListOppdatert.CompareTo(objL) > 0)
                        sistListOppdatert = (DateTime)objL;
                }

                if (objU != DBNull.Value)
                {
                    if (sistListOppdatert.CompareTo(objU) > 0)
                        sistListOppdatert = (DateTime)objU;
                }
            }
            catch
            {
                return sistListOppdatert;
            }
            _logger.LogDebug("Exiting from FindLastModifiedDateForList");
            return sistListOppdatert;
        }

        public async Task<List<long>> GetListsToRefreshDueToUpdate(Puma.Shared.UtvalgList utvalgList)
        {
            List<long> ids = new List<long>();
            ids.AddRange(await GetListsToRefreshDueToUpdateToBasisList(utvalgList));
            ids.AddRange(await GetListsToRefreshDueToUpdateToBasisListChild(utvalgList));
            return ids;
        }

        /// <summary>
        /// Return List of Utvalg ids based on list id
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public List<long> GetUtvalgIdsFromListId(long listId)
        {
            _logger.LogDebug("Inside into GetUtvalgIdsFromListId");
            List<long> utvalgids = new List<long>();

            DataTable dt;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Numeric);
            npgsqlParameters[0].Value = listId;

            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    dt = dbhelper.FillDataTable("kspu_db.getutvalgidsfromlistid", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg: " + exception.Message);
                throw;
            }
            foreach (DataRow row in dt.Rows)
                utvalgids.Add(System.Convert.ToInt64(row["r_utvalgid"]));
            _logger.LogInformation("Number of rows returned: ", dt.Rows.Count);
            _logger.LogDebug("Exiting from GetUtvalgListIdsWhereUtvalgListHasDifferentKundenummerThanMemberUtvalg");
            return utvalgids;




        }

        /// <summary>
        /// Delete Routes from utvalgs
        /// </summary>
        /// <param name="utvalgIds">Comma seperated utvalgs </param>
        /// <param name="routeIds">comma seperated reols</param>
        /// <returns></returns>
        public async Task DeleteRoutesOfUtvalgs(string utvalgIds,string routeIds)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for DeleteRoutesOfUtvalgs");
            try
            {
                string query = "delete from kspu_db.utvalgreol where utvalgid in (" + utvalgIds + ") and reolid in (" + routeIds + ")";

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                     dbhelper.ExecuteNonQuery(query, CommandType.Text, null);
                }
               

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteEmptyUtvalgLists: " + exception.Message);
            }
            _logger.LogDebug("Exiting from DeleteEmptyUtvalgLists");
        }

        /// <summary>
        /// Update Antall Information
        /// </summary>
        /// <param name="type">Pass "L" for list else pass "U"</param>
        /// <param name="Id">Id</param>
        /// <param name="antall">antall that needs to update</param>
        /// <returns></returns>
        public async Task UpdateAntallInformation(string type,long Id,Int64 antall)
        {
            await Task.Run(() => { });
            _logger.LogDebug("UpdateAntallInformation");
            try
            {
                int result;
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];

                npgsqlParameters[0] = new NpgsqlParameter("p_type", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[0].Value = type;

                npgsqlParameters[1] = new NpgsqlParameter("p_antal", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[1].Value = antall;

                npgsqlParameters[2] = new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Numeric);
                npgsqlParameters[2].Value = Id;

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.updateantalinformation", CommandType.StoredProcedure, npgsqlParameters);
                }

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in DeleteEmptyUtvalgLists: " + exception.Message);
            }
            _logger.LogDebug("Exiting from DeleteEmptyUtvalgLists");
        }

        #region Private Methods
        private List<int> _utvalgListIdsWithIllegalCustomerComposition = null;

        private UtvalgList GetConnectedListWithAllReferences(int utvalgId)
        {
            _logger.LogDebug("Preparing the data for GetConnectedListWithAllReferences");
            StringBuilder query = new StringBuilder();
            //query.AppendLine("select utvalglistId");
            //query.AppendLine("from KSPU_DB.utvalg");
            //query.AppendLine("where utvalgId = :UtvalgId");

            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetConnectedListWithAllReferences: " + exception.Message);
                throw;
            }
            _logger.LogDebug("Exiting from GetConnectedListWithAllReferences");
            return GetUtvalgListWithAllReferences(System.Convert.ToInt32(result)).Result;
        }

        private UtvalgList GetConnectedParentListWithAllReferences(int utvalgId)
        {
            _logger.LogDebug("Preparing the data for GetConnectedParentListWithAllReferences");
            //StringBuilder query = new StringBuilder();
            //query.AppendLine("select parentUtvalgListId");
            //query.AppendLine("from KSPU_DB.utvalgList");
            //query.AppendLine("where utvalgListId = ");
            //query.AppendLine("(");
            //query.AppendLine("  select utvalgListId");
            //query.AppendLine("  from KSPU_DB.utvalg");
            //query.AppendLine("  where utvalgId = :UtvalgId");
            //query.AppendLine(")");
            int result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[0];

            npgsqlParameters[0] = new NpgsqlParameter("p_utvalgid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = utvalgId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.GetNrOfDoubleCoverageReolsForList", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetConnectedParentListWithAllReferences: " + exception.Message);
                throw;
            }
            return GetUtvalgListWithAllReferences(System.Convert.ToInt32(result)).Result;
        }

        private CampaignDescription GetCampaignDescriptionFromListDataRow(DataRow row, bool isDisconnected)
        {
            CampaignDescription cd = new CampaignDescription();
            Utils utils = new Utils();
            cd.ID = Convert.ToInt64(row["r_utvalglistid"]);
            cd.Name = Convert.ToString(row["r_utvalglistname"]);
            cd.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "r_ordertype", typeof(OrdreType), "Null");
            cd.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "r_orderstatus", typeof(OrdreStatus), "Null");
            cd.DistributionDate = Convert.IsDBNull(row["r_distribusjonsdato"]) ? DateTime.MinValue : Convert.ToDateTime(row["r_distribusjonsdato"]);
            cd.IsDisconnected = isDisconnected;
            return cd;
        }


        /// <summary>
        ///     ''' Henter ut:
        ///     '''  - alle tilhørende utvalg til lista 
        ///     '''  - alle childlister
        ///     '''  - alle utvalg som tilhører childlisten 
        ///     ''' </summary>
        ///     ''' <param name="utvLData"></param>
        ///     ''' <param name="includeReols"></param>
        ///     ''' <param name="includeParentList"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private UtvalgsListCollection GetAllListContent(DataTable utvLData, bool includeReols, bool includeParentList)
        {
            _logger.LogDebug("Preparing the data for GetAllListContent");
            UtvalgsListCollection utvLColl = new UtvalgsListCollection();
            Utils utils = new Utils();
            foreach (DataRow row in utvLData.Rows)
            {
                UtvalgList list = CreateListFromRow(row);
                int listId = list.ListId;

                // swap if list is basedOn           
                if (list.BasedOn > 0)
                    listId = list.BasedOn;

                // tilhørende utvalg
                _logger.LogDebug("Calling the SearchUtvalgByUtvalListId from utvalgRepository");
                UtvalgCollection utvalgCollection = _utvalgRepository.SearchUtvalgByUtvalListId(listId, includeReols).Result;
                foreach (Puma.Shared.Utvalg utvalg in utvalgCollection)
                    list.MemberUtvalgs.Add(utvalg);

                if (includeParentList && (row["r_parentutvalglistid"]) != null)
                {
                    if (list.BasedOn < 0)
                    {
                        list.ParentList = GetUtvalgList(utils.GetIntFromRow(row, "r_parentutvalglistid")).Result;
                        if (!list.ParentList.MemberLists.Contains(list))
                            list.ParentList.MemberLists.Add(list);
                    }
                }
                else
                {
                    DataTable utvData;
                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

                    npgsqlParameters[0] = new NpgsqlParameter("p_parentutvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
                    npgsqlParameters[0].Value = listId;
                    using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                    {
                        utvData = dbhelper.FillDataTable("kspu_db.getalllistcontent", CommandType.StoredProcedure, npgsqlParameters);
                    }
                    // Children lister? - Kun dersom det ikke finnes en parent liste.
                    int i = 0;
                    foreach (DataRow r in utvData.Rows)
                    {
                        _logger.LogDebug("Calling the GetUtvalgList");
                        UtvalgList childList = GetUtvalgList(Convert.ToInt64(r["r_utvalglistid"]), false).Result;
                        list.MemberLists.Add(childList);
                        childList.ParentList = list;
                        // Children utvalg
                        _logger.LogDebug("Calling the SearchUtvalgByUtvalListId from utvalgRepository");
                        utvalgCollection = _utvalgRepository.SearchUtvalgByUtvalListId(Convert.ToInt64(r["r_utvalglistid"])).Result;
                        foreach (Puma.Shared.Utvalg utvalg in utvalgCollection)
                            list.MemberLists[i].MemberUtvalgs.Add(utvalg);
                        i += 1;
                    }
                }

                if (list.BasedOn > 0)
                {
                    _logger.LogDebug("Calling the GetUtvalgListNoChild");
                    UtvalgList checkIfBasedOnList = GetUtvalgListNoChild(list.BasedOn).Result;
                    list = AddBasedOnValuesToList(list, checkIfBasedOnList);
                }

                utvLColl.Add(list);
            }
            _logger.LogDebug("Exiting from GetAllListContent");
            return utvLColl;
        }

        /// <summary>
        ///     ''' Get Modifications for list
        ///     ''' </summary>
        ///     ''' <param name="list"></param>
        ///     ''' <remarks></remarks>
        private void GetUtvalgListModifications(UtvalgList list)
        {
            _logger.LogDebug("Preparing the data for GetUtvalgListModifications");
            DataTable utvData;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];



            npgsqlParameters[0] = new NpgsqlParameter("p_utvalglistid", NpgsqlTypes.NpgsqlDbType.Integer);
            npgsqlParameters[0].Value = list.ListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    utvData = dbhelper.FillDataTable("kspu_db.getutvalglistmodifications", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListModifications: " + exception.Message);
                throw;
            }
            foreach (DataRow dataRow in utvData.Rows)
            {
                UtvalgModification utvalgModification = new UtvalgModification();
                utvalgModification.UserId = dataRow["r_userid"].ToString();
                utvalgModification.ModificationId = Convert.ToInt32(dataRow["r_utvalglistmodificationid"]);
                utvalgModification.ModificationTime = (DateTime)dataRow["r_modificationdate"];
                list.Modifications.Add(utvalgModification);
                _logger.LogInformation("Result is: ", utvalgModification);
            }

            _logger.LogDebug("Exiting from GetUtvalgListModifications");

        }

        private UtvalgList AddBasedOnValuesToList(UtvalgList inputList, UtvalgList BasedOnList)
        {
            if (BasedOnList.BasedOn > 0)
            {
                inputList.BasedOn = BasedOnList.BasedOn;
                inputList.Avtalenummer = BasedOnList.Avtalenummer;
                inputList.DistributionDate = BasedOnList.DistributionDate;
                inputList.DistributionType = BasedOnList.DistributionType;
                inputList.InnleveringsDato = BasedOnList.InnleveringsDato;
                inputList.KundeNavn = BasedOnList.KundeNavn;
                inputList.KundeNummer = BasedOnList.KundeNummer;
                inputList.ListId = BasedOnList.ListId;
                inputList.Logo = BasedOnList.Logo;
                inputList.Name = BasedOnList.Name;
                inputList.IsBasis = BasedOnList.IsBasis;
                inputList.OrdreReferanse = BasedOnList.OrdreReferanse;
                inputList.OrdreStatus = BasedOnList.OrdreStatus;
                inputList.OrdreType = BasedOnList.OrdreType;
                inputList.WasBasedOn = BasedOnList.WasBasedOn;
                inputList.Weight = BasedOnList.Weight;
                inputList.AllowDouble = BasedOnList.AllowDouble;
                inputList.Thickness = BasedOnList.Thickness;
                if (inputList.BasedOn > 0)
                {
                    inputList.BasedOnName = SetBasedOnWasBasedOnName(inputList.BasedOn).Result;
                }

                if (inputList.WasBasedOn > 0)
                {
                    inputList.WasBasedOnName = SetBasedOnWasBasedOnName(inputList.WasBasedOn).Result;
                }

            }
            return inputList;
        }

        public async Task<List<long>> GetIdsForFordelingToQue(Puma.Shared.UtvalgList utvalgList)
        {
            _logger.LogDebug("Preparing the data for GetIdsForFordelingToQue");
            List<long> ids = new List<long>();
            if (utvalgList.IsBasis)
            {
                //Get List CampainData
                _logger.LogDebug("calling the GetUtvalgListCampaigns");
                var listCampainData = await GetUtvalgListCampaigns(utvalgList.ListId);

                ids.AddRange(listCampainData?.Where(k => k.OrdreType == Puma.Shared.PumaEnum.OrdreType.T).Select(x => x.ID));

                //Get Parent CampainList Data
                _logger.LogDebug("calling the GetUtvalgListParentCampaigns");
                var parentListCampainData = await GetUtvalgListParentCampaigns(utvalgList.ListId);

                ids.AddRange(parentListCampainData?.Where(k => k.OrdreType == Puma.Shared.PumaEnum.OrdreType.T).Select(x => x.ID));

                //If found any data call for SendBasisUtvalgFordelingToQue

            }
            _logger.LogDebug("Exiting from GetIdsForFordelingToQue");
            return ids;
        }

        public async Task SendBasisUtvalgFordelingToQue(List<long> ids, string utvalgType)
        {
            _logger.LogDebug("Preparing the data for SendBasisUtvalgFordelingToQue");
            foreach (var item in ids)
            {
                UtvalgBasisFordeling utvalgBasisFordeling = new UtvalgBasisFordeling
                {
                    Dato = DateTime.Now,
                    ID = item,
                    Utvalgtype = utvalgType
                };

                //Check this data exists in Db or not
                _logger.LogDebug("calling the BasisUtvalgFordelingExistsOnQue from utvalgRepository ");
                bool isBasis = await _utvalgRepository.BasisUtvalgFordelingExistsOnQue(utvalgBasisFordeling);
                if (!isBasis)
                {
                    //if data not found then insert data in Db
                    _logger.LogDebug("calling the CreateBasisUtvalgFordelingOppdatering from utvalgRepository ");
                    await _utvalgRepository.CreateBasisUtvalgFordelingOppdatering(utvalgBasisFordeling);
                }
            }
            _logger.LogDebug("Exiting from SendBasisUtvalgFordelingToQue");
        }


        public async Task<string> GetUtvalgListName(int utvalgListId)
        {
            await Task.Run(() => { });
            _logger.LogDebug("Preparing the data for GetUtvalgListName");
            string result;
            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_listid", NpgsqlTypes.NpgsqlDbType.Numeric, 50);
            npgsqlParameters[0].Value = utvalgListId;
            try
            {
                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
                {
                    result = dbhelper.ExecuteScalar<string>("kspu_db.getutvalglistname", CommandType.StoredProcedure, npgsqlParameters);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in GetUtvalgListName:", exception.Message);
                throw;
            }
            if (result == null) // | result == DBNull.Value)
                return null;
            _logger.LogInformation("Result is: ", result);
            _logger.LogDebug("Exiting from GetUtvalgListName");
            return result;
        }

        public async Task CopySaveUtvalgListCopies(UtvalgList newUtvalgList, UtvalgList oldUtvalgList, string user, string suffix, bool forceSave, bool forceOrdreInfo)
        {
            //TODO: This we need to set in config table, i am not able to find it in config xml so default setting 50
            int maxUtvalgListName = 50;

            List<string> newListNames = new List<string>();
            foreach (UtvalgList oldUtvListMember in oldUtvalgList.MemberLists.ToList())
            {
                string newListNameWithSuffix = CreateNewNameWithSuffix(oldUtvListMember.Name, maxUtvalgListName, true, newListNames, suffix, forceSave);
                if (!newListNames.Contains(newListNameWithSuffix))
                    newListNames.Add(newListNameWithSuffix);
                UtvalgList newUtvList = new UtvalgList();
                newUtvList.Name = newListNameWithSuffix;
                newUtvList.SistEndretAv = user;
                newUtvList.SistOppdatert = DateTime.Now;
                newUtvList.ParentList = newUtvalgList;
                newUtvList.Logo = oldUtvListMember.Logo;
                newUtvList.KundeNummer = "";
                newUtvList.KundeNavn = "";
                // Legg på ny valgt kunde om noen.
                if (!string.IsNullOrEmpty(newUtvalgList.KundeNummer))
                {
                    newUtvList.KundeNummer = newUtvalgList.KundeNummer;
                    newUtvList.KundeNavn = newUtvalgList.KundeNavn;
                }
                // 20130418 Supportsak #621933 Frigjøring av kampanjer må arve ordreinformasjonen
                if (forceOrdreInfo)
                {
                    newUtvList.OrdreReferanse = oldUtvalgList.OrdreReferanse;
                    newUtvList.OrdreStatus = oldUtvalgList.OrdreStatus;
                    newUtvList.OrdreType = oldUtvalgList.OrdreType;
                }
                newUtvList.AntallWhenLastSaved = oldUtvListMember.Antall;
                await SaveUtvalgListData(newUtvList, user);
                await CopySaveUtvalgCopies(newUtvList, oldUtvListMember.MemberUtvalgs, user, suffix, forceSave, forceOrdreInfo);
            }


        }

        public async Task CopySaveUtvalgCopies(UtvalgList newUtvalgList, List<Puma.Shared.Utvalg> orgUtvalgList, string user, string suffix, bool forceSave, bool forceOrdreInfo)
        {
            //TODO: This we need to set in config table, i am not able to find it in config xml so default setting 50
            int maxUtvalgName = 50;
            List<string> newUtvalgNames = new List<string>();
            foreach (Puma.Shared.Utvalg orgUtv in orgUtvalgList.ToList())
            {
                string newUtvNameWithSuffix = CreateNewNameWithSuffix(orgUtv.Name, maxUtvalgName, false, newUtvalgNames, suffix, forceSave);
                if (!newUtvalgNames.Contains(newUtvNameWithSuffix))
                    newUtvalgNames.Add(newUtvNameWithSuffix);

                Puma.Shared.Utvalg newUtv = Puma.Shared.Utvalg.CreateUtvalgCopy(orgUtv, false);
                newUtv.Name = newUtvNameWithSuffix;
                newUtv.List = newUtvalgList;
                newUtv.ListId = newUtvalgList.ListId.ToString();
                newUtv.KundeNummer = "";
                newUtv.KundeNavn = "";
                // Legg på ny valgt kunde om noen.
                if (!string.IsNullOrEmpty(newUtvalgList.KundeNummer))
                {
                    newUtv.KundeNummer = newUtvalgList.KundeNummer;
                    newUtv.KundeNavn = newUtvalgList.KundeNavn;
                }

                // 20130418 Supportsak #621933 Frigjøring av kampanjer må arve ordreinformasjonen
                if (forceOrdreInfo)
                {
                    newUtv.OrdreReferanse = newUtvalgList.OrdreReferanse;
                    newUtv.OrdreStatus = newUtvalgList.OrdreStatus;
                    newUtv.OrdreType = newUtvalgList.OrdreType;
                }
                _ = await _utvalgRepository.SaveUtvalgData(newUtv, user);
            }
        }

        /// <summary>
        /// Deletes the updated list if empty.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldParentList">The old parent list.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUpdatedListIfEmpty(string userName, Puma.Shared.UtvalgList oldParentList)
        {
            if (oldParentList != null)
            {
                List<long> ids = new List<long>();
                if (oldParentList.IsBasis)
                {
                    ids = await GetListsToRefreshDueToUpdateToBasisList(oldParentList);
                    List<long> ids2 = await GetListsToRefreshDueToUpdateToBasisListChild(oldParentList);
                    ids2.AddRange(ids);
                    await SendBasisUtvalgFordelingToQue(ids, "L");
                }

                if (ids.Count == 0)
                {
                    return (await CheckAndDeleteUtvalgListIfEmpty(oldParentList.ListId, userName));
                }
            }
            return false;
        }

        public string CreateNewNameWithSuffix(string name, int maxNameLength, bool IsList, List<string> newNameList, string suffix, bool forceNewName)
        {
            if (forceNewName)
                return CreateNewNameWithSuffixForced(name, maxNameLength, IsList, newNameList, suffix).Result;
            else
                return CreateNewNameWithSuffix(name, maxNameLength, IsList, newNameList, suffix).Result;
        }

        public async Task<string> CreateNewNameWithSuffixForced(string name, int maxNameLength, bool isList, List<string> newNameList, string suffix)
        {
            await Task.Run(() => { });
            int maxPrefixLength = maxNameLength - suffix.Length - 4;
            if (name.Length > maxPrefixLength) name = name.Substring(0, maxPrefixLength);
            name += suffix;
            string origName = name;
            int counter = 1;
            while (newNameList.Contains(name) || DoesNameExist(name, isList))
            {
                name = origName + "_" + counter;
                counter++;
            }
            return name;
        }

        private bool DoesNameExist(string name, bool isList)
        {
            if (!isList)
            {
                return _utvalgRepository.UtvalgNameExists(name).Result;
            }
            else
            {
                return UtvalgListNameExists(name).Result;
            }
        }

        public async Task<string> CreateNewNameWithSuffix(string name, int maxNameLength, bool IsList, List<string> newNameList, string suffix)
        {
            string newNameWithSuffix = name + suffix;
            string newNameWithoutSuffix = name;
            if (newNameWithSuffix.Length > maxNameLength)
            {
                string oldName = name;

                int maxOldNameLength = maxNameLength - suffix.Length;
                if (maxOldNameLength < 0)
                    maxOldNameLength = 0;

                if (oldName.Length > suffix.Length)
                {
                    if (oldName.Length > maxOldNameLength)
                    {
                        newNameWithoutSuffix = oldName.Remove(maxOldNameLength, oldName.Length - maxOldNameLength);
                        newNameWithSuffix = newNameWithoutSuffix + suffix;
                    }
                }
                else //vil kun oppstå dersom man justerer config for max suffix length til mer enn 25 tegn.
                {
                    if (oldName.Length > 25)
                    {
                        oldName = oldName.Remove(25, (oldName.Length - 25));
                    }
                    if (suffix.Length > 25)
                    {
                        suffix = suffix.Remove(25, (suffix.Length - 25));
                    }
                    newNameWithoutSuffix = oldName;
                    newNameWithSuffix = newNameWithoutSuffix + suffix;
                }
            }

            if (newNameList.Contains(newNameWithSuffix))
            {
                return await CreateNewNameWithSuffix(newNameWithoutSuffix.Substring(0, newNameWithoutSuffix.Length - 1), maxNameLength, false, newNameList, suffix);
            }

            if (!IsList)
            {
                if (_utvalgRepository.UtvalgNameExists(newNameWithSuffix).Result)
                {
                    return await CreateNewNameWithSuffix(newNameWithoutSuffix.Substring(0, newNameWithoutSuffix.Length - 1), maxNameLength, false, newNameList, suffix);
                }
            }

            if (IsList)
            {
                if (UtvalgListNameExists(newNameWithSuffix).Result)
                {
                    //uxErrorMsgLagreSom.Text = KSPUMessages.errMsgListExists.Replace("*", newNameWithSuffix);
                    throw new Exception("Egendefinert tekst gir navnekonflikt på minst ett utvalg eller en utvalgsliste. Velg en annen tekst.");
                }
            }
            else
            {
                if (_utvalgRepository.UtvalgNameExists(newNameWithSuffix).Result)
                {
                    //uxErrorMsgLagreSom.Text = KSPUMessages.errMsgUtvalgNameExists.Replace("*", newNameWithSuffix);
                    throw new Exception("Egendefinert tekst gir navnekonflikt på minst ett utvalg eller en utvalgsliste. Velg en annen tekst.");

                }
            }

            return newNameWithSuffix;
        }

        public async Task<string> SetBasedOnWasBasedOnName(int id)
        {
            //string basedonWasbasedonName = await GetUtvalgName(id);
            return await GetUtvalgListName(id);

        }

        #endregion
    }
}
