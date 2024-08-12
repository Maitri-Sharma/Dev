using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.Hangfire
{
    /// <summary>
    /// HangfireJobScheduler
    /// </summary>
    public class HangfireJobScheduler
    {
        /// <summary>
        /// Schedules the job.
        /// </summary>
        public static void ScheduleJob()
        {
            try
            {

                RecurringJob.AddOrUpdate<IHangfireJob>("SelectionDistribution", x => x.SelectionDistribution(null), "0/10 * * * * ?");

                RecurringJob.AddOrUpdate<IHangfireJob>("RestCapacityJob", x => x.RestCapacityJob(null), "0 1,2 * * *");

                RecurringJob.AddOrUpdate<IHangfireJob>("PumaLogCleanService", x => x.PumaLogCleanService(null), "0 23 * * *");

                //* 0,1 *  * *
            }
            catch 
            {
                //TODO : log this error using logger
                //throw;
            }
        }
    }
}