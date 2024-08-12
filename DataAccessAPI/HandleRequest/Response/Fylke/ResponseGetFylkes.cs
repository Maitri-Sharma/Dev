using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Fylke
{
    /// <summary>
    /// ResponseGetFylkes
    /// </summary>
    public class ResponseGetFylkes
    {
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
    }
}
