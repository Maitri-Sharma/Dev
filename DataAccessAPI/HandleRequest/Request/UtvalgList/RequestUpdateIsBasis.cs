using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestUpdateIsBasis : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public long ListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is basis.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is basis; otherwise, <c>false</c>.
        /// </value>
        public bool IsBasis { get; set; }

        /// <summary>
        /// Gets or sets the based on.
        /// </summary>
        /// <value>
        /// The based on.
        /// </value>
        public int BasedOn { get; set; }
    }
}
