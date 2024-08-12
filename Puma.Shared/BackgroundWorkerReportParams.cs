namespace Puma.Shared
{
    public class BackgroundWorkerReportParams
    {
        private int _ProcessID = int.MinValue;
        private ImportData.ImportStatus _ImportStatus = ImportData.ImportStatus.NoImport;
        private string _ImportFile = string.Empty;
        private int _NumberOfRecordsInImportFile = 0;
        private int _NumberOfExecuted = 0;
        private int _NumberOfImported = 0;
        private int _NumberOfRejects = 0;
        private int _NumberOfErrors = 0;

        public int ProcessID
        {
            get { return _ProcessID; }
            set { _ProcessID = value; }
        }

        public ImportData.ImportStatus ImportStatus
        {
            get { return _ImportStatus; }
            set { _ImportStatus = value; }
        }

        public string ImportFile
        {
            get { return _ImportFile; }
            set { _ImportFile = value; }
        }

        public int NumberOfExecuted
        {
            get { return _NumberOfExecuted; }
            set { _NumberOfExecuted = value; }
        }

        public int NumberOfRecordsInImportFile
        {
            get { return _NumberOfRecordsInImportFile; }
            set { _NumberOfRecordsInImportFile = value; }
        }

        public int NumberOfRejects
        {
            get { return _NumberOfRejects; }
            set { _NumberOfRejects = value; }
        }

        public int NumberOfErrors
        {
            get { return _NumberOfErrors; }
            set { _NumberOfErrors = value; }
        }

        public int NumberOfImported
        {
            get { return _NumberOfImported; }
            set { _NumberOfImported = value; }
        }

    }
}
