using ConsultasDBRomss.Interfaces;
using ConsultasDBRomss.Models;
using ConsultasDBRomss.Utils;
using System;
using ConsultasDBRomss.Connection;
using System.Data;
using System.Data.SqlClient;

namespace ConsultasDBRomss.Services
{
    public class RecordServices : IRecordServices
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
                    ConnectDB con = new ConnectDB();
                    IDataParameter[] parameters = new IDataParameter[]
                    {
                        new SqlParameter("@DateIni", SqlDbType.DateTime) { Value = Convert.ToDateTime(XMLRomssQuery.FechaInicial) },
                        new SqlParameter("@DateFin", SqlDbType.DateTime) { Value = Convert.ToDateTime(XMLRomssQuery.FechaFinal) }
                    };
                    SqlCommand response = con.processProcedure(XMLRomssQuery.SQLName, parameters);
                    strXMLResponse = "";
                }
                else
                {
                    strXMLResponse = "";
                }
            }
            catch (Exception e)
            {
                Log.save(this, e, null);
                strXMLResponse  = "Ha ocurrido un error en la serializacion" + e.Message;
            }

            return strXMLResponse;
        }

        private bool validateObject(XMLRomssQuery XMLromssQuery)
        {
            int validados = 0;
            if (XMLromssQuery.SQLName != String.Empty || XMLromssQuery.SQLName != null) { validados++; }
            if (XMLromssQuery.FechaInicial != String.Empty || XMLromssQuery.FechaInicial != null) { validados++; }
            if (XMLromssQuery.FechaFinal != String.Empty || XMLromssQuery.FechaFinal != null) { validados++; }

            if (validados == 3) { return true; }
            else { return false; }
        }
    }
}