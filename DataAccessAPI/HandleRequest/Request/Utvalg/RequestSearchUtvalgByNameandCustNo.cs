using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestSearchUtvalgByNameandCustNo
    /// </summary>
    public class RequestSearchUtvalgByNameandCustNo : IRequest<List<ResponseGetUtlvagDetailByNameAndCustNo>>
    {

        /// <summary>
        /// Gets or sets the name of the utvalg.
        /// </summary>
        /// <value>
        /// The name of the utvalg.
        /// </value>
        public string UtvalgName { get; set; }
        /// <summary>
        /// Gets or sets the customer nos.
        /// </summary>
        /// <value>
        /// The customer nos.
        /// </value>
        public string[] customerNos { get; set; }
        /// <summary>
        /// Gets or sets the agreement nos.
        /// </summary>
        /// <value>
        /// The agreement nos.
        /// </value>
        public int[] agreementNos { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [force customer and agreement check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force customer and agreement check]; otherwise, <c>false</c>.
        /// </value>
        public bool forceCustomerAndAgreementCheck { get; set; }
        /// <summary>
        /// Gets or sets the only basis utvalg.
        /// </summary>
        /// <value>
        /// The only basis utvalg.
        /// </value>
        public int onlyBasisUtvalg { get; set; }

        /// <summary>
        /// Gets or sets the current reol table bame.
        /// </summary>
        /// <value>
        /// The current reol table bame.
        /// </value>
        [JsonIgnore]
        public string currentReolTableBame { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [extended information].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [extended information]; otherwise, <c>false</c>.
        /// </value>
        public bool extendedInfo { get; set; }
    }
}
