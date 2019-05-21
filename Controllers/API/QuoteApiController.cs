using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        public HttpResponseMessage GetForesightIllustration(HttpRequestMessage request, QuoteViewModel model)
        {
            return GetHttpResponse(request, () =>
            {
                var soapRequest = ForesightServiceHelpers.GenerateRequestXml(model.smokerStatusInfo, model.genderInfo,
                    model.riskClass, model.birthday, model.CoverageAmount);

                var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);

                var response = request.CreateResponse(HttpStatusCode.OK, txLife.TxLifeResponse);
                return response;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("createPassword")]
        public HttpResponseMessage CreateNewUserPassword(HttpRequestMessage request, QuoteViewModel model)
        {
            return GetHttpResponse(request, () => {
                bool succeeded;
                if (ModelState.IsValid)
                {
                    var proposal = new ProposalHistory {
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
                        WavierPremium = model.WavierPremium,
                        WholeLife = model.WholeLife,
                        WholeLifePerMonthCost = model.WholeLifePerMonthCost,
                    };
                    _db.ProposalHistories.Add(proposal);
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
