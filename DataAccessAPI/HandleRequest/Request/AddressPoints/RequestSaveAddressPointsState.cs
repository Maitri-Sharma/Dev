using MediatR;
using Puma.Shared;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.AddressPoints
{
    public class RequestSaveAddressPointsState : IRequest<int>
    {

        public string userId { get; set; }
        public AddressPointList addressPointList { get; set; }
    }
}
