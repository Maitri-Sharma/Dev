using DataAccessAPI.HandleRequest.Request.Report;
using DataAccessAPI.Hangfire;
using Hangfire;
using MediatR;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Report
{
    public class GenerateReportHandler : IRequestHandler<ReportRequest, MediatR.Unit>
    {
        
        public GenerateReportHandler(IUtvalgListRepository utvalgListRepository, IHttpClientFactory httpClientFactory)
        {
        }

        public async Task<MediatR.Unit> Handle(ReportRequest request, CancellationToken cancellationToken)
        {
            await Task.Run(() => { });
            BackgroundJob.Enqueue<IHangfireJob>(x => x.ReportJob(request.listId, request.DistrDate, request.strDayDetails, request.reportType, request.emailTo, request.level, request.uptoLevel, null, request.selectedAddress, request.showBusiness, request.showHouseholds, request.showHouseholdReserved, request.isCustomerWeb));

            return Unit.Value;
        }
       
    }

}
