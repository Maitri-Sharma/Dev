using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestIsOrderedUtvalgAndListsReadyForUpdate
    /// </summary>
    public class RequestIsOrderedUtvalgAndListsReadyForUpdate : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the delivery date.
        /// </summary>
        /// <value>
        /// The delivery date.
        /// </value>
        public DateTime DeliveryDate { get; set; }
    }
}
