using DataAccessAPI.HandleRequest.Request.Calendar;
using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Calendar
{
    /// <summary>
    /// GetPrsAdminDataHandler
    /// </summary>
    public class GetPrsAdminDataHandler : IRequestHandler<RequestGetPrsAdminData, ResponseGetPrsAdminData>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetPrsAdminDataHandler> _logger;

        /// <summary>
        /// The get PRS calendar admin details repository
        /// </summary>
        private readonly IGetPrsCalendarAdminDetailsRepository _getPrsCalendarAdminDetailsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPrsAdminDataHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="getPrsCalendarAdminDetailsRepository">The get PRS calendar admin details repository.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// getPrsCalendarAdminDetailsRepository
        /// </exception>
        public GetPrsAdminDataHandler(ILogger<GetPrsAdminDataHandler> logger, IGetPrsCalendarAdminDetailsRepository getPrsCalendarAdminDetailsRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<ResponseGetPrsAdminData> Handle(RequestGetPrsAdminData request, CancellationToken cancellationToken)
        {
            ResponseGetPrsAdminData response = new ResponseGetPrsAdminData();
            if (request.Id == 0)
            {
                response.ErrorMessage = "Id er ikke angitt.";
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            if (!(request.Type.ToUpper().Equals("U") | request.Type.ToUpper().Equals("L")))
            {
                response.ErrorMessage = string.Format("Ugyldig type er angitt: {0}.", request.Type);
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }



            return await GetCapacity(request);

        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<ResponseGetPrsAdminData> GetCapacity(RequestGetPrsAdminData request)
        {
            var allDates = await _getPrsCalendarAdminDetailsRepository.GetPRSAdminCalendar(request.StartDato, request.SluttDato.AddDays(6d)); // add 6 more to get enough dates
            var datesInPeriod = (from d in allDates
                                 where !(d.Dato > request.SluttDato)
                                 orderby d.Dato ascending
                                 select d).ToList();

            // get all dates in Early Week(S) and Midweek
            List<GetPrsAdminCalendarData> deliveryDates;
            if (request.Distribusjonstype.ToUpper().Equals("S"))
            {
                deliveryDates = (from d in datesInPeriod
                                 where !d.IsHoliday & (d.IsEarlyWeekFirstDay | d.IsEarlyWeekSecondDay)
                                 orderby d.Dato ascending
                                 select d).ToList();
            }
            else // distribusjonType=B
            {
                deliveryDates = (from d in datesInPeriod
                                 where !d.IsHoliday & (d.IsMidWeekFirstDay | d.IsMidWeekSecondDay)
                                 orderby d.Dato ascending
                                 select d).ToList();
            }



            //Fetch date from tables which has less capacicty for any route

            List<DateTime> dateTimes = await _getPrsCalendarAdminDetailsRepository.GetDateWiseBookedCapacity(request.StartDato, request.SluttDato.AddDays(6d), request.Id, request.Type, "HH", request.Vekt, request.Thickness);

            var response = new ResponseGetPrsAdminData();
            response.Kapasitet = new List<RestCapacity>();
            List<int> WeekNos = new List<int>();
            foreach (GetPrsAdminCalendarData kDato in deliveryDates)
            {
                var rc = new RestCapacity();
                response.Kapasitet.Add(rc);
                rc.Dato = kDato.Dato;
                rc.U = kDato.WeekNo;
                rc.DD = true;
                rc.VD = true;
                rc.K = true;
                rc.IsSelectable = true;
                rc.MK = new List<List<long>>();
                if (WeekNos.Contains(kDato.WeekNo))
                {
                    rc.IsFullyBokked = true;
                }
                bool isFullyBooked = dateTimes?.Where(x => x.Date == kDato.Dato.Date)?.Any() == true;
                if (isFullyBooked)
                {
                    rc.IsFullyBokked = dateTimes?.Where(x => x.Date == kDato.Dato.Date)?.Any() == true;
                    response.Kapasitet.Where(x => x.U == kDato.WeekNo).ToList().ForEach(x => x.IsFullyBokked = true);
                    WeekNos.Add(kDato.WeekNo);
                }

                // rc.MK.Add(New List(Of Long)(New Long() {11111111, 0, 0}))
            }



            return response;

        }
    }
}
