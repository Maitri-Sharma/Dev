using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB.Reol
{
    /// <summary>
    /// IReolGeneratorRepository
    /// </summary>
    public interface IReolGeneratorRepository : IKsupDBGenericRepository<utvalg>
    {
        /// <summary>
        /// Calculates the statistics.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> CalculateStatistics(string tableName, bool gdbuser = false);
        /// <summary>
        /// Doeses the table exist.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> DoesTableExist(string tableName, bool gdbuser = false);

        /// <summary>
        /// Doeses the view exist.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> DoesViewExist(string viewName, bool gdbuser = false);

        /// <summary>
        /// Renames the table.
        /// </summary>
        /// <param name="fromTableName">Name of from table.</param>
        /// <param name="toTableName">Name of to table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> RenameTable(string fromTableName, string toTableName, bool gdbuser = false);

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="IndexName">Name of the index.</param>
        /// <param name="TablenameWithFields">The tablename with fields.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> CreateIndex(string IndexName, string TablenameWithFields, bool gdbuser = false);

        /// <summary>
        /// Drops the index.
        /// </summary>
        /// <param name="IndexName">Name of the index.</param>
        /// <param name="GDBUser">if set to <c>true</c> [GDB user].</param>
        /// <returns></returns>
        Task<bool> DropIndex(string IndexName, bool GDBUser = false);
        /// <summary>
        /// Updates the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="tableNameWithFields">The table name with fields.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> UpdateIndex(string indexName, string tableNameWithFields, bool gdbuser = false);
        /// <summary>
        /// Truncates the table.
        /// </summary>
        /// <param name="piTableNAme">The pi table n ame.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> Truncate_Table(string piTableNAme, bool gdbuser = false);
        /// <summary>
        /// Grants the select.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="toUser">To user.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> GrantSelect(string tableName, string toUser, bool gdbuser = false);
        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="asGDB">if set to <c>true</c> [as GDB].</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        Task<string> getFields(string fromTable, bool asGDB, string prefix);

        /// <summary>
        /// Gets the rapport information.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        Task<string> GetRapportInfo(string tablename);
        /// <summary>
        /// Calculates the total fields.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <param name="sumSQL">The sum SQL.</param>
        /// <returns></returns>
        Task<bool> CalculateTotalFields(string tablename, string sumSQL);
        /// <summary>
        /// Updates the total fields.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fromTable">From table.</param>
        /// <returns></returns>
        Task<bool> UpdateTotalFields(string tableName, string fromTable);
        /// <summary>
        /// Gets the in reol string for stat rest points.
        /// </summary>
        /// <returns></returns>
        Task<string> GetInReolStringForStatRestPoints();
        /// <summary>
        /// Inserts the missing reols.
        /// </summary>
        /// <param name="statTableName">Name of the stat table.</param>
        /// <param name="RouteTableName">Name of the route table.</param>
        /// <returns></returns>
        Task<bool> InsertMissingReols(string statTableName, string RouteTableName);

        /// <summary>
        /// Determines whether [is xy present] [the specified table name].
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        Task<bool> IsXYPresent(string TableName);
        /// <summary>
        /// Creates the rest points gk.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateRestPointsGK();
        /// <summary>
        /// Updates the reol points gk.
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateReolPointsGK();

        /// <summary>
        /// Creates the statistic table.
        /// </summary>
        /// <param name="statTableName">Name of the stat table.</param>
        /// <param name="baseStatTableName">Name of the base stat table.</param>
        /// <param name="RoundNum">if set to <c>true</c> [round number].</param>
        /// <param name="statTotFields">The stat tot fields.</param>
        /// <returns></returns>
        Task<bool> CreateStatisticTable(string statTableName, string baseStatTableName, bool RoundNum, string statTotFields);

        /// <summary>
        /// Creates the address reol table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAddressReolTable();

        /// <summary>
        /// Updates the address reol table.
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateAddressReolTable();

        /// <summary>
        /// Creates the address reol postbox table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAddressReolPostboxTable();

        /// <summary>
        /// Inserts the address reol postbox table.
        /// </summary>
        /// <returns></returns>
        Task<bool> InsertAddressReolPostboxTable();

        /// <summary>
        /// Creates the sum pop reol.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSumPopReol();

        /// <summary>
        /// Updates the addr reol tab with sum pop gk tab.
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateAddrReolTabWithSumPopGKTab();

        /// <summary>
        /// Updates the addr reol tab with sum pop reol tab.
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateAddrReolTabWithSumPopReolTab();

        /// <summary>
        /// Creates the temporary input reolpunkter adr nr gab.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateTmpInputReolpunkterAdrNrGab();

        /// <summary>
        /// Creates the temporary input reolpunkter with valid xy.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateTmpInputReolpunkterWithValidXY();

        /// <summary>
        /// Creates the temporary input boksanlegg adr nr gab.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateTmpInputBoksanleggAdrNrGab();

        /// <summary>
        /// Creates the temporary input boksanlegg with valid xy.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateTmpInputBoksanleggWithValidXY();

        /// <summary>
        /// Creates the gk reol overforing.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateGKReolOverforing();

        /// <summary>
        /// Creates the index on GKGK reol overforing.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIndexOnGKGKReolOverforing();

        /// <summary>
        /// Creates the index on reol gk reol overforing.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIndexOnReolGKReolOverforing();

        /// <summary>
        /// Oppdaters the temporary reol adr med riktig reol identifier.
        /// </summary>
        /// <param name="pinyReolID">The piny reol identifier.</param>
        /// <returns></returns>
        Task<bool> Oppdater_tmp_reol_adrMedRiktigReolID(string pinyReolID);

        /// <summary>
        /// Antalls the adre punkter i tem reol adr.
        /// </summary>
        /// <param name="sReolID">The s reol identifier.</param>
        /// <returns></returns>
        Task<int> AntallAdrePunkterITem_reol_adr(string sReolID);

        /// <summary>
        /// Creates the temporary reolid.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateTMP_REOLID();

        /// <summary>
        /// Creates the reol ant adr table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateReolAntAdrTable();

        /// <summary>
        /// Inserts the into temporary reolid.
        /// </summary>
        /// <param name="piReolID">The pi reol identifier.</param>
        /// <returns></returns>
        Task<bool> InsertInto_TMP_REOLID(string piReolID);

        /// <summary>
        /// Creates the sum hh.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSumHH();

        /// <summary>
        /// Creates the reol komm faktor.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateReolKommFaktor();

        /// <summary>
        /// Creates the adjust reols.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAdjustReols();

        /// <summary>
        /// Creates the avisdekning.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAvisdekning();

        /// <summary>
        /// Creates the avis mapping table.
        /// </summary>
        /// <returns></returns>
        Task<bool> createAvisMappingTable();

        /// <summary>
        /// Updates the avis mapping table.
        /// </summary>
        /// <returns></returns>
        Task<bool> updateAvisMappingTable();


        /// <summary>
        /// Creates the post coverage table.
        /// </summary>
        /// <returns></returns>
        Task<bool> createPostCoverageTable();

        /// <summary>
        /// Creates the newspaper table.
        /// </summary>
        /// <returns></returns>
        Task<bool> createNewspaperTable();
        /// <summary>
        /// Inserts the data to post cover table.
        /// </summary>
        /// <returns></returns>
        Task<bool> InsertDataToPostCoverTable();
        /// <summary>
        /// Inserts the komm i ds to news paper table.
        /// </summary>
        /// <returns></returns>
        Task<bool> InsertKommIDsToNewsPaperTable();
        /// <summary>
        /// Updates the news paper table.
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateNewsPaperTable();
        /// <summary>
        /// Creates the segment temporary table 1.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSegmentTempTable_1();
        /// <summary>
        /// Creates the segment temporary table 2.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSegmentTempTable_2();
        /// <summary>
        /// Creates the segment temporary table 3.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSegmentTempTable_3();
        /// <summary>
        /// Creates the reol segment table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateReolSegmentTable();
        /// <summary>
        /// Creates the input reoler segment table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateInputReolerSegmentTable();
        /// <summary>
        /// Creates the antall bef og reol tabell.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateAntallBefOgReolTabell();
        /// <summary>
        /// Deletes the antall bef reol tabell.
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAntallBefReolTabell();
        /// <summary>
        /// Creates the slutt rapport tabell.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSluttRapportTabell();
        /// <summary>
        /// Skrivs the til slutt rapport tabellen.
        /// </summary>
        /// <param name="piTekst">The pi tekst.</param>
        /// <returns></returns>
        Task<bool> SkrivTilSluttRapportTabellen(string piTekst);
        /// <summary>
        /// Gets the rapport information.
        /// </summary>
        /// <returns></returns>
        Task<string> GetRapportInfo();

        /// <summary>
        /// Loggs the slutt rapport.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        Task<bool> LoggSluttRapport(string tablename);
        /// <summary>
        /// Sletts the slutt rapport tabell.
        /// </summary>
        /// <returns></returns>
        Task<bool> SlettSluttRapportTabell();
        /// <summary>
        /// Gets the alias table.
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getAliasTable();
        /// <summary>
        /// Gets the dublikate reolkrets.
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetDublikateReolkrets();
        /// <summary>
        /// Adds the variable cha r2 field to table.
        /// </summary>
        /// <param name="piTableNAme">The pi table n ame.</param>
        /// <param name="piFeltNAme">The pi felt n ame.</param>
        /// <param name="piLength">Length of the pi.</param>
        /// <returns></returns>
        Task<bool> AddVarCHAR2FieldToTable(string piTableNAme, string piFeltNAme, int piLength);

        /// <summary>
        /// Selects the count.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <returns></returns>
        Task<int> SelectCount(string tablename);

        /// <summary>
        /// Gets the total fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        Task<string> getTotalFields(string fromTable, string prefix = "");
        /// <summary>
        /// Gets the statistic fields.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="AdditionalIgnoreFields">The additional ignore fields.</param>
        /// <param name="additionSelect">The addition select.</param>
        /// <returns></returns>
        Task<string> GetStatisticFields(string fromTable, string prefix = "", string AdditionalIgnoreFields = "", string additionSelect = "");

        /// <summary>
        /// Creates the sum pop table.
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateSumPopTable();

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> DropTable(string tableName, bool gdbuser = false);

        /// <summary>
        /// Analyzes the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="gdbuser">if set to <c>true</c> [gdbuser].</param>
        /// <returns></returns>
        Task<bool> AnalyzeTable(string tableName, bool gdbuser = false);
        //TODO : Check these methods
        Task<DataTable> getData(string sql);
        Task<int> SelectCountAsDBUser(string piSQL);

        Task<int> SelectCountAsGDBUser(string piSQL);

    }
}
