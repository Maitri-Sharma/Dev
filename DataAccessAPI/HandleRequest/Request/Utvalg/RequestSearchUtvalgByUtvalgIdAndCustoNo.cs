using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestSearchUtvalgByUtvalgIdAndCustoNo : IRequest<List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo>>
    {
        /// <summary>
        /// Gets or sets the utvag identifier.
        /// </summary>
        /// <value>
        /// The utvag identifier.
        /// </value>
        public int UtvagId { get; set; }

        /// <summary>
        /// Gets or sets the name of the curretn reol table.
        /// </summary>
        /// <value>
        /// The name of the curretn reol table.
        /// </value>
        
        [JsonIgnore]
        public string CurretnReolTableName { get; set; }

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
        /// Gets or sets a value indicating whether [include reol].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include reol]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeReol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [extend information].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [extend information]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtendInfo { get; set; }
    }
}
