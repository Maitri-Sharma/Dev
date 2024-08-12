using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Puma.Shared
{
    public class BackgroundWorkerInputParams
    {
        private int _ProcessID = int.MinValue;
        private string _ImportFile = string.Empty;

        public int ProcessID
        {
            get { return _ProcessID; }
            set { _ProcessID = value; }
        }

        public string ImportFile
        {
            get { return _ImportFile; }
            set { _ImportFile = value; }
        }

    }
}
