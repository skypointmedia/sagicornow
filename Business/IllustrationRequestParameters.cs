using System;
using SagicorNow.Business.Models;

namespace SagicorNow.Business
{
    public class BaseParameter
    {
        public BaseParameter()
        {
            CoverageAmount = 250000;
            WaiverOfPremium = false;
            AccidentalDeath = false;
            ChildrenCoverage = false;
        }
        public AccordOlifeValue SmokerStatusInfo { get; set; }
        public AccordOlifeValue GenderInfo { get; set; }
        public AccordOlifeValue RiskClass { get; set; }
        public decimal CoverageAmount { get; set; }
        public bool WaiverOfPremium { get; set; }
        public bool AccidentalDeath { get; set; }
        public bool ChildrenCoverage { get; set; }
        public decimal RiderAmountAccidentalDeath { get; set; }
        public decimal RiderAmountChildrenCoverage { get; set; }
        public int AgeOfYoungest { get; set; }
        public DateTime? Birthday { get; set; }
    }
    public class IllustrationRequestParameters: BaseParameter
    {

    }

    public class EAppRequestParameters: BaseParameter
    {
        public string CUSIP { get; set; }
        public StateInfo StateInfo { get; set; }
        public bool TenYearTerm { get; set; }
        public bool FifteenYearTerm { get; set; }
        public bool TwentyYearTerm { get; set; }
        public bool WholeLife { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}