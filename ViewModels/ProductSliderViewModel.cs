using SagicorNow.Foresight;

namespace SagicorNow.ViewModels
{
    public class ProductSliderModel
    {
        public decimal CoverageAmount { get; set; }
        public decimal TenYearTermPerMonthCost { get; set; }
        public bool TenYearTerm { get; set; }

        public decimal FifteenYearTermPerMonthCost { get; set; }
        public bool FifteenYearTerm { get; set; }

        public decimal TwentyYearTermPerMonthCost { get; set; }
        public bool TwentyYearTerm { get; set; }

        public decimal WholeLifePerMonthCost { get; set; }
        public bool WholeLife { get; set; }

        public bool WavierPremium { get; set; }
        public bool AccidentalDeath { get; set; }
        public bool ChildrenCoverage { get; set; }
    }

    public class FraudWaringViewModel
    {
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}