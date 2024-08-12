using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using System;

namespace DataAccessAPI.HandleRequest.Request.Calendar
{
    /// <summary>
    /// RequestGetPrsAdminData
    /// </summary>
    public class RequestGetPrsAdminData : IRequest<ResponseGetPrsAdminData>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the vekt.
        /// </summary>
        /// <value>
        /// The vekt.
        /// </value>
        public int Vekt
        {
            get; set;
        }


        /// <summary>
        /// Gets or sets the distribusjonstype.
        /// </summary>
        /// <value>
        /// The distribusjonstype.
        /// </value>
        public string Distribusjonstype
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start dato.
        /// </summary>
        /// <value>
        /// The start dato.
        /// </value>
        public DateTime StartDato
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the slutt dato.
        /// </summary>
        /// <value>
        /// The slutt dato.
        /// </value>
        public DateTime SluttDato
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double Thickness
        {
            get; set;
        }
    }
}
