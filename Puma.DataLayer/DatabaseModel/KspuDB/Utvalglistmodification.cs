using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puma.DataLayer.DatabaseModel.KspuDB
{
    /// <summary>
    /// UtvalgListModification
    /// </summary>
    [Table("utvalglistmodification")]

    public class UtvalgListModification
    {
        /// <summary>
        /// Gets or sets the utvalg list modification identifier.
        /// </summary>
        /// <value>
        /// The utvalg list modification identifier.
        /// </value>
        [Key]
        [Column("utvalglistmodificationid")]
        public int UtvalgListModificationId { get; set; }

        /// <summary>
        /// Gets or sets the utvalg list identifier.
        /// </summary>
        /// <value>
        /// The utvalg list identifier.
        /// </value>
        [Column("utvalglistid")]
        public int UtvalgListId { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        [Column("modificationdate")]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Column("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        [Column("info")]
        public string Info { get; set; }


    }
}
