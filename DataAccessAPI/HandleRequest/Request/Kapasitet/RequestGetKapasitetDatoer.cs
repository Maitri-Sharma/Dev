

using DataAccessAPI.HandleRequest.Response.Kapasitet;
using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestGetKapasitetDatoer
    /// </summary>
    public class RequestGetKapasitetDatoer : IRequest<List<ResponseGetKapasitetDatoer>>
    {
        /// <summary>
        /// Gets or sets from date.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Converts to date.
        /// </summary>
        /// <value>
        /// To date.
        /// </value>
        public DateTime ToDate { get; set; }
    }
}
