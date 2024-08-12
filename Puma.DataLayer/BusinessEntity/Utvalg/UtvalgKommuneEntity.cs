using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
   public class UtvalgKommuneEntity
    {
        private string _KommuneId;

        public string KommuneId
        {
            get
            {
                return _KommuneId;
            }
            set
            {
                _KommuneId = value;
            }
        }


        private string _KommuneMapName;

        public string KommuneMapName
        {
            get
            {
                return _KommuneMapName;
            }
            set
            {
                _KommuneMapName = value;
            }
        }

        public int UtvalgId { get; set; }
    }
}
