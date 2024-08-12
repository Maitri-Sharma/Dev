using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgDistrict
    {
        private string _DistrictId;
        public string DistrictId
        {
            get
            {
                return _DistrictId;
            }
            set
            {
                _DistrictId = value;
            }
        }

        private string _DistrictMapName;
        public string DistrictMapName
        {
            get
            {
                return _DistrictMapName;
            }
            set
            {
                _DistrictMapName = value;
            }
        }
    }
}
