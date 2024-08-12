using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.DatabaseModel
{
    [Table("addresspointsstate")]
    [Keyless]
    public class AddressPointsState
    {
        [Column("name1")]
        public string Name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public string userid { get; set; }
        public DateTime timecreated { get; set; }
    }
}
