using System;

namespace ConsultasDBRomss.Models
{
    public class RomssQuery
    {
        public string SQLName { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}