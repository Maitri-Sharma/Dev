using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    /// <summary>
    /// Interface for app setting repository
    /// </summary>
    public interface IAppSettingRepository
    {

        /// <summary>
        /// Get app setting value by appsetting key
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        Task<string> GetAppSettingValue(string appSettingKey, bool isFromAzureKeyVault = false);


        /// <summary>
        /// Get Connectionstring
        /// </summary>
        /// <returns></returns>
        Task<string> GetConnectionString();
    }
}
