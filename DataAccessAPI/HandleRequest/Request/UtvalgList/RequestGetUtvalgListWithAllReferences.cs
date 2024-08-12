using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using static Puma.Shared.PumaEnum;
namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestGetUtvalgListWithAllReferences
    /// </summary>
    public class RequestGetUtvalgListWithAllReferences : IRequest<ResponseGetUtvalgListWithAllReferences>
    {
        /// <summary>
        /// Gets or sets the utvalglist identifier.
        /// </summary>
        /// <value>
        /// The utvalglist identifier.
        /// </value>
        public int UtvalglistId { get; set; }
    }
}
