using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Postnr
    {
        public string DiplayText
        {
            get
            {
                return this.Post_nr + " - " + this.Poststed;
            }
        }

        private string _Postnr;
        public string Post_nr
        {
            get
            {
                return _Postnr;
            }
            set
            {
                _Postnr = value;
            }
        }

        private string _Poststed;
        public string Poststed
        {
            get
            {
                return _Poststed;
            }
            set
            {
                _Poststed = value;
            }
        }

        public Postnr()
        {
        }

        public Postnr(string id, string name)
        {
            Post_nr = id;
            Poststed = name;
        }
    }

    public class PostnrCollection : System.Collections.Generic.List<Postnr>
    {

        // Public Function GroupByPostalZone() As ReolTree
        // SortByPostalZoneReol()
        // Dim result As New ReolTree()
        // For Each r As Reol In Me
        // result.InsertByPostalZone(r)
        // Next
        // Return result
        // End Function

        // Public Sub SortByPostalZoneReol()
        // Me.Sort(New PostalzoneComparerByPostalZoneReol())
        // End Sub

        public Postnr FindPostNr(string postNumber)
        {
            foreach (Postnr p in this)
            {
                if (p.Post_nr == postNumber)
                    return p;
            }

            return null;
        }
    }

    public class PostalzoneComparerByPostalZoneReol : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.PostalZone.CompareTo(y.PostalZone);
            // If result = 0 Then result = x.PostalZone.CompareTo(y.PostalZone)
            if (result == 0)
                result = x.PostalArea.CompareTo(y.PostalArea);
            return result;
        }
    }

    public class PostalzoneComparerByPostalZoneReolNo : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.PostalZone.CompareTo(y.PostalZone);
            // If result = 0 Then result = x.PostalZone.CompareTo(y.PostalZone)
            if (result == 0)
                result = x.PostalArea.CompareTo(y.PostalArea);
            if (result == 0)
                result = x.ReolNumber.CompareTo(y.ReolNumber);
            return result;
        }
    }
}
