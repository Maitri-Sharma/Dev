using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class UtvalgPostalZoneEntity
    {
        private int _PostalZone;
        public int PostalZone
        {
            get
            {
                return _PostalZone;
            }
            set
            {
                _PostalZone = value;
            }
        }

        private string _PostalZoneMapName;
        public string PostalZoneMapName
        {
            get
            {
                return _PostalZoneMapName;
            }
            set
            {
                _PostalZoneMapName = value;
            }
        }

        public int UtvalgId { get; set; }
    }
}
