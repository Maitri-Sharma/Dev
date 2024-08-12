using MediatR;
using System;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Kapasitet
{
    /// <summary>
    /// RequestSubtractRestkapasitetAbsoluttDag
    /// </summary>
    public class RequestSubtractRestkapasitetAbsoluttDag : IRequest<bool>
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
        /// Gets or sets the type of the mottaker.
        /// </summary>
        /// <value>
        /// The type of the mottaker.
        /// </value>
        public string mottakerType { get; set; }

        public double restthickness { get; set; }
    }
}
