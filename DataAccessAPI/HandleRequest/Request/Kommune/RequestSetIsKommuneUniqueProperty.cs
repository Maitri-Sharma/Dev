

using MediatR;
using Puma.Shared;

namespace DataAccessAPI.HandleRequest.Request.Kommune
{
    public class RequestSetIsKommuneUniqueProperty : IRequest<bool>
    {
        public KommuneCollection kommunes { get; set; }
    }
}
