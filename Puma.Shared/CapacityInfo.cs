using System;
using System.Collections.Generic;

namespace Puma.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CapacityInfo
    {
        #region Private Variables

        private int numberOfMessagesField = 0;
        private List<string> unzipFilesField = new List<string>();
        private List<string> zipFilesField = new List<string>();
        private string freshFileField = string.Empty;
        private System.DateTime freshFileDateField = System.DateTime.MinValue;
        private bool isFreshField = false;

        #endregion

        #region Public Properties

        public bool IsFresh
        {
            get
            {
                return isFreshField;
            }

            set
            {
                isFreshField = value;
            }
        }

        public DateTime FreshFileDate
        {
            get
            {
                return freshFileDateField;
            }

            set
            {
                freshFileDateField = value;
            }
        }

        public string FreshFile
        {
            get
            {
                return freshFileField;
            }

            set
            {
                freshFileField = value;
            }
        }

        public List<string> UnzipFiles
        {
            get
            {
                return unzipFilesField;
            }

            set
            {
                unzipFilesField = value;
            }
        }

        public List<string> ZipFiles
        {
            get
            {
                return zipFilesField;
            }

            set
            {
                zipFilesField = value;
            }
        }

        public int NumberOfMessages
        {
            get
            {
                return numberOfMessagesField;
            }

            set
            {
                numberOfMessagesField = value;
            }
        }

        #endregion
    }
}