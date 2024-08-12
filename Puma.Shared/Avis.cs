using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Avis : IComparable<Avis>
    {
        public Avis()
        {
        }

        public Avis(string utgave, string feltnavn)
        {
            this._utgave = utgave;
            this._feltnavn = feltnavn;
        }

        private string _utgave;
        public string Utgave
        {
            get
            {
                return _utgave;
            }
            set
            {
                _utgave = value;
            }
        }

        private string _feltnavn;
        public string Feltnavn
        {
            get
            {
                return _feltnavn;
            }
            set
            {
                _feltnavn = value;
            }
        }

        public int CompareTo(Avis other)
        {
            return this.Utgave.CompareTo(other.Utgave);
        }
    }
}
