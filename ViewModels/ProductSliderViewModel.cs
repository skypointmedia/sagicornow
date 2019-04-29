using SagicorNow.Foresight;

namespace SagicorNow.ViewModels
{
    public class ProductSliderModel
    {
        //public ProductSliderViewModel()
        //{
        //    var val = Response[0].IllustrationResult.ResultBasis[0].Vector[0].V[0].Value;
        //}
        //public TXLifeResponse[] Response { get; set; }
        public decimal CoverageAmount { get; set; }
        public bool TenYearTerm { get; set; }
        public bool FifteenYearTerm { get; set; }
        public bool TwentyYearTerm { get; set; }
        public bool WholeLife { get; set; }
        public decimal WholeLifePerMonthCost { get; set; }
        public decimal TenYearPerMonthCost { get; set; }
        public decimal FifteenYearPerMonthCost { get; set; }
        public decimal TwentyYearPerMonthCost { get; set; }
        public bool WavierPremium { get; set; }
        public bool AccidentalDeath { get; set; }
        public bool ChildrenCoverage { get; set; }
    }
}