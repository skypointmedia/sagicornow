using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
        public string Email { get; set; }
        public string ActivityId { get; set; }
       
        public string HashedPassword { get; set; }
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
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        public bool EnableSaving { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime LastActiveDateTime { get; set; }
    }
}