﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.Migrations;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sagicor.Core.Common.Contracts;
using SagicorNow.Business;
using SagicorNow.Core;
using SagicorNow.Data;
using SagicorNow.Data.Entities;
using SagicorNow.ViewModels;

namespace SagicorNow.Controllers.API
{

    [Export]
    [Authorize]
    [UsesDisposableService]
    [RoutePrefix("api/quote")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuoteApiController : ApiControllerBase
    {

        [ImportingConstructor]
        public QuoteApiController(/*IProcessTXLifeRequestClient processTxLifeRequestClient*/)
        {
            //_processTxLifeRequestClient = processTxLifeRequestClient;
        }

        //private readonly IProcessTXLifeRequestClient _processTxLifeRequestClient;
        private readonly SageNowContext _db = new SageNowContext();

        protected override void RegisterServices(List<IServiceContract> disposableServices)
        {
            //disposableServices.Add(_processTxLifeRequestClient);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("getIllustration")]
        public HttpResponseMessage GetForesightIllustration(HttpRequestMessage request, QuoteViewModel quoteViewModel)
        {
            return GetHttpResponse(request, () =>
            {
                var illustrationRequest = new IllustrationRequestParameters {
                    SmokerStatusInfo = quoteViewModel.smokerStatusInfo,
                    GenderInfo = quoteViewModel.genderInfo,
                    RiskClass = quoteViewModel.riskClass,
                    Birthday = quoteViewModel.birthday,
                    CoverageAmount = quoteViewModel.CoverageAmount,
                    ChildrenCoverage = quoteViewModel.ChildrenCoverage,
                    WaiverOfPremium = quoteViewModel.WaiverPremium,
                    AccidentalDeath = quoteViewModel.AccidentalDeath,
                    AgeOfYoungest = quoteViewModel.AgeOfYoungest,
                    RiderAmountAccidentalDeath = quoteViewModel.AccidentalDeathRiderAmount,
                    RiderAmountChildrenCoverage = quoteViewModel.ChildrenCoverageRiderAmount
                };

                var soapRequest = ForesightServiceHelpers.GenerateRequestXml(illustrationRequest);

                var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);

                var response = request.CreateResponse(HttpStatusCode.OK, txLife.TxLifeResponse);
                return response;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("createPassword")]
        public HttpResponseMessage CreateNewUserPassword(HttpRequestMessage request, ProposalHistory model)
        {
            return GetHttpResponse(request, () => {
                bool succeeded;
                if (ModelState.IsValid)
                {
                    model.HashedPassword = SecurityHelpers.HashPassword(model.Password);
                    _db.ProposalHistories.AddOrUpdate(model);
                    _db.SaveChanges();
                    succeeded = true;
                }
                else
                    succeeded = false;

                var response = request.CreateResponse(HttpStatusCode.OK, succeeded);
                return response;
            });
        }
    }
}