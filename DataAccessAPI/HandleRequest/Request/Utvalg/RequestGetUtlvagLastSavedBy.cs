using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestGetUtlvagLastSavedBy
    /// </summary>
    public class RequestGetUtlvagLastSavedBy : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public int UtvalgId { get; set; }
    }
}
