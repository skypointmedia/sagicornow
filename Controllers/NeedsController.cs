using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SagicorNow.Models;
using SagicorNow.ViewModels;

namespace SagicorNow.Controllers
{
    public class NeedsController : Controller
    {
        public ActionResult Index()
        {
            NeedsViewModel modl = new NeedsViewModel();
            return View(modl);
        }

        private const string SCHOOLTYPES_PUBLIC = "PUBLIC";
        private const string SCHOOLTYPES_PRIVATE = "PRIVATE";

        [HttpPost]
        public ActionResult Index(NeedsViewModel vm)
        {

            try
            {
                //min 25,000
                //max 500,000
                //25,000 increments

                //calculate needs 
                decimal insuredNeedsRate = Decimal.Divide(1.06M, 1.03M) - 1.00M;
                decimal spouseTaxRate = 0.25M;
                decimal inflationRate = 0.03M;

                var numKidsNeedFunding = GetDefaultIfNull(vm.numKidsNeedFunding);
                var collegeFunding = Decimal.Multiply(numKidsNeedFunding, (vm.schoolType == SCHOOLTYPES_PUBLIC ? 75772.00M : 169676.00M));

                var lumpSumNeedsAtDeath = GetDefaultIfNull(vm.finalExpenses) + GetDefaultIfNull(vm.otherDebtExpenses) + GetDefaultIfNull(vm.mortgageExpenses) + collegeFunding;

                var incomeNeeds = vm.totalAnnualIncome;
                decimal currentInvestmentCapital = GetDefaultIfNull(vm.currentRetirementSavings) + GetDefaultIfNull(vm.currentSavings);
                decimal existingLifeInsurance = GetDefaultIfNull(vm.existingLifeInsuranceValue);

                decimal presentValueOfSpouseIncomeNeeds = Math.Abs(BusinessRulesClass.CalculatePresentValueOfIncomeNeeds(inflationRate, vm.spouseTotalAnnualIncome == null ? 0.0M : Decimal.Multiply((1 - spouseTaxRate), GetDefaultIfNull(vm.spouseTotalAnnualIncome)),
                                                                                                                         vm.spouseYearsOfIncomeLeft == null ? 0 : vm.spouseYearsOfIncomeLeft.Value));
                decimal presentValueOfIncomeNeeds = BusinessRulesClass.CalculatePresentValueOfIncomeNeeds(insuredNeedsRate, Decimal.Multiply(-1.0M, (GetDefaultIfNull(vm.totalAnnualIncome))),
                                                                                                          vm.yearsOfIncomeLeft == null ? 0 : vm.yearsOfIncomeLeft.Value);

                var overallNeeds = BusinessRulesClass.CalculateOverallNeeds(presentValueOfIncomeNeeds,
                                                                            lumpSumNeedsAtDeath,
                                                                            currentInvestmentCapital,
                                                                            existingLifeInsurance,
                                                                            presentValueOfSpouseIncomeNeeds);
                var nm = new NeedsModel()
                {
                    collegeFunding = collegeFunding,
                    currentInvestmentCapital = currentInvestmentCapital,
                    existingLifeInsurance = existingLifeInsurance,
                    incomeNeeds = GetDefaultIfNull(incomeNeeds),
                    lumpSumNeedsAtDeath = lumpSumNeedsAtDeath,
                    overallNeeds = overallNeeds,
                    presentValueOfIncomeNeeds = presentValueOfIncomeNeeds,
                    presentValueOfSpouseIncomeNeeds = presentValueOfSpouseIncomeNeeds
                };

                vm.CoverageAmount = this.GetOverallNeedsBasedOnBand(overallNeeds);
                vm.needsModel = nm;

                return View(vm); //send to view with the results 

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var msg = "There was an error during the needs calculation.";
                vm.ViewMessages.Add(msg);
                return View(vm);
            }
        }

        //get default decimal value if null
        private decimal GetDefaultIfNull(decimal? val)
        {
            return val == null ? 0.0M : val.Value;
        }

        private decimal GetOverallNeedsBasedOnBand(decimal need)
        {
            decimal retVal = 25000.0M;

            if (need <= 25000.00M)
                return retVal;

            retVal = (Math.Ceiling(need / 25000.00M)) * 25000.00M;

            return retVal;
        }
    }
}
