using Puma.Shared;

namespace DataAccessAPI.HandleRequest.Response.Reol
{
    /// <summary>
    /// ResponseGetReolerByFylkeSearch
    /// </summary>
    public class ResponseGetReolerByFylkeSearch
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reol number.
        /// </summary>
        /// <value>
        /// The reol number.
        /// </value>
        public string ReolNumber { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the name of the descriptive.
        /// </summary>
        /// <value>
        /// The name of the descriptive.
        /// </value>
        public string DescriptiveName { get; set; }



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
        /// Gets or sets the kommune.
        /// </summary>
        /// <value>
        /// The kommune.
        /// </value>
        public string Kommune { get; set; }



        /// <summary>
        /// Gets or sets the kommune full distribusjon.
        /// </summary>
        /// <value>
        /// The kommune full distribusjon.
        /// </value>
        public string KommuneFullDistribusjon { get; set; }



        /// <summary>
        /// Gets or sets the fylke identifier.
        /// </summary>
        /// <value>
        /// The fylke identifier.
        /// </value>
        public string FylkeId { get; set; }


        /// <summary>
        /// Gets or sets the fylke.
        /// </summary>
        /// <value>
        /// The fylke.
        /// </value>
        public string Fylke { get; set; }



        /// <summary>
        /// Gets or sets the team number.
        /// </summary>
        /// <value>
        /// The team number.
        /// </value>
        public string TeamNumber { get; set; }



        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        /// <value>
        /// The name of the team.
        /// </value>
        public string TeamName { get; set; }



        /// <summary>
        /// Gets or sets the postal zone.
        /// </summary>
        /// <value>
        /// The postal zone.
        /// </value>
        public string PostalZone { get; set; }



        /// <summary>
        /// Gets or sets the postal area.
        /// </summary>
        /// <value>
        /// The postal area.
        /// </value>
        public string PostalArea { get; set; }



        /// <summary>
        /// Gets or sets the segment identifier.
        /// </summary>
        /// <value>
        /// The segment identifier.
        /// </value>
        public string SegmentId { get; set; }



        /// <summary>
        /// Gets or sets the antall.
        /// </summary>
        /// <value>
        /// The antall.
        /// </value>
        public AntallInformation Antall { get; set; }

        
        /// <summary>
        /// Gets or sets the avis deknings.
        /// </summary>
        /// <value>
        /// The avis deknings.
        /// </value>
        public AvisDekningCollection AvisDeknings { get; set; }



        /// <summary>
        /// Gets or sets the pris sone.
        /// </summary>
        /// <value>
        /// The pris sone.
        /// </value>
        public int PrisSone { get; set; }



        /// <summary>
        /// Gets or sets the type of the rute.
        /// </summary>
        /// <value>
        /// The type of the rute.
        /// </value>
        public string RuteType { get; set; }



        /// <summary>
        /// Gets or sets the postkontor navn.
        /// </summary>
        /// <value>
        /// The postkontor navn.
        /// </value>
        public string PostkontorNavn { get; set; }



        /// <summary>
        /// Gets or sets the PRS enhets identifier.
        /// </summary>
        /// <value>
        /// The PRS enhets identifier.
        /// </value>
        public string PrsEnhetsId { get; set; }



        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        public string PrsName { get; set; }



        /// <summary>
        /// Gets or sets the PRS description.
        /// </summary>
        /// <value>
        /// The PRS description.
        /// </value>
        public string PrsDescription { get; set; }
        // Added for RDF

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public string Frequency { get; set; }


        /// <summary>
        /// Gets or sets the sondag flag.
        /// </summary>
        /// <value>
        /// The sondag flag.
        /// </value>
        public string SondagFlag { get; set; }

    }
}
