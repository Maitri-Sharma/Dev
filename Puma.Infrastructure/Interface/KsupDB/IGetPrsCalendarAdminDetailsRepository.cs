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
    public interface IGetPrsCalendarAdminDetailsRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<List<GetPrsAdminCalendarData>> GetPRSAdminCalendar(DateTime fromDate, DateTime toDate);

        Task<GetPrsAdminCalendarData> GetPRSAdminCalendarDayDetail(DateTime FindDate);

        Task<List<DateTime>> GetDateWiseBookedCapacity(DateTime fromDate, DateTime toDate, long id, string type, string receiverType, int weight, double thickness = 0.0);
    }
}
