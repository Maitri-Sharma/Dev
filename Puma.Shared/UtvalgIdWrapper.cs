using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    [Serializable]
    public class UtvalgIdWrapper
    {
        public int Id { get; set; }
        public PumaEnum.UtvalgType TypeUtvalg { get; set; }

        public UtvalgIdWrapper() { }

        public UtvalgIdWrapper(int id, PumaEnum.UtvalgType type)
        {
            Id = id;
            TypeUtvalg = type;
        }

    }

    [Serializable]
    public class UtvalgIdWrapperExtented : UtvalgIdWrapper
    {
        public bool IsBasis { get; set; }
        public bool IsBasedOn { get; set; }
        public bool MustCopyWheanAddingToHandlekurv { get; set; }

        public UtvalgIdWrapperExtented() { }

        public UtvalgIdWrapperExtented(int id, PumaEnum.UtvalgType type, bool isBasis, bool isBasedOn, bool mustCopyWheanAddingToHandlekurv)
        {
            Id = id;
            TypeUtvalg = type;
            IsBasis = isBasis;
            IsBasedOn = isBasedOn;
            MustCopyWheanAddingToHandlekurv = mustCopyWheanAddingToHandlekurv;
        }

    }
}
