using SagicorNow.Models;
using SagicorNow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Sagicor.Core.Common.Contracts;
using SagicorNow.Business;
using SagicorNow.Business.Models;
using SagicorNow.Client.Contracts;
using SagicorNow.Common;
using SagicorNow.Data;
using SagicorNow.Foresight;
using SagicorNow.Properties;

namespace SagicorNow.Controllers
{

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuoteController : FirelightAwareControllerBase
    {
        
       [ImportingConstructor]
        public QuoteController(/*IProcessTXLifeRequestClient processTxLifeRequestClient*/)
        {
            //_serviceFactory= serviceFactory;
            //_processTxLifeRequestClient = processTxLifeRequestClient;
        }

        private readonly SageNowContext _db = new SageNowContext();
        //private readonly IServiceFactory _serviceFactory;
        //private readonly IProcessTXLifeRequestClient _processTxLifeRequestClient;

        protected override void RegisterServices(List<IServiceContract> disposableServices)
        {
            //disposableServices.Add(_processTxLifeRequestClient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new QuoteViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(QuoteViewModel vm)
        {
            try
            {
                //if coverage supplied, limit coverage to max of 1,000,000 based on age
                var maxCoverage = QuoteViewModel.GetMaxCoverageBasedOnAge(vm.Age);

                vm.CoverageAmount = vm.CoverageAmount > maxCoverage ? maxCoverage : vm.CoverageAmount; //update coverage based on max 
                                                                                                       //vm.SocialSecurityNumber = vm.SocialSecurityNumber.Replace("-", String.Empty);

                //override coverage based on risk class
                if (vm.Age < 56 && vm.CoverageAmount < 525000M && (vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED_TOBACCO ||
                    vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_NONTOBACCO ||
                    vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_TOBACCO))
                {
                    vm.CoverageAmount = 525000M;
                }

                // Determine eligibility
                var eligibility = BusinessRules.IsEligible(vm.Age, vm.stateInfo.Name, vm.tobacco, vm.health, vm.ReplacementPolicy ?? false);

                // If eligible build XML and send to firelight
                if (eligibility.IsEligible)
                {
                    // Build XMLs
                    var client = new NewBusinessService.NewBusinessClient("CustomBinding_NewBusiness");
                    var addrUri = client.Endpoint.Address.Uri;
                    var newUri = string.Format(addrUri.ToString(), Session["GCId"] ?? "");

                    var newAddress = new System.ServiceModel.EndpointAddress(newUri);
                    client.Endpoint.Address = newAddress;

                    var user1228Str = Create1228(vm);
                    string accessToken;

                    if (FirelightTokeIsNoneExistentOrExpired())
                    {
                        var token = await GetFirelightTokenAsync(user1228Str);
                        var tokenReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightTokenReturn>(token);
                        accessToken = tokenReturn.access_token;
                    }
                    else
                    {
                        accessToken = FirelightAccessToken;
                    }



                    var evm = new EmbeddedViewModel {
                        AccessToken = accessToken,
                        //AccessToken = FirelightAccessToken,
                        FirelightBaseUrl = FireLightSession.BaseUrl,
                        IsNew = vm.IsNewProposal
                    };

                    var proposalHistory = _db.ProposalHistories.Find(vm.SocialSecurityNumber);

                    if (vm.IsNewProposal)
                    {
                        var activityRequestApiString = this.CreateEAppActivity(accessToken, "26", vm); // 26 = Sage Term 
                        var activityReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightActivityReturn>(activityRequestApiString);

                        //if (proposalHistory == null)
                        //{
                        //    proposalHistory = new ProposalHistory
                        //        { SSN = vm.SocialSecurityNumber, ActivityId = activityReturn.ActivityId };

                        //    _db.ProposalHistories.Add(proposalHistory);
                        //}
                        //else
                        //{
                        //    proposalHistory.ActivityId = activityReturn.ActivityId;
                        //    proposalHistory.LastActiveDateTime = DateTime.Now;
                        //}
                        evm.ActivityId = activityReturn.ActivityId;
                    }
                    else
                    {
                        evm.ActivityId = proposalHistory.ActivityId;
                        proposalHistory.LastActiveDateTime = DateTime.Now;
                    }

                    //_db.SaveChanges();

                    return View("EmbeddedApp", evm);
                }

                TempData["ContactViewModel"] = new ContactModel { denialMessage = eligibility.EligibilityMessage, isReplacementReject = eligibility.IsReplacememtReject, state = eligibility.State };
                return RedirectToActionPermanent("Contact", "Contact");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var msg = "There was an error connecting to the application portal.";
                vm.ViewMessages.Add(msg);
                return View(vm);
            }
        }


        public ViewResult ProductSlider(QuoteViewModel quoteViewModel)
        {
            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;
           
            return View("FraudWarning");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForesightQuoteRequest(QuoteViewModel model)
        {
            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;
            ViewBag.QuoteViewModel = model;

            var soapDocument = ForesightServiceHelpers.GenerateRequestXml(model.smokerStatusInfo, model.genderInfo,
                model.birthday, model.CoverageAmount);
         
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(FireLightSession.ForeSightUrl);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "application/soap+xml; charset=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            using (Stream stream = webRequest.GetRequestStream()){
                soapDocument.Save(stream);
            }
            
            using (WebResponse response = webRequest.GetResponse()){
                using (StreamReader rd = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException("The response object is null.")))
                {
                    //var xmlDocument = XDocument.Load(XmlReader.Create(rd));
                    //var soapResult = rd.ReadToEnd();

                    var txLife = ForesightServiceHelpers.ExtractTxLife(rd);

                    return View("ProductSlider", txLife.TxLifeResponse);
                }
            }
        }

        [HttpPost]
        public JsonResult GetFraudWarningUrlData(ProductSliderModel productSliderModel, QuoteViewModel quoteViewModel)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ProductSlider",new{productSliderModel, quoteViewModel });
            return Json(new {Url = redirectUrl});
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult QuoteFromNeeds()
        {
            QuoteViewModel vm = new QuoteViewModel();

            if (TempData["QuoteViewModelFromNeeds"] != null)
                vm = (QuoteViewModel)TempData["QuoteViewModelFromNeeds"];

            return View("Index", vm); //return view with model
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socialSecurityNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ProposalExist(string socialSecurityNumber)
        {
            var ssn = socialSecurityNumber.Replace("-", string.Empty);
            var exist = _db.ProposalHistories.Any(record => record.SSN == ssn);
            return Json(exist,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socialSecurityNumber"></param>
        /// <returns></returns>
        public PartialViewResult ConfirmContinue(string socialSecurityNumber)
        {
            var ssn = socialSecurityNumber.Replace("-", string.Empty);
            var model = _db.ProposalHistories.Find(ssn);

            return PartialView("_QuoteModal", model);
        }

        

        /// <summary>
        /// creates a 1228 request XML
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public string Create1228(QuoteViewModel vm)
        {
            var builder = new StringBuilder();
            builder.Append("<TXLife xmlns=\"http://ACORD.org/Standards/Life/2\">");
            builder.Append("<TXLifeRequest>");
            builder.Append("<TransRefGUID>"+Guid.NewGuid()+"</TransRefGUID>");
            builder.Append("<TransType tc=\"1228\">OLI_TRANS_TRNPRODINQ</TransType>");
            builder.Append("<OLifE>");
            builder.Append("<SourceInfo>");
            builder.Append("<CreationDate>"+DateTime.Today.ToString("yyyy-MM-dd")+"</CreationDate>");
            builder.Append("<CreationTime>"+DateTime.Now.ToString("hh:mm:ss.fffffffzzz")+"</CreationTime>");
            builder.Append("<SourceInfoName>"+FireLightSession.SagOrgId+"</SourceInfoName>");
            builder.Append("</SourceInfo>");
            builder.Append("<Party id=\""+ FireLightSession.AgentPartyId+ "\">");
            builder.Append("<PartyTypeCode tc=\"1\">OLI_PT_Person</PartyTypeCode>");
            builder.Append("<FullName>Sagicor D2C</FullName>");
            builder.Append("<EmailAddress>");
            builder.Append("<AddrLine>sagicornow@sagicorlifeusa.com</AddrLine>");
            builder.Append("</EmailAddress>");
            builder.Append("<Producer>");
            builder.Append("<CarrierAppointment PartyID=\""+ FireLightSession.AgentPartyId + "\">");
            builder.Append("<CompanyProducerID>"+ FireLightSession.ProducerId +"</CompanyProducerID>");
            builder.Append("<CarrierCode>"+ FireLightSession.SagCarrierCode + "</CarrierCode>");
            builder.Append("</CarrierAppointment>");
            builder.Append("</Producer>");
            builder.Append("</Party>");
            builder.Append("<OLifEExtension VendorCode=\"25\">");
            builder.Append("<UserRoleCode tc=\"InsTech\">InsTech</UserRoleCode>");
            builder.Append("</OLifEExtension>");
            builder.Append("<Relation OriginatingObjectID=\""+ FireLightSession.AgentPartyId + "\">");
            builder.Append("<RelationRoleCode tc=\"11\">OLI_REL_AGENT</RelationRoleCode>");
            builder.Append("</Relation>");
            builder.Append("</OLifE>");
            builder.Append("</TXLifeRequest>");
            builder.Append("</TXLife>");

            return builder.ToString();
        }

        #region Helpers

        /// <summary>
        /// Converts a SOAP string to an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="soap">SOAP string</param>
        /// <returns>The object of the specified type</returns>
        public static T SoapToObject<T>(string soap)
        {
            if (string.IsNullOrEmpty(soap)){
                throw new ArgumentException("SOAP can not be null/empty");
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(soap))){
                var formatter = new SoapFormatter();
                return (T) formatter.Deserialize(stream);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //webRequest.Headers.Add("SOAPAction", action);
            webRequest.Headers.Add("Action", "http://ACORD.org/Standards/Life/2/ProcessTXLifeRequest/ProcessTXLifeRequest");
            webRequest.Headers.Add("MessageID", Guid.NewGuid().ToString());
            webRequest.Headers.Add("ReplyTo", "<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            webRequest.Headers.Add("To", "https://illustration.test.sagicorlifeusa.com/SLI6/Core/Acord/TXLifeService.svc");
            webRequest.ContentType = "application/soap+xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private string CreateEAppActivity(string token, string cusip, QuoteViewModel vm)
        {
            var reqId = Guid.NewGuid().ToString();

            var actBody = new FirelightActivityBody {
                Id = reqId,
                CUSIP = cusip,
                Jurisdiction = vm.stateInfo.TC,
                CarrierCode = FireLightSession.SagCarrierCode,
                TransactionType = 1,
                DataItems = new List<FirelightActivityDataItem>
                {
                    new FirelightActivityDataItem { DataItemId = "Owner_NonNaturalName", Value = $"" },
                    new FirelightActivityDataItem { DataItemId = "SourceInfoName", Value = "D2C" },
                    new FirelightActivityDataItem { DataItemId = "HiddenField_DOB", Value = vm.birthday.Value.ToString("MM/dd/yyyy") },
                    new FirelightActivityDataItem { DataItemId = "PROPOSED_OWNER_SIGNED_STATE", Value = vm.stateInfo.TC.ToString() },
                    new FirelightActivityDataItem { DataItemId = "INSURED_STATE_NAME", Value = vm.stateInfo.Name },
                    new FirelightActivityDataItem { DataItemId = "PROPOSED_INSURED_GENDER", Value = (vm.genderInfo.TC == 1 ? "M" : vm.genderInfo.TC == 2 ? "F" : "") },
                    new FirelightActivityDataItem { DataItemId = "RISK_CLASS", Value = GetRickClassFromTC(vm.riskClass.TC) },
                    new FirelightActivityDataItem { DataItemId = "PREMIUM_TOBACCO_USER", Value = vm.smokerStatusInfo.TC == 1 ? "N" : "Y"},
                    new FirelightActivityDataItem { DataItemId = "APP_FACE_AMOUNT", Value = vm.CoverageAmount > 0? vm.CoverageAmount.ToString(CultureInfo.InvariantCulture) : FireLightSession.DefaultCoverage}
                }
            };

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(actBody);
            return CreateActivity(token, body);
        }

        /// <summary>
        /// create Activity on Firelight 
        /// </summary>
        /// <returns></returns>
        private string CreateActivity(string token, string requestBody)
        {
            string url = $"{FireLightSession.BaseUrl}/api/Activity/CreateActivity";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Headers.Add("authorization", "Bearer " + token);
                request.Method = "POST";
                Byte[] contentBytes = Encoding.UTF8.GetBytes(requestBody);
                request.ContentLength = contentBytes.LongLength;

                // Send request
                using (Stream post = request.GetRequestStream())
                {
                    post.Write(contentBytes, 0, contentBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();

                    Console.WriteLine(result);
                    return result;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response.ContentLength > 0)
                {
                    using (StreamReader s = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        string message = s.ReadToEnd();
                        Console.WriteLine(message);
                    }
                }

                return String.Empty;
            }
        }
        #endregion



        public string GetRickClassFromTC(int tc)
        {
            switch (tc)
            {
                case 1:
                case 2:
                    return "2";
                case 3:
                case 4:
                    return "1";
                case 5:
                case 6:
                    return "3";
                case 7:
                case 8:
                    return "19";
                case 9:
                case 10:
                    return "8";
                default:
                    return "";
            }
        }

        /// <summary>
        /// set the Google analytics code from client side
        /// </summary>
        /// <param name="gcid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetGoogleCid(string gcid)
        {
            Session["GCId"] = gcid; //store value in session
            return Content("SUCCESS");
        }

        




    }
}
