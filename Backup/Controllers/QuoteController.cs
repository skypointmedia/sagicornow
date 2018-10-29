﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using System.Web.Mvc;
using SagicorNow.Models;
using System.Globalization;
using SagicorNow.ViewModels;

namespace SagicorNow.Controllers
{
    public class QuoteController : Controller
    {

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

                // Determine eligibility
                EligibilityInfo eligibility = BusinessRulesClass.IsEligible(vm.Age, vm.stateInfo.Name, vm.tobacco, vm.health, null == vm.ReplacementPolicy ? false : vm.ReplacementPolicy.Value);

                // If eligible build XML and send to firelight
                if (eligibility.IsEligible)
                {
                    // Build XMLs
                    var client = new NewBusinessService.NewBusinessClient("CustomBinding_NewBusiness");
                    var txLife = Create103(vm);
                    NewBusinessService.TXLife response = client.SubmitNewBusinessApplication(txLife);

                    var code = response.TXLifeResponse[0].TransResult.ResultCode.Value; //response code??
                    if (code == "RESULT_SUCCESS")
                    {
                        //redirect to url 
                        string url = String.Format("https://uat.firelighteapp.com/EGApp/PassiveCall.aspx?O=3138&C=D2C&refid={0}", response.TXLifeResponse[0].TransRefGUID ?? String.Empty);
                        return Redirect(url);
                    }
                    else
                    {
                        //failed 
                        var msg = response.TXLifeResponse[0].TransResult.ResultInfo[0].ResultInfoDesc;
                        vm.ViewMessages.Add("Web Service Returned a Failure Code: " + msg);
                        return View(vm);
                    }
                }
                else
                {
                    TempData["ContactViewModel"] = new ContactViewModel { DenialMessage = eligibility.EligibilityMessage, IsReplacementReject = eligibility.IsReplacememtReject, State = eligibility.State };
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

        //accepts the view model and returns a TX Request object 
        private NewBusinessService.TXLife Create103(QuoteViewModel vm)
		{
			var txLife = new NewBusinessService.TXLife();
			txLife.TXLifeRequest = new NewBusinessService.TXLifeRequest[1];

            var req = new NewBusinessService.TXLifeRequest();

            var holdingId = Guid.NewGuid().ToString();
            var fileControlId = Guid.NewGuid().ToString();
            var clientPartyId = Guid.NewGuid().ToString();
            var agentPartyId = "14529e88-60ed-4877-847a-3f682862c14f"; //Guid.NewGuid().ToString();

            //set properties 
            req.TransRefGUID = Guid.NewGuid().ToString();
            req.TransType = new NewBusinessService.TransType() { tc = "103", Value="OLI_TRANS_NBSUB" };
            req.TransExeDate = DateTime.Today;
            req.TransExeTime = DateTime.Now;
            req.TestIndicator = new NewBusinessService.TestIndicator() { Value = "false"};
            req.OLifE = new NewBusinessService.OLifE()
            {
                SourceInfo = new NewBusinessService.SourceInfo()
                {
                    SourceInfoName = "D2C", //Sagicor Life Insurance USA
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
                            CarrierCode = "SAG",
                            Jurisdiction = new NewBusinessService.Jurisdiction() { tc = vm.stateInfo.TC.ToString(), Value = vm.stateInfo.Value}, //neet to get the OLIFE state code based on the state entered. Should the state be an autocomplete dropdown?
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
                                                UnderwritingClass = new NewBusinessService.UnderwritingClass() { tc = vm.healthInfo.TC.ToString() }, 
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
                            new NewBusinessService.KeyedValue(){ KeyName = "replReplacement", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = (vm.ReplacementPolicy == null || vm.stateInfo.Code != "FL" ? "" : vm.ReplacementPolicy.Value == true ? "Y" : "N") } } },
							new NewBusinessService.KeyedValue(){ KeyName = "FLI_CONS_PORTAL_START_VIEW", KeyValue = new NewBusinessService.KeyValue[] { new NewBusinessService.KeyValue() { Value = "Quote" } } }
                        }
                    }
                },
                Party = new NewBusinessService.Party[] {
                    new NewBusinessService.Party() {
                        id = agentPartyId,
                        PartyTypeCode = new NewBusinessService.PartyTypeCode() { tc = "1", Value = "OLI_PT_PERSON"},
                        Person = new NewBusinessService.Person(),
                        Producer = new NewBusinessService.Producer() {
                            CarrierAppointment = new NewBusinessService.CarrierAppointment[] {
                                new NewBusinessService.CarrierAppointment() {
                                    PartyID = agentPartyId,
                                    CompanyProducerID = "SAG0301",
                                    CarrierCode = "SAG",
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
                        RelatedObjectID = agentPartyId,
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
    }
}
