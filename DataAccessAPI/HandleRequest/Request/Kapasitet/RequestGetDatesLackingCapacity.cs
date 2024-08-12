using DataAccessAPI.HandleRequest.Response.Kapasitet;
using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestGetDatesLackingCapacity
    /// </summary>
    public class RequestGetDatesLackingCapacity : IRequest<List<ResponseGetDatesLackingCapacity>>
    {
        /// <summary>
        /// Gets or sets from date.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public DateTime fromDate { get; set; }
        /// <summary>
        /// Gets or sets to date.
        /// </summary>
        /// <value>
        /// To date.
        /// </value>
        public DateTime toDate { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long id { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string type { get; set; }
        /// <summary>
        /// Gets or sets the type of the receiver.
        /// </summary>
        /// <value>
        /// The type of the receiver.
        /// </value>
        public string receiverType { get; set; }
        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public int weight { get; set; }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double thickness { get; set; } = 0.0;
    }
}
