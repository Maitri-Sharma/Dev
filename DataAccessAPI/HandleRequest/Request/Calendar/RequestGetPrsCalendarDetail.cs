using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using System;

namespace DataAccessAPI.HandleRequest.Request.Calendar
{
    public class RequestGetPrsCalendarDetail : IRequest<ResponseGetPrsCalendarDetail>
    {
        public DateTime FindDate
        {
            get; set;
        }
    }
}
