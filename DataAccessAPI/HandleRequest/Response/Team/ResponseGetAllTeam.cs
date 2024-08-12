using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Team
{
    /// <summary>
    /// ResponseGetAllTeam
    /// </summary>
    public class ResponseGetAllTeam
    {
        /// <summary>
        /// Gets or sets the team nr.
        /// </summary>
        /// <value>
        /// The team nr.
        /// </value>
        public string TeamNr { get; set; }


        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        /// <value>
        /// The name of the team.
        /// </value>
        public string TeamName { get; set; }
    }
}
