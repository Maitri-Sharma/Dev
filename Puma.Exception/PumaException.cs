using System.Collections.Generic;
using System.Net;

namespace Puma.CustomException
{
    /// <summary>
    /// Custom Exception Class
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class PumaException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PumaException"/> class.
        /// </summary>
        public PumaException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PumaException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PumaException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PumaException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public PumaException(string message, System.Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// The puma exception models
        /// </summary>
        public List<PumaExceptionModel> pumaExceptionModels;
        /// <summary>
        /// Initializes a new instance of the <see cref="PumaException"/> class.
        /// </summary>
        /// <param name="erros">The erros.</param>
        public PumaException(List<PumaExceptionModel> erros)

        {
            this.pumaExceptionModels = erros;
        }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode statusCode { get; set; }
    }

    /// <summary>
    /// Model class for custom exception
    /// </summary>
    public class PumaExceptionModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}
