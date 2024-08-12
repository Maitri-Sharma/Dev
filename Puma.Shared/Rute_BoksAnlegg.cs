using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Puma.Shared
{
    public class Rute_BoksAnlegg
    {
        #region Private Entity Variables

        private string _ReolID = string.Empty;
        private string _TeamNr = string.Empty;
        private string _ReolNr = string.Empty;
        private int _BoksAnlNr = 0;
        private string _AktorNrHent = string.Empty;
        private string _NavnVirksomhet = string.Empty;
        private string _NavnGate = string.Empty;
        private int _HusNr = 0;
        private string _Bokstav = string.Empty;
        private string _PostNr = string.Empty;
        private string _PostAdresse = string.Empty;
        private int _AdrNrGAB = 0;
        private int _X = 0;
        private int _Y = 0;
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
                _TeamNr = value; 
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
                _ReolNr = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BoksAnlNr
        {
            get { return _BoksAnlNr; }
            set 
            {
                _CurrentField = "BOKSANLNR";
                _BoksAnlNr = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AktorNrHent
        {
            get { return _AktorNrHent; }
            set 
            {
                _CurrentField = "AKTORNRHENT";
                _AktorNrHent = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NavnVirksomhet
        {
            get { return _NavnVirksomhet; }
            set 
            {
                _CurrentField = "NAVNVIRKSOMHET";
                _NavnVirksomhet = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NavnGate
        {
            get { return _NavnGate; }
            set 
            {
                _CurrentField = "NAVNGATE";
                _NavnGate = value; 
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
                _Bokstav = value; 
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
                _PostNr = value; 
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
                _PostAdresse = value; 
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
        public string CurrentField
        {
            get { return _CurrentField; }
            set { _CurrentField = value; }
        }

        #endregion
    }
}
