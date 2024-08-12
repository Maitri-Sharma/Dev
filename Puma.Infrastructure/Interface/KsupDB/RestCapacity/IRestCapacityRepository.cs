using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB.RestCapacity
{
    public interface IRestCapacityRepository
    {
        Task<bool> DownloadFile();
    }
}
