using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestSearchUtvalgListWithChildrenByKundeNummer
    /// </summary>
    public class RequestSearchUtvalgListWithChildrenByKundeNummer : IRequest<List<ResponseSearchUtvalgListWithChildrenByKundeNummer>>
    {
        /// <summary>
        /// Gets or sets the kunde nummer.
        /// </summary>
        /// <value>
        /// The kunde nummer.
        /// </value>
        public string kundeNummer { get; set; }
        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod searchMethod { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [include reols].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reols]; otherwise, <c>false</c>.
        /// </value>
        public bool includeReols { get; set; } = true;

       public bool onlyBasisUtvalglist { get; set;}
    }
}
