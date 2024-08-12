using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestGetNumberOfBudruterInTeamByTeamNR
    /// </summary>
    public class RequestGetNumberOfBudruterInTeamByTeamNR : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the teamnr.
        /// </summary>
        /// <value>
        /// The teamnr.
        /// </value>
        public string Teamnr { get; set; }
    }
}
