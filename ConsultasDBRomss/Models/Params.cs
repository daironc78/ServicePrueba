using System;
using System.Xml.Serialization;

namespace ConsultasDBRomss.Models
{
    public class Params
    {
        [XmlElement("NAME")]
        public String Name { get; set; }
        [XmlElement("TYPE")]
        public String Type { get; set; }
        [XmlElement("VALUE")]
        public String Value { get; set; }
    }
}