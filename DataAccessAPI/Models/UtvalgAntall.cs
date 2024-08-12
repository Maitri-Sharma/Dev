using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - Fordeling
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "UtvalgAntall")]
    public partial class Utvalgantall
    {
        private System.String FordelingsTypeField;

        [DataMember(IsRequired = true, Name = "FordelingsType", Order = 0)]
        public System.String FordelingsType
        {
            get { return FordelingsTypeField; }
            set { FordelingsTypeField = value; }
        }

        private Helper.SoneAntall antallSone;

        [DataMember(IsRequired = true, Name = "AntallSone", Order = 3)]
        public Helper.SoneAntall AntallSone
        {
            get { return antallSone; }
            set { antallSone = value; }
        }

        private System.String UtvalgsrefField;

        [DataMember(IsRequired = true, Name = "Utvalgsref", Order = 1)]
        public System.String Utvalgsref
        {
            get { return UtvalgsrefField; }
            set { UtvalgsrefField = value; }
        }

        private System.DateTime DatoOppdatertField;

        [DataMember(IsRequired = true, Name = "DatoOppdatert", Order = 2)]
        public System.DateTime DatoOppdatert
        {
            get { return DatoOppdatertField; }
            set { DatoOppdatertField = value; }
        }

        private System.Int32 AntallSegmenterField;

        [DataMember(IsRequired = true, Name = "AntallSegmenter", Order = 4)]
        public System.Int32 AntallSegmenter
        {
            get { return AntallSegmenterField; }
            set { AntallSegmenterField = value; }
        }

        private System.Int32 AntallDemografiField;

        [DataMember(IsRequired = true, Name = "AntallDemografi", Order = 5)]
        public System.Int32 AntallDemografi
        {
            get { return AntallDemografiField; }
            set { AntallDemografiField = value; }
        }

        private System.Int32 VektField;

        [DataMember(IsRequired = false, Name = "Vekt", Order = 6)]
        public System.Int32 Vekt
        {
            get { return VektField; }
            set { VektField = value; }
        }

        private DateTime DistribusjonsDatoField;

        [DataMember(IsRequired = false, Name = "DistribusjonsDato", Order = 7)]
        public DateTime DistribusjonssDato
        {
            get { return DistribusjonsDatoField; }
            set { DistribusjonsDatoField = value; }
        }

        private string DistribusjonsTypeField;

        [DataMember(IsRequired = false, Name = "DistribusjonsType", Order = 8)]
        public string DistribusjonsType
        {
            get { return DistribusjonsTypeField; }
            set { DistribusjonsTypeField = value; }
        }

        //private string DistribusjonsTypeField;

        //[DataMember(IsRequired = false, Name = "DistribusjonsType", Order = 8)]
        //public string DistribusjonsType
        //{
        //    get { return DistribusjonsTypeField; }
        //    set { DistribusjonsTypeField = value; }
        //}


        private Helper.UtvalgPRSListe UtvalgPRSListeField;

        [DataMember(IsRequired = false, Name = "UtvalgPRSListe", Order = 9)]
        public Helper.UtvalgPRSListe UtvalgPRSListe
        {
            get { return UtvalgPRSListeField; }
            set { UtvalgPRSListeField = value; }
        }

        private System.Double TykkelseField;

        [DataMember(IsRequired = false, Name = "Tykkelse", Order = 10)]
        public System.Double Tykkelse
        {
            get { return TykkelseField; }
            set { TykkelseField = value; }
        }
    }
}
