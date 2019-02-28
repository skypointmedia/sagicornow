//using InsTech.EverGreen.Acord;
//using InsTech.EverGreen.BackOffice;
//using InsTech.EverGreen.Models;
//using InsTech.Instrumentation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NewBusinessService;
using SagicorNow.ForeSightQuoting;

namespace InsTech.EverGreen.Integration.ForeSightQuoting
{
    /// <summary>
    /// Utility class to help with creating ACORD models
    /// </summary>
    public class Util
    {
        private const string HOLDING_ID = "HOLDING1";
        private const string PRIM_PARTY_ID = "PRIMARY";
        private const string AGENT_PARTY_ID = "AGENT";

        static OLifEExtension EXPRESS_EXT;
        static OLifEExtension TERM_EXT;

        private static string _periods;

        /// <summary>
        /// Initializes the static members of <see cref="Util"/> class.
        /// </summary>
        static Util()
        {
            TxLifeUtility helper = new TxLifeUtility("");
            EXPRESS_EXT = helper.CreateOLifEExtension("ProductType", "Express");

            Relation relation;
            Agent = CreateAgent(out relation);
            AgentRelation = relation;
        }

        public static void InitLists(Configuration providerConfig)
        {
            string periods = providerConfig.AppSettings.Settings[ConfigKeys.QuotingTermPeriods].Value;
            if (periods != _periods)
            {
                _periods = periods;
                TermPeriodList = _periods.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// Gets the term period list.
        /// </summary>
        public static List<String> TermPeriodList { get; private set; }

        public static List<String> RiderList { get; private set; }

        /// <summary>
        /// Gets the static Agent object used for all TxLifeRequests
        /// </summary>
        public static Party Agent { get; private set; }

        /// <summary>
        /// Gets the static Agent Relation used for all TxLifeRequests.
        /// </summary>
        public static Relation AgentRelation { get; private set; }

        /// <summary>
        /// Creates the ACORD Holding object using the specified parameters.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="smokerStat">The smoker stat.</param>
        /// <param name="underClass">The under class.</param>
        /// <param name="coverageAmt">The coverage amt.</param>
        /// <param name="payMode">The pay mode.</param>
        /// <param name="jurisdiction">The jurisdiction.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="termPeriod">The term period.</param>
        /// <returns></returns>
        public static Holding CreateHolding(string productCode, OLI_LU_SMOKERSTAT_Enum smokerStat, int underClass, decimal coverageAmt, OLI_LU_PAYMODE_Enum payMode, string jurisdiction = null, string currency = null, string termPeriod = null, string termOption = null)
        {
            // Create the Participant for the Term coverage
            string rating = ((int)OLI_LU_RATINGS_Enum.OLI_TBLRATE_NONE).ToString();
            string smoker = ((int)smokerStat).ToString();
            string underwritingClass = ((int)underClass).ToString();
            LifeParticipant primary = new LifeParticipant
            {
                PartyID = PRIM_PARTY_ID,
                UnderwritingClass = new UnderwritingClass { tc = underwritingClass },
                PermTableRating = new PermTableRating { tc = rating, Value = ACORDEnum.GetCodeName<OLI_LU_RATINGSEnum>(rating) },
                SmokerStat = new SmokerStat { tc = smoker, Value = ACORDEnum.GetCodeName<OLI_LU_SMOKERSTATEnum>(smoker) }
            };

            // Create the primary term coverage 
            List<Coverage> coverages = new List<Coverage>();
            string covIndCode = ((int)OLI_LU_COVINDCODE_Enum.OLI_COVIND_BASE).ToString();
            Coverage primCov = new Coverage
            {
                id = "COV_PRIMARY",
                EffDate = DateTime.Now,
                IndicatorCode = new IndicatorCode { tc = covIndCode, Value = ACORDEnum.GetCodeName<OLI_LU_COVINDCODEEnum>(covIndCode) },
                LifeParticipant = new List<LifeParticipant> { primary },
                CurrentAmt = coverageAmt,
                PaymentMode = new PaymentMode { tc = ((int)payMode).ToString() },
                OLifEExtension = new List<OLifEExtension> { EXPRESS_EXT }
            };
            if (!String.IsNullOrEmpty(termPeriod))
                primCov.LevelPremiumPeriod = termPeriod;

            // FSE only supports these term options
            if (!String.IsNullOrEmpty(termOption) && "[20][65][70][80]".Contains($"[{termOption}]"))
            {
                TxLifeUtility helper = new TxLifeUtility("");
                TERM_EXT = helper.CreateOLifEExtension("TermPlan", termOption);
                primCov.OLifEExtension.Add(TERM_EXT);
            }                

            coverages.Add(primCov);

            // Finally create the holding which is returned. 
            string lineOfBiz = ((int)OLI_LU_LINEBUS_Enum.OLI_LINEBUS_LIFE).ToString();
            Holding holding = new Holding
            {
                id = HOLDING_ID,
                Policy = new Policy
                {
                    ProductCode = productCode,
                    LineOfBusiness = new LineOfBusiness { tc = lineOfBiz, Value = ACORDEnum.GetCodeName<OLI_LU_LINEBUSEnum>(lineOfBiz) },
                    Life = new Life { Coverage = coverages }
                }
            };

            // Only add Currency and Jurisdiction if we have a valid value. 
            // Otherwise the Quoting service may error with empty values. 
            if (!String.IsNullOrEmpty(currency))
            {
                holding.CurrencyTypeCode = new CurrencyTypeCode { tc = currency };
            }
            if (!String.IsNullOrEmpty(jurisdiction))
            {
                holding.Policy.Jurisdiction = new Jurisdiction { tc = jurisdiction };
            }

            return holding;
        }

        /// <summary>
        /// Creates the ACORD Life object for a Term product.
        /// </summary>
        /// <param name="childTerm">if greater than 0 then add [child rider].</param>
        /// <param name="accidentalDB">if greater than 0 then add [accidental database].</param>
        /// <param name="accelerateBene">if set to <c>true</c> [accelerate bene].</param>
        /// <param name="waiverOfPrem">if set to <c>true</c> [waiver of prem].</param>
        /// <returns></returns>
        public static List<Coverage> CreateTermCoverages(decimal childTerm, decimal accidentalDb, bool accelerateBene, bool waiverOfPrem)
        {
            List<Coverage> coverages = new List<Coverage>();

            // Create the coverages for each rider.
            if (accelerateBene)
                coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_ACCPAY, 0));
            if (waiverOfPrem)
                coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_WAIVSCHED, 0));
            if (accidentalDb > 0)
                coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_ACCDTHBENE, accidentalDb));
            if (childTerm > 0)
                coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_CHILDTERM, childTerm));

            return coverages;
        }
        public static List<Coverage> CreateWLCoverages(decimal childTerm, DateTime youngestChildDOB, decimal accidentalDb, bool waiverOfPrem, TXLifeRequest request)
        {
            List<Coverage> coverages = new List<Coverage>();

            // Create the coverages for each rider.
            if (waiverOfPrem)
                coverages.Add(CreateRiderCov(1002500014.ToString(), 0)); //waiver of premium, Sag has special number for this for WL.
            if (accidentalDb > 0)
                coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_ACCDTHBENE, accidentalDb));
            if (childTerm > 0)
            {
                string riderCode = ((int)OLI_LU_COVTYPE_Enum.OLI_COVTYPE_CHILDTERM).ToString();
                string covIndCode = ((int)OLI_LU_COVINDCODE_Enum.OLI_COVIND_RIDER).ToString();
                Coverage covRider = new Coverage
                {
                    id = "COV_RIDER", // riderType.ToString(),
                    IndicatorCode = new IndicatorCode { tc = covIndCode, Value = ACORDEnum.GetCodeName<OLI_LU_COVINDCODEEnum>(covIndCode) },
                    LifeCovTypeCode = new LifeCovTypeCode { tc = riderCode, Value = ACORDEnum.GetCodeName<OLI_LU_COVTYPEEnum>(riderCode) },
                };

                covRider.CurrentAmt = childTerm;
                covRider.NumChildren = 1.ToString();
                var youngestPartyId = (Guid.NewGuid()).ToString();
                covRider.LifeParticipant.Add(new LifeParticipant
                {
                    PartyID = youngestPartyId,
                });
                var youngestParty = new Party
                {
                    id = youngestPartyId,
                };
                youngestParty.Person.BirthDate = youngestChildDOB;
                youngestParty.Person.Age = ((int)((DateTime.UtcNow - youngestChildDOB).TotalDays / 356)).ToString(); //TODO Make better
                request.OLifE.Party.Add(youngestParty);
                coverages.Add(covRider);
            }

           // coverages.Add(CreateRiderCov(OLI_LU_COVTYPE_Enum.OLI_COVTYPE_CHILDTERM, childTerm));


            return coverages;
        }

        /// <summary>
        /// Creates the ACORD Rider covervage object for the specified rider type nad coverage amount.
        /// </summary>
        /// <param name="riderType">Type of the rider.</param>
        /// <param name="coverageAmt">The coverage amt.</param>
        /// <returns></returns>
        public static Coverage CreateRiderCov(OLI_LU_COVTYPE_Enum riderType, decimal coverageAmt)
        {
            return CreateRiderCov(((int)riderType).ToString(), coverageAmt); ;
        }
        public static Coverage CreateRiderCov(string riderCode, decimal coverageAmt)
        {
            string covIndCode = ((int)OLI_LU_COVINDCODE_Enum.OLI_COVIND_RIDER).ToString();
            Coverage covRider = new Coverage
            {
                id = "COV_RIDER", // riderType.ToString(),
                IndicatorCode = new IndicatorCode { tc = covIndCode, Value = ACORDEnum.GetCodeName<OLI_LU_COVINDCODEEnum>(covIndCode) },
                LifeCovTypeCode = new LifeCovTypeCode { tc = riderCode, Value = ACORDEnum.GetCodeName<OLI_LU_COVTYPEEnum>(riderCode) },
            };

            if (coverageAmt > 0)
                covRider.CurrentAmt = coverageAmt;

            return covRider;
        }

        /// <summary>
        /// Creates the ACORD TxLifeRequest with the specified primary party object. 
        /// NOTE: The static Agent party object will be included as well.
        /// </summary>
        /// <param name="primary">The primary.</param>
        /// <param name="primaryRel">The primary relative.</param>
        /// <returns></returns>
        public static TXLifeRequest CreateTxLifeRequest(Party primary, Relation primaryRel, bool isTerm = false)
        {
            var illustReq = new IllustrationRequest { RequestBasis = new List<RequestBasis>() };
            illustReq.RequestBasis.Add(new RequestBasis());


            string interestAssumption = ((int)TC_INTASSUMPTION_Enum.OLI_INTASSUM_CURRENT).ToString();
            illustReq.RequestBasis[0].InterestAssumption = new InterestAssumption { tc = interestAssumption, Value = ACORDEnum.GetCodeName<TC_INTASSUMPTIONEnum>(interestAssumption) };

            string mortalityAssumption = ((int)TC_MORTASSUMPTION_Enum.OLI_MORTASSMP_CURRENT).ToString();
            illustReq.RequestBasis[0].MortalityAssumption = new MortalityAssumption { tc = mortalityAssumption, Value = ACORDEnum.GetCodeName<TC_MORTASSUMPTIONEnum>(mortalityAssumption) };

            // Specify that we only want to receive the Scheduled Modal Premium Vector Type. 
            string vectorType = ((int)TC_VECTOR_Enum.VECTOR_SCHEDMODALPREM).ToString();
            var vType = new VectorType { tc = vectorType, Value = ACORDEnum.GetCodeName<TC_VECTOREnum>(vectorType) };
            illustReq.RequestBasis[0].VectorRequest = new List<VectorRequest>();
            illustReq.RequestBasis[0].VectorRequest.Add(new VectorRequest());
            illustReq.RequestBasis[0].VectorRequest[0].VectorItem.Add(new VectorItem { VectorType = vType });

            var tc = (int)OLI_LU_TRANS_TYPE_CODES_Enum.OLI_TRANS_ILLCAL;
            TXLifeRequest req = new TXLifeRequest
            {
                TransRefGUID = Guid.NewGuid().ToString(),
                TransType = new TransType { tc = tc.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_TRANS_TYPE_CODESEnum>(tc) },
                IllustrationRequest = illustReq,
                OLifE = new OLifE()
            };

            req.OLifE.Party.Add(primary);
            req.OLifE.Relation.Add(primaryRel);

            req.OLifE.Party.Add(Agent);
            req.OLifE.Relation.Add(AgentRelation);

            return req;
        }

        public static IllustrationTxn CreateIllustTxnWL(decimal amount)
        {
            string txnSecondary = ((int)TC_ILLUSSECTYPE_Enum.ILL_SEC_DIST_MAX).ToString();
            IllustrationTxn txn = new IllustrationTxn();
            txn.IllusTxnAmt = amount;
            txn.IllusTxnPrimaryType = new IllusTxnPrimaryType { tc = "1002500002", Value = "ILL_PRI_FACE_AMT" };
            txn.IllusTxnSecondaryType = new IllusTxnSecondaryType { tc = txnSecondary, Value = ACORDEnum.GetCodeName<TC_ILLUSSECTYPEEnum>(txnSecondary) };

            return txn;
        }

        /// <summary>
        /// Creates a Party object using a standard ID and creates an ACORD Relation which is returned. 
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <param name="relation">The relation.</param>
        /// <returns></returns>
        public static Party CreatePrimary(String gender, string firstName, string lastName, DateTime birthdate, out Relation relation)
        {
            int tc = (int)OLI_LU_REL_Enum.OLI_REL_INSURED;
            relation = new Relation
            {
                id = "RELATION_PRIMARY",
                OriginatingObjectID = PRIM_PARTY_ID,
                RelatedObjectID = HOLDING_ID,
                RelationRoleCode = new RelationRoleCode { tc = tc.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_RELEnum>(tc) }
            };

            var party = new Party
            {
                id = PRIM_PARTY_ID,
                Person = new Person
                {
                    Gender = new Gender { tc = gender, Value = ACORDEnum.GetCodeName<OLI_LU_GENDEREnum>(gender) },
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthdate
                }
            };

            return party;
        }

        public static Address CreateAddress(string city, int state, string zip, string line1, string line2, OLI_LU_ADTYPE_Enum addrType = OLI_LU_ADTYPE_Enum.OLI_ADTYPE_HOME)
        {
            int addrTypeCode = (int)addrType;
            Address addr = new Address
            {
                City = city,
                AddressStateTC = new AddressStateTC { tc = state.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_STATEEnum>(state) },
                Zip = zip,
                Line1 = line1,
                Line2 = line2,
                AddressTypeCode = new AddressTypeCode { tc = addrTypeCode.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_ADTYPEEnum>(addrTypeCode) }
            };

            return addr;
        }

        public static Party CreateAgent(out Relation relation)
        {
            int tc = (int)OLI_LU_REL_Enum.OLI_REL_AGENT; ;
            relation = new Relation
            {
                id = "RELATION_AGENT",
                OriginatingObjectID = AGENT_PARTY_ID,
                RelatedObjectID = HOLDING_ID,
                RelationRoleCode = new RelationRoleCode { tc = tc.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_RELEnum>(tc) }
            };

            tc = (int)OLI_LU_PARTY_Enum.OLI_PT_PERSON;
            var party = new Party
            {
                id = AGENT_PARTY_ID,
                PartyTypeCode = new PartyTypeCode { tc = tc.ToString(), Value = ACORDEnum.GetCodeName<OLI_LU_PARTYEnum>(tc) }
            };

            return party;
        }
    }

    /// <summary>
    /// Defines all the keys used in the provider config
    /// </summary>
    public class ConfigKeys
    {
        /// <summary>
        /// Gets the Term Product Code. 
        /// </summary>
        public static string QuotingTermProdCode { get { return "QuotingTermProdCode"; } }

        /// <summary>
        /// Gets the Whole Life Product Code. 
        /// </summary>
        public static string QuotingWholeLifeProdCode { get { return "QuotingWholeLifeProdCode"; } }

        /// <summary>
        /// Gets the delmited list of Term Periods. Delimited by a bar '|'.
        /// </summary>
        public static string QuotingTermPeriods { get { return "QuotingTermPeriods"; } }

        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupA { get { return "QuotingCovGroupA"; } }

        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupB { get { return "QuotingCovGroupB"; } }

        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupC { get { return "QuotingCovGroupC"; } }

        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupD { get { return "QuotingCovGroupD"; } }

        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupE { get { return "QuotingCovGroupE"; } }
        
        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupF { get { return "QuotingCovGroupF"; } }
        
        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupG { get { return "QuotingCovGroupG"; } }
        
        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupH { get { return "QuotingCovGroupH"; } }
        
        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupI { get { return "QuotingCovGroupI"; } }
        
        /// <summary>
        /// Gets the delmited list for the Quoting Coverage group. 
        /// Identifies a group of coverage amounts to quote from ForeSight. 
        /// Delimited by a bar '|'.
        /// </summary>
        public static string QuotingCovGroupJ { get { return "QuotingCovGroupJ"; } }

    }

}

namespace SagicorNow.ForeSightQuoting
{
    enum OLI_LU_SMOKERSTAT_Enum
    {
        OLI_LU_RATINGS_Enum
    }
}