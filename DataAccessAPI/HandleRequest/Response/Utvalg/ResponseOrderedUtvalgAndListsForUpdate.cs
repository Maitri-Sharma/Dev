using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Utvalg
{
    /// <summary>
    /// ResponseOrderedUtvalgAndListsForUpdate
    /// </summary>
    public class ResponseOrderedUtvalgAndListsForUpdate
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is list; otherwise, <c>false</c>.
        /// </value>
        public bool IsList { get; set; }
        /// <summary>
        /// Gets or sets the ordrereferanse.
        /// </summary>
        /// <value>
        /// The ordrereferanse.
        /// </value>
        public string Ordrereferanse { get; set; }
    }
}
