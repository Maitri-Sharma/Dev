using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestGetSperrefristDates
    /// </summary>
    public class RequestGetSperrefristDates : IRequest<List<DateTime>>
    {
        /// <summary>
        /// Gets or sets the dato.
        /// </summary>
        /// <value>
        /// The dato.
        /// </value>
        public DateTime dato { get; set; }
        /// <summary>
        /// Gets or sets the day count.
        /// </summary>
        /// <value>
        /// The day count.
        /// </value>
        public int dayCount{ get; set; }
    }
}
