using System.Collections.Generic;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestSearchUtvalgListByIsBasis
    /// </summary>
    public class RequestSearchUtvalgListByIsBasis : IRequest<List<ResponseSearchUtvalgListSimple>>
    {
        /// <summary>
        /// Gets or sets the utvalglistname.
        /// </summary>
        /// <value>
        /// The utvalglistname.
        /// </value>
        public string utvalglistname { get; set; }
        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>
        /// The search method.
        /// </value>
        public SearchMethod searchMethod { get; set; }

        /// <summary>
        /// Gets or sets the only basis utvalg.
        /// </summary>
        /// <value>
        /// The only basis utvalg.
        /// </value>
        public int onlyBasisLists { get; set; }

        /// <summary>
        /// Set false if do not want to return camp data in list else set true it will return all data
        /// </summary>
        public bool isBasedOn { get; set;  }
    }
}
