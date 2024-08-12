using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puma.DataLayer.DatabaseModel.KspuDB
{
    /// <summary>
    /// Model for utvalglist
    /// </summary>
    [Table("utvalglist")]
    public class utvalglist 
    {
        /// <summary>
        /// Gets or sets the utvalglistid.
        /// </summary>
        /// <value>
        /// The utvalglistid.
        /// </value>
        [Key]
        [Column("utvalglistid")]
        public int Listid { get; set; }
        /// <summary>
        /// Gets or sets the utvalglistname.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string utvalglistname { get; set; }
        /// <summary>
        /// Gets or sets the parentutvalglistid.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public int? parentutvalglistid { get; set; }
        /// <summary>
        /// Gets or sets the antall.
        /// </summary>
        /// <value>
        /// The antall.
        /// </value>
        public int? antall { get; set; }
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
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string logo { get; set; }
        /// <summary>
        /// Gets or sets the avtalenummer.
        /// </summary>
        /// <value>
        /// The avtalenummer.
        /// </value>
        public int? avtalenummer { get; set; }
        /// <summary>
        /// Gets or sets the isbasis.
        /// </summary>
        /// <value>
        /// The isbasis.
        /// </value>
        public int? isbasis { get; set; }
        /// <summary>
        /// Gets or sets the basedon.
        /// </summary>
        /// <value>
        /// The basedon.
        /// </value>
        public int? basedon { get; set; }
        /// <summary>
        /// Gets or sets the wasbasedon.
        /// </summary>
        /// <value>
        /// The wasbasedon.
        /// </value>
        public int? wasbasedon { get; set; }
        /// <summary>
        /// Gets or sets the vekt.
        /// </summary>
        /// <value>
        /// The vekt.
        /// </value>
        public int? vekt { get; set; }
        /// <summary>
        /// Gets or sets the distributiondate.
        /// </summary>
        /// <value>
        /// The distributiondate.
        /// </value>
        [Column("distribusjonsdato")]
        public DateTime? distributiondate { get; set; }
        /// <summary>
        /// Gets or sets the distributiontype.
        /// </summary>
        /// <value>
        /// The distributiontype.
        /// </value>
        [Column("distribusjonstype")]
        public string distributiontype { get; set; }
        /// <summary>
        /// Gets or sets the allowdouble.
        /// </summary>
        /// <value>
        /// The allowdouble.
        /// </value>
        public int? allowdouble { get; set; }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double? thickness { get; set; }
       
      
    }
}
