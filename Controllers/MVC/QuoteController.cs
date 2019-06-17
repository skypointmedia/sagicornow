using SagicorNow.Models;
using SagicorNow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sagicor.Core.Common.Contracts;
using SagicorNow.Business;
using SagicorNow.Business.Models;
using SagicorNow.Common;
using SagicorNow.Core;
using SagicorNow.Data;
using SagicorNow.Data.Entities;

namespace SagicorNow.Controllers
{

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuoteController : FirelightAwareControllerBase
    {
        
       [ImportingConstructor]
        public QuoteController(/*IProcessTXLifeRequestClient processTxLifeRequestClient*/)
        {
            //_serviceFactory = serviceFactory;
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
        [System.Web.Mvc.HttpPost]
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
                    var accessToken = await GetAccessToken();

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

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> EmbeddedApp([FromJson]ProposalHistory vm)
        {
            try
            {
                
                //if coverage supplied, limit coverage to max of 1,000,000 based on age
                var maxCoverage = QuoteViewModel.GetMaxCoverageBasedOnAge(vm.Age);

                vm.CoverageAmount = vm.CoverageAmount > maxCoverage ? maxCoverage : vm.CoverageAmount; //update coverage based on max 
                                                                                                       //vm.SocialSecurityNumber = vm.SocialSecurityNumber.Replace("-", String.Empty);

                //override coverage based on risk class
                if (vm.Age < 56 && vm.CoverageAmount < 525000M && (int.Parse(vm.RiskClassTc) == (int)QuoteViewModel.RiskClasses.RATED_TOBACCO ||
                    int.Parse(vm.RiskClassTc) == (int)QuoteViewModel.RiskClasses.RATED2_NONTOBACCO ||
                    int.Parse(vm.RiskClassTc) == (int)QuoteViewModel.RiskClasses.RATED2_TOBACCO))
                {
                    vm.CoverageAmount = 525000M;
                }

                // Determine eligibility
                var eligibility = BusinessRules.IsEligible(vm.Age, vm.StateName, vm.Tobacco, vm.Health, vm.ReplacementPolicy ?? false);

                // If eligible build XML and send to firelight
                if (eligibility.IsEligible)
                {
                    var accessToken = await GetAccessToken();
                    var activityRequestApiString = this.CreateEAppActivity(accessToken, "26", vm); // 26 = Sage Term 
                    var activityReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightActivityReturn>(activityRequestApiString);

                    var evm = new EmbeddedViewModel {
                        AccessToken = accessToken,
                        //AccessToken = FirelightAccessToken,
                        FirelightBaseUrl = FireLightSession.BaseUrl,
                        IsNew = true, //vm.IsNewProposal
                        ActivityId = activityReturn.ActivityId
                    };

                    return View("EmbeddedApp", evm);
                }

                TempData["ContactViewModel"] = new ContactModel { denialMessage = eligibility.EligibilityMessage, isReplacementReject = eligibility.IsReplacememtReject, state = eligibility.State };
                return RedirectToActionPermanent("Contact", "Contact");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var msg = "There was an error connecting to the application portal.";
                //vm.ViewMessages.Add(msg);
                return View("FraudWarning",vm);
            }
        }

        [System.Web.Mvc.HttpPost]
        public ViewResult FraudWarning([FromJson]ProposalHistory model)
        {

            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;
           
            return View(model);
        }

        [System.Web.Mvc.HttpGet]
        public ViewResult RetrievePrevious()
        {
            return View(new RetrievePreviousQuoteViewModel());
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ViewResult> RetrievePrevious(RetrievePreviousQuoteViewModel model)
        {
            try
            {
                var proposalHistory = _db.ProposalHistories.Find(model.Email);
                var isVerified = SecurityHelpers.VerifyHashedPassword(proposalHistory.HashedPassword, model.Password);
                if (!isVerified)
                {
                    var msg = "Incorrect email address or password.";
                    model.ViewMessages.Add(msg);
                    return View(model);
                }

                var accessToken = await GetAccessToken();

                var evm = new EmbeddedViewModel {
                    AccessToken = accessToken,
                    FirelightBaseUrl = FireLightSession.BaseUrl,
                    IsNew = false
                };

                return View("EmbeddedApp", evm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var msg = "There was an error connecting to the application portal.";
                model.ViewMessages.Add(msg);
                return View(model);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult ProductSlider(QuoteViewModel quote)
        {
            quote.CoverageAmount = decimal.Parse(FireLightSession.DefaultCoverage);
            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;
            ViewBag.QuoteViewModel = quote;


            var maxCoverage = QuoteViewModel.GetMaxCoverageBasedOnAge(quote.Age);

            quote.CoverageAmount = quote.CoverageAmount > maxCoverage ? maxCoverage : quote.CoverageAmount; //update coverage based on max 
            //vm.SocialSecurityNumber = vm.SocialSecurityNumber.Replace("-", String.Empty);

            //override coverage based on risk class
            if (quote.Age < 56 && quote.CoverageAmount < 525000M && (quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED_TOBACCO ||
                                                               quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_NONTOBACCO ||
                                                               quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_TOBACCO))
            {
                quote.CoverageAmount = 525000M;
            }

            var illustrationRequest = new IllustrationRequestParameters
            {
                SmokerStatusInfo = quote.smokerStatusInfo,
                GenderInfo = quote.genderInfo,
                RiskClass = quote.riskClass,
                Birthday = quote.birthday,
                CoverageAmount = quote.CoverageAmount
            };

            var soapRequest = ForesightServiceHelpers.GenerateRequestXml(illustrationRequest);

            var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);
            return View(txLife.TxLifeResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public ActionResult Composition(QuoteViewModel quote)
        {
            // Should the user refresh the page the quote is stored with the session.
            if (Session["quote"] == null)
                Session["quote"] = quote;
            else
                quote = (QuoteViewModel) Session["quote"]; // A refresh on the page will be a get without the supplied parameter.
            
            quote.CoverageAmount = decimal.Parse(FireLightSession.DefaultCoverage);
            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;
            ViewBag.QuoteViewModel = quote;


            var maxCoverage = QuoteViewModel.GetMaxCoverageBasedOnAge(quote.Age);

            quote.CoverageAmount = quote.CoverageAmount > maxCoverage ? maxCoverage : quote.CoverageAmount; //update coverage based on max 
            //vm.SocialSecurityNumber = vm.SocialSecurityNumber.Replace("-", String.Empty);

            //override coverage based on risk class
            if (quote.Age < 56 && quote.CoverageAmount < 525000M && (quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED_TOBACCO ||
                                                               quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_NONTOBACCO ||
                                                               quote.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_TOBACCO))
            {
                quote.CoverageAmount = 525000M;
            }

            var illustrationRequest = new IllustrationRequestParameters
            {
                SmokerStatusInfo = quote.smokerStatusInfo,
                GenderInfo = quote.genderInfo,
                RiskClass = quote.riskClass,
                Birthday = quote.birthday,
                CoverageAmount = quote.CoverageAmount
            };

            var soapRequest = ForesightServiceHelpers.GenerateRequestXml(illustrationRequest);

            var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);
            return View(txLife.TxLifeResponse);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult ReturnToProductSlider([FromJson]ProposalHistory proposal)
        {
            ViewBag.FirelightBaseUrl = FireLightSession.BaseUrl;

            var quote = new QuoteViewModel
            {
                genderInfo = new AccordOlifeValue { TC = int.Parse(proposal.GenderTc), Value = proposal.Gender },
                riskClass = new AccordOlifeValue { TC = int.Parse(proposal.RiskClassTc), Value = proposal.Health },
                smokerStatusInfo = new AccordOlifeValue { TC = int.Parse(proposal.SmokerStatusTc), Value = proposal.Tobacco },
                birthday = DateTime.Parse(proposal.Birthday),
                CoverageAmount = proposal.CoverageAmount,
                ChildrenCoverage = proposal.ChildrenCoverage,
                //WaiverOfPremium = proposal.WaiverPremium,
                AccidentalDeath = proposal.AccidentalDeath,
                AgeOfYoungest = proposal.AgeOfYoungest,
                //RiderAmountAccidentalDeath = proposal.AccidentalDeathRiderAmount,
                //RiderAmountChildrenCoverage = proposal.ChildrenCoverageRiderAmount,
                stateInfo = new StateInfo
                {
                    Value = proposal.StateName,
                    TC = int.Parse(proposal.StateTc),
                    Name = proposal.StateName,
                    Code = proposal.StateCode
                }
            };

            ViewBag.QuoteViewModel = quote;

            var illustrationRequest = new IllustrationRequestParameters {
                SmokerStatusInfo = quote.smokerStatusInfo,
                GenderInfo = quote.genderInfo,
                RiskClass = quote.riskClass,
                Birthday = quote.birthday,
                CoverageAmount = quote.CoverageAmount,
                ChildrenCoverage = quote.ChildrenCoverage,
                WaiverOfPremium = proposal.WaiverPremium,
                AccidentalDeath = quote.AccidentalDeath,
                AgeOfYoungest = proposal.AgeOfYoungest,
                RiderAmountAccidentalDeath = proposal.AccidentalDeathRiderAmount,
                RiderAmountChildrenCoverage = proposal.ChildrenCoverageRiderAmount
            };

            var soapRequest = ForesightServiceHelpers.GenerateRequestXml(illustrationRequest);

            var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);

            return View("ProductSlider", txLife.TxLifeResponse);
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
        /// <param name="email"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public JsonResult ProposalExist(string email)
        {
            var ssn = email.Replace("-", string.Empty);
            var exist = _db.ProposalHistories.Any(record => record.Email == ssn);
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
        /// set the Google analytic code from client side
        /// </summary>
        /// <param name="gcid"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult SetGoogleCid(string gcid)
        {
            Session["GCId"] = gcid; //store value in session
            return Content("SUCCESS");
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> CreatePassword(QuoteViewModel model)
        {
            bool succeeded;
            if (ModelState.IsValid)
            {
                var proposal = new ProposalHistory
                {
                    CoverageAmount = model.CoverageAmount,
                    FirstName = model.FirstName,
                    //EnableSaving = model.EnableSaving,
                    AccidentalDeath = model.AccidentalDeath,
                    ChildrenCoverage = model.ChildrenCoverage,
                    Email = model.EmailAddress,
                    FifteenYearTerm = model.FifteenYearTerm,
                    FifteenYearTermPerMonthCost = model.FifteenYearTermPerMonthCost,
                    HashedPassword = SecurityHelpers.HashPassword(model.PhoneNumber),
                    PhoneNumber = model.PhoneNumber,
                    TenYearTerm = model.TenYearTerm,
                    TenYearTermPerMonthCost = model.FifteenYearTermPerMonthCost,
                    TwentyYearTerm = model.TwentyYearTerm,
                    TwentyYearTermPerMonthCost = model.TwentyYearTermPerMonthCost,
                    WaiverPremium = model.WaiverPremium,
                    WholeLife = model.WholeLife,
                    WholeLifePerMonthCost = model.WholeLifePerMonthCost,
                };
                _db.ProposalHistories.Add(proposal);
                await _db.SaveChangesAsync();
                succeeded = true;
            }
            else
             succeeded = false;

            return Json(succeeded, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private async Task<string> GetAccessToken()
        {
            // Build XMLs
            var client = new NewBusinessService.NewBusinessClient("CustomBinding_NewBusiness");
            var addUri = client.Endpoint.Address.Uri;
            var newUri = string.Format(addUri.ToString(), Session["GCId"] ?? "");

            var newAddress = new System.ServiceModel.EndpointAddress(newUri);
            client.Endpoint.Address = newAddress;

            var user1228Str = Create1228();
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

            return accessToken;
        }

        private string Create1228()
        {
            var builder = new StringBuilder();
            builder.Append("<TXLife xmlns=\"http://ACORD.org/Standards/Life/2\">");
            builder.Append("<TXLifeRequest>");
            builder.Append("<TransRefGUID>" + Guid.NewGuid() + "</TransRefGUID>");
            builder.Append("<TransType tc=\"1228\">OLI_TRANS_TRNPRODINQ</TransType>");
            builder.Append("<OLifE>");
            builder.Append("<SourceInfo>");
            builder.Append("<CreationDate>" + DateTime.Today.ToString("yyyy-MM-dd") + "</CreationDate>");
            builder.Append("<CreationTime>" + DateTime.Now.ToString("hh:mm:ss.fffffffzzz") + "</CreationTime>");
            builder.Append("<SourceInfoName>" + FireLightSession.SagOrgId + "</SourceInfoName>");
            builder.Append("</SourceInfo>");
            builder.Append("<Party id=\"" + FireLightSession.AgentPartyId + "\">");
            builder.Append("<PartyTypeCode tc=\"1\">OLI_PT_Person</PartyTypeCode>");
            builder.Append("<FullName>Sagicor D2C</FullName>");
            builder.Append("<EmailAddress>");
            builder.Append("<AddrLine>sagicornow@sagicorlifeusa.com</AddrLine>");
            builder.Append("</EmailAddress>");
            builder.Append("<Producer>");
            builder.Append("<CarrierAppointment PartyID=\"" + FireLightSession.AgentPartyId + "\">");
            builder.Append("<CompanyProducerID>" + FireLightSession.ProducerId + "</CompanyProducerID>");
            builder.Append("<CarrierCode>" + FireLightSession.SagCarrierCode + "</CarrierCode>");
            builder.Append("</CarrierAppointment>");
            builder.Append("</Producer>");
            builder.Append("</Party>");
            builder.Append("<OLifEExtension VendorCode=\"25\">");
            builder.Append("<UserRoleCode tc=\"InsTech\">InsTech</UserRoleCode>");
            builder.Append("</OLifEExtension>");
            builder.Append("<Relation OriginatingObjectID=\"" + FireLightSession.AgentPartyId + "\">");
            builder.Append("<RelationRoleCode tc=\"11\">OLI_REL_AGENT</RelationRoleCode>");
            builder.Append("</Relation>");
            builder.Append("</OLifE>");
            builder.Append("</TXLifeRequest>");
            builder.Append("</TXLife>");

            return builder.ToString();
        }

        

        private string CreateEAppActivity(string token, string cusip, QuoteViewModel quote) {
            var body = InternalCreateEAppActivity(cusip, quote.stateInfo, quote.birthday, quote.genderInfo,
                quote.riskClass, quote.smokerStatusInfo, quote.CoverageAmount);
            return CreateActivity(token, body);
        }

        private string CreateEAppActivity(string token, string cusip, ProposalHistory proposal)
        {
            var smokerStatusInfo = new AccordOlifeValue { TC = int.Parse(proposal.SmokerStatusTc), Value = proposal.Tobacco };
            var genderInfo = new AccordOlifeValue { TC = int.Parse(proposal.GenderTc), Value = proposal.Gender };
            var sateInfo = new StateInfo
            {
                Code = proposal.StateCode, Value = proposal.StateName, TC = int.Parse(proposal.StateTc),
                Name = proposal.StateName
            };

            var riskClass = new AccordOlifeValue();

            var body = InternalCreateEAppActivity(cusip, sateInfo, DateTime.Parse(proposal.Birthday), genderInfo,
                riskClass, smokerStatusInfo, proposal.CoverageAmount);

            return CreateActivity(token, body);
        }

        private string InternalCreateEAppActivity(string cusip, StateInfo stateInfo, DateTime? birthday,
            AccordOlifeValue genderInfo, AccordOlifeValue riskClass, AccordOlifeValue smokerStatusInfo,
            decimal coverageAmount)
        {
            var reqId = Guid.NewGuid().ToString();

            var actBody = new FirelightActivityBody
            {
                Id = reqId,
                CUSIP = cusip,
                Jurisdiction = stateInfo.TC,
                CarrierCode = FireLightSession.SagCarrierCode,
                TransactionType = 1,
                DataItems = new List<FirelightActivityDataItem>
                {
                    new FirelightActivityDataItem {DataItemId = "Owner_NonNaturalName", Value = $""},
                    new FirelightActivityDataItem {DataItemId = "SourceInfoName", Value = "D2C"},
                    new FirelightActivityDataItem
                        {DataItemId = "HiddenField_DOB", Value = birthday.Value.ToString("MM/dd/yyyy")},
                    new FirelightActivityDataItem
                        {DataItemId = "PROPOSED_OWNER_SIGNED_STATE", Value = stateInfo.TC.ToString()},
                    new FirelightActivityDataItem {DataItemId = "INSURED_STATE_NAME", Value = stateInfo.Name},
                    new FirelightActivityDataItem
                    {
                        DataItemId = "PROPOSED_INSURED_GENDER",
                        Value = (genderInfo.TC == 1 ? "M" : genderInfo.TC == 2 ? "F" : "")
                    },
                    new FirelightActivityDataItem {DataItemId = "RISK_CLASS", Value = GetRickClassFromTC(riskClass.TC)},
                    new FirelightActivityDataItem
                        {DataItemId = "PREMIUM_TOBACCO_USER", Value = smokerStatusInfo.TC == 1 ? "N" : "Y"},
                    new FirelightActivityDataItem
                    {
                        DataItemId = "APP_FACE_AMOUNT",
                        Value = coverageAmount > 0
                            ? coverageAmount.ToString(CultureInfo.InvariantCulture)
                            : FireLightSession.DefaultCoverage
                    }
                }
            };

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(actBody);
            return body;
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

        private string GetRickClassFromTC(int tc)
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


        #endregion
    }
}
