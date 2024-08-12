using DataAccessAPI.HandleRequest.Request.Calendar;
using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Calendar
{
    /// <summary>
    /// GetPrsCalendarDetailHandler
    /// </summary>
    public class GetPrsCalendarDetailHandler : IRequestHandler<RequestGetPrsCalendarDetail, ResponseGetPrsCalendarDetail>
    {
        

        /// <summary>
        /// The get PRS calendar admin details repository
        /// </summary>
        private readonly IGetPrsCalendarAdminDetailsRepository _getPrsCalendarAdminDetailsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPrsCalendarDetailHandler"/> class.
        /// </summary>
        /// <param name="getPrsCalendarAdminDetailsRepository">The get PRS calendar admin details repository.</param>
        /// <exception cref="System.ArgumentNullException">getPrsCalendarAdminDetailsRepository</exception>
        public GetPrsCalendarDetailHandler(IGetPrsCalendarAdminDetailsRepository getPrsCalendarAdminDetailsRepository)
        {
           
            _getPrsCalendarAdminDetailsRepository = getPrsCalendarAdminDetailsRepository ?? throw new ArgumentNullException(nameof(getPrsCalendarAdminDetailsRepository));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ResponseGetPrsCalendarDetail> Handle(RequestGetPrsCalendarDetail request, CancellationToken cancellationToken)
        {
            var response = new ResponseGetPrsCalendarDetail();
            var data = new GetPrsAdminCalendarData();
            data = await _getPrsCalendarAdminDetailsRepository.GetPRSAdminCalendarDayDetail(request.FindDate);
            string strDayDetails = "";
            if (data is object & data.FrequencyType is object)
            {
                if (!string.IsNullOrEmpty(data.FrequencyType.Trim()))
                {
                    switch (data.FrequencyType.Trim() ?? "")
                    {
                        case "A":
                            {
                                if (data.IsEarlyWeekFirstDay | data.IsEarlyWeekSecondDay)
                                {
                                    strDayDetails = "A-uke, tidliguke";
                                }
                                else if (data.IsMidWeekFirstDay | data.IsMidWeekSecondDay)
                                {
                                    strDayDetails = "A-uke, midtuke";
                                }

                                break;
                            }

                        case "B":
                            {
                                if (data.IsEarlyWeekFirstDay | data.IsEarlyWeekSecondDay)
                                {
                                    strDayDetails = "B-uke, tidliguke";
                                }
                                else if (data.IsMidWeekFirstDay | data.IsMidWeekSecondDay)
                                {
                                    strDayDetails = "B-uke, midtuke";
                                }

                                break;
                            }
                    }
                }
            }

            response.DateDetailFromPrs = strDayDetails;
            return response;
        }
    }
}
