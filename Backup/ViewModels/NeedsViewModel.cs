using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SagicorNow.Models;

namespace SagicorNow.ViewModels
{
    public class NeedsViewModel : QuoteViewModel
    {
        //constructor
        public NeedsViewModel()
        {
            
        }

        //properties
        public String zipcode { get; set; }

        public string familyMakeup { get; set; }

        public List<FamilyMakeup> FamilyMakeupOptions = QuoteModel.FamilyMakeupOptions();
        public List<SchoolType> SchoolTypes = QuoteModel.SchoolTypes();

        public decimal? finalExpenses { get; set; }

        public decimal? mortgageExpenses { get; set; }

        public decimal? otherDebtExpenses { get; set; }

        public decimal? totalAnnualIncome { get; set; }

        public int? yearsOfIncomeLeft { get; set; }

        public decimal? currentSavings { get; set; }

        public decimal? currentRetirementSavings { get; set; }

        public decimal? existingLifeInsuranceValue { get; set; }

		public decimal? spouseTotalAnnualIncome { get; set; }

		public int? spouseYearsOfIncomeLeft { get; set; }

        //family makeup
        public int? numKidsNeedFunding { get; set; }
        public string schoolType { get; set; }

        public NeedsModel needsModel { get; set; }
		
    }
}
