using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace WIFSampleAccenture.Controllers
{
    [ValidateInput(false)]
    public class HomeController : Controller
    {
        [RequireHttps]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            if (Request.Form.Get(WSFederationConstants.Parameters.Result) != null)
            {
                SignInResponseMessage message =
                    WSFederationMessage.CreateFromFormPost(System.Web.HttpContext.Current.Request) as SignInResponseMessage;

                XmlDocument signInResponseXml = new XmlDocument();
                signInResponseXml.LoadXml(message.Result);

                var securityTokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();

                string samlTokenXml = signInResponseXml
               .DocumentElement  // <trust:RequestSecurityTokenResponseCollection>
               .ChildNodes[0] // <trust:RequestSecurityTokenResponse>
               .ChildNodes[2] // <trust:RequestedSecurityToken>
               .InnerXml; // <Assertion>

                var xmlTextReader = new XmlTextReader(new StringReader(samlTokenXml));

                // read the token
                SecurityToken securityToken = securityTokenHandlers.ReadToken(xmlTextReader);

                string str = string.Empty;
                str += "Valid From: " + securityToken.ValidFrom;
                str += "Valid Till: " + securityToken.ValidTo;
                str += "Id: " + securityToken.Id;
               
            }

            return View();
        }



        [RequireHttps]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
