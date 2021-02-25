using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace ConsultasDBRomss.Utils
{
    /// <summary>
    /// Clase de Logs, esta clase se usar para registrar los logs en la carpeta log dentro del archivo que tendra el nombre de la fecha actual .txt
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Metodo de Guardado de Logs
        /// </summary>
        /// <param name="obj">Objeto actual en el que se llama el log</param>
        /// <param name="ex">Excepción de error puede ser nulo</param>
        /// <param name="Message">Mensaje del log puede ser nulo</param>
        public static void save(object obj, Exception ex, String Message)
        {
            String date = DateTime.Now.ToString("dd-MM-yyyy");
            String time = DateTime.Now.ToString("HH:mm:ss");
            String path = HttpContext.Current.Request.MapPath("~/log/" + date + ".txt");

            StreamWriter sw = new StreamWriter(path, true);

            StackTrace stackTrace = new StackTrace();
            sw.WriteLine(obj.GetType().FullName + "\t" + time);

            if (ex != null)
            {
                sw.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);
                sw.WriteLine("Message: " + ex.Message);
                sw.WriteLine("StackTrace" + ex.StackTrace + "\n");
            }
            else
            {
                sw.WriteLine(stackTrace.GetFrame(1).GetMethod().Name + " - " + Message + "\n");
            }

            sw.Flush();
            sw.Close();
        }
    }
}