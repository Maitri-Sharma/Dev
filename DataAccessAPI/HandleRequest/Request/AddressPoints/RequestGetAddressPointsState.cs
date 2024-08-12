using DataAccessAPI.HandleRequest.Response.AddressPoints;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.AddressPoints
{
    /// <summary>
    /// Class to request address point states
    /// </summary>
    public class RequestGetAddressPointsState : IRequest<List<ResponseAddressPointState>>
    {
        /// <summary>
        /// user Id
        /// </summary>
        public string userId { get; set; }
    }
}
