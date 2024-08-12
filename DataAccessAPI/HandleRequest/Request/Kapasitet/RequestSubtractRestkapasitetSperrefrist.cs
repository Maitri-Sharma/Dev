using MediatR;
using System;
using System.Collections.Generic;


namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestSubtractRestkapasitetSperrefrist
    /// </summary>
    public class RequestSubtractRestkapasitetSperrefrist : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the rute ids.
        /// </summary>
        /// <value>
        /// The rute ids.
        /// </value>
        public List<long> ruteIds { get; set; }
        /// <summary>
        /// Gets or sets the restvekt.
        /// </summary>
        /// <value>
        /// The restvekt.
        /// </value>
        public int restvekt { get; set; }
        /// <summary>
        /// Gets or sets the dato.
        /// </summary>
        /// <value>
        /// The dato.
        /// </value>
        public DateTime dato { get; set; }
        /// <summary>
        /// Gets or sets the mottakertype.
        /// </summary>
        /// <value>
        /// The mottakertype.
        /// </value>
        public string mottakertype { get; set; }

        public double restthickness { get; set; }
    }
}
