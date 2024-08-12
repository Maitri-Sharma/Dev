using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.EC_Data
{
    /// <summary>
    /// OrderStatusResponseEntity
    /// </summary>
    public class OrderStatusResponseEntity
    {
        /// <summary>
        /// Gets or sets the corrid.
        /// </summary>
        /// <value>
        /// The corrid.
        /// </value>
        public string CORRID { get; set; }

        /// <summary>
        /// Gets or sets the MSG.
        /// </summary>
        /// <value>
        /// The MSG.
        /// </value>
        public string MSG { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string SOURCE { get; set; }
    }
}
