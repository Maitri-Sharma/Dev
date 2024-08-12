using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Puma.Shared
{
    public class ImportData
    {
        #region Definisjon av private variable

        private BackgroundWorker backgroundWorker = null;
        private bool is_backgroundWorker_Completed = true;
        private bool is_backgroundWorker_FinishedImport = false;
        private bool isToBeImported = false;
        private ImportStatus isImportOK = ImportStatus.NoImport;

        private string archivePath = string.Empty;
        private string searchPattern = string.Empty;

        private string filePath = string.Empty;
        private string fileExtension = string.Empty;
        private string fileFullName = string.Empty;

        private int daysToKeepImportFiles = 0;
        private int daysToArchiveImportFiles = 0;

        public enum ImportStatus
        {
            ImportOngoing,
            ImportOK,
            ImportFailed,
            NoImport,
            NotStarted
        }

        #endregion

        #region Public Properties

        public BackgroundWorker BackgroundWorker
        {
            get { return backgroundWorker; }
            set { backgroundWorker = value; }
        }

        public bool Is_backgroundWorker_Completed
        {
            get { return is_backgroundWorker_Completed; }
            set { is_backgroundWorker_Completed = value; }
        }

        public bool Is_backgroundWorker_FinishedImport
        {
            get { return is_backgroundWorker_FinishedImport; }
            set { is_backgroundWorker_FinishedImport = value; }
        }

        public bool IsToBeImported
        {
            get { return isToBeImported; }
            set { isToBeImported = value; }
        }

        public string ArchivePath
        {
            get { return archivePath; }
            set { archivePath = value; }
        }

        public string SearchPattern
        {
            get { return searchPattern; }
            set { searchPattern = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }

        public string FileFullName
        {
            get { return fileFullName; }
            set { fileFullName = value; }
        }

        public ImportStatus IsImportOK
        {
            get { return isImportOK; }
            set { isImportOK = value; }
        }

        public int DaysToKeepImportFiles
        {
            get { return daysToKeepImportFiles; }
            set { daysToKeepImportFiles = value; }
        }

        public int DaysToArchiveImportFiles
        {
            get { return daysToArchiveImportFiles; }
            set { daysToArchiveImportFiles = value; }
        }

        #endregion
    }
}
