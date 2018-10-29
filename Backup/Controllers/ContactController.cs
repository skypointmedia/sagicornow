using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SagicorNow.ViewModels;
using System.Web.Mvc;
using SagicorNow.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;

namespace SagicorNow.Controllers
{
    public class ContactController : Controller
    {
		public ActionResult Index(string fn, string email, string ph, int j)
		{

            if (fn != null && email != null && ph != null && j != 0)
            {
                ContactModel cmodl = new ContactModel()
                {
                    firstName = fn,
                    email = email,
                    phone = ph,
                    state = (QuoteViewModel.GetStateInfoFromTC(j) == null ? "" : QuoteViewModel.GetStateInfoFromTC(j).Name),
                    denialMessage = BusinessRulesClass.MedicalRejectMessage()
                };

                //data being posted from I.T.
                this.SendContactEmail(cmodl);
                return RedirectPermanent("~/pages/thankyou");
            }
            else
            {
                return View("Contact");
            }
		}

        public ActionResult Contact()
		{
            ContactViewModel m = (ContactViewModel)TempData["ContactViewModel"];
            if (m != null)
            {
				return View(m);
            }
            else
            {
                return View();
            }
		}

        [HttpPost]
        [HttpGet]
        public ActionResult SendEmail(ContactModel m)
        {
            if (this.SendContactEmail(m) == "SUCCESS")
            {
                return RedirectPermanent("~/pages/thankyou");
            }
            else
            {
                return View("Contact", m); //send back to contact view
            }

        }

        /// <summary>
        /// Sends the contact email.
        /// </summary>
        /// <returns>The contact email.</returns>
        /// <param name="m">M.</param>
        private string SendContactEmail(ContactModel m)
        {
			try
			{
				string htmlBody = "<p>{0}, was unable to complete the SagicorNow application online in {3}, on {5} at {6}.</p><p>The reason for being unable to proceed was:<br /><br /><b>{4}</b></p><p>The client would like a Sagicor Life agent to contact them at {1}, or {2}.</p>";

				Utils.EmailManager mgr = new Utils.EmailManager();
				mgr.SendEmail(new Utils.SmtpOptions(), //options, use defaults
							  new List<string>() { "melinda_mcclean@Sagicor.com", "courtney_groves@sagicor.com" }, //recipients
							  "sagicornow@sagicor.com", //from address
							  String.Format("SagicorNow client ({0}) unable to complete application online", m.firstName), //subject
							  "", //plain text?
							  htmlBody //html text
							 );

                return "SUCCESS";

			}
			catch (Exception ex)
			{
				ViewBag.Status = "Problem while sending email, Please check details." + ex.Message;
                return "ERROR";
			}
        }

    }
}
