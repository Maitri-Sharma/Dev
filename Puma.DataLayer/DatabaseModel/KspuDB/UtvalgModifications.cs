using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Puma.DataLayer.DatabaseModel.KspuDB
{
    [Table("utvalgmodification")]

    public class UtvalgModifications
    {
        [Key]
        [Column("utvalgmodificationid")]
        public decimal UtvalgModificationId { get; set; }

        [Column("utvalgid")]
        public decimal UtvalgId { get; set; }

        [Column("modificationdate")]
        public DateTime ModificationDate { get; set; }

        [Column("userid")]
        public string UserIds { get; set; }

        [Column("info")]
        public string info { get; set; }

    }
}
