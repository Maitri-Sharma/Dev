using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
    public abstract class UtvalgBase : UtvalgsId
    {
        private int ParentIdField;

        [DataMember(IsRequired = false, Name = "ParentId", Order = 2)]
        public int ParentId
        {
            get { return ParentIdField; }
            set { ParentIdField = value; }
        }


        private string NavnField;

        [DataMember(IsRequired = true, Name = "Navn", Order = 3)]
        public string Navn
        {
            get { return NavnField; }
            set { NavnField = value; }
        }
    }
}
