using DataAccessAPI.HandleRequest.Request.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataAccessAPI.Controllers
{
    /// <summary>
    /// LoginController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// mediator
        /// </exception>
        public LoginController( IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Provides token based on provided Client Details.
        /// </summary>
        /// <param name="requestLogin">Login Credentials</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [Route("gettoken")]
        public IActionResult Login([FromBody] RequestLogin requestLogin)
        {
            return Ok(_mediator.Send(requestLogin).Result);
        }
    }
}
