using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Team
{
    /// <summary>
    /// ResponseGetAllTeamKommuneKeys
    /// </summary>
    public class ResponseGetAllTeamKommuneKeys
    {
        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneId { get; set; }
        /// <summary>
        /// Gets or sets the team identifier.
        /// </summary>
        /// <value>
        /// The team identifier.
        /// </value>
        public string TeamId { get; set; }
       
    }
}
