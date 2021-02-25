using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ConsultasDBRomss.Models
{
    [XmlRoot("ROMSSQUERY")]
    public class XMLRomssQuery
    {
        [XmlElement("SQLNAME")]
        public String SQLName { get; set; }
        [XmlElement("FECHAINICIAL")]
        public String FechaInicial { get; set; }
        [XmlElement("FECHAFINAL")]
        public String FechaFinal { get; set; }
    }
}