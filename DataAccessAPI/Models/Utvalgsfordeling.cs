using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - UtvalgDetaljer
    /// </summary>
    [XmlRoot(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", ElementName = "Utvalgsfordeling")]
    public partial class Utvalgsfordeling : UtvalgsId
    {
        private System.String OEBSTypeField;

        [DataMember(IsRequired = true, Name = "OEBSType", Order = 2)]
        public System.String OEBSType
        {
            get { return OEBSTypeField; }
            set { OEBSTypeField = value; }
        }

        private System.String OEBSRefField;

        [DataMember(IsRequired = true, Name = "OEBSRef", Order = 3)]
        public System.String OEBSRef
        {
            get { return OEBSRefField; }
            set { OEBSRefField = value; }
        }

        private System.String UtvalgsRefField;

        [DataMember(IsRequired = true, Name = "UtvalgsRef", Order = 4)]
        public System.String UtvalgsRef
        {
            get { return UtvalgsRefField; }
            set { UtvalgsRefField = value; }
        }

        private System.DateTime DatoOppdatertField;

        [DataMember(IsRequired = true, Name = "DatoOppdatert", Order = 6)]
        public System.DateTime DatoOppdatert
        {
            get { return DatoOppdatertField; }
            set { DatoOppdatertField = value; }
        }

        private Helper.Utvalgsdetaljer FordelingerField;

        [DataMember(IsRequired = false, Name = "Utvalgsdetaljer", Order = 8)]
        public Helper.Utvalgsdetaljer Utvalgsdetaljer
        {
            get { return FordelingerField; }
            set { FordelingerField = value; }
        }

        private System.String TopplisteLogoField;

        [DataMember(IsRequired = false, Name = "TopplisteLogo", Order = 10)]
        public System.String TopplisteLogo
        {
            get { return TopplisteLogoField; }
            set { TopplisteLogoField = value; }
        }

        private System.String ReasonField;

        [DataMember(IsRequired = false, Name = "Reason", Order = 12)]
        public System.String Reason
        {
            get { return ReasonField; }
            set { ReasonField = value; }
        }

    }
}
