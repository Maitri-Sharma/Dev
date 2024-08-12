using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IKapasitetRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<List<KapasitetDato>> GetKapasitetDatoer(DateTime fromDate, DateTime toDate);
        Task<long> GetTotalAntall(long id, string type);

        Task<List<LackingCapacity>> GetDatesLackingCapacity(DateTime fromDate, DateTime toDate, long id, string type, string receiverType, int weight, double thickness = 0.0);

        Task<List<KapasitetRuter>> GetRuterLackingCapacity(List<string> dates, long id, string type, string receiverType, int weight, double thickness = 0.0);

        Task SubtractRestkapasitetAbsoluttDag(List<long> ruteIds, int restvekt, DateTime dato, string mottakerType, double restthickness);

        Task<List<DateTime>> GetSperrefristDates(DateTime dato, int dayCount);

        Task SubtractRestkapasitetSperrefrist(List<long> ruteIds, int restvekt, DateTime dato, string mottakertype, double restthickness);
    }
}
