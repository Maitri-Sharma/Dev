using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Shared
{
    public class TempResultData
    {
        private double _OldArea;

        public double OldArea
        {
            get
            {
                return _OldArea;
            }

            set
            {
                _OldArea = value;
            }
        }

        private double _NewArea;

        public double NewArea
        {
            get
            {
                return _NewArea;
            }

            set
            {
                _NewArea = value;
            }
        }

        private string _RemovedBoxes;

        public string RemovedBoxes
        {
            get
            {
                return _RemovedBoxes;
            }

            set
            {
                _RemovedBoxes = value;
            }
        }

        private string _ReplacedBoxes;

        public string ReplacedBoxes
        {
            get
            {
                return _ReplacedBoxes;
            }

            set
            {
                _ReplacedBoxes = value;
            }
        }

        private long _OldQuantity;

        public long OldQuantity
        {
            get
            {
                return _OldQuantity;
            }

            set
            {
                _OldQuantity = value;
            }
        }

        private long _NewQuantity;

        public long NewQuantity
        {
            get
            {
                return _NewQuantity;
            }

            set
            {
                _NewQuantity = value;
            }
        }

        private object _OldUtvalgGeometry;

        public object OldUtvalgGeometry
        {
            get
            {
                return _OldUtvalgGeometry;
            }

            set
            {
                _OldUtvalgGeometry = value;
            }
        }

        private ReturnCodes _ReturnCode;

        public ReturnCodes returnCode
        {
            get
            {
                return _ReturnCode;
            }

            set
            {
                _ReturnCode = value;
            }
        }

        private object _OldUtvalgGeometryBuffer;

        public object OldUtvalgGeometryBuffer
        {
            get
            {
                return _OldUtvalgGeometryBuffer;
            }

            set
            {
                _OldUtvalgGeometryBuffer = value;
            }
        }

        private bool _DoubleCoverageRemoved = false;

        public bool DoubleCoverageRemoved
        {
            get
            {
                return _DoubleCoverageRemoved;
            }

            set
            {
                _DoubleCoverageRemoved = value;
            }
        }

        private string _BestillerUserId;

        public string BestillerUserId
        {
            get
            {
                return _BestillerUserId;
            }

            set
            {
                _BestillerUserId = value;
            }
        }

        private long _NumberOfOldHH;

        public long NumberOfOldHH
        {
            get
            {
                return _NumberOfOldHH;
            }

            set
            {
                _NumberOfOldHH = value;
            }
        }

        private long _NumberOfNewHH;

        public long NumberOfNewHH
        {
            get
            {
                return _NumberOfNewHH;
            }

            set
            {
                _NumberOfNewHH = value;
            }
        }

        // Antallsavvik HH
        private double _NumberOfDeviationHH;

        public double NumberOfDeviationHH
        {
            get
            {
                return _NumberOfDeviationHH;
            }

            set
            {
                _NumberOfDeviationHH = value;
            }
        }

        // Antallsdifferanse HH
        private long _NumberOfDifferenceHH;

        public long NumberOfDifferenceHH
        {
            get
            {
                return _NumberOfDifferenceHH;
            }

            set
            {
                _NumberOfDifferenceHH = value;
            }
        }

        private long _NumberOfOldV;

        public long NumberOfOldV
        {
            get
            {
                return _NumberOfOldV;
            }

            set
            {
                _NumberOfOldV = value;
            }
        }

        private long _NumberOfNewV;

        public long NumberOfNewV
        {
            get
            {
                return _NumberOfNewV;
            }

            set
            {
                _NumberOfNewV = value;
            }
        }

        private long _NumberOfOldRes;

        public long NumberOfOldRes
        {
            get
            {
                return _NumberOfOldRes;
            }

            set
            {
                _NumberOfOldRes = value;
            }
        }

        private long _NumberOfNewRes;

        public long NumberOfNewRes
        {
            get
            {
                return _NumberOfNewRes;
            }

            set
            {
                _NumberOfNewRes = value;
            }
        }

        private long _NumberOfOldTot;

        public long NumberOfOldTot
        {
            get
            {
                return _NumberOfOldTot;
            }

            set
            {
                _NumberOfOldTot = value;
            }
        }

        private long _NumberOfNewTot;

        public long NumberOfNewTot
        {
            get
            {
                return _NumberOfNewTot;
            }

            set
            {
                _NumberOfNewTot = value;
            }
        }

        private double _NetAreaOld;

        public double NetAreaOld
        {
            get
            {
                return _NetAreaOld;
            }

            set
            {
                _NetAreaOld = value;
            }
        }

        private double _NetAreaNew;

        public double NetAreaNew
        {
            get
            {
                return _NetAreaNew;
            }

            set
            {
                _NetAreaNew = value;
            }
        }

        private double _NetAreaDeviationPercent;

        public double NetAreaDeviationPercent
        {
            get
            {
                return _NetAreaDeviationPercent;
            }

            set
            {
                _NetAreaDeviationPercent = value;
            }
        }

        private double _GrosAreaOld;

        public double GrosAreaOld
        {
            get
            {
                return _GrosAreaOld;
            }

            set
            {
                _GrosAreaOld = value;
            }
        }

        private double _GrosAreaNew;

        public double GrosAreaNew
        {
            get
            {
                return _GrosAreaNew;
            }

            set
            {
                _GrosAreaNew = value;
            }
        }

        private double _GrosAreaDeviationPercent;

        public double GrosAreaDeviationPercent
        {
            get
            {
                return _GrosAreaDeviationPercent;
            }

            set
            {
                _GrosAreaDeviationPercent = value;
            }
        }

        private long _GrosChangeInNumber;

        public long GrosChangeInNumber
        {
            get
            {
                return _GrosChangeInNumber;
            }

            set
            {
                _GrosChangeInNumber = value;
            }
        }

        private double _GrosChangeInPercent;

        public double GrosChangeInPercent
        {
            get
            {
                return _GrosChangeInPercent;
            }

            set
            {
                _GrosChangeInPercent = value;
            }
        }

        private string _NewBoxes;

        public string NewBoxes
        {
            get
            {
                return _NewBoxes;
            }

            set
            {
                _NewBoxes = value;
            }
        }
    }
}
