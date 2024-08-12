using Hangfire.Server;
using MediatR;
using System;

namespace DataAccessAPI.HandleRequest.Request.Report
{
    public class ReportJobRequest : IRequest<Unit>
    {
        public int listId { get; set; }
        public bool showBusiness { get; set; }
        public bool showHouseholds { get; set; }
        public bool showHouseholdReserved { get; set; }
        public bool isCustomerWeb { get; set; }

        public string DistrDate { get; set; }

        public string strDayDetails { get; set; }

        public int level { get; set; }
        public int uptoLevel { get; set; }
        public string emailTo { get; set; }

        public string reportType { get; set; }

        public string selectedAddress { get; set; }

        public PerformContext context;

    }
}


