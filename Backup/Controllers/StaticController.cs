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
            return View(page.ToLower()); //always name static pages in all lower case
		}

    }
}
