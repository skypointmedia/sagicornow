using System;
namespace SagicorNow.Models
{
    public class NeedsModel
    {
        public NeedsModel()
        {
        }

        public decimal collegeFunding { get; set; }
        public decimal lumpSumNeedsAtDeath { get; set; }
        public decimal incomeNeeds { get; set; }
        public decimal currentInvestmentCapital { get; set; }
        public decimal existingLifeInsurance { get; set; }
        public decimal presentValueOfSpouseIncomeNeeds { get; set; }
        public decimal presentValueOfIncomeNeeds { get; set; }
        public decimal overallNeeds { get; set; }
    }
}