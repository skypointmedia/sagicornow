using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SagicorNow.Business.Models;
using SagicorNow.Common;
using SagicorNow.Properties;

namespace SagicorNow.Business
{
    internal sealed class ForesightServiceHelpers
    {
        internal static ForesightTxLifeReturn GetForesightTxLifeReturn(XmlDocument soapDocument)
        {
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(FireLightSession.ForeSightUrl);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "application/soap+xml; charset=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapDocument.Save(stream);
            }

            ForesightTxLifeReturn txLife;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream() ??
                                                          throw new InvalidOperationException(
                                                              "The response object is null.")))
                {
                   txLife = ForesightServiceHelpers.ExtractTxLife(rd);
                }
            }

            return txLife;
        }

        internal static ForesightTxLifeReturn ExtractTxLife(StreamReader reader)
        {
            var txLife = ExtractTxLifeString(reader);
            var serializer = new XmlSerializer(typeof(ForesightTxLifeReturn));

            StringReader read = new StringReader(txLife);

            return (ForesightTxLifeReturn)serializer.Deserialize(read);
        }

        internal static string GenerateRequestXmLString(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo,
            AccordOlifeValue riskClass, DateTime? birthday, decimal coverageAmount = 250000m)
        {
            var sb = new StringBuilder(Resources.FS_Quote_Request_Template5);

            sb.Replace("<<transaction-guid>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid1>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid2>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid3>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid4>>", Guid.NewGuid().ToString());
            sb.Replace("<<default-coverage>>", coverageAmount.ToString(CultureInfo.InvariantCulture));
            sb.Replace("<<coverage>>", coverageAmount.ToString(CultureInfo.InvariantCulture));
            sb.Replace("<<smoker-status-tc>>", smokerStatusInfo.TC.ToString());
            sb.Replace("<<smoker-status>>", smokerStatusInfo.Value);
            sb.Replace("<<gender-tc>>", genderInfo.TC.ToString());
            sb.Replace("<<gender>>", genderInfo.Value);
            sb.Replace("<<dob>>",
                birthday != null
                    ? birthday.Value.ToString("yyyy-MM-dd")
                    : DateTime.Today.ToString("yyyy-MM-dd"));
            sb.Replace("<<uuid>>", Guid.NewGuid().ToString());
            sb.Replace("{{risk-class}}", "riskClass");
            sb.Replace("{{risk-class-tc}}", GetRiskClassFromTC(riskClass.TC));
            return sb.ToString();
        }

        internal static XmlDocument GenerateRequestXml(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo, AccordOlifeValue riskClass,
            DateTime? birthday, decimal coverageAmount = 250000m) {
            var xmlRequestString =
                ForesightServiceHelpers.GenerateRequestXmLString(smokerStatusInfo, genderInfo, riskClass, birthday,
                    coverageAmount);

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlRequestString);

            return document;
        }
        private static string GetRiskClassFromTC(int tc)
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
        private static string ExtractTxLifeString(StreamReader reader)
        {
            var xmlDocument = XDocument.Load(reader);

            XNamespace s = "http://www.w3.org/2003/05/soap-envelope";

            var responseXml = xmlDocument.Element(s + "Envelope")
                ?.Element(s + "Body")?.FirstNode;

            return responseXml?.ToString().Replace("xmlns=\"http://ACORD.org/Standards/Life/2\"", "");
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
    }
}