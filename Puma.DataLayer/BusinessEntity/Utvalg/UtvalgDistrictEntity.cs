using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class UtvalgDistrictEntity
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

        public long UtvalgId { get; set; }
    }
}
