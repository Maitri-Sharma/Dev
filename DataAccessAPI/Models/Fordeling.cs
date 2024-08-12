using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataAccessAPI.Models
{
	/// <summary>
	/// Data Contract Class - Fordeling
	/// </summary>
	[DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "Fordeling")]
	public partial class Fordeling
	{
		private System.String FylkeField;

		[DataMember(IsRequired = true, Name = "Fylke", Order = 0)]
		public System.String Fylke
		{
			get { return FylkeField; }
			set { FylkeField = value; }
		}

		private System.String KommuneRuteField;

		[DataMember(IsRequired = true, Name = "KommuneRute", Order = 1)]
		public System.String KommuneRute
		{
			get { return KommuneRuteField; }
			set { KommuneRuteField = value; }
		}

		private System.String KommuneBydelField;

		[DataMember(IsRequired = false, Name = "KommuneBydel", Order = 2)]
		public System.String KommuneBydel
		{
			get { return KommuneBydelField; }
			set { KommuneBydelField = value; }
		}

		private System.String PostnummerField;

		[DataMember(IsRequired = false, Name = "Postnummer", Order = 3)]
		public System.String Postnummer
		{
			get { return PostnummerField; }
			set { PostnummerField = value; }
		}

		private System.String PoststedField;

		[DataMember(IsRequired = true, Name = "Poststed", Order = 4)]
		public System.String Poststed
		{
			get { return PoststedField; }
			set { PoststedField = value; }
		}

		private string TeamNrField;

		[DataMember(IsRequired = true, Name = "TeamNr", Order = 5)]
		public string TeamNr
		{
			get { return TeamNrField; }
			set { TeamNrField = value; }
		}
	

		private System.String TeamField;

		[DataMember(IsRequired = true, Name = "Team", Order = 6)]
		public System.String Team
		{
			get { return TeamField; }
			set { TeamField = value; }
		}

		
		private string TeamKomplettField;
		
		[DataMember(IsRequired = false, Name = "TeamKomplett", Order = 7)]
		public string TeamKomplett
		{
			get { return TeamKomplettField; }
			set { TeamKomplettField = value; }
		}


		private string RuteNrField;

		[DataMember(IsRequired = true, Name = "RuteNr", Order = 8)]
		public string RuteNr
		{
			get { return RuteNrField; }
			set { RuteNrField = value; }
		}
	

		private System.String RuteField;

		[DataMember(IsRequired = true, Name = "Rute", Order = 9)]
		public System.String Rute
		{
			get { return RuteField; }
			set { RuteField = value; }
		}

		private System.String SoneField;

		[DataMember(IsRequired = true, Name = "Sone", Order = 10)]
		public System.String Sone
		{
			get { return SoneField; }
			set { SoneField = value; }
		}

		private Helper.Antall AntallField;

		[DataMember(IsRequired = false, Name = "Antall", Order = 11)]
		public Helper.Antall Antall
		{
			get { return AntallField; }
			set { AntallField = value; }
		}


        private System.String PRSField;

        [DataMember(IsRequired = true, Name = "PRS", Order = 12)]
        public System.String PRS
        {
            get { return PRSField; }
            set { PRSField = value; }
        }

	}
}
