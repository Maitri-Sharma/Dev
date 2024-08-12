using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public class Rute_Antall
    {
        #region Private Entity Variables

        #region Definisjon av INPUT_DATA VARIABLE

        private string _TeamNr = string.Empty;
        private string _ReolNr = string.Empty;
        private string _ReolType = string.Empty;
        private string _TeamNavn = string.Empty;
        private string _PrsNr = string.Empty;
        private string _PrsNavn = string.Empty;
        private string _PrsBeskrivelse = string.Empty;
        private string _Beskrivelse = string.Empty;
        private string _PbKontNavn = string.Empty;
        private string _PostNr = string.Empty;
        private string _PostAdr = string.Empty;
        private string _KommuneNr = string.Empty;
        private string _Kommune = string.Empty;
        private string _FylkeNr = string.Empty;
        private string _Fylke = string.Empty;
        //Added for RDF
        private string _RuteDistFreq = string.Empty;
        private string _SondagFlag = string.Empty;
        //End
        #endregion

        #region Definisjon av TILLEGGSVARIABLE

        private string _ReolID = string.Empty;
        private string _CurrentField = string.Empty;

        #endregion

        #region Definisjon av ANTALL VARIABLE

        private int _Antall_HH_U_RES = 0;
        private int _Antall_VH_U_RES = 0;
        private int _Antall_L_U_RES = 0;
        private int _Antall_HH_M_RES = 0;
        private int _Antall_VH_M_RES = 0;
        private int _Antall_L_M_RES = 0;

        private int _Antall_HH_GR_RES = 0;
        private int _Antall_ANTP = 0;
        private int _PrisSone = 0;
        //Added for RDF
        private int _Antall_P_HH_U_RES = 0;
        private int _Antall_P_VH_U_RES = 0;
        private int _Antall_NP_HH_U_RES = 0;
        private int _Antall_NP_VH_U_RES = 0;
        private int _Antall_P_HH_MM_RES = 0;
        private int _Antall_P_VH_MM_RES = 0;
        private int _Antall_NP_HH_MM_RES = 0;
        private int _Antall_NP_VH_MM_RES = 0;
        //End
        			   
        			

  

        #endregion

        #endregion

        #region Public Properties

        #region INPUT_DATA

        public string TeamNr
        {
            get { return _TeamNr; }
            set
            {
                _CurrentField = "TeamNr";
                _TeamNr = value.Trim();
            }
        }

        public string ReolNr
        {
            get
            { return _ReolNr; }
            set
            {
                _CurrentField = "ReolNr";
                _ReolNr = value.Trim();
            }
        }

        public string ReolType
        {
            get
            { return _ReolType; }
            set
            {
                _CurrentField = "ReolType";
                _ReolType = value.Trim();
            }
        }

        public string TeamNavn
        {
            get
            {
                return _TeamNavn;
            }
            set
            {
                _CurrentField = "TeamNavn";
                _TeamNavn = value.Trim();
            }
        }

        public string PrsNr
        {
            get
            {
                return _PrsNr;
            }
            set
            {
                _CurrentField = "PrsNr";
                _PrsNr = value.Trim();
            }
        }

        public string PrsNavn
        {
            get
            {
                return _PrsNavn;
            }
            set
            {
                _CurrentField = "PrsNavn";
                _PrsNavn = value.Trim();
            }
        }

        public string PrsBeskrivelse
        {
            get
            {
                return _PrsBeskrivelse;
            }
            set
            {
                _CurrentField = "PrsBeskrivelse";
                _PrsBeskrivelse = value.Trim();
            }
        }

        public string PbKontNavn
        {
            get
            {
                return _PbKontNavn;
            }
            set
            {
                _CurrentField = "PbKontNavn";
                _PbKontNavn = value.Trim();
            }
        }

        public string PostNr
        {
            get
            {
                return _PostNr;
            }
            set
            {
                _CurrentField = "PostNr";
                _PostNr = value.Trim();
            }
        }

        public string PostAdresse
        {
            get
            {
                return _PostAdr;
            }
            set
            {
                _CurrentField = "PostAdresse";
                _PostAdr = value.Trim();
            }
        }

        public string KommuneNr
        {
            get
            {
                return _KommuneNr;
            }
            set
            {
                _CurrentField = "KommuneNr";
                _KommuneNr = value.Trim();
            }
        }

        public string KommuneNavn
        {
            get
            {
                return _Kommune;
            }
            set
            {
                _CurrentField = "KommuneNavn";
                _Kommune = value.Trim();
            }
        }

        public string FylkesNr
        {
            get
            {
                return _FylkeNr;
            }
            set
            {
                _CurrentField = "FylkesNr";
                _FylkeNr = value.Trim();
            }
        }

        public string FylkesNavn
        {
            get
            {
                return _Fylke;
            }
            set
            {
                _CurrentField = "FylkesNavn";
                _Fylke = value.Trim();
            }
        }
        //Added for RDF
        public string RuteDistFreq
        {
            get
            {
                return _RuteDistFreq;
            }
            set
            {
                _CurrentField = "RuteDistFreq";
                _RuteDistFreq = value.Trim();
            }
        }
        public string SondagFlag
        {
            get
            {
                return _SondagFlag;
            }
            set
            {
                _CurrentField = "SondagFlag";
                _SondagFlag = value.Trim();
            }
        }
        #endregion

        #region TILLEGGSVARIABLE

        public string ReolID
        {
            get { return _ReolID; }
            set
            {
                _CurrentField = "ReolID";
                _ReolID = value.Trim();
            }
        }

        public string Beskrivelse
        {
            get
            {
                return _Beskrivelse;
            }
            set
            {
                _CurrentField = "Beskrivelse";
                _Beskrivelse = value.Trim();
            }
        }

         #endregion

        #region ANTALL VARIABLE

        public int Antall_HH_U_RES
        {
            get 
            {
                return _Antall_HH_U_RES; 
            }
            set
            {
                _CurrentField = "Antall_HH_U_RES";
                _Antall_HH_U_RES = value;
            }
        }

        public int Antall_VH_U_RES
        {
            get 
            {
                return _Antall_VH_U_RES; 
            }
            set
            {
                _CurrentField = "Antall_VH_U_RES";
                _Antall_VH_U_RES = value;
            }
        }

        public int Antall_L_U_RES
        {
            get 
            {
                return _Antall_L_U_RES; 
            }
            set
            {
                _CurrentField = "Antall_L_U_RES";
                _Antall_L_U_RES = value;
            }
        }

        public int Antall_HH_M_RES
        {
            get 
            {
                return _Antall_HH_M_RES; 
            }
            set
            {
                _CurrentField = "Antall_HH_M_RES";
                _Antall_HH_M_RES = value;
            }
        }

        public int Antall_VH_M_RES
        {
            get 
            {
                return _Antall_VH_M_RES; 
            }
            set
            {
                _CurrentField = "Antall_VH_M_RES";
                _Antall_VH_M_RES = value;
            }
        }


        public int Antall_L_M_RES
        {
            get 
            { 
                return _Antall_L_M_RES; 
            }

            set 
            {
                _CurrentField = "Antall_L_M_RES";
                _Antall_L_M_RES = value; 
            }
        }

        public int Antall_HH_GR_RES
        {
            get
            {
                return _Antall_HH_GR_RES;
            }
            set
            {
                _CurrentField = "Antall_HH_GR_RES";
                _Antall_HH_GR_RES = value;
            }
        }
        public int Antall_ANTP
        {
            get
            {
                return _Antall_ANTP;
            }
            set
            {
                _CurrentField = "Antall_ANTP";
                _Antall_ANTP = value;
            }
        }

        public int PrisSone
        {
            get 
            {
                return _PrisSone; 
            }
            set 
            {
                _CurrentField = "PrisSone";
                _PrisSone = value;
            }
        }

        public string CurrentField
        {
            get { return _CurrentField; }
            set
            {
                _CurrentField = value;
            }
        }
        //Added for RDF
        
        public int Antall_P_HH_U_RES
        {
            get
            {
                return _Antall_P_HH_U_RES;
            }
            set
            {
                _CurrentField = "Antall_P_HH_U_RES";
                _Antall_P_HH_U_RES = value;
            }
        }
        public int Antall_P_VH_U_RES
        {
            get
            {
                return _Antall_P_VH_U_RES;
            }
            set
            {
                _CurrentField = "Antall_P_VH_U_RES";
                _Antall_P_VH_U_RES = value;
            }
        }
        public int Antall_NP_HH_U_RES
        {
            get
            {
                return _Antall_NP_HH_U_RES;
            }
            set
            {
                _CurrentField = "Antall_NP_HH_U_RES";
                _Antall_NP_HH_U_RES = value;
            }
        }
        public int Antall_NP_VH_U_RES
        {
            get
            {
                return _Antall_NP_VH_U_RES;
            }
            set
            {
                _CurrentField = "Antall_NP_VH_U_RES";
                _Antall_NP_VH_U_RES = value;
            }
        }
        public int Antall_P_HH_MM_RES
        {
            get
            {
                return _Antall_P_HH_MM_RES;
            }
            set
            {
                _CurrentField = "Antall_P_HH_MM_RES";
                _Antall_P_HH_MM_RES = value;
            }
        }
        public int Antall_P_VH_MM_RES
        {
            get
            {
                return _Antall_P_VH_MM_RES;
            }
            set
            {
                _CurrentField = "Antall_P_VH_MM_RES";
                _Antall_P_VH_MM_RES = value;
            }
        }
        public int Antall_NP_HH_MM_RES
        {
            get
            {
                return _Antall_NP_HH_MM_RES;
            }
            set
            {
                _CurrentField = "Antall_NP_HH_MM_RES";
                _Antall_NP_HH_MM_RES = value;
            }
        }
        public int Antall_NP_VH_MM_RES
        {
            get
            {
                return _Antall_NP_VH_MM_RES;
            }
            set
            {
                _CurrentField = "Antall_NP_VH_MM_RES";
                _Antall_NP_VH_MM_RES = value;
            }
        }

        #endregion

        #endregion

    }
}
