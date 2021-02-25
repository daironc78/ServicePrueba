using System;
using System.Web.Mvc;
using ConsultasDBRomss.Services;
using ConsultasDBRomss.Utils;

namespace ConsultasDBRomss.Controllers
{
    public class HomeController : Controller
    {
        String strXMLResponse;

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Romss() 
        {

            XMLHelper XMLRequest = new XMLHelper();
            RecordServices Response = new RecordServices();
            String StrXMLRequest = XMLRequest.GetRequestContentAsString(Request);
            try
            {
                strXMLResponse = Response.XMLResponse(StrXMLRequest.ToUpper());
                //strXMLResponse = Response.XMLResponse(StrXMLRequest.ToUpper());
            }
            catch(Exception e)
            {
                strXMLResponse = "Error en los parametros de envio" + e.Message;
            }

            return Content(strXMLResponse);
        }

    }
}
