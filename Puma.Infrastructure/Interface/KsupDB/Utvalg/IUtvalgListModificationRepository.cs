using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB.Utvalg
{
    public interface IUtvalgListModificationRepository : IKsupDBGenericRepository<UtvalgListModification>
    {
        Task<List<UtvalgList>> GetUtvalgListModifications(string utvalglistname, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg);
    }
}
