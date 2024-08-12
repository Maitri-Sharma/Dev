using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.ByDel
{
    /// <summary>
    /// ResponseGetAllBydels
    /// </summary>
    public class ResponseGetAllBydels
    {
        /// <summary>
        /// Gets or sets the bydel identifier.
        /// </summary>
        /// <value>
        /// The bydel identifier.
        /// </value>
        public string BydelID { get; set; }


        /// <summary>
        /// Gets or sets the by delete.
        /// </summary>
        /// <value>
        /// The by delete.
        /// </value>
        public string By_del { get; set; }


        /// <summary>
        /// Gets or sets the by.
        /// </summary>
        /// <value>
        /// The by.
        /// </value>
        public string By { get; set; }

    }
}
