using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgKommune
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
    }

}
