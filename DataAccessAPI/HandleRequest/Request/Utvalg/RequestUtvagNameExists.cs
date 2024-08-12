using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestUtvagNameExists
    /// </summary>
    public class RequestUtvagNameExists : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the name of the utvalg.
        /// </summary>
        /// <value>
        /// The name of the utvalg.
        /// </value>
        public string UtvalgName { get; set; }
    }
}
