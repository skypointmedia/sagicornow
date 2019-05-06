using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
//using AttributeRouting.Web.Http;
using Sagicor.Core.Common.Contracts;
using SagicorNow.Business;
using SagicorNow.Business.Models;
using SagicorNow.Foresight;
using SagicorNow.Client.Contracts;
using SagicorNow.Common;
using SagicorNow.Core;
using SagicorNow.Foresight;
using SagicorNow.Properties;
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

        protected override void RegisterServices(List<IServiceContract> disposableServices)
        {
            //disposableServices.Add(_processTxLifeRequestClient);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("getIllustration")]
        public HttpResponseMessage GetForesightIllustration(HttpRequestMessage request, QuoteViewModel model)
        {
            return GetHttpResponse(request,  () =>
            {
               var soapRequest = ForesightServiceHelpers.GenerateRequestXml(model.smokerStatusInfo, model.genderInfo,
                    model.birthday, model.CoverageAmount);

                var txLife = ForesightServiceHelpers.GetForesightTxLifeReturn(soapRequest);

                var response = request.CreateResponse(HttpStatusCode.OK, txLife.TxLifeResponse);
                return response;
            });
        }
    }
}
