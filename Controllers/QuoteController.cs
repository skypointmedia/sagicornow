using SagicorNow.Models;
using SagicorNow.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using SagicorNow.Data;
using SagicorNow.Data.Entities;

namespace SagicorNow.Controllers
{
    public class QuoteController : Controller
    {

#if DEBUG
        //private const string _firelightBaseUrl = "https://uat.firelighteapp.com/EGApp/"; //UAT
        private const string _firelightBaseUrl = "https://firelight.insurancetechnologies.com/EGApp/"; //QE
#else
        private const string _firelightBaseUrl = "https://www.firelighteapp.com/EGApp/";                       
#endif

        //private const string _sagApiSecret = "ec511cdf9f8a4bd3b6bd90aea74c853d"; //UAT
        private const string _sagApiSecret = "43983160a16d4f0996f98d04fe5ea36d"; //QE
        private const string _sagOrgId = "D2C";
        private const string _sagCarrierCode = "SAG";
        private const string _certSerialNum = "24000001c5e9e39d3274150b7b0002000001c5";
        private const string _agentPartyId = "14529e88-60ed-4877-847a-3f682862c14f";
        private const string _producerId = "SAG0301";
        private const string _defaultCoverage = "250000";

        private readonly SageNowContext _db = new SageNowContext();

        public ActionResult Index()
        {
            return View(new QuoteViewModel());
        }

        public ActionResult QuoteFromNeeds()
        {
            QuoteViewModel vm = new QuoteViewModel();

            if (TempData["QuoteViewModelFromNeeds"] != null)
                vm = (QuoteViewModel)TempData["QuoteViewModelFromNeeds"];

            return View("Index", vm); //return view with model
        }

        [HttpPost]
        public ActionResult Index(QuoteViewModel vm)
        {

            try
            {
                //if coverage supplied, limit coverage to max of 1,000,000 based on age
                decimal maxCoverage = 0M;
                maxCoverage = QuoteViewModel.GetMaxCoverageBasedOnAge(vm.Age);

                vm.CoverageAmount = vm.CoverageAmount > maxCoverage ? maxCoverage : vm.CoverageAmount; //update coverage based on max 

                /*
                //override coverage based on health and smoker status
                if ((vm.health == "OLI_UNWRITE_RATED" || vm.health == "OLI_UNWRITE_POOR") && vm.tobacco == "OLI_TOBACCO_CURRENT")
                    vm.CoverageAmount = 750000M;
                */

                //override coverage based on risk class
                if (vm.Age < 56 && vm.CoverageAmount < 525000M && (vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED_TOBACCO ||
                    vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_NONTOBACCO ||
                    vm.riskClass.TC == (int)QuoteViewModel.RiskClasses.RATED2_TOBACCO))
                {
                    vm.CoverageAmount = 525000M;
                }

                // Determine eligibility
                EligibilityInfo eligibility = BusinessRulesClass.IsEligible(vm.Age, vm.stateInfo.Name, vm.tobacco, vm.health, null == vm.ReplacementPolicy ? false : vm.ReplacementPolicy.Value);

                // If eligible build XML and send to firelight
                if (eligibility.IsEligible)
                {
                    // Build XMLs
                    var client = new NewBusinessService.NewBusinessClient("CustomBinding_NewBusiness");
                    var addrUri = client.Endpoint.Address.Uri;
                    var newUri = String.Format(addrUri.ToString(), Session["GCId"] ?? "");

                    var newAddress = new System.ServiceModel.EndpointAddress(newUri);
                    client.Endpoint.Address = newAddress;

                    var user1228Str = Create1228(vm);
                    string token = GetFirelightTokenAsync(user1228Str).Result;
                    FirelightTokenReturn tokenReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightTokenReturn>(token);

                    EmbeddedViewModel evm = new EmbeddedViewModel
                    {
                        AccessToken = tokenReturn.access_token,
                        FirelightBaseUrl = _firelightBaseUrl,
                        IsNew = vm.IsNewProposal
                    }; 

                    var proposalHistory = _db.ProposalHistories.Find(vm.SocialSecurityNumber);

                    if (vm.IsNewProposal)
                    {
                        string activityRequestApiString = this.CreateEAppActivity(tokenReturn.access_token, "26", vm); // 26 = Sage Term 
                        FirelightActivityReturn activityReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightActivityReturn>(activityRequestApiString);

                        
                        if (proposalHistory == null)
                        {
                            proposalHistory = new ProposalHistory
                                { SSN = vm.SocialSecurityNumber, ActivityId = activityReturn.ActivityId };

                            _db.ProposalHistories.Add(proposalHistory);
                        }
                        else
                        {
                            proposalHistory.ActivityId = activityReturn.ActivityId;
                            proposalHistory.LastActiveDateTime = DateTime.Now;
                        }
                        evm.ActivityId = activityReturn.ActivityId;
                    }
                    else
                    {
                        evm.ActivityId = proposalHistory.ActivityId;
                        proposalHistory.LastActiveDateTime = DateTime.Now;
                    }
                    
                    _db.SaveChanges();

                    return View("EmbeddedApp", evm);

                    /*
                    var txLife = Create103(vm);
                    Console.Write(txLife.ToString());
                    NewBusinessService.TXLife response = client.SubmitNewBusinessApplication(txLife);

                    var code = response.TXLifeResponse[0].TransResult.ResultCode.Value; //response code??
                    if (code == "RESULT_SUCCESS")
                    {
                        //redirect to url 
                        string url = $"{_firelightBaseUrl}PassiveCall.aspx?O=3138&C=D2C&refid={response.TXLifeResponse[0].TransRefGUID ?? String.Empty}&GAT=UA-97044577-1&GAC={Session["GCId"] ?? String.Empty}";
                        return Redirect(url);
                    }
                    else
                    {
                        //failed 
                        var msg = response.TXLifeResponse[0].TransResult.ResultInfo[0].ResultInfoDesc;
                        vm.ViewMessages.Add("Web Service Returned a Failure Code: " + msg);
                        return View(vm);
                    }
                    */
                }
                else
                {
                    TempData["ContactViewModel"] = new Models.ContactModel { denialMessage = eligibility.EligibilityMessage, isReplacementReject = eligibility.IsReplacememtReject, state = eligibility.State };
                    return RedirectToActionPermanent("Contact", "Contact");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var msg = "There was an error connecting to the application portal.";
                vm.ViewMessages.Add(msg);
                return View(vm);
            }
        }

        [HttpGet]
        public JsonResult ProposalExist(string socialSecurityNumber)
        {
            var exist = _db.ProposalHistories.Any(record => record.SSN == socialSecurityNumber);
            return Json(exist,JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ConfirmContinue(string socialSecurityNumber)
        {
            var model = _db.ProposalHistories.Find(socialSecurityNumber);

            return PartialView("_QuoteModal", model);
        }

        //accepts the view model and returns a TX Request object 
        private NewBusinessService.TXLife Create103(QuoteViewModel vm)
        {
            var txLife = new NewBusinessService.TXLife();
            txLife.TXLifeRequest = new NewBusinessService.TXLifeRequest[1];

            var req = new NewBusinessService.TXLifeRequest();

            var holdingId = Guid.NewGuid().ToString();
            var fileControlId = Guid.NewGuid().ToString();
            var clientPartyId = Guid.NewGuid().ToString();
             //Guid.NewGuid().ToString();

            //set properties 
            req.TransRefGUID = Guid.NewGuid().ToString();
            req.TransType = new NewBusinessService.TransType() { tc = "103", Value = "OLI_TRANS_NBSUB" };
            req.TransExeDate = DateTime.Today;
            req.TransExeTime = DateTime.Now;
            req.TestIndicator = new NewBusinessService.TestIndicator() { Value = "false" };

            req.OLifE = new NewBusinessService.OLifE()
            {
                SourceInfo = new NewBusinessService.SourceInfo()
                {
                    SourceInfoName = _sagOrgId, //Sagicor Life Insurance USA
                    SourceInfoDescription = "Version: 1.0",
                    FileControlID = fileControlId
                },
                //ability to add multiple holdings
                Holding = new NewBusinessService.Holding[] {
                    new NewBusinessService.Holding() {
                        id = holdingId,
                        HoldingTypeCode =  new NewBusinessService.HoldingTypeCode() { tc="2", Value = "OLI_HOLDTYPE_POLICY" },
                        HoldingName = "Consumer Portal 3/17",
                        Policy = new NewBusinessService.Policy() {
                            ProductCode = "26",
                            CarrierCode = _sagCarrierCode,
                            Jurisdiction = new NewBusinessService.Jurisdiction() { tc = vm.stateInfo.TC.ToString(), Value = vm.stateInfo.Value}, //need to get the OLIFE state code based on the state entered. Should the state be an autocomplete dropdown?
                            Life = new NewBusinessService.Life() {
                                Coverage = new NewBusinessService.Coverage[] {
                                    //ability to add multiple coverages 
                                    new NewBusinessService.Coverage() {
                                        IndicatorCode = new NewBusinessService.IndicatorCode() { tc = "1", Value = "OLI_COVIND_BASE"}, // from UI? 
                                        CurrentAmt = vm.CoverageAmount == default(decimal) ? 250000.00M : vm.CoverageAmount, //if value coming from needs then use else default
                                        CurrentAmtSpecified = true,
                                        LifeParticipant = new NewBusinessService.LifeParticipant[] {
                                            //add multiple participants ??
                                            new NewBusinessService.LifeParticipant() {
                                                PartyID = clientPartyId,
                                                SmokerStat = new NewBusinessService.SmokerStat() { tc= vm.smoketStatusInfo.TC.ToString(), Value = vm.smoketStatusInfo.Value}, // from UI
                                                PermTableRating = new NewBusinessService.PermTableRating() { tc = "1", Value = "OLI_TBLRATE_NONE"},
                                                UnderwritingClass = new NewBusinessService.UnderwritingClass() { tc = vm.riskClass.TC.ToString() },
                                            }
                                        }
                                    }
                                }
                            },
                            ApplicationInfo = new NewBusinessService.ApplicationInfo() {
                                RequestedIssueDate = DateTime.Now,
                                RequestedIssueDateSpecified = true,
                                QuotedPremiumMode = new NewBusinessService.QuotedPremiumMode() { tc = "4", Value = "OLI_PAYMODE_MONTHLY"}
                            }
                        },
                        KeyedValue = new NewBusinessService.KeyedValue[]
                        {
                            //new NewBusinessService.KeyedValue(){ KeyName = "FLI_CONS_PORTAL_START_VIEW", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = "Quote" } } },
                            new NewBusinessService.KeyedValue(){ KeyName = "Needs", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = "False" } } },
                            new NewBusinessService.KeyedValue(){ KeyName = "INSURED_STATE_NAME", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = vm.stateInfo.Name } } },

                            new NewBusinessService.KeyedValue(){ KeyName = "replReplacement", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = (vm.ReplacementPolicy == null || !QuoteModel.GetReplacementPolicyStates().Contains(vm.stateInfo.Code) ? "" : vm.ReplacementPolicy.Value == true ? "Y" : "N") } } },

                            new NewBusinessService.KeyedValue(){ KeyName = "FLI_CONS_PORTAL_START_VIEW", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = "Quote" } } }
                        }
                    }
                },
                Party = new NewBusinessService.Party[] {
                    new NewBusinessService.Party() {
                        id = _agentPartyId,
                        PartyTypeCode = new NewBusinessService.PartyTypeCode() { tc = "1", Value = "OLI_PT_PERSON"},
                        Person = new NewBusinessService.Person(),
                        Producer = new NewBusinessService.Producer() {
                            CarrierAppointment = new NewBusinessService.CarrierAppointment[] {
                                new NewBusinessService.CarrierAppointment() {
                                    PartyID = _agentPartyId,
                                    CompanyProducerID = _producerId,
                                    CarrierCode = _sagCarrierCode,
                                }
                            }
                        }
                    },
                    new NewBusinessService.Party() {
                        id = clientPartyId,
                        PartyTypeCode = new NewBusinessService.PartyTypeCode() { tc = "1", Value = "OLI_PT_PERSON"},
                        Address = new NewBusinessService.Address[] {
                            new NewBusinessService.Address() {
                                AddressTypeCode = new NewBusinessService.AddressTypeCode() { tc = "1", Value = "Residence" },
                                //City = "Tampa",
                                AddressState = vm.stateInfo.Code,
                                AddressStateTC = new NewBusinessService.AddressStateTC() { tc = vm.stateInfo.TC.ToString(), Value = vm.stateInfo.Value },
                                //Zip = "33605",
                                AddressCountryTC = new NewBusinessService.AddressCountryTC() { tc= "1", Value = "United States of America"}
                            }
                        },
                        Person = new NewBusinessService.Person() {
                            Gender = new NewBusinessService.Gender() { tc = vm.genderInfo.TC.ToString(), Value = vm.genderInfo.Value}, //change TC values 
                            BirthDate = vm.birthday.Value,
                            BirthDateSpecified = true,
                            DOBEstimated = new NewBusinessService.DOBEstimated() { tc = "0", Value = "False"},
                            Age = vm.Age.ToString()
                        }
                    }
                },
                Relation = new NewBusinessService.Relation[]
                {
                    new NewBusinessService.Relation() {
                        OriginatingObjectID = holdingId,
                        RelatedObjectID = _agentPartyId,
                        OriginatingObjectType = new NewBusinessService.OriginatingObjectType() { tc="4", Value ="OLI_HOLDING"},
                        RelatedObjectType = new NewBusinessService.RelatedObjectType() { tc="6", Value = "OLI_PARTY" },
                        RelationRoleCode = new NewBusinessService.RelationRoleCode() { tc = "37", Value="OLI_REL_PRIMAGENT" }
                    },
                    new NewBusinessService.Relation() {
                        OriginatingObjectID = holdingId,
                        RelatedObjectID = clientPartyId,
                        RelationRoleCode = new NewBusinessService.RelationRoleCode() {tc="32", Value = "OLI_REL_INSURED"}
                    }
                }
            };

            txLife.TXLifeRequest[0] = req;

            return txLife;
        }

        /// <summary>
        /// creates a 1228 request XML
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public string Create1228(QuoteViewModel vm)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<TXLife xmlns=\"http://ACORD.org/Standards/Life/2\">");
            builder.Append("<TXLifeRequest>");
            builder.Append("<TransRefGUID>"+Guid.NewGuid()+"</TransRefGUID>");
            builder.Append("<TransType tc=\"1228\">OLI_TRANS_TRNPRODINQ</TransType>");
            builder.Append("<OLifE>");
            builder.Append("<SourceInfo>");
            builder.Append("<CreationDate>"+DateTime.Today.ToString("yyyy-MM-dd")+"</CreationDate>");
            builder.Append("<CreationTime>"+DateTime.Now.ToString("hh:mm:ss.fffffffzzz")+"</CreationTime>");
            builder.Append("<SourceInfoName>"+_sagOrgId+"</SourceInfoName>");
            builder.Append("</SourceInfo>");
            builder.Append("<Party id=\""+ _agentPartyId + "\">");
            builder.Append("<PartyTypeCode tc=\"1\">OLI_PT_Person</PartyTypeCode>");
            builder.Append("<FullName>Sagicor D2C</FullName>");
            builder.Append("<EmailAddress>");
            builder.Append("<AddrLine>sagicornow@sagicorlifeusa.com</AddrLine>");
            builder.Append("</EmailAddress>");
            builder.Append("<Producer>");
            builder.Append("<CarrierAppointment PartyID=\""+_agentPartyId + "\">");
            builder.Append("<CompanyProducerID>"+ _producerId +"</CompanyProducerID>");
            builder.Append("<CarrierCode>"+_sagCarrierCode+"</CarrierCode>");
            builder.Append("</CarrierAppointment>");
            builder.Append("</Producer>");
            builder.Append("</Party>");
            builder.Append("<OLifEExtension VendorCode=\"25\">");
            builder.Append("<UserRoleCode tc=\"InsTech\">InsTech</UserRoleCode>");
            builder.Append("</OLifEExtension>");
            builder.Append("<Relation OriginatingObjectID=\""+_agentPartyId+"\">");
            builder.Append("<RelationRoleCode tc=\"11\">OLI_REL_AGENT</RelationRoleCode>");
            builder.Append("</Relation>");
            builder.Append("</OLifE>");
            builder.Append("</TXLifeRequest>");
            builder.Append("</TXLife>");

            return builder.ToString();
        }

        /// <summary>
        /// request Firelight token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetFirelightTokenAsync(string user1228)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{_firelightBaseUrl}api/Security/GetToken");
            req.Method = "POST";
            req.ContentType = "text/plain";
            byte[] secretBinary = Encoding.UTF8.GetBytes(_sagApiSecret);
            byte[] hashBinary = new SHA256Managed().ComputeHash(secretBinary);
            //convert to hex string
            StringBuilder builder = new StringBuilder();

            for (Int32 idx = 0; idx < hashBinary.Length; idx++)
            {
                builder.Append(hashBinary[idx].ToString("X2"));
            }

            string hashValue = builder.ToString();
            DateTime reqDate = DateTime.UtcNow;

            string xml1228 = String.Empty; //get 1228 XML
            if (!string.IsNullOrWhiteSpace(user1228))
                xml1228 = Convert.ToBase64String(Encoding.UTF8.GetBytes(user1228));
            //remember, the user 1228 is optional
            String textToSign = _sagApiSecret + reqDate.ToString("MMddyyyyHHmmss") + _sagCarrierCode + xml1228;
            byte[] nonSignedBinary = Encoding.UTF8.GetBytes(textToSign);

            //load up the test cert - in this example, it comes from a pfx file in a local directory - you
            // may need to get it from your machine's certificate store
            X509Certificate2 cert = this.FindClientCertificate(_certSerialNum);
            RSA rsa = cert.GetRSAPrivateKey();
            byte[] signedBinary = rsa.SignData(nonSignedBinary, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            //base64-encoded, then url-encoded
            String signed64 = HttpUtility.UrlEncode(Convert.ToBase64String(signedBinary));
            //optionally place the user's 1228 on the request if it has value (url-encode it)
            String body = $"grant_type=hashsig&id={_sagCarrierCode}&secret={hashValue}&sig={signed64}"
            + (string.IsNullOrWhiteSpace(xml1228) ? "" : $"&xml={HttpUtility.UrlEncode(xml1228)}");

            byte[] bodyBinary = Encoding.UTF8.GetBytes(body);
            req.Date = reqDate;
            req.ContentLength = bodyBinary.LongLength;

            using (Stream post = req.GetRequestStream())
            {
                post.Write(bodyBinary, 0, bodyBinary.Length);
            }

            // Get response
            String result = "";
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

        private string CreateEAppActivity(string token, string cusip, QuoteViewModel vm)
        {
            string reqId = Guid.NewGuid().ToString();
            //string body = $"{{ \"Id\": \"{reqId}\", \"CUSIP\": \"{cusip}\", \"Jurisdiction\": {jurisdiction}, \"TransactionType\": 1, \"CarrierCode\": \"{_sagCarrierCode}\", " +
            //    $"\"DataItems\": [ {{\"DataItemId\":\"Owner_NonNaturalName\",\"Value\":\"{firstName} {lastName}\"}}] }}";

            FirelightActivityBody actBody = new FirelightActivityBody() {
                Id = reqId,
                CUSIP = cusip,
                Jurisdiction = vm.stateInfo.TC,
                CarrierCode = _sagCarrierCode,
                TransactionType = 1,// create activity "
                DataItems = new List<FirelightActivityDataItem>()
                {
                    new FirelightActivityDataItem() { DataItemId = "Owner_NonNaturalName", Value = $"" },
                    //new FirelightActivityDataItem() { DataItemId = "Owner_FirstName", Value = $"{firstName}" },
                    //new FirelightActivityDataItem() { DataItemId = "Owner_LastName", Value = $"{lastName}" },
                    new FirelightActivityDataItem() { DataItemId = "HiddenField_DOB", Value = vm.birthday.Value.ToString("yyyy/MM/dd") },
                    new FirelightActivityDataItem() { DataItemId = "PROPOSED_INSURED_GENDER", Value = (vm.genderInfo.TC == 1 ? "M" : vm.genderInfo.TC == 2 ? "F" : "") },
                    new FirelightActivityDataItem() { DataItemId = "RISK_CLASS", Value = GetRickClassFromTC(vm.riskClass.TC) },
                    new FirelightActivityDataItem() { DataItemId = "PREMIUM_TOBACCO_USER", Value = vm.smoketStatusInfo.TC == 1 ? "N" : "Y"},
                    new FirelightActivityDataItem() { DataItemId = "APP_FACE_AMOUNT", Value = vm.CoverageAmount > 0? vm.CoverageAmount.ToString() : _defaultCoverage}
                }
            };

            string body = Newtonsoft.Json.JsonConvert.SerializeObject(actBody);
            return CreateActivity(token, body);
        }

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
        /// create Activity on Firelight 
        /// </summary>
        /// <returns></returns>
        private string CreateActivity(string token, string requestBody)
        {
            string url = $"{_firelightBaseUrl}/api/Activity/CreateActivity";          

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
                    using (StreamReader s = new StreamReader(ex.Response.GetResponseStream())) {
                        string message = s.ReadToEnd();
                        Console.WriteLine(message);
                    }
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// find cert on machine
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public X509Certificate2 FindClientCertificate(string serialNumber)
        {
            return
                FindCertificate(StoreLocation.CurrentUser) ??
                FindCertificate(StoreLocation.LocalMachine);

            X509Certificate2 FindCertificate(StoreLocation location)
            {
                X509Store store = new X509Store(location);
                store.Open(OpenFlags.OpenExistingOnly);
                IEnumerable certs = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true);
                return certs.OfType<X509Certificate2>().FirstOrDefault();
            };
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
