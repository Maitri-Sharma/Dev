using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.ReolerKommune
{
    /// <summary>
    /// ResponseGetReolerKommune
    /// </summary>
    public class ResponseGetReolerKommune
    {
        /// <summary>
        /// Gets or sets the reol identifier.
        /// </summary>
        /// <value>
        /// The reol identifier.
        /// </value>
        public long ReolId { get; set; }


        /// <summary>
        /// Gets or sets the kommune identifier.
        /// </summary>
        /// <value>
        /// The kommune identifier.
        /// </value>
        public string KommuneId { get; set; }



        /// <summary>
        /// Gets or sets the hh.
        /// </summary>
        /// <value>
        /// The hh.
        /// </value>
        public int HH { get; set; }



        /// <summary>
        /// Gets or sets the er.
        /// </summary>
        /// <value>
        /// The er.
        /// </value>
        public int ER { get; set; }



        /// <summary>
        /// Gets or sets the gb.
        /// </summary>
        /// <value>
        /// The gb.
        /// </value>
        public int GB { get; set; }



        /// <summary>
        /// Gets or sets the vh.
        /// </summary>
        /// <value>
        /// The vh.
        /// </value>
        public int VH { get; set; }



        /// <summary>
        /// Gets or sets the hh resource.
        /// </summary>
        /// <value>
        /// The hh resource.
        /// </value>
        public int HH_RES { get; set; }


        /// <summary>
        /// Gets or sets the er resource.
        /// </summary>
        /// <value>
        /// The er resource.
        /// </value>
        public int ER_RES { get; set; }


        /// <summary>
        /// Gets or sets the gb resource.
        /// </summary>
        /// <value>
        /// The gb resource.
        /// </value>
        public int GB_RES { get; set; }
       
    }
}
