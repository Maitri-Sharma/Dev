using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.DataLayer.Helper
{
   public class Utils
    {
        /// <summary>
        ///     ''' Get Enum From Data row
        ///     ''' </summary>
        ///     ''' <param name="row"></param>
        ///     ''' <param name="columnName"></param>
        ///     ''' <param name="enumType"></param>
        ///     ''' <param name="nullName"></param>
        ///     ''' <returns></returns>

        public object GetEnumFromNameFromRow(DataRow row, string columnName, Type enumType, string nullName)
        {
            //string name =Convert.ToString(row[columnName]) + ""+ nullName;
            string name = "";
            if (Convert.ToString(row[columnName]) == "")
                name = nullName;
            else
                name = Convert.ToString(row[columnName]);
            string[] enumNames = System.Enum.GetNames(enumType);
            // if (Array.IndexOf(enumNames, name) <= 1)
            if (Array.IndexOf(enumNames, name) < -1)
                throw new Exception("Value named " + name + " from column " + columnName + " in table " + row.Table.TableName + " is not a valid value name for enumeration " + enumType.FullName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (String.IsNullOrWhiteSpace(name))
                return null;
            return System.Enum.Parse(enumType, name);
        }


        //public  object GetEnumFromRow(DataRow row, string columnName, Type enumType)
        //{
        //    int i = GetIntFromRow(row, columnName);
        //    if (!System.Enum.IsDefined(enumType, i))
        //        throw new Exception("Value " + i + " from column " + columnName + " in table " + row.Table.TableName + " is not a valid value for enumeration " + enumType.FullName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
        //    return System.Enum.ToObject(enumType, i);
        //}

        public int GetIntFromRow(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnName))
                return 0;
            try
            {
                //return Convert.ToInt32(row.ItemArray[0]);
                return Convert.ToInt32(row[columnName]);
            }
            catch (Exception ex)
            {
                throw new Exception("Column " + columnName + " is not an integer." + "Det oppsto en feil ved uthenting av data fra databasen." + ex.Message);
            }
        }


        public int GetIntFromRow(DataRow row, int columnIndex)
        {
            if (columnIndex < 0 | columnIndex > row.Table.Columns.Count - 1)
                throw new Exception("Columnindex on " + columnIndex + " is out of bounds for the table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnIndex))
                return 0;
            try
            {
                //return row.Item(columnIndex);
                //return Convert.ToInt32(row.ItemArray[0]);
                return Convert.ToInt32(row.Field<int>(columnIndex));
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("The column on index " + columnIndex + " is not an integer." + ex + "Det oppsto en feil ved uthenting av data fra databasen.");
            }
        }


        public double GetDoubleFromRow(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnName))
                return 0;
            try
            {
                return double.MaxValue;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("Column " + columnName + " is not a double." + ex + " Det oppsto en feil ved uthenting av data fra databasen.");
            }
        }


        public DateTime GetDateFromRow(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnName))
                return DateTime.MinValue;
            try
            {
                // return row.Item(columnName);
                return DateTime.MinValue;

            }
            catch (InvalidCastException ex)
            {
                throw new Exception("Column " + columnName + " is not a date." + ex + "Det oppsto en feil ved uthenting av data fra databasen.");
            }
        }

        public bool GetBooleanFromRow(DataRow row, string columnName)
        {
            return GetIntFromRow(row, columnName) != 0;
        }
        public long GetLongFromRow(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnName))
                return 0;
            try
            {
                return Convert.ToInt64(row.Field<decimal>(columnName));
                //return 0;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("Column " + columnName + " is not a long integer." + ex + "Det oppsto en feil ved uthenting av data fra databasen.");
            }
        }

        public string GetStringFromRow(DataRow row, string columnName, string nullValue = null, bool returnNullValueOnColumnNotFound = false)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                if (returnNullValueOnColumnNotFound)
                    return nullValue;
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            }
            if (row.IsNull(columnName))
                return nullValue;
            return Convert.ToString(row.Field<string>(columnName));
        }

        public string GetStringFromRow(DataRow row, int columnIndex, string nullValue = null, bool returnNullValueOnColumnNotFound = false)
        {
            if (columnIndex < 0 | columnIndex > row.Table.Columns.Count - 1)
            {
                if (returnNullValueOnColumnNotFound)
                    return nullValue;
                throw new Exception("Columnindex on " + columnIndex + " is out of bounds for the table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            }
            if (row.IsNull(columnIndex))
                return nullValue;
            //return row.ItemArray(columnIndex).ToString();
            return Convert.ToString(row.ItemArray[0]);
        }


        public DateTime GetTimestampFromRow(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new Exception("Column " + columnName + " does not exist in table " + row.Table.TableName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            if (row.IsNull(columnName))
                return DateTime.MinValue;
            try
            {
                return Convert.ToDateTime(row.Field<DateTime>(columnName));

            }
            catch (InvalidCastException ex)
            {
                throw new Exception("Column " + columnName + " is not a datetime." + "Det oppsto en feil ved uthenting av data fra databasen." + ex.Message);
            }
        }

        //public  CampaignDescription GetCampaignDescriptionFromListDataRow(DataRow row, bool isDisconnected)
        //{
        //    CampaignDescription cd = new CampaignDescription();
        //    Utils utils = new Utils();
        //    cd.ID = utils.GetIntFromRow(row, "UtvalglistId");
        //    cd.Name = utils.GetStringFromRow(row, "UtvalglistName");
        //    cd.OrdreType = (OrdreType)utils.GetEnumFromNameFromRow(row, "OrdreType", typeof(OrdreType), "Null");
        //    cd.OrdreStatus = (OrdreStatus)utils.GetEnumFromNameFromRow(row, "OrdreStatus", typeof(OrdreStatus), "Null");
        //    cd.DistributionDate = utils.GetDateFromRow(row, "Distribusjonsdato");
        //    cd.IsDisconnected = isDisconnected;
        //    return cd;
        //}

        public string CreateSearchString(string searchString, SearchMethod searchMethod)
        {
            searchString = searchString.Replace("%", "");
            searchString = searchString.Replace(@"\", @"\\");
            searchString = searchString.Replace("_", @"\_");
            if (searchMethod == SearchMethod.ContainsIgnoreCase)
                searchString = "%" + searchString + "%";
            if (searchMethod == SearchMethod.StartsWithIgnoreCase)
                searchString = "%" + searchString;
            if (searchMethod == SearchMethod.EqualsIgnoreCase)
            {
                string seaString;
                seaString = searchString.ToUpper();
                return seaString;
            }
            searchString = searchString.ToUpper();
            return searchString;
        }
        public string GetCustomerNoSearchString(string[] arr, SearchMethod searchMethod)
        {
            StringBuilder strB = new StringBuilder();
            string sql = " KUNDENUMMER LIKE '{0}' OR ";
            string sqlLast = " KUNDENUMMER LIKE '{0}' ";
            int i = 1;
            if (arr != null && arr.Length > 0)
            {
                foreach (string astr in arr)
                {
                    if (i < arr.Length)
                        strB.Append(string.Format(sql, CreateSearchString(astr, searchMethod)));
                    else
                        strB.Append(string.Format(sqlLast, CreateSearchString(astr, searchMethod)));
                    i += 1;
                }
            }
            else
            {
                strB.Append(string.Format(sqlLast, "%")); strB.Append(" OR KUNDENUMMER is null ");
            }

            return strB.ToString();
        }

        /// <summary>
        ///     ''' Used with customer web. A customer has access to a set of agreement numbers. This method builds a sql-string
        ///     ''' with these numbers. If no numbers is sent in, it returns a false statement which means that no hits on agreement numbers
        ///     ''' will occur.
        ///     ''' </summary>
        ///     ''' <param name="arr"></param>
        ///     ''' <returns>Sql-string witn numbers og a false statement if no numbers are sent in</returns>
        ///     ''' <remarks></remarks>
        public string GetAgreementNoSearchString(int[] arr)
        {
            string strFinalSQL = " 1 = 2 "; // To prevent hits on wrong agreement numbers if none is sent in
            StringBuilder strB = new StringBuilder();
            string sql = " AVTALENUMMER IN ({0}) ";

            int i = 1;
            if (arr != null && arr.Length > 0)
            {
                foreach (int no in arr)
                {
                    if (no > 0)
                    {
                        if (i < arr.Length)
                            strB.Append(no.ToString() + ",");
                        else
                            strB.Append(no.ToString());
                    }
                    i += 1;
                }

                strFinalSQL = strB.ToString();
                if (strFinalSQL.EndsWith(","))
                    strFinalSQL = strFinalSQL.Remove(strFinalSQL.Length - 1);
                if (strFinalSQL.Length != 0)
                    strFinalSQL = string.Format(sql, strFinalSQL);
            }

            return strFinalSQL;
        }
        public bool CanSearch(string[] customerNos, int[] agreementNos)
        {
            if (customerNos != null)
            {
                foreach (string custNo in customerNos)
                {
                    if (!string.IsNullOrEmpty(custNo))
                        return true;
                }
            }
            if (agreementNos != null)
            {
                foreach (int agrNo in agreementNos)
                {
                    if (agrNo > 0)
                        return true;
                }
            }
            return false;
        }

        public object GetEnumFromRow(DataRow row, string columnName, Type enumType)
        {
            int i = GetIntFromRow(row, columnName);
            if (!System.Enum.IsDefined(enumType, i))
                throw new Exception("Value " + i + " from column " + columnName + " in table " + row.Table.TableName + " is not a valid value for enumeration " + enumType.FullName + "." + "Det oppsto en feil ved uthenting av data fra databasen.");
            return System.Enum.ToObject(enumType, i);
        }
    }
}
