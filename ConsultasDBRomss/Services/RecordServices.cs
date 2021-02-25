using ConsultasDBRomss.Models;
using ConsultasDBRomss.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace ConsultasDBRomss.Services
{
    public class RecordServices
    {
        XMLHelper xmlHelper;
        RomssQuery romssQuery;

        public RecordServices()
        {
            this.xmlHelper = new XMLHelper();
            this.romssQuery = new RomssQuery();
        }

        public String XMLResponse(String strXML)
        {
            XMLRomssQuery XMLRomssQuery = new XMLRomssQuery();
            String strXMLResponse;
            try
            {
                XMLRomssQuery = this.xmlHelper.Deserialize<XMLRomssQuery>(strXML, "ROMSSQUERY");
                if (validateObject(XMLRomssQuery))
                {
                    strXMLResponse = Convert.ToString(romssQuery.FechaInicial);
                }
                else
                {
                    strXMLResponse = "";
                }
            }
            catch (Exception e)
            {
                strXMLResponse  = "Ha ocurrido un error en la serializacion" + e.Message;
            }
            return strXMLResponse;
        }

        private bool validateObject(XMLRomssQuery XMLromssQuery)
        {
            int validados = 0;
            if (XMLromssQuery.SQLName != String.Empty || XMLromssQuery.SQLName != null) {
                romssQuery.SQLName = XMLromssQuery.SQLName;
                validados = validados++;
            }
            if (XMLromssQuery.FechaInicial != String.Empty || XMLromssQuery.FechaInicial != null)
            {
                romssQuery.FechaInicial = Convert.ToDateTime(XMLromssQuery.FechaInicial);
                validados = validados++;
            }
            if (XMLromssQuery.FechaFinal != String.Empty || XMLromssQuery.FechaFinal != null)
            {
                romssQuery.FechaFinal = Convert.ToDateTime(XMLromssQuery.FechaFinal);
                validados = validados++;
            }

            if (validados == 3) 
            { 
                return true; 
            }
            else 
            { 
                return false; 
            }
        }
    }
}