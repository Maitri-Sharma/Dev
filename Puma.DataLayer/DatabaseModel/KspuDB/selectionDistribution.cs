using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.DatabaseModel.KspuDB
{
    /// <summary>
    ///selectionDistribution
    /// </summary>
    [Table("selectionDistribution")]
    public class selectionDistribution
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the selectionid.
        /// </summary>
        /// <value>
        /// The selectionid.
        /// </value>
        public long Selectionid { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is order status updated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is order status updated; otherwise, <c>false</c>.
        /// </value>
        public bool IsOrderStatusUpdated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is data posted to oebs.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is data posted to oebs; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataPostedToOEBS { get; set; }
    }
}
