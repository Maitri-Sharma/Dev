using DataAccessAPI.HandleRequest.Response.Kapasitet;
using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestGetRuterLackingCapacity
    /// </summary>
    public class RequestGetRuterLackingCapacity : IRequest<List<ResponseGetRuterLackingCapacity>>
    {
        /// <summary>
        /// Gets or sets the dates.
        /// </summary>
        /// <value>
        /// The dates.
        /// </value>
        public List<string> dates { get; set; }
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
