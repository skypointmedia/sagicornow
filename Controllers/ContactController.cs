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
using System.Globalization;


namespace SagicorNow.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index(string fn, string email, string ph, int j, object rc)
        {

            if (fn != null && email != null && ph != null && j != 0 && rc != null)
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
            ContactModel m = (ContactModel)TempData["ContactViewModel"];
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
        public ActionResult SendEmail(ContactModel m)
        {

            string data = Request.Form["g-Recaptcha-Response"];

            if (string.IsNullOrEmpty(data))
            {
                TempData["ContactSendEmailMessage"] = "Please confirm you are not a robot!";
                return RedirectPermanent("~/pages/contact-us");
            }


            if (this.SendContactEmail(m) == "SUCCESS")
            {
                return RedirectPermanent("~/pages/thankyou");
            }
            else
            {
                TempData["ContactSendEmailMessage"] = "Error sending email.";
                TempData["ContactViewModel"] = m;

                if (!String.IsNullOrEmpty(m.comment))
                {
                    return RedirectPermanent("~/pages/contact-us");
                }
                else
                {
                    return RedirectToAction("Contact"); //send back to contact view
                }
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
                string htmlBody = "";
                string subject = "";

                if (!String.IsNullOrEmpty(m.comment))
                {
                    subject = "SagicorNow client ({0}) sent a comment";
                    htmlBody = String.Format("<p>{0}, shared the comment below, on {5} at {6}: </p><p><br /><br /><b>{4}</b></p><p>The client would like to be contacted at {1}, or {2}.</p>", m.firstName, m.email, m.phone, m.state, m.comment,
                        DateTime.Today.ToString("D", CultureInfo.CreateSpecificCulture("en-US")), DateTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                }
                else
                {
                    subject = "SagicorNow client ({0}) unable to complete application online";
                    htmlBody = String.Format("<p>{0}, was unable to complete the SagicorNow application online in {3}, on {5} at {6}.</p><p>The reason for being unable to proceed was:<br /><br /><b>{4}</b></p><p>The client would like a Sagicor Life agent to contact them at {1}, or {2}.</p>", m.firstName, m.email, m.phone, m.state, m.denialMessage,
                        DateTime.Today.ToString("D", CultureInfo.CreateSpecificCulture("en-US")), DateTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                }

                Utils.EmailManager mgr = new Utils.EmailManager();
                mgr.SendEmail(new Utils.SmtpOptions(), //options, use defaults
                              new List<string>() { "sagicornow@sagicorlifeusa.com" }, //recipients
                              "sagicornow@sagicor.com", //from address
                              String.Format(subject, m.firstName), //subject
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
