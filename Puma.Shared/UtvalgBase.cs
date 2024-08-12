using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public abstract class UtvalgBase : UtvalgsId
    {
        private string navn;

        private int parentId;

        public string Navn
        {
            get { return navn; }
            set { navn = value; }
        }

        
        public Nullable<int> ParentId
        {
            get { return parentId; }
            set { parentId = (int)value; }
        }
    }
}
