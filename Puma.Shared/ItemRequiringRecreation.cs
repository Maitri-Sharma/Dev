using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    [Serializable]
    public class ItemRequiringRecreation
    {
        public int Id { get; set; }
        public bool IsUtvalg { get; set; }
    }
}
