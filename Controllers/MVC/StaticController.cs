using SagicorNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SagicorNow.Controllers
{
    public class StaticController : Controller
    {
        public ActionResult Index()
        {
            return View ();
        }

        public ActionResult GetStaticContent(string page)
		{
            page = String.IsNullOrEmpty(page) ? "homepage" : page;
            if (page.ToLower() == "contact-us" && TempData["ContactViewModel"] != null)
            {
                return View("contact-us", (ContactModel)TempData["ContactViewModel"]); //return contact view with model 
            }
            else
            {
                /*
                //route D2C caribbean traffic 
                List<string> caribbeanCountryCodes = new List<string>() {
                    "BD", //Barbados
                    "AW", //Aruba
                    "AG", //Antigua and Barbuda
                    "GD", //Grenada
                    "BZ", //Belize 
                    "LC", //Saint Lucia
                    "DM", //Dominica
                    "VC", //Saint Vincent and The Grenadines
                    "KN", //Saint Kitts and Nevis
                    "TT", //Trinidad and Tobago
                    "CW"  //Curacao
                };
                Location loc = base.CurrentLocation;
                if (null != loc && caribbeanCountryCodes.Contains(loc.countryCode))
                    return RedirectPermanent("http://caribbean.sagicornow.com");
                */

                return View(page.ToLower()); //always name static pages in all lower case
            }
            
		}

    }
}
