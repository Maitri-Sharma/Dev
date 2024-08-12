using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class NewReolTreeNameComparer : IComparer<NewReolTree>
    {
        public int Compare(NewReolTree x, NewReolTree y)
        {
            return System.String.Compare(x.Text, y.Text);
        }
    }
}
