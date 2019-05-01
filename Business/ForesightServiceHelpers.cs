using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SagicorNow.Business.Models;
using SagicorNow.Foresight;
using SagicorNow.Properties;

namespace SagicorNow.Business
{
    internal sealed class ForesightServiceHelpers
    {
        internal static ForesightTxLifeReturn ExtractTxLife(StreamReader reader)
        {
            var txLife = ExtractTxLifeString(reader);
            var serializer = new XmlSerializer(typeof(ForesightTxLifeReturn));

            StringReader read = new StringReader(txLife);

            return (ForesightTxLifeReturn)serializer.Deserialize(read);
        }

        internal static string GenerateRequestXmLString(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo,
            DateTime? birthday, decimal coverageAmount = 250000m)
        {
            var sb = new StringBuilder(Resources.FS_Quote_Request_Template4);

            sb.Replace("<<transaction-guid>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid1>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid2>>", Guid.NewGuid().ToString());
            sb.Replace("<<transaction-guid3>>", Guid.NewGuid().ToString());
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
            return sb.ToString();
        }

        internal static XmlDocument GenerateRequestXml(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo,
            DateTime? birthday, decimal coverageAmount = 250000m) {
            var xmlRequestString =
                ForesightServiceHelpers.GenerateRequestXmLString(smokerStatusInfo, genderInfo, birthday,
                    coverageAmount);

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlRequestString);

            return document;
        }

        //internal static TXLifeRequest GenerateST10NRequest(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo, DateTime? birthday, decimal coverageAmount = 250000m)
        //{
        //    var request = GenerateBaseRequest(smokerStatusInfo, genderInfo, birthday, coverageAmount);
        //    request.OLifE.Holding[0].Policy.Life.Coverage[0].LevelPremiumPeriod = "10";
        //    return request;
        //}

        //internal static TXLifeRequest GenerateST15NQRequest(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo, DateTime? birthday, decimal coverageAmount = 250000m)
        //{
        //    var request = GenerateBaseRequest(smokerStatusInfo, genderInfo, birthday, coverageAmount);
        //    request.OLifE.Holding[0].Policy.Life.Coverage[0].LevelPremiumPeriod = "15";
        //    return request;
        //}

        //internal static TXLifeRequest GenerateST20NIRequest(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo, DateTime? birthday, decimal coverageAmount = 250000m)
        //{
        //    var request = GenerateBaseRequest(smokerStatusInfo, genderInfo, birthday, coverageAmount);
        //    request.OLifE.Holding[0].Policy.Life.Coverage[0].LevelPremiumPeriod = "20";
        //    return request;
        //}

        //private static TXLifeRequest GenerateBaseRequest(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo, DateTime? birthday, decimal coverageAmount = 250000m)
        //{
        //    var request = new Foresight.TXLifeRequest {
        //        TransRefGUID = Guid.NewGuid().ToString(),
        //        TransType = new TransType { tc = "111", Value = "OLI_TRANS_ILLCAL" },
        //        IllustrationRequest = new IllustrationRequest {
        //            RequestBasis = new[]
        //            {
        //                new RequestBasis{
        //                    VectorRequest = new []{new VectorRequest{ VectorSetType = new VectorSetType{tc = "2",Value = "OLI_VECTORSET_ALL" } }}
        //                }
        //            }
        //        },
        //        OLifE = new OLifE {
        //            Holding = new[]{
        //                new Holding
        //                {

        //                    id = "HOLDING1",
        //                    CurrencyTypeCode = new CurrencyTypeCode{tc="840",Value = "OLI_CURRENCY_USD"},
        //                    Policy = new Policy
        //                    {
        //                        LineOfBusiness = new LineOfBusiness{tc="1",Value = "OLI_LINEBUS_LIFE"},
        //                        ProductCode = "29",
        //                        IssueNation = new IssueNation{tc="1", Value = "OLI_NATION_USA"},
        //                        Jurisdiction = new Jurisdiction{tc="7", Value = "OLI_USA_CO"},
        //                        Life = new Life
        //                        {
        //                            Coverage = new[]{
        //                                new Coverage {
        //                                    id= "COV_PRIMARY",
        //                                    IndicatorCode = new IndicatorCode { tc="1", Value = "OLI_COVIND_BASE" },
        //                                    PaymentMode = new PaymentMode{tc="4", Value = "OLI_PAYMODE_MNTHLY"},
        //                                    CurrentAmt = coverageAmount,
        //                                    LevelPremiumPeriod = "15",
        //                                    LifeParticipant = new []{
        //                                        new LifeParticipant {
        //                                            PartyID = "PRIMARY",
        //                                            SmokerStat = new SmokerStat{tc = smokerStatusInfo.TC.ToString(), Value = smokerStatusInfo.Value},
        //                                            PermTableRating =  new PermTableRating{tc = "1",Value = "OLI_TBLRATE_NONE"},
        //                                            UnderwritingClass = new UnderwritingClass{tc="2", Value = "OLI_UNWRITE_PREFERRED"}
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            },
        //            Party = new[]
        //            {
        //                new Party{id = "PRIMARY", Person = new Person{FirstName = "Insured", LastName = "Proposed",Gender = new Gender{tc = genderInfo.TC.ToString(), Value = genderInfo.Value}, BirthDate = birthday??DateTime.Today}},
        //                new Party{id = "AGENT", Person = new Person{FirstName = "Advisor",LastName = "Support"}}
        //            },
        //            Relation = new[]
        //            {
        //                new Relation{id = "RELATION_PRIMARY", OriginatingObjectID = "PRIMARY", RelatedObjectID = "HOLDING1", RelationRoleCode = new RelationRoleCode{tc="32",Value = "OLI_REL_INSURED"}},
        //                new Relation{id = "RELATION_AGENT", OriginatingObjectID = "AGENT", RelatedObjectID = "HOLDING1", RelationRoleCode = new RelationRoleCode{tc="11",Value = "OLI_REL_AGENT"}}
        //            }
        //        }
        //    };

        //    return request;
        //}

        private static string ExtractTxLifeString(StreamReader reader)
        {
            var xmlDocument = XDocument.Load(reader);

            XNamespace s = "http://www.w3.org/2003/05/soap-envelope";

            var responseXml = xmlDocument.Element(s + "Envelope")
                ?.Element(s + "Body")?.FirstNode;

            return responseXml?.ToString().Replace("xmlns=\"http://ACORD.org/Standards/Life/2\"", "");
        }
    }
}