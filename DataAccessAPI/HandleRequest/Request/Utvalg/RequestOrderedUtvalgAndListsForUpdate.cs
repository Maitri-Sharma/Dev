using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestOrderedUtvalgAndListsForUpdate
    /// </summary>
    public class RequestOrderedUtvalgAndListsForUpdate : IRequest<List<ResponseOrderedUtvalgAndListsForUpdate>>
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
