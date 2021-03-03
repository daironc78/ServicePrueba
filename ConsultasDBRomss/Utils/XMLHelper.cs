using System;
using System.Web;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Data;
using System.Configuration;

namespace ConsultasDBRomss.Utils
{
    public class XMLHelper
    {
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

        public string Encriptarmd5(string texto)
        {
            string key = ConfigurationManager.AppSettings["KeyEncript"].ToString();
            byte[] keyArray;
            byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(texto);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
            tdes.Clear();
            texto = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);

            return texto;
        }

        public string Desencriptarmd5(string textoEncriptado)
        {
            string key = ConfigurationManager.AppSettings["KeyEncript"].ToString();
            byte[] keyArray;
            byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);
            tdes.Clear();
            textoEncriptado = UTF8Encoding.UTF8.GetString(resultArray);

            return textoEncriptado;
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

        public String Serialize(DataTable dt)
        {
            MemoryStream str = new MemoryStream();
            dt.WriteXml(str, true);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            String xmlstr;
            xmlstr = sr.ReadToEnd();
            return (xmlstr);
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