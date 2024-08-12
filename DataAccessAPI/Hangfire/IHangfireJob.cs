using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.Hangfire
{
    /// <summary>
    ///IHangfireJob 
    /// </summary>
    public interface IHangfireJob
    {
        /// <summary>
        /// Recreateds the service.
        /// </summary>
        /// <returns></returns>
        Task SelectionDistribution(PerformContext context);

        Task ReportJob(int listId, string distrDate, string strDayDetails, string reportType, string emailTo, int level, int uptolevel, PerformContext context, string selectedAddress, bool showBusiness, bool showHouseholds, bool showHouseholdsReserved, bool isCustomerWeb);

        Task RestCapacityJob(PerformContext context);

        Task PumaLogCleanService(PerformContext context);
    }
}