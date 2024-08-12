using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.Logger
{
    public interface ILoggerRepository : IKsupDBGenericRepository<utvalglist>
    {
        /// <summary>
        /// Add debug logs to DB
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="callerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task LogDebug(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string fileName = "");

        /// <summary>
        /// Add information to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task LogInformation(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "");

        /// <summary>
        /// Add warning to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task LogWarning(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "");

        /// <summary>
        /// Add Error to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task LogError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "");

        /// <summary>
        /// Add exception with DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task LogError(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "");

        /// <summary>
        /// Delete logs from database
        /// </summary>
        /// <returns></returns>
        Task DeleteLogs();
    }
}
