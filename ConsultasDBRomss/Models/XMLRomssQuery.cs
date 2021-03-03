using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConsultasDBRomss.Models
{
    [XmlRoot("ROMSSQUERY")]
    public class XMLRomssQuery
    {
        [XmlElement("SQLNAME")]
        public String SQLName { get; set; }
        [XmlElement("PARAM")]
        public List<Params> Params = new List<Params>();
    }
}