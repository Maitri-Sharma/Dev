using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// Request class to check Utlvag exists
    /// </summary>
    public class RequestIsUtvalgSkrivebeskyttetInDB : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public int utvalgId { get; set; }
    }
}
