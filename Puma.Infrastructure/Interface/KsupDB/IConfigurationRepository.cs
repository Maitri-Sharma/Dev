using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IConfigurationRepository 
    {
        Task<string> GetConfigValue(string configKey);

        Task SetConfigValue(string configKey, string configValue);
    }
}
