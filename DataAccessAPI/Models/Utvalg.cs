using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
    public partial class Utvalg : UtvalgBase, IExtensibleDataObject
    {
        [NonSerialized()]
        private ExtensionDataObject extensionDataField;
        public ExtensionDataObject ExtensionData
        {
            get { return extensionDataField; }
            set { extensionDataField = value; }
        }

        private int AntallField;

        [DataMember(IsRequired = true, Name = "Antall", Order = 4)]
        public int Antall
        {
            get { return AntallField; }
            set { AntallField = value; }
        }

        private string PreviewURLField;

        [DataMember(IsRequired = false, Name = "PreviewURL", Order = 5)]
        public string PreviewURL
        {
            get { return PreviewURLField; }
            set { PreviewURLField = value; }
        }

        private DateTime SistOppdatertField;

        [DataMember(IsRequired = true, Name = "DatoOppdatert", Order = 6)]
        public DateTime DatoOppdatert
        {
            get { return SistOppdatertField; }
            set { SistOppdatertField = value; }
        }

        private string oppdatertAv;

        [DataMember(IsRequired = true, Order = 7)]
        public string OppdatertAv
        {
            get { return oppdatertAv; }
            set { oppdatertAv = value; }
        }
    }
}

