using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Login
{
    /// <summary>
    /// RequestLogin
    /// </summary>
    public class RequestLogin : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Secret Key / Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// If request is from kundeweb set true
        /// </summary>
        public bool IsFromKundeWeb { get; set; }
    }
}
