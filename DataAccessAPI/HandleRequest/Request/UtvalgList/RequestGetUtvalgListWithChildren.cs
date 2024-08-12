using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestGetUtvalgListWithChildren
    /// </summary>
    public class RequestGetUtvalgListWithChildren : IRequest<ResponseGetUtvalgListWithChildren>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public int listId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [get parent list member utvalg].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get parent list member utvalg]; otherwise, <c>false</c>.
        /// </value>
        public bool getParentListMemberUtvalg { get; set; } = false;
    }
}
