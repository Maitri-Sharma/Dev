using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Kommune
{
    /// <summary>
    /// ResponseGetAllKommunes
    /// </summary>
    public class ResponseGetAllKommunes
    {
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneID { get; set; }


        /// <summary>
        /// Gets or sets the name of the kommune.
        /// </summary>
        /// <value>
        /// The name of the kommune.
        /// </value>
        public string KommuneName { get; set; }



        /// <summary>
        /// Gets or sets the fylke identifier.
        /// </summary>
        /// <value>
        /// The fylke identifier.
        /// </value>
        public string FylkeID { get; set; }


        /// <summary>
        /// Gets or sets the name of the fylke.
        /// </summary>
        /// <value>
        /// The name of the fylke.
        /// </value>
        public string FylkeName { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is kommune name unique.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is kommune name unique; otherwise, <c>false</c>.
        /// </value>
        public bool IsKommuneNameUnique { get; set; }

    }
}
