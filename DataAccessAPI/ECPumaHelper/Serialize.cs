using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DataAccessAPI.Models;

namespace DataAccessAPI.ECPumaHelper
{

    /// <summary>
    /// Serialiserer og deserialiserer objekter for lagring eller reprosessering
    /// </summary>
    public class Serialize
    {
        #region ToString

        /// <summary>
        /// Serialiserer ethvert objekt til string som så kan lagres til fil
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05");

            using (StringWriter writer = new StringWriter())
            {
                writer.NewLine = string.Empty;
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }


        public static string ToUTF8String(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            System.IO.Stream stream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter xtWriter = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

          

           // serializer.Serialize(xtWriter, obj);


            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("tns", "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05");
            serializer.Serialize(xtWriter, obj, namespaces);



            xtWriter.Flush();

            stream.Seek(0, System.IO.SeekOrigin.Begin);


            System.IO.StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);

            return reader.ReadToEnd();

        }

        #endregion

    }
}
