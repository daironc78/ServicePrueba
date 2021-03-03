using System;
using System.Web.Mvc;
using ConsultasDBRomss.Auth;
using ConsultasDBRomss.Interfaces;
using ConsultasDBRomss.Services;
using ConsultasDBRomss.Utils;

namespace ConsultasDBRomss.Controllers
{
    [BasicAuthorize]
    public class RomssController : Controller
    {
        String strXMLResponse;
        // GET: Romss
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult ConsultData()
        {
            XMLHelper XMLRequest = new XMLHelper();
            IRecordServices Response = new RecordServices();
            String StrXMLRequest = XMLRequest.GetRequestContentAsString(Request);
            try
            {
                strXMLResponse = Response.XMLResponse(StrXMLRequest);
            }
            catch (Exception e)
            {
                Log.save(this, e, null);
                strXMLResponse = "Error en los parametros de envio" + e.Message;
            }

            return Content(strXMLResponse);
        }

        public ActionResult Decode()
        {
            try
            {
                XMLHelper XMLRequest = new XMLHelper();
                String StrXMLRequest = XMLRequest.GetRequestContentAsString(Request);
                String StrDecode = XMLRequest.Desencriptarmd5(StrXMLRequest);
                return Content(StrDecode);
            }
            catch (Exception e)
            {
                return Content("Error al decodificar " + e);
            }

        }

        public ActionResult Encode()
        {
            try
            {
                XMLHelper XMLRequest = new XMLHelper();
                String StrXMLRequest = XMLRequest.GetRequestContentAsString(Request);
                String StrDecode = XMLRequest.Encriptarmd5(StrXMLRequest);
                return Content(StrDecode);
            }
            catch (Exception e)
            {
                return Content("Error al decodificar " + e);
            }
        }
    }
}