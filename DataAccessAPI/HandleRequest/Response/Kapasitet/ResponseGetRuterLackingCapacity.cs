using System;


namespace DataAccessAPI.HandleRequest.Response.Kapasitet
{
    /// <summary>
    /// ResponseGetRuterLackingCapacity
    /// </summary>
    public class ResponseGetRuterLackingCapacity
    {
        /// <summary>
        /// Gets or sets the dato.
        /// </summary>
        /// <value>
        /// The dato.
        /// </value>
        public DateTime Dato { get; set; }


        /// <summary>
        /// Gets or sets the rute nr.
        /// </summary>
        /// <value>
        /// The rute nr.
        /// </value>
        public long RuteNr { get; set; }


        /// <summary>
        /// Gets or sets the rest vekt.
        /// </summary>
        /// <value>
        /// The rest vekt.
        /// </value>
        public int RestVekt { get; set; }

        /// <summary>
        /// Gets or sets the rest antall.
        /// </summary>
        /// <value>
        /// The rest antall.
        /// </value>
        public int RestAntall { get; set; }


       
        /// <summary>
        /// Gets or sets the type of the mottaker.
        /// </summary>
        /// <value>
        /// The type of the mottaker.
        /// </value>
        public string MottakerType { get; set; }

        /// <summary>
        /// Gets or sets the rest thickness.
        /// </summary>
        /// <value>
        /// The rest thickness.
        /// </value>
        public double RestThickness { get; set; }

    }
}
