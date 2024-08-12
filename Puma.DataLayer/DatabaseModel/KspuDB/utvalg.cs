using Microsoft.EntityFrameworkCore;
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
    /// Model for utvalg
    /// </summary>
    [Table("utvalg")]
    public class utvalg
    {
        /// <summary>
        /// Gets or sets the utvalgid.
        /// </summary>
        /// <value>
        /// The utvalgid.
        /// </value>
        [Key]
		public decimal utvalgid { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string logo { get; set; }
        /// <summary>
        /// Gets or sets the reolmapname.
        /// </summary>
        /// <value>
        /// The reolmapname.
        /// </value>
        public string reolmapname { get; set; }
        /// <summary>
        /// Gets or sets the utvalglistid.
        /// </summary>
        /// <value>
        /// The utvalglistid.
        /// </value>
        public decimal utvalglistid { get; set; }
        /// <summary>
        /// Gets or sets the antall.
        /// </summary>
        /// <value>
        /// The antall.
        /// </value>
        public decimal antall { get; set; }
        /// <summary>
        /// Gets or sets the ordretype.
        /// </summary>
        /// <value>
        /// The ordretype.
        /// </value>
        public string ordretype { get; set; }
        /// <summary>
        /// Gets or sets the ordrereferanse.
        /// </summary>
        /// <value>
        /// The ordrereferanse.
        /// </value>
        public string ordrereferanse { get; set; }
        /// <summary>
        /// Gets or sets the ordrestatus.
        /// </summary>
        /// <value>
        /// The ordrestatus.
        /// </value>
        public string ordrestatus { get; set; }
        /// <summary>
        /// Gets or sets the kundenummer.
        /// </summary>
        /// <value>
        /// The kundenummer.
        /// </value>
        public string kundenummer { get; set; }
        /// <summary>
        /// Gets or sets the innleveringsdato.
        /// </summary>
        /// <value>
        /// The innleveringsdato.
        /// </value>
        public DateTime? innleveringsdato { get; set; }
        /// <summary>
        /// Gets or sets the oldreolmapname.
        /// </summary>
        /// <value>
        /// The oldreolmapname.
        /// </value>
        public string oldreolmapname { get; set; }
        /// <summary>
        /// Gets or sets the skrivebeskyttet.
        /// </summary>
        /// <value>
        /// The skrivebeskyttet.
        /// </value>
        public short skrivebeskyttet { get; set; }
        /// <summary>
        /// Gets or sets the avtalenummer.
        /// </summary>
        /// <value>
        /// The avtalenummer.
        /// </value>
        public long? avtalenummer { get; set; }
        /// <summary>
        /// Gets or sets the arealavvik.
        /// </summary>
        /// <value>
        /// The arealavvik.
        /// </value>
        public decimal arealavvik { get; set; }
        /// <summary>
        /// Gets or sets the isbasis.
        /// </summary>
        /// <value>
        /// The isbasis.
        /// </value>
        public short isbasis { get; set; }
        /// <summary>
        /// Gets or sets the basedon.
        /// </summary>
        /// <value>
        /// The basedon.
        /// </value>
        public decimal basedon { get; set; }
        /// <summary>
        /// Gets or sets the wasbasedon.
        /// </summary>
        /// <value>
        /// The wasbasedon.
        /// </value>
        public decimal wasbasedon { get; set; }
        /// <summary>
        /// Gets or sets the vekt.
        /// </summary>
        /// <value>
        /// The vekt.
        /// </value>
        public int vekt { get; set; }
        /// <summary>
        /// Gets or sets the distribusjonsdato.
        /// </summary>
        /// <value>
        /// The distribusjonsdato.
        /// </value>
        public DateTime? distribusjonsdato { get; set; }
        /// <summary>
        /// Gets or sets the distribusjonstype.
        /// </summary>
        /// <value>
        /// The distribusjonstype.
        /// </value>
        public string distribusjonstype { get; set; }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double thickness { get; set; }

	}
}
