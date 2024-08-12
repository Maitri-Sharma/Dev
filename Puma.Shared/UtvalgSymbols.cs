using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgSymbol
    {
        private string _Symbol;
        private string _SymbolImg;

        public UtvalgSymbol(string symbol, string symbolImg)
        {
            this.Symbol = symbol;
            this.SymbolImg = symbolImg;
        }

        public string Symbol
        {
            get
            {
                return _Symbol;
            }
            set
            {
                _Symbol = value;
            }
        }

        public string SymbolImg
        {
            get
            {
                return _SymbolImg;
            }
            set
            {
                _SymbolImg = value;
            }
        }
    }

    public class UtvalgSymbolLookUp
    {
        private Dictionary<UtvalgSymbol, UtvalgCollection> _utvalgsBySymbol = new Dictionary<UtvalgSymbol, UtvalgCollection>();

        public UtvalgSymbolLookUp(string[] configuration)
        {
            UtvalgSymbol utvalgSymb;
            foreach (string s in configuration)
            {
                utvalgSymb = new UtvalgSymbol(s.Split("#")[0], s.Split("#")[1]);
                this._utvalgsBySymbol.Add(utvalgSymb, new UtvalgCollection());
            }
        }

        public UtvalgSymbol FindUtvalgSymbol(Utvalg utv)
        {
            foreach (KeyValuePair<UtvalgSymbol, UtvalgCollection> symbUtvalgCollPair in this._utvalgsBySymbol)
            {
                if (symbUtvalgCollPair.Value.Contains(utv))
                    return symbUtvalgCollPair.Key;
            }

            UtvalgSymbol result = this.GetSymbolWithLeastConnectedUtvalgs();
            this._utvalgsBySymbol[result].Add(utv);
            return result;
        }

        private UtvalgSymbol GetSymbolWithLeastConnectedUtvalgs()
        {
            UtvalgSymbol result = null;

            foreach (KeyValuePair<UtvalgSymbol, UtvalgCollection> symbUtvalgCollPair in this._utvalgsBySymbol)
            {
                if (result == null)
                    result = symbUtvalgCollPair.Key;
                else if (symbUtvalgCollPair.Value.Count < this._utvalgsBySymbol[result].Count)
                    result = symbUtvalgCollPair.Key;

                if (symbUtvalgCollPair.Value.Count == 0)
                    break;
            }

            return result;
        }

        public void CleanUp(UtvalgCollection currentUtvalgs)
        {
            foreach (KeyValuePair<UtvalgSymbol, UtvalgCollection> symbUtvalgCollPair in this._utvalgsBySymbol)
            {
                foreach (Utvalg utv in symbUtvalgCollPair.Value.ToArray())
                {
                    if (!currentUtvalgs.Contains(utv))
                        symbUtvalgCollPair.Value.Remove(utv);
                }
            }
        }
    }
}
