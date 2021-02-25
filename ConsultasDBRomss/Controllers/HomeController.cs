using System;
using System.Web.Mvc;
using ConsultasDBRomss.Interfaces;
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
            IRecordServices Response = new RecordServices();
            String StrXMLRequest = XMLRequest.GetRequestContentAsString(Request);
            try
            {
                strXMLResponse = Response.XMLResponse(StrXMLRequest);
            }
            catch(Exception e)
            {
                Log.save(this, e, null);
                strXMLResponse = "Error en los parametros de envio" + e.Message;
            }

            return Content(strXMLResponse);
        }

    }
}
