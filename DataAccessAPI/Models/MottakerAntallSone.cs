using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - AntallPrSone
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "MottakerAntallSone")]
    public partial class MottakerAntallSone
    {
        private System.Int32 AntallField;

        [DataMember(IsRequired = true, Name = "Antall", Order = 2)]
        public System.Int32 Antall
        {
            get { return AntallField; }
            set { AntallField = value; }
        }

        private System.String SoneField;

        [DataMember(IsRequired = true, Name = "Sone", Order = 1)]
        public System.String Sone
        {
            get { return SoneField; }
            set { SoneField = value; }
        }

        private System.String MottakerTypeField;
        [DataMember(IsRequired = true, Name = "MottakerType", Order = 0)]
        public System.String MottakerType
        {
            get { return MottakerTypeField; }
            set { MottakerTypeField = value; }
        }

    }
}
