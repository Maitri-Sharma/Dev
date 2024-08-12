using MediatR;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    /// <summary>
    /// RequestUpdateListLogo
    /// </summary>
    public class RequestUpdateListLogo : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        public long ListId { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string Logo { get; set; }

        /// <summary>
        /// Gets or sets the request update utvagl logos.
        /// </summary>
        /// <value>
        /// The request update utvagl logos.
        /// </value>
        public List<RequestUpdateUtvaglLogo> requestUpdateUtvaglLogos { get; set; }

        public List<RequestUpdateListLogo> MemberList { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string UserName { get; set; }
    }

    /// <summary>
    /// RequestUpdateUtvaglLogo
    /// </summary>
    public class RequestUpdateUtvaglLogo
    {
        /// <summary>
        /// Gets or sets the utvalg identifier.
        /// </summary>
        /// <value>
        /// The utvalg identifier.
        /// </value>
        public long UtvalgId { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string Logo { get; set; }
    }
}
