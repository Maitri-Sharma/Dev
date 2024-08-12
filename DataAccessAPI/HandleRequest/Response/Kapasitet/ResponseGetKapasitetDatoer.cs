using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Kapasitet
{
    /// <summary>
    /// ResponseGetKapasitetDatoer
    /// </summary>
    public class ResponseGetKapasitetDatoer
    {
        /// <summary>
        /// Gets or sets the dato.
        /// </summary>
        /// <value>
        /// The dato.
        /// </value>
        public DateTime Dato { get; set; }


        /// <summary>
        /// Gets or sets the uke nr.
        /// </summary>
        /// <value>
        /// The uke nr.
        /// </value>
        public int UkeNr { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResponseGetKapasitetDatoer"/> is distribusjonsdag.
        /// </summary>
        /// <value>
        ///   <c>true</c> if distribusjonsdag; otherwise, <c>false</c>.
        /// </value>
        public bool Distribusjonsdag { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResponseGetKapasitetDatoer"/> is virkedag.
        /// </summary>
        /// <value>
        ///   <c>true</c> if virkedag; otherwise, <c>false</c>.
        /// </value>
        public bool Virkedag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is early week first day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is early week first day; otherwise, <c>false</c>.
        /// </value>
        public bool IsEarlyWeekFirstDay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is early week second day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is early week second day; otherwise, <c>false</c>.
        /// </value>
        public bool IsEarlyWeekSecondDay { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is mid week first day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mid week first day; otherwise, <c>false</c>.
        /// </value>
        public bool IsMidWeekFirstDay { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is mid week second day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mid week second day; otherwise, <c>false</c>.
        /// </value>
        public bool IsMidWeekSecondDay { get; set; }
       
    }
}
