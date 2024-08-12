using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListSimpleByIDAndCustomerNo :  IRequest<List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo>>
    {
        /// <summary>
        /// Gets or sets the utvalglistname.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string utvalglistname { get; set; }
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
        /// Gets or sets a value indicating whether [extendedInfo].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [extendedInfo]; otherwise, <c>false</c>.
        /// </value>
        public bool extendedInfo { get; set; }

        /// <summary>
        /// Gets or sets the only basis utvalg.
        /// </summary>
        /// <value>
        /// The only basis utvalg.
        /// </value>
        public int onlyBasisLists { get; set; }

        /// <summary>
        /// Gets or sets the includeChildrenUtvalg
        /// </summary>
        /// <value>
        /// The only basis utvalg.
        /// </value>
        public bool includeChildrenUtvalg { get; set; }
    }
}
