using System;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace ConsultasDBRomss.Utils
{
    public class Log
    {
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