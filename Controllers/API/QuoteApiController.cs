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
                //var smokerStatus = model.smokerStatusInfo;
                //var gender = model.genderInfo;
                //var birthday = model.birthday;
                //var coverage = model.CoverageAmount;
                //var txLife = new TXLife {
                //    TXLifeRequest = new[]
                //    {
                //        ForesightServiceHelpers.GenerateST10NRequest(smokerStatus, gender, birthday, coverage),
                //        ForesightServiceHelpers.GenerateST15NQRequest(smokerStatus, gender, birthday, coverage),
                //        ForesightServiceHelpers.GenerateST20NIRequest(smokerStatus, gender, birthday, coverage)
                //    }
                //};

                //var result = await _processTxLifeRequestClient.ProcessTXLifeRequestAsync(new AcordTXLifeRequestMessageContract(txLife));
                //

                var soapDocument = ForesightServiceHelpers.GenerateRequestXml(model.smokerStatusInfo, model.genderInfo,
                    model.birthday, model.CoverageAmount);

                var webRequest = (HttpWebRequest)WebRequest.Create(FireLightSession.ForeSightUrl);
                webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "application/soap+xml; charset=utf-8";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";

                using (var stream = webRequest.GetRequestStream())
                {
                    soapDocument.Save(stream);
                }

                using (var webResponse = webRequest.GetResponse())
                {
                    using (var rd = new StreamReader(webResponse.GetResponseStream() ?? throw new InvalidOperationException("The response object is null.")))
                    {
                        var txLife = ForesightServiceHelpers.ExtractTxLife(rd);

                        var response = request.CreateResponse(HttpStatusCode.OK, txLife.TxLifeResponse);
                        return response;
                    }
                }
            });
        }
    }
}
