using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Puma.Shared
{
    public class Rute_RutePunkter
    {

        #region Private Entity Variables

        private string _ReolID = string.Empty; 
        private string _TeamNr = string.Empty;
        private string _ReolNr = string.Empty;
        private int _X = 0;
        private int _Y = 0;
        private int _AdrNrGAB = 0;
        private int _AdrNrInt = 0;
        private string _AdrKategori = string.Empty;
        private string _AdrNr = string.Empty;
        private string _KommuneNr = string.Empty;
        private string _NavnSted = string.Empty;
        private int _HusNr = 0;
        private string _Bokstav = string.Empty;
        private string _Oppgang = string.Empty;
        private string _PostNr = string.Empty;
        private string _PostAdresse = string.Empty;
        private string _CurrentField = string.Empty;
        //Added for RDF
        private string _RuteDistFreq = string.Empty;
        private string _SondagFlag = string.Empty;
        private string _Priority = string.Empty;
        //End
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
                _CurrentField = "ReolID";
                _ReolID = value.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TeamNr
        {
            get 
            {
                _CurrentField = "TEAMNR";
                return _TeamNr; 
            }
            set { _TeamNr = value.Trim(); }
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
        public int X
        {
            get { return _X; }
            set 
            {
                _CurrentField = "KARTKOORDX";
                _X = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set 
            {
                _CurrentField = "KARTKOORDY";
                _Y = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AdrNrGAB
        {
            get { return _AdrNrGAB; }
            set 
            {
                _CurrentField = "ADRNRGAB";
                _AdrNrGAB = value; 
            }
        }

        public int AdrNrInt
        {
            get { return _AdrNrInt; }
            set
            {
                _CurrentField = "AdrNrInt";
                _AdrNrInt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AdrKategori
        {
            get 
            {
                _CurrentField = "KODEADRKATEGORI";
                return _AdrKategori; 
            }
            set { _AdrKategori = value.Trim(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AdrNr
        {
            get { return _AdrNr; }
            set 
            {
                _CurrentField = "ADRNR";
                _AdrNr = value.Trim(); 
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
                _KommuneNr = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NavnSted
        {
            get { return _NavnSted; }
            set 
            {
                _CurrentField = "NAVNSTED";
                _NavnSted = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int HusNr
        {
            get { return _HusNr; }
            set 
            {
                _CurrentField = "HUSNR";
                _HusNr = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Bokstav
        {
            get { return _Bokstav; }
            set 
            {
                _CurrentField = "BOKSTAV";
                _Bokstav = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Oppgang
        {
            get { return _Oppgang; }
            set 
            {
                _CurrentField = "OPPGANG";
                _Oppgang = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PostNr
        {
            get { return _PostNr; }
            set 
            {
                _CurrentField = "POSTNR";
                _PostNr = value.Trim(); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PostAdresse
        {
            get { return _PostAdresse; }
            set 
            {
                _CurrentField = "POSTADRESSE";
                _PostAdresse = value.Trim(); 
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
        //Added for RDF
        /// <summary>
        /// 
        /// </summary>
        public string RuteDistFreq
        {
            get { return _RuteDistFreq; }
            set { _RuteDistFreq = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SondagFlag
        {
            get { return _SondagFlag; }
            set { _SondagFlag = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }
        //End
        #endregion

    }
}
