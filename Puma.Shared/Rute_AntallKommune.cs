using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Puma.Shared
{
    public class Rute_AntallKommune
    {

        #region Private Entity Variables

        private string _TeamNr = string.Empty;
        private string _ReolNr = string.Empty;
        private string _KommuneNr = string.Empty;

       
        //Added for rdf
        private string _RuteDistfreq = string.Empty;
        private string _SondagFlag = string.Empty;
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

        //Added for RDF
        private int _Antall_P_HH_U_RES = 0;
        private int _Antall_P_VH_U_RES = 0;
        private int _Antall_NP_HH_U_RES = 0;
        private int _Antall_NP_VH_U_RES = 0;
        private int _Antall_P_HH_M_RES = 0;
        private int _Antall_P_VH_M_RES = 0;
        private int _Antall_NP_HH_M_RES = 0;
        private int _Antall_NP_VH_M_RES = 0;

        #endregion

        #region Definisjon av TILLEGGSVARIABLE

        private string _ReolID = string.Empty;
        private string _CurrentField = string.Empty;

        #endregion


        #region Public Entity Properties

        /// <summary>
        /// 
        /// </summary>
        public string ReolID
        {
            get { return _ReolID; }
            set
            {
                _CurrentField = "REOLID";
                _ReolID = value.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TeamNr
        {
            get { return _TeamNr; }
            set 
            {
                _CurrentField = "TEAMNR";
                _TeamNr = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReolNr
        {
            get { return _ReolNr; }
            set 
            {
                _CurrentField = "REOLNR";
                _ReolNr = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string KommuneNr
        {
            get { return _KommuneNr; }
            set
            {
                _CurrentField = "KOMMUNENR";
                _KommuneNr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_HH_U_RES
        {
            get { return _Antall_HH_U_RES; }
            set 
            {
                _CurrentField = "ANTALL_HH";
                _Antall_HH_U_RES = value; 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Antall_VH_U_RES
        {
            get { return _Antall_VH_U_RES; }
            set 
            {
                _CurrentField = "ANTALL_VH";
                _Antall_VH_U_RES = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_L_U_RES
        {
            get { return _Antall_L_U_RES; }
            set 
            {
                _CurrentField = "ANTALL_L";
                _Antall_L_U_RES = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_HH_M_RES
        {
            get { return _Antall_HH_M_RES; }
            set 
            {
                _CurrentField = "ANTALL_HH_RES";
                _Antall_HH_M_RES = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_VH_M_RES
        {
            get { return _Antall_VH_M_RES; }
            set 
            {
                _CurrentField = "ANTALL_VH_RES";
                _Antall_VH_M_RES = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_L_M_RES
        {
            get { return _Antall_L_M_RES; }
            set 
            {
                _CurrentField = "ANTALL_L_RES";
                _Antall_L_M_RES = value; 
            }
        }

        public int Antall_HH_GR_RES
        {
            get { return _Antall_HH_GR_RES; }
            set 
            {
                _CurrentField = "ANTALL_HH_GR_RES";
                _Antall_HH_GR_RES = value; 
            }
        }

        public int Antall_ANTP
        {
            get { return _Antall_ANTP; }
            set 
            {
                _CurrentField = "ANTALL_ANTP";
                _Antall_ANTP = value; 
            }
        }

        //Added for RDF

        /// <summary>
        /// 
        /// </summary>
        public string RuteDistfreq
        {
            get { return _RuteDistfreq; }
            set
            {
                _CurrentField = "RUTEDISTFREQ";
                _RuteDistfreq = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string SondagFlag
        {
            get { return _SondagFlag; }
            set
            {
                _CurrentField = "SONDAGFLAG";
                _SondagFlag = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_P_HH_U_RES
        {
            get { return _Antall_P_HH_U_RES; }
            set
            {
                _CurrentField = "ANTALL_P_HH_U_RES";
                _Antall_P_HH_U_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_P_VH_U_RES
        {
            get { return _Antall_P_VH_U_RES; }
            set
            {
                _CurrentField = "ANTALL_P_VH_U_RES";
                _Antall_P_VH_U_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_NP_HH_U_RES
        {
            get { return _Antall_NP_HH_U_RES; }
            set
            {
                _CurrentField = "ANTALL_NP_HH_U_RES";
                _Antall_NP_HH_U_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_NP_VH_U_RES
        {
            get { return _Antall_NP_VH_U_RES; }
            set
            {
                _CurrentField = "ANTALL_NP_VH_U_RES";
                _Antall_NP_VH_U_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_P_HH_M_RES
        {
            get { return _Antall_P_HH_M_RES; }
            set
            {
                _CurrentField = "ANTALL_P_HH_M_RES";
                _Antall_P_HH_M_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_P_VH_M_RES
        {
            get { return _Antall_P_VH_M_RES; }
            set
            {
                _CurrentField = "ANTALL_P_VH_M_RES";
                _Antall_P_VH_M_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_NP_HH_M_RES
        {
            get { return _Antall_NP_HH_M_RES; }
            set
            {
                _CurrentField = "ANTALL_NP_HH_M_RES";
                _Antall_NP_HH_M_RES = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Antall_NP_VH_M_RES
        {
            get { return _Antall_NP_VH_M_RES; }
            set
            {
                _CurrentField = "ANTALL_NP_VH_M_RES";
                _Antall_NP_VH_M_RES = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string CurrentField
        {
            get { return _CurrentField; }
            set { _CurrentField = value; }
        }

        #endregion
    }
}
