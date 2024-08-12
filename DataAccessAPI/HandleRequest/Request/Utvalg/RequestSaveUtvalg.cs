using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestSaveUtvalg : IRequest<ResponseSaveUtvalg>
    {
        public Puma.Shared.Utvalg utvalg { get; set; }

        public string userName { get; set; }

        public bool saveOldReoler { get; set; } = false;

        public bool skipHistory { get; set; } = false;

        public int forceUtvalgListId { get; set; } = 0;
    }
}
