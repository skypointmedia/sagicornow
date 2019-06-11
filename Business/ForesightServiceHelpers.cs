using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SagicorNow.Business.Models;
using SagicorNow.Common;
using SagicorNow.Properties;
using SagicorNow.Business.DataContracts;

namespace SagicorNow.Business
{
    internal sealed class ForesightServiceHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="soapDocument"></param>
        /// <returns></returns>
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
                    txLife = ExtractTxLife(rd);
                }
            }

            return txLife;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static XmlDocument GenerateRequestXml(IllustrationRequestParameters parameters)
        {
            var xmlRequestString =
                GenerateRequestXmLString(parameters);

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlRequestString);

            return document;
        }

       
        private static string GenerateRequestXmLString(IllustrationRequestParameters parameters)
        {
            var sb = new StringBuilder(Resources.FS_Quote_Request_Template6);
            sb.Replace("<<uuid>>", Guid.NewGuid().ToString());

            string xmlString;
            var serializer = new XmlSerializer(typeof(TXLife));
            var coverageOdj = GenerateRequest(parameters);

            using (var writer = new StringWriter())
            {
                // The XML string must not have the version and /or encoding declaration.
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    serializer.Serialize(xmlWriter, coverageOdj);
                    xmlString = writer.ToString();
                }
            }

            sb.Replace("{{request-body}}", xmlString);

            return sb.ToString();
        }

        private static ForesightTxLifeReturn ExtractTxLife(StreamReader reader)
        {
            var txLife = ExtractTxLifeString(reader);
            var serializer = new XmlSerializer(typeof(ForesightTxLifeReturn));

            StringReader read = new StringReader(txLife);

            return (ForesightTxLifeReturn) serializer.Deserialize(read);
        }

        private static TXLife GenerateRequest(IllustrationRequestParameters parameters)
        {
            var txLife = new TXLife();

            var tenYearsRequest =
                GetTermRequest("10", parameters.SmokerStatusInfo, parameters.GenderInfo, parameters.RiskClass, parameters.Birthday, parameters.CoverageAmount);
            var fifteenYearsRequest =
                GetTermRequest("15", parameters.SmokerStatusInfo, parameters.GenderInfo, parameters.RiskClass, parameters.Birthday, parameters.CoverageAmount);
            var twentyYearsRequest =
                GetTermRequest("20", parameters.SmokerStatusInfo, parameters.GenderInfo, parameters.RiskClass, parameters.Birthday, parameters.CoverageAmount);
            var wholeLifeRequest =
                GetWholeLifeRequest(parameters.SmokerStatusInfo, parameters.GenderInfo, parameters.RiskClass, parameters.Birthday, parameters.CoverageAmount);

            txLife.TXLifeRequest = new[] {tenYearsRequest, fifteenYearsRequest, twentyYearsRequest, wholeLifeRequest};

            if (!parameters.AccidentalDeath && !parameters.ChildrenCoverage && !parameters.WaiverOfPremium)
                return txLife;

            var txLifeTxLifeRequest = txLife.TXLifeRequest;
            AddRiders(parameters, ref txLifeTxLifeRequest, ProductTypes.WholeLife);
            AddRiders(parameters, ref txLifeTxLifeRequest, ProductTypes.TermLife);

            // The variable 'txLifeTxLifeRequest' value has the changes 
            txLife.TXLifeRequest = txLifeTxLifeRequest;

            return txLife;
        }

        private static void AddRiders(IllustrationRequestParameters parameters, ref TXLifeRequest[] requests, string productType)
        {
            if (parameters.AccidentalDeath)
                AddRider(parameters, ref requests, RiderType.AccidentalDeath, productType);

            if (parameters.ChildrenCoverage)
                AddRider(parameters, ref requests, RiderType.ChildrenCoverage, productType);

            if (parameters.WaiverOfPremium)
                AddRider(parameters, ref requests, RiderType.WavierOfPremium, productType);
        }

        private static void AddRider(IllustrationRequestParameters parameters, ref TXLifeRequest[] requests, RiderType type, string productType )
        {
            foreach (var txLifeRequest in requests)
            {
                foreach (var holding in txLifeRequest.OLifE.Holding)
                {
                    //if (holding.Policy.ProductCode != ProductCodes.TermLife) continue;

                    // Need to expand the array by a single element (the space for the new record).
                    var lifeCoverage = holding.Policy.Life.Coverage;
                    Array.Resize(ref lifeCoverage, holding.Policy.Life.Coverage.Length + 1);

                    switch (type)
                    {
                        case RiderType.AccidentalDeath:
                            lifeCoverage[holding.Policy.Life.Coverage.Length] = GetAccidentalDeathRider(parameters);
                            break;
                        case RiderType.ChildrenCoverage:
                            if (productType == ProductTypes.TermLife)
                                lifeCoverage[holding.Policy.Life.Coverage.Length] =
                                    GetTermChildrenCoverageRider(parameters);
                            else if (productType == ProductTypes.WholeLife)
                                lifeCoverage[holding.Policy.Life.Coverage.Length] =
                                    GetWholeLifeChildrenCoverageRider(parameters);
                            else
                                throw new ArgumentOutOfRangeException(nameof(productType), productType, null);

                            lifeCoverage[holding.Policy.Life.Coverage.Length] = GetTermChildrenCoverageRider(parameters);
                            break;
                        case RiderType.WavierOfPremium:
                            lifeCoverage[holding.Policy.Life.Coverage.Length] = GetWavierOfPremiumRider(parameters);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    // The variable 'lifeCoverage' stored the changes required.
                    holding.Policy.Life.Coverage = lifeCoverage;
                }

                if (productType != ProductTypes.WholeLife) continue;
                if (type != RiderType.ChildrenCoverage) continue;
                var parties = txLifeRequest.OLifE.Party;
                AddChildInformationForRider(ref parties, parameters);

                // The variable has all the modification required.
                txLifeRequest.OLifE.Party = parties;
            }
        }

        private static void AddChildInformationForRider(ref Party[] parties, IllustrationRequestParameters parameters)
        {
            Array.Resize(ref parties, parties.Length + 1);
            parties[parties.Length - 1] = new Party
            {
                id = "CHILD",
                Person = new Person
                {
                    FirstName = "Insured",
                    LastName = "Proposed",
                    Gender = new Gender
                    {
                        Value = "OLI_GENDER_MALE",
                        tc = "1"
                    },
                    BirthDate = (DateTime.Today.Year - parameters.AgeOfYoungest) + "-" +
                                parameters.Birthday.GetValueOrDefault().Month.ToString("D2") + "-" +
                                parameters.Birthday.GetValueOrDefault().Day.ToString("D2"),
                    Age = parameters.AgeOfYoungest
                }
            };
        }

        private static Coverage GetWavierOfPremiumRider(IllustrationRequestParameters parameters)
        {
            return new Coverage {
                id = "COV_RIDER_WOP",
                IndicatorCode = new IndicatorCode {
                    Value = "OLI_COVIND_RIDER",
                    tc = "2"
                },
                LifeCovTypeCode = new LifeCovTypeCode {
                    Value = "OLI_COVTYPE_WAIVPREMIUM",
                    tc = "21"
                }
            };
        }

        private static Coverage GetWholeLifeChildrenCoverageRider(IllustrationRequestParameters parameters)
        {
            return new Coverage {
                IndicatorCode = new IndicatorCode {
                    Value = "OLI_COVIND_RIDER",
                    tc = "2"
                },
                LifeCovTypeCode = new LifeCovTypeCode {
                    Value = "OLI_COVTYPE_CHILDTERM",
                    tc = "116"
                },
                CurrentAmt = parameters.RiderAmountChildrenCoverage,
                NumChildren = 1,
                LifeParticipant = new []
                {
                    new LifeParticipant 
                    {
                        PartyID = "CHILD",
                        LifeParticipantRoleCode = new LifeParticipantRoleCode
                        {
                            Value = "OLI_PARTICROLE_CHILD",
                            tc = "3"
                        }
                    }
                }
            };
        }

        private static Coverage GetTermChildrenCoverageRider(IllustrationRequestParameters parameters)
        {
            return new Coverage {
                IndicatorCode = new IndicatorCode {
                    Value = "OLI_COVIND_RIDER",
                    tc = "2"
                },
                LifeCovTypeCode = new LifeCovTypeCode {
                    Value = "OLI_COVTYPE_CHILDTERM",
                    tc = "116"
                },
                CurrentAmt = parameters.RiderAmountChildrenCoverage,
                NumChildren = 16
            };
        }

        private static Coverage GetAccidentalDeathRider(IllustrationRequestParameters parameters)
        {
            return new Coverage
            {
                id = "COV_RIDER_ADB",
                IndicatorCode = new IndicatorCode
                {
                    Value = "OLI_COVIND_RIDER",
                    tc = "2"
                },
                LifeCovTypeCode = new LifeCovTypeCode
                {
                    Value = "OLI_COVTYPE_ACCDTHBENE",
                    tc = "23"
                },
                CurrentAmt = parameters.RiderAmountAccidentalDeath
            };
        }

        private static TXLifeRequest GetWholeLifeRequest(AccordOlifeValue smokerStatusInfo, AccordOlifeValue genderInfo,
            AccordOlifeValue riskClass, DateTime? birthday, decimal coverageAmount = 250000m)
        {
            var request = new TXLifeRequest
            {
                TransRefGUID = Guid.NewGuid().ToString(),
                TransType = new TransType {tc = "111", Value = "OLI_TRANS_ILLCAL"},

                IllustrationRequest = new IllustrationRequest
                {
                    RequestBasis = new[]
                    {
                        new RequestBasis
                        {
                            InterestAssumption = new InterestAssumption
                            {
                                Value = "OLI_INTASSUM_CURRENT",
                                tc = "2"
                            },
                            MortalityAssumption = new MortalityAssumption
                            {
                                Value = "OLI_MORTASSMP_CURRENT",
                                tc = "3"
                            },
                            VectorRequest = new[]
                            {
                                new VectorRequest
                                {
                                    VectorItem = new[]
                                    {
                                        new VectorItem
                                        {
                                            VectorType = new VectorType
                                            {
                                                Value = "VECTOR_SCHEDMODALPREM",
                                                tc = "5"
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                },

                OLifE = new OLifE
                {
                    SourceInfo = new SourceInfo
                    {
                        SourceInfoName = "SampleApp",
                        SourceInfoDescription = "Version: 5.4.1.18",
                        FileControlID = Guid.NewGuid().ToString()
                    },
                    Holding = new[]
                    {
                        new Holding
                        {
                            id = Guid.NewGuid().ToString(),
                            HoldingTypeCode = new HoldingTypeCode
                            {
                                Value = "OLI_HOLDTYPE_POLICY",
                                tc = "2"
                            },
                            Policy = new Policy
                            {
                                LineOfBusiness = new LineOfBusiness
                                {
                                    Value = "OLI_LINEBUS_LIFE",
                                    tc = "1"
                                },
                                ProductCode = "24",
                                CarrierCode = "SAG",
                                Jurisdiction = new Jurisdiction
                                {
                                    Value = "OLI_USA_CO",
                                    tc = "7"
                                },
                                Life = new Life
                                {
                                    Coverage = new[]
                                    {
                                        new Coverage
                                        {
                                            IndicatorCode = new IndicatorCode
                                            {
                                                Value = "OLI_COVIND_BASE",
                                                tc = "1"
                                            },
                                            PaymentMode = new PaymentMode
                                            {
                                                Value = "OLI_PAYMODE_MNTHLY",
                                                tc = "4"
                                            },
                                            CurrentAmt = coverageAmount,
                                            ModalPremAmt = 459.15m,
                                            LifeParticipant = new[]
                                            {
                                                new LifeParticipant
                                                {
                                                    PartyID = "PRIMARY",
                                                    LifeParticipantRoleCode = new LifeParticipantRoleCode
                                                    {
                                                        Value = "OLI_PARTICROLE_PRIMARY",
                                                        tc = "1"
                                                    },
                                                    SmokerStat = new SmokerStat
                                                    {
                                                        Value = smokerStatusInfo.Value,
                                                        tc = smokerStatusInfo.TC.ToString()
                                                    },
                                                    TempFlatExtraAmt = 0,
                                                    UnderwritingClass = new UnderwritingClass
                                                    {
                                                        Value = "riskClass",
                                                        tc = GetRiskClassFromTC(riskClass.TC)
                                                    },
                                                    TempFlatExtraDuration = "1"
                                                }
                                            }
                                        }
                                    }
                                },
                                //ApplicationInfo = new ApplicationInfo
                                //{
                                //    RequestedIssueDate = (new DateTime(2012, 11, 24)).ToString("yyyy-MM-dd"),
                                //    QuotedPremiumAmt = 5000,
                                //    QuotedPremiumMode = new QuotedPremiumMode
                                //    {
                                //        Value = "OLI_PAYMODE_ANNUAL",
                                //        tc = "1"
                                //    }
                                //}
                            }
                        }
                    },
                    Party = new[]
                    {
                        new Party
                        {
                            id = "PRIMARY",
                            Person = new Person
                            {
                                FirstName = "Insured",
                                LastName = "Proposed",
                                Gender = new Gender
                                {
                                    Value = genderInfo.Value,
                                    tc = genderInfo.TC.ToString()
                                },
                                BirthDate = birthday.GetValueOrDefault().ToString("yyyy-MM-dd")
                            }
                        },
                        new Party
                        {
                            id = "AGENT",
                            PartyTypeCode = new PartyTypeCode
                            {
                                Value = "OLI_PT_PERSON",
                                tc = "1"
                            }
                        },
                    },
                    Relation = new[]
                    {
                        new Relation
                        {
                            id = "RELATION_PRIMARY",
                            OriginatingObjectID = "PRIMARY",
                            RelatedObjectID = "HOLDING1",
                            RelationRoleCode = new RelationRoleCode
                            {
                                Value = "OLI_REL_INSURED",
                                tc = "32"
                            }
                        },
                        new Relation
                        {
                            id = "RELATION_AGENT",
                            OriginatingObjectID = "AGENT",
                            RelatedObjectID = "HOLDING1",
                            RelationRoleCode = new RelationRoleCode
                            {
                                Value = "OLI_REL_AGENT",
                                tc = "11"
                            }
                        }
                    }
                }
            };

            return request;
        }

        private static TXLifeRequest GetTermRequest(string levelPremiumPeriod, AccordOlifeValue smokerStatusInfo,
            AccordOlifeValue genderInfo,
            AccordOlifeValue riskClass, DateTime? birthday, decimal coverageAmount = 250000m)
        {
            var request = new TXLifeRequest();

            request.TransRefGUID = Guid.NewGuid().ToString();
            request.TransType = new TransType {tc = "111", Value = "OLI_TRANS_ILLCAL"};
            request.IllustrationRequest = new IllustrationRequest
            {
                RequestBasis = new[]
                {
                    new RequestBasis
                    {
                        VectorRequest = new[]
                        {
                            new VectorRequest
                            {
                                VectorSetType = new VectorSetType {tc = "2", Value = "OLI_VECTORSET_ALL"}
                            }
                        }
                    }
                }
            };

            request.OLifE = new OLifE
            {
                Holding = new[]
                {
                    new Holding
                    {
                        id = "HOLDING1",
                        CurrencyTypeCode = new CurrencyTypeCode
                        {
                            tc = "840",
                            Value = "OLI_CURRENCY_USD"
                        },
                        Policy = new Policy
                        {
                            LineOfBusiness = new LineOfBusiness
                            {
                                Value = "OLI_LINEBUS_LIFE",
                                tc = "1"
                            },
                            ProductCode = "29",
                            IssueNation = new IssueNation
                            {
                                Value = "OLI_NATION_USA",
                                tc = "1"
                            },
                            Jurisdiction = new Jurisdiction
                            {
                                Value = "OLI_USA_CO",
                                tc = "7"
                            },
                            Life = new Life
                            {
                                Coverage = new[]
                                {
                                    new Coverage
                                    {
                                        IndicatorCode = new IndicatorCode
                                        {
                                            Value = "OLI_COVIND_BASE",
                                            tc = "1"
                                        },
                                        PaymentMode = new PaymentMode
                                        {
                                            Value = "OLI_PAYMODE_MNTHLY",
                                            tc = "4"
                                        },
                                        CurrentAmt = coverageAmount,
                                        LevelPremiumPeriod = levelPremiumPeriod,
                                        LifeParticipant = new[]
                                        {
                                            new LifeParticipant
                                            {
                                                PartyID = "PRIMARY",
                                                SmokerStat = new SmokerStat
                                                {
                                                    Value = smokerStatusInfo.Value,
                                                    tc = smokerStatusInfo.TC.ToString()
                                                },
                                                PermTableRating = new PermTableRating
                                                {
                                                    Value = "OLI_TBLRATE_NONE",
                                                    tc = "1"
                                                },
                                                UnderwritingClass = new UnderwritingClass
                                                {
                                                    Value = "riskClass",
                                                    tc = GetRiskClassFromTC(riskClass.TC)
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Party = new[]
                {
                    new Party
                    {
                        id = "PRIMARY",
                        Person = new Person
                        {
                            FirstName = "Insured",
                            LastName = "Proposed",
                            Gender = new Gender
                            {
                                Value = genderInfo.Value,
                                tc = genderInfo.TC.ToString()
                            },
                            BirthDate = birthday.GetValueOrDefault().ToString("yyyy-MM-dd")
                        }
                    },
                    new Party
                    {
                        id = "AGENT",
                        Person = new Person
                        {
                            FirstName = "Adivsor",
                            LastName = "Support"
                        }
                    },
                },
                Relation = new[]
                {
                    new Relation
                    {
                        id = "RELATION_PRIMARY",
                        OriginatingObjectID = "PRIMARY",
                        RelatedObjectID = "HOLDING1",
                        RelationRoleCode = new RelationRoleCode
                        {
                            Value = "OLI_REL_INSURED",
                            tc = "32"
                        }
                    },
                    new Relation
                    {
                        id = "RELATION_AGENT",
                        OriginatingObjectID = "AGENT",
                        RelatedObjectID = "HOLDING1",
                        RelationRoleCode = new RelationRoleCode
                        {
                            Value = "OLI_REL_AGENT",
                            tc = "11"
                        }
                    }
                }
            };

            return request;
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
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
            //webRequest.Headers.Add("SOAPAction", action);
            webRequest.Headers.Add("Action",
                "http://ACORD.org/Standards/Life/2/ProcessTXLifeRequest/ProcessTXLifeRequest");
            webRequest.Headers.Add("MessageID", Guid.NewGuid().ToString());
            webRequest.Headers.Add("ReplyTo", "<a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>");
            webRequest.Headers.Add("To",
                "https://illustration.test.sagicorlifeusa.com/SLI6/Core/Acord/TXLifeService.svc");
            webRequest.ContentType = "application/soap+xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }

    internal static class ProductCodes
    {
        public static string WholeLife = "24";
        public static string TermLife = "29";
    }

    internal static class ProductTypes
    {
        public static string WholeLife = "Whole Life";
        public static string TermLife = "Term Life";
    }

    internal enum RiderType
    {
        AccidentalDeath,
        ChildrenCoverage,
        WavierOfPremium
    }
}