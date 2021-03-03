using ConsultasDBRomss.Interfaces;
using ConsultasDBRomss.Models;
using ConsultasDBRomss.Utils;
using System;
using ConsultasDBRomss.Connection;
using System.Data;
using System.Xml;
using System.Web;

namespace ConsultasDBRomss.Services
{
    public class RecordServices : IRecordServices
    {
        public String XMLResponse(String strXML)
        {
            XMLHelper xmlHelper = new XMLHelper();
            ConnectDB con = new ConnectDB();
            String strXMLResponse;
            string validacion = String.Empty;
            try
            {
                XMLRomssQuery XMLRomssQuery = xmlHelper.Deserialize<XMLRomssQuery>(strXML, "ROMSSQUERY");
                String query = XMLScripts(XMLRomssQuery.SQLName);
                validacion = validateObject(XMLRomssQuery);
                DataTable resonseDB = con.processQuery(query, XMLRomssQuery);
                strXMLResponse = xmlHelper.Serialize(resonseDB);
                strXMLResponse = strXMLResponse.Replace("DocumentElement", "DBRomss");
            }
            catch (Exception e)
            {
                Log.save(this, e, validacion);
                strXMLResponse = "Ha ocurrido un error en la serializacion" + e.Message;
            }

            return strXMLResponse;
        }

        private string XMLScripts(string queryRomss)
        {
            XmlDocument XMLDocument = new XmlDocument();
            String Query = String.Empty;
            string Route = HttpContext.Current.Server.MapPath("~/RomssQuerys.xml");
            XMLDocument.Load(Route);
            XmlNodeList RomssQuerys = XMLDocument.GetElementsByTagName("RomssQuerys");
            XmlNodeList Querys = ((XmlElement)RomssQuerys[0]).GetElementsByTagName("querys");
            foreach (XmlElement Script in Querys)
            {
                Query = Script.GetElementsByTagName(queryRomss).Item(0).InnerText;
            }
            return Query;
        }

        private string validateObject(XMLRomssQuery XMLromssQuery)
        {
            String validatorResponse = String.Empty;
            int validados = 0;
            if (XMLromssQuery.SQLName != String.Empty || XMLromssQuery.SQLName != null)
            {
                validados++;
            }
            else
            {
                validatorResponse += "No se ingreso el SQLNAME, ";
            }
            foreach (var item in XMLromssQuery.Params)
            {
                if (item.Name != String.Empty || item.Name != null)
                    validados++;
                else
                    validatorResponse += "No se ingreso el 'Name' nombre parametro, ";

                if (item.Type != String.Empty || item.Type != null)
                    validados++;
                else
                    validatorResponse += "No se ingreso el 'TYPE' Tipo parametro, ";

            }
            if (validados == 3)
                return "OK";
            else
                return validatorResponse;
        }
    }
}