using System;

namespace DataAccessAPI.HandleRequest.Response.Kapasitet
{
    /// <summary>
    /// ResponseGetDatesLackingCapacity
    /// </summary>
    public class ResponseGetDatesLackingCapacity
    {
        /// <summary>
        /// Gets or sets the dato.
        /// </summary>
        /// <value>
        /// The dato.
        /// </value>
        public DateTime Dato { get; set; }
        /// <summary>
        /// Gets or sets the households lacking capacity.
        /// </summary>
        /// <value>
        /// The households lacking capacity.
        /// </value>
        public long HouseholdsLackingCapacity { get; set; }
    }
}
