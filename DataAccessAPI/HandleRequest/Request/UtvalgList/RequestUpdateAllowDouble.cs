using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestUpdateAllowDouble
    /// </summary>
    public class RequestUpdateAllowDouble : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public long ListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow double].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow double]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDouble { get; set; }
    }
}
