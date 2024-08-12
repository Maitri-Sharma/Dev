using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestGetUtvalgByUtvalgIds : IRequest<List<ResponseGetUtlvagByUtvalgId>>
    {
        public List<long> UtvalgIds { get; set; }
    }
}
