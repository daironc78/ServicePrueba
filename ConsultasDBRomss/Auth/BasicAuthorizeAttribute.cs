using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web.Configuration;
using ConsultasDBRomss.Utils;

namespace ConsultasDBRomss.Auth
{
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentException("filterContext");

            string auth = filterContext.HttpContext.Request.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                byte[] encodeDataAsBytes = Convert.FromBase64String(auth.Replace("Basic ", ""));
                string value = Encoding.ASCII.GetString(encodeDataAsBytes);
                string username = value.Substring(0, value.IndexOf(':'));
                string password = value.Substring(value.IndexOf(':') + 1);

                if (validateUser(username, password))
                {
                    filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(username), null);
                }
                else
                {
                    filterContext.Result = new ContentResult();
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusDescription = "Unauthorized";
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.Write("401, please authenticate");
                    filterContext.HttpContext.Response.End();
                }
            }
            else
            {
                if (AuthorizeCore(filterContext.HttpContext))
                {
                    HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                    cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                    cachePolicy.AddValidationCallback(CacheValidateHandler, null);
                }
                else
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusDescription = "Unauthorized";
                    filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                    filterContext.HttpContext.Response.Write("401, please authenticate");
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new EmptyResult();
                    filterContext.HttpContext.Response.End();
                }
            }
            base.OnAuthorization(filterContext);
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
        
        public bool validateUser(string user, string password)
        {
            XMLHelper utils = new XMLHelper();
            string username = WebConfigurationManager.AppSettings["USER"];
            string strPass = utils.Desencriptarmd5(WebConfigurationManager.AppSettings["PASS"]);
            return user == username && password == strPass;
        }
    }
}