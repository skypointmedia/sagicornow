using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SagicorNow.Data.Entities
{

    public class ProposalHistory
    {
        public ProposalHistory()
        {
            CreatedDateTime = DateTime.Now;
            LastActiveDateTime = DateTime.Now;
        }

        [Key] 
        public string Email { get; set; }
        public string ActivityId { get; set; }
        public string HashedPassword { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string StateTc { get; set; }
        public string Gender { get; set; }
        public string GenderTc { get; set; }
        public string Birthday { get; set; }
        public int Age { get; set; }
        public string Tobacco { get; set; }
        public string SmokerStatusTc { get; set; }
        public string Health { get; set; }
        public string RiskClassTc { get; set; }
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
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public bool? ReplacementPolicy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastActiveDateTime { get; set; }
        
    }
}