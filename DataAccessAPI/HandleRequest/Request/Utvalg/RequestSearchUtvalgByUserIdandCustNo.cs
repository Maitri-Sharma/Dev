using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    /// <summary>
    /// RequestSearchUtvalgByUserIdandCustNo
    /// </summary>
    public class RequestSearchUtvalgByUserIdandCustNo : IRequest<List<ResponseGetUtlvagDetailByUserIdAndCustNo>>
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string userID { get; set; }
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
    }
}
