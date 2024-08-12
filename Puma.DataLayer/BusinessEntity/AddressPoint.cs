using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity
{
    public class AddressPoint
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private double _X;
        public double X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }

        private double _Y;
        public double Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }

        private bool _Checked;
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
            }
        }

        public AddressPoint()
        {
        }

        public AddressPoint(double x, double y, string name = "")
        {
            _X = x;
            _Y = y;
            _Name = name;
            _Checked = true;
        }
    }

    public class AddressPointList : List<AddressPoint>
    {
        public AddressPointList GetCheckedPoints()
        {
            AddressPointList result = new AddressPointList();
            foreach (AddressPoint p in this)
            {
                if (p.Checked)
                    result.Add(p);
            }
            return result;
        }
    }
}
