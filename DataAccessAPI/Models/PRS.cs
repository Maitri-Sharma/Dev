using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "PRS")]
    public partial class PRS
    {
        private string PRSField;

        [DataMember(IsRequired = true, Name = "PRSNr", Order = 0)]
        public string PRSNr
        {
            get { return PRSField; }
            set { PRSField = value; }
        }
    }
}
