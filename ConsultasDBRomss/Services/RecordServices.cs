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
                Query = Script.GetElementsByTagName(queryRomss).Item(0).InnerText;

            return Query;
        }

        private string validateObject(XMLRomssQuery XMLromssQuery)
        {
            String validatorResponse = String.Empty;
            int validados = 0;
            int invalidadosParam = 0;
            if (XMLromssQuery.SQLName == String.Empty || XMLromssQuery.SQLName == null)
                validatorResponse += "No se ingreso el SQLNAME, ";
            else
                validados++;

            foreach (var item in XMLromssQuery.Params)
            {
                if (item.Name == String.Empty || item.Name == null)
                {
                    invalidadosParam++;
                    validatorResponse += "No se ingreso el 'Name' nombre parametro, ";
                }
                else
                    validados++;

                if (item.Type == String.Empty || item.Type == null)
                {
                    invalidadosParam++;
                    validatorResponse += "No se ingreso el 'TYPE' Tipo parametro, ";
                }
                else
                    validados++;
            }
            if (invalidadosParam < 0)
                return "OK";
            else
                return validatorResponse;
        }
    }
}