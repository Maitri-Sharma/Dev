using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestGetUtvalgList
    /// </summary>
    public class RequestGetUtvalgList : IRequest<ResponseGetUtvalgList>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public int listId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [get parent list].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get parent list]; otherwise, <c>false</c>.
        /// </value>
        public bool getParentList { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether [get member utvalg].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get member utvalg]; otherwise, <c>false</c>.
        /// </value>
        public bool getMemberUtvalg { get; set; } = false;
    }
}
