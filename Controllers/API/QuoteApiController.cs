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
        public QuoteApiController(IProcessTXLifeRequestClient processTxLifeRequestClient)
        {
            _processTxLifeRequestClient = processTxLifeRequestClient;
        }

        private readonly IProcessTXLifeRequestClient _processTxLifeRequestClient;

        protected override void RegisterServices(List<IServiceContract> disposableServices)
        {
            disposableServices.Add(_processTxLifeRequestClient);
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

                var sb = new StringBuilder(Resources.FS_Quote_Request_Template4);

                sb.Replace("<<transaction-guid>>", Guid.NewGuid().ToString());
                sb.Replace("<<transaction-guid1>>", Guid.NewGuid().ToString());
                sb.Replace("<<transaction-guid2>>", Guid.NewGuid().ToString());
                sb.Replace("<<transaction-guid3>>", Guid.NewGuid().ToString());
                sb.Replace("<<default-coverage>>", model.CoverageAmount.ToString(CultureInfo.InvariantCulture));
                sb.Replace("<<coverage>>", model.CoverageAmount.ToString(CultureInfo.InvariantCulture));
                sb.Replace("<<smoker-status-tc>>", model.smokerStatusInfo.TC.ToString());
                sb.Replace("<<smoker-status>>", model.smokerStatusInfo.Value);
                sb.Replace("<<gender-tc>>", model.genderInfo.TC.ToString());
                sb.Replace("<<gender>>", model.genderInfo.Value);
                sb.Replace("<<dob>>",
                    model.birthday != null
                        ? model.birthday.Value.ToString("yyyy-MM-dd")
                        : DateTime.Today.ToString("yyyy-MM-dd"));
                sb.Replace("<<uuid>>", Guid.NewGuid().ToString());

                var soapDocument = new XmlDocument();
                soapDocument.LoadXml(sb.ToString());

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
