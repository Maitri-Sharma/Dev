using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessAPI.HandleRequest.Request.Logger;
using DataAccessAPI.HandleRequest.Request.PumaRestCapacity;
using DataAccessAPI.HandleRequest.Request.Report;
using DataAccessAPI.HandleRequest.Request.SelectionDistribution;
using Hangfire.Server;
using MediatR;
namespace DataAccessAPI.Hangfire
{
    /// <summary>
    /// IHangfireJob
    /// </summary>
    /// <seealso cref="DataAccessAPI.Hangfire.IHangfireJob" />
    public class HangfireJob : IHangfireJob
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;



        /// <summary>
        /// Initializes a new instance of the <see cref="HangfireJob"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">mediator</exception>
        public HangfireJob(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Recreateds the service.
        /// </summary>
        public async Task SelectionDistribution(PerformContext context)
        {
            RequestSelectionDistribution request = new RequestSelectionDistribution()
            {
                context = context
            };
            await _mediator.Send(request);
        }

        public async Task ReportJob(int listId, string distrDate, string strDayDetails, string reportType, string emailTo, int level, int uptolevel, PerformContext context, string selectedAddress, bool showBusiness = false, bool showHouseholds = false, bool showHouseholdsReserved = false, bool isCustomerWeb = false)
        {
            ReportJobRequest request = new ReportJobRequest()
            {
                listId = listId,
                showBusiness = showBusiness,
                showHouseholdReserved = showHouseholdsReserved,
                showHouseholds = showHouseholds,
                DistrDate = distrDate,
                strDayDetails = strDayDetails,
                level = level,
                uptoLevel = uptolevel,
                emailTo = emailTo,
                isCustomerWeb = isCustomerWeb,
                reportType = reportType,
                context = context,
                selectedAddress = selectedAddress
            };
            await _mediator.Send(request);
        }

        /// <summary>
        /// Resy capacity service.
        /// </summary>
        public async Task RestCapacityJob(PerformContext context)
        {
            RequestRestCapacity request = new RequestRestCapacity()
            {
                context = context
            };
            await _mediator.Send(request);
        }


        /// <summary>
        /// Puma log clean service.
        /// </summary>
        public async Task PumaLogCleanService(PerformContext context)
        {
            RequestPumaLogCleanService request = new RequestPumaLogCleanService()
            {
                context = context
            };
            await _mediator.Send(request);
        }
    }
}