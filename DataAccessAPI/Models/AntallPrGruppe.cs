using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - Mottakertype
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "MottakerAntall")]
    public partial class MottakerAntall
    {
        private System.String MottakerTypeField;

        [DataMember(IsRequired = true, Name = "MottakerType", Order = 0)]
        public System.String MottakerType
        {
            get { return MottakerTypeField; }
            set { MottakerTypeField = value; }
        }

        private System.Int32 AntallField;

        [DataMember(IsRequired = true, Name = "Antall", Order = 1)]
        public System.Int32 Antall
        {
            get { return AntallField; }
            set { AntallField = value; }
        }

        private System.Int32 ReserverteField;

        [DataMember(IsRequired = false, Name = "Reserverte", Order = 2)]
        public System.Int32 Reserverte
        {
            get { return ReserverteField; }
            set { ReserverteField = value; }
        }
    }
}
