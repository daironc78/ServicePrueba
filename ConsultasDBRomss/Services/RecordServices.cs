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
        string MessageError = String.Empty;
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
                if (query != String.Empty)
                {
                    validacion = validateObject(XMLRomssQuery);
                    DataTable resonseDB = con.processQuery(query, XMLRomssQuery);
                    if (resonseDB.Rows.Count > 0)
                    {
                        strXMLResponse = xmlHelper.Serialize(resonseDB);
                        strXMLResponse = strXMLResponse.Replace("DocumentElement", "DBRomss");
                    }
                    else
                    {
                        strXMLResponse = "<DBRomss></DBRomss>";
                    }
                }
                else
                {
                    strXMLResponse = "<DBRomss><CodeError>-1</CodeError><MessageError>" + MessageError + "</MessageError></DBRomss>";
                }
            }
            catch (Exception e)
            {
                Log.save(this, e, validacion);
                strXMLResponse = "<DBRomss><CodeError>-1</CodeError><MessageError>ValidacionMessage: " + validacion + ", Error Message: " + e.Message + "</MessageError></DBRomss>";
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
            try
            {
                foreach (XmlElement Script in Querys)
                    Query = Script.GetElementsByTagName(queryRomss).Item(0).InnerText;
            }
            catch(Exception e)
            {
                Query = String.Empty;
                MessageError = "Error al leer el item '" + queryRomss + "' en el archivo 'RomssQuerys.xml'";
                Log.save(this, e, "Error al leer el item '" + queryRomss + "' en el archivo 'RomssQuerys.xml'");
            }

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
                int posicion = 1;
                if (item.Name == String.Empty || item.Name == null)
                {
                    invalidadosParam++;
                    validatorResponse += "No se ingreso el 'NAME' nombre parametro PARAM: " + posicion + ", ";
                }
                else
                {
                    validados++;
                }

                if (item.Type == String.Empty || item.Type == null)
                {
                    invalidadosParam++;
                    validatorResponse += "No se ingreso el 'TYPE' Tipo parametro, PARAM: " + posicion + ", ";
                }
                else
                {
                    validados++;
                }

                posicion++;
            }

            if (invalidadosParam < 0)
            {
                return "OK";
            }
            else
            {
                return validatorResponse;
            }
        }
    }
}