using DataAccessAPI.Extensions;
using DataAccessAPI.HandleRequest.Request.Report;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public ReportsController(ILogger<ReportsController> logger, IMediator mediator = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("GenerateReport", Name = nameof(GenerateReport))]
        public IActionResult GenerateReport([FromBody] RequestSelectedAddress requestSelectedAddress,int listId, string distrDate, string strDayDetails, string reportType, string emailTo, int level, int uptolevel,bool showBusiness = false, bool showHouseholds =false, bool showHouseholdsReserved = false,bool isCustomerWeb = false)
        {
            _logger.BeginScope("Inside into GenerateReport");
            ReportRequest req = new ReportRequest()
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
                selectedAddress = requestSelectedAddress.selectedAddress
            };
            return Ok(_mediator.Send(req).Result);
        }

        [HttpPost("GenerateSelectionReport", Name = nameof(GenerateSelectionReport))]
        public IActionResult GenerateSelectionReport([FromBody] RequestSelectionReportData requestSelectedAddress,  string distrDate, string strDayDetails, string reportType,  int level, int uptolevel, bool showBusiness = false, bool showHouseholds = false, bool showHouseholdsReserved = false, bool isCustomerWeb = false)
        {
            _logger.BeginScope("Inside into GenerateReport");
            RequestSelectionReport req = new RequestSelectionReport()
            {
                UtvalgData = requestSelectedAddress.UtvalgData,
                showBusiness = showBusiness,
                showHouseholdReserved = showHouseholdsReserved,
                showHouseholds = showHouseholds,
                DistrDate = distrDate,
                strDayDetails = strDayDetails,
                level = level,
                uptoLevel = uptolevel,
                isCustomerWeb = isCustomerWeb,
                reportType = reportType,
                selectedAddress = requestSelectedAddress.selectedAddress
            };
           // return Ok(_mediator.Send(req).Result);
           if(req.reportType == "pdf")
                return File(_mediator.Send(req).Result, "application/pdf",req.UtvalgData.UtvalgId.ToString());
           else
                return File(_mediator.Send(req).Result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", req.UtvalgData.UtvalgId.ToString());
        }

        /// <summary>
        /// Generates the report token.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GenerateReportToken", Name = nameof(GenerateReportToken))]
        public IActionResult GenerateReportToken()
        {
            _logger.BeginScope("Inside into GenerateReport");
           
            return Ok(_mediator.Send(new RequestGenerateReportToken()).Result);
        }
    }
}
