using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ConsultasDBRomss.Utils
{
    public class XMLHelper
    {
        public XMLHelper() { }

        public String GetRequestContentAsString(HttpRequestBase request)
        {
            using (var reciveStream = request.InputStream)
            {
                using (var readStream = new StreamReader(reciveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }

        public string decodeDataByBase64(String str)
        {
            byte[] data = Convert.FromBase64String(str);
            return ASCIIEncoding.ASCII.GetString(data);
        }

        public string encodeBase64(String str)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(str);
            return Convert.ToBase64String(data);
        }

        public T Deserialize<T>(string input, String xmlRoot) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRoot));

            using (StringReader sr = new StringReader(input))
            {
                try
                {
                    return (T)ser.Deserialize(sr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new Utf8StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }


        public class Utf8StringWriter : StringWriter { public override Encoding Encoding { get { return Encoding.UTF8; } } }
    }
}