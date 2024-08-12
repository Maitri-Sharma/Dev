using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - UtvalgsId
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "UtvalgsId")]
    [XmlRoot(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", ElementName = "UtvalgsId")]

    public partial class UtvalgsId
    {
        private System.Int32 IdField;

        /// <summary>
        /// Id til utvalg eller utvalgsliste.
        /// </summary>
        [DataMember(IsRequired = true, Name = "Id", Order = 0)]
        public System.Int32 Id
        {
            get { return IdField; }
            set { IdField = value; }
        }

        private Helper.UtvalgsTypeKode TypeField;

        /// <summary>
        /// Angir om id tilhører utvalg eller utvalgsliste.
        /// </summary>
        [DataMember(IsRequired = true, Name = "Type", Order = 1)]
        public Helper.UtvalgsTypeKode Type
        {
            get { return TypeField; }
            set { TypeField = value; }
        }

    }
}
