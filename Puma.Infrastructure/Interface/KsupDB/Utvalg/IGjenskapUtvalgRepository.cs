using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Interface.KsupDB.Utvalg
{
    public interface IGjenskapUtvalgRepository : IKsupDBGenericRepository<utvalg>
    {
        Task<bool> TableExistsforrecreateutvalg(string tableName);

        Task CreateUtvalgRecreationProblemTable();
        Task CreateUtvalgRecreationRuntimeTable();

        Task ClearUtvalgRecreationRuntimeTable();

        Task CreateUtvalgRecreationLogTable();

        Task ClearUtvalgRecerationLogTable();

        Task CreateUtvalgRecreationVerficationTable();

        Task CreateUtvalgRecreationWorstCases();

        Task ClearUtvalgRecerationVerificationTable();

        Task ClearUtvalgRecerationWorstCasesTable();

        Task<string> Get_Utvalg_ByID_SQL(Nullable<int> processId, string ID, bool forLocking);

        Task<string> Get_Utvalg_Tilbud_Or_Bestilt_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking);

        Task<string> Get_UtvalgListID_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking);

        Task<string> Get_Utvalg_In_List_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool IsOrderedorDelivered, bool forLocking);

        Task<string> Get_Andre_Utvalg_Recently_First_SQL(Nullable<int> processId, int maxNrOfUtvalg, bool forLocking);

        Task<string> GetLockingSQLPredicatePart(bool forLocking, Nullable<int> processId);

        Task<DataTable> GetAllRowsInUtvalgRecreationLogTable();

        Task<DataTable> GetAllRowsInUtvalgRecreationVerificationTable();

        Task<DataTable> GetSomeRowsInWorstCasesLogTable(bool onlyOrder, DateTime fromDate, DateTime toDate, bool bWhereDistDate);

        Task<DataTable> GetAllRowsInWorstCasesLogTable();

        Task DeleteFromOldUtvalgGeometryIfPresent(int utvalgId);

        Task<bool> IsOldUtvalgGeometryPresent(int utvalgId);

        Task DeleteAllRowsInTable(string tableName);

        Task GrantStandardPermissionsOnTable(string tableName);

        Task<bool> FailureOnUtvalgRegistred(int utvalgid);

        Task UpdateRecreationFailure(int utvalgId, string errorText);

        Task InsertIntoUtvalgWorstCasesLog(Puma.Shared.Utvalg utv, double antAvvik, double arealAvvik, long antDiff, TempResultData resData);

        Task InsertRecreationFailure(int utvalgId, string errorText);

        Task InsertIntoUtvalgRecreationLog(int utvalgId, string utvalgName, string typeGjenskaping);

        Task<DataTable> GetAllRowsWithLessThan1500InRecreationLogTable();

        Task<DataTable> GetXnrOfRowsFromTable(string tableName, int nrOfRows);

        Task InsertIntoUtvalgRecreationVerification(int utvalgId, string utvalgName, string ordrereferanse, string kundenummer, DateTime innleveringsDato, int antallForGjenskaping, int antallEtterGjenskaping);

        Task<int> NumberOfUtvalgToRecreate();

        Task<int> NumberOfUtvalgInRecreationProblemTable();


        Task<int> NrOfUtvalgWithOldReolMapAndAntallZero();

        Task ReleaseLocks();

        Task<string> FindOldReolmap(string strUtvalgID);

        Task<string> addBoxesText(string strBoxes, string strReolId);
        Task<string> FindAllRemovedBoxes(string strUtvalgID, string strOldReolMapName);
        Task<string> FindAllNewBoxes(string strUtvalgID, string strOldReolMapName);

        Task<DataTable> GetAllRowsInTableWhereAndOrderBy(string tableName, string strWhere, string orderBy);

        Task<DataTable> GetAllRowsInTableOrderBy(string tableName, string orderBy);

        Task<DataTable> GetAllRowsInTable(string tableName);

        Task DeleteFromTableBasedOnUtvalgId(string tableName, DataRow dr);

        Task<bool> IsChangedBeforRecreate(long piUtvalgID, DateTime LastModifiedDate);

        Task<int> MaxAntallUtvalgKandidaterForGjenskap(string sqlString);
        Task LockUtvalgRows(string insertSelect);
        Task RemoveUtvalgLocks(string utvalgIdSqlSelect);
        Task<DataTable> GetReols(string sql);
    }

}
