using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Calendar
{
    /// <summary>
    /// ResponseGetPrsAdminData
    /// </summary>
    public class ResponseGetPrsAdminData
    {
        /// <summary>
        /// Gets or sets the kapasitet.
        /// </summary>
        /// <value>
        /// The kapasitet.
        /// </value>
        public List<RestCapacity> Kapasitet { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
    }
}
