using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequesSearchUtvalgByKundeNr
    /// </summary>
    public class RequesSearchUtvalgByKundeNr : IRequest<List<ResponseSearchUtvalgByKundeNr>>
    {
        /// <summary>
        /// Gets or sets the kunde nummer.
        /// </summary>
        /// <value>
        /// The kunde nummer.
        /// </value>
        public string KundeNummer { get; set; }

        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod SearchMethod { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [include reols].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reols]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeReols { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [extended information].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [extended information]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtendedInfo { get; set; } = true;

        /// <summary>
        /// Gets or sets the current reol table.
        /// </summary>
        /// <value>
        /// The current reol table.
        /// </value>
        [JsonIgnore]
        public string CurrentReolTableName { get; set; }

        public bool onlyBasisUtvalg { get; set; }
    }
}
