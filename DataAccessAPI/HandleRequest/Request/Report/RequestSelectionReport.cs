using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Report
{
    public class RequestSelectionReport : IRequest<byte[]>
    {
        //public int utvalgId { get; set; }

        public Puma.Shared.Utvalg UtvalgData { get; set; }
        public bool showBusiness { get; set; }
        public bool showHouseholds { get; set; }
        public bool showHouseholdReserved { get; set; }
        public bool isCustomerWeb { get; set; }

        public string DistrDate { get; set; }

        public string strDayDetails { get; set; }

        public int level { get; set; }
        public int uptoLevel { get; set; }

        public string reportType { get; set; }

        public string selectedAddress { get; set; }
    }


    public class RequestSelectionReportData
    {
        public Puma.Shared.Utvalg UtvalgData { get; set; }

        public string selectedAddress { get; set; }
    }
}
