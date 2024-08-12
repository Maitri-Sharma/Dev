using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - UtvalgFordeling
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "UtvalgDetaljer")]
    public partial class UtvalgDetaljer : UtvalgBase
    {
        private System.String LogoField;

        [DataMember(IsRequired = false, Name = "Logo", Order = 4)]
        public System.String Logo
        {
            get { return LogoField; }
            set { LogoField = value; }
        }

        private Helper.Fordelinger FordelingerField;

        [DataMember(IsRequired = true, Name = "Fordelinger", Order = 5)]
        public Helper.Fordelinger Fordelinger
        {
            get { return FordelingerField; }
            set { FordelingerField = value; }
        }

        private System.String FordelingstypeField;

        [DataMember(IsRequired = true, Name = "Fordelingstype", Order = 6)]
        public System.String Fordelingstype
        {
            get { return FordelingstypeField; }
            set { FordelingstypeField = value; }
        }
    }
}
