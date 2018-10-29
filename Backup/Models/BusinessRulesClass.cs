﻿﻿using System;
using System.Web;
using SagicorNow.Models; 
using System.ComponentModel.DataAnnotations;

namespace SagicorNow.Models
{
    public static class BusinessRulesClass
    {

		// Determin if the user is eligible to receive a quote
        public static EligibilityInfo IsEligible(int age, string state, string tobacco, string health, Boolean replacementPolicy)
        {
            var validStates = new System.Collections.Generic.List<String>() { "florida", "texas", "arizona" };

            // If the user is not between 18 and 65 return false
            if ((age < 18 || age > 65) && tobacco == "OLI_TOBACCO_CURRENT" && health == "OLI_UNWRITE_RATED")
            {
                //age risk reject 
                return new EligibilityInfo() { IsEligible = false, EligibilityMessage = "Because of age and health factors, we are unable to issue coverage online. Dedicated Sagicor Life agents are available to review other options.", State = state };
            }
			else if (age < 18 || age > 65)
			{
                return new EligibilityInfo() { IsEligible = false, EligibilityMessage = "Because of an age limitation, we are unable to issue coverage online. Dedicated Sagicor Life agents are available to review other options.", State = state };
			}
			// If the user has ever smoked tobacco return false
			else if (tobacco == "OLI_TOBACCO_CURRENT" && health == "OLI_UNWRITE_RATED")
			{
				return new EligibilityInfo() { IsEligible = false, EligibilityMessage = "Because of health factors, we are unable to issue coverage online. Dedicated Sagicor Life agents are available to review other options.", State = state };
			}
            // If the user does not live in one of these states return false
            else if (!validStates.Contains(state.ToLower()))
			{
                return new EligibilityInfo() { IsEligible = false, EligibilityMessage = "Dedicated Sagicor Life agents are available to review other options.", State = state };
			}
			// if replacement
            else if (replacementPolicy == true)
			{
                return new EligibilityInfo() { IsEligible = false, EligibilityMessage = "Because you are replacing an existing policy, we are unable to issue coverage online. Dedicated Sagicor Life agents are available to review other options.", IsReplacememtReject = true, State = state };
			}
			else
            {
                return new EligibilityInfo() { IsEligible = true, EligibilityMessage = "Eligibility Confirmed."}; 
            }
		}

        /// <summary>
        /// Medicals the reject message.
        /// </summary>
        /// <returns>The reject message.</returns>
        public static string MedicalRejectMessage()
        {
            return "Because of an answer provided, we are unable to issue coverage online. Dedicated Sagicor Life agents are available to review other options.";
        }


		public static decimal CalculateLumpSumNeedsAtDeath(decimal finalExpenses)
		{
            return finalExpenses; //need calculation for this
		}

        /// <summary>
        /// Calculates the present value of income needs.
        /// </summary>
        /// <returns>The present value of income needs.</returns>
        public static decimal CalculatePresentValueOfIncomeNeeds(decimal rate, decimal income, int period)
        {
            return (decimal)(Microsoft.VisualBasic.Financial.PV((double)rate, (double)period, (double)income, 0, Microsoft.VisualBasic.DueDate.BegOfPeriod)); 
        }


        /// <summary>
        /// Calculates the overall needs.
        /// </summary>
        /// <returns>The overall needs.</returns>
        /// <param name="presentValueOfIncomeNeeds">Present value of income needs.</param>
        /// <param name="lumpSumNeeds">Lump sum needs.</param>
        /// <param name="currentInvestmentCapital">Current investment capital.</param>
        /// <param name="existingLifeInsurance">Existing life insurance.</param>
        /// <param name="presentValueOfSpouseIncome">Present value of spouse income.</param>
		public static decimal CalculateOverallNeeds(decimal presentValueOfIncomeNeeds, decimal lumpSumNeeds, decimal currentInvestmentCapital, decimal existingLifeInsurance, decimal presentValueOfSpouseIncome)
		{
            try
            {
                return (presentValueOfIncomeNeeds + lumpSumNeeds) - currentInvestmentCapital - existingLifeInsurance - presentValueOfSpouseIncome;
            }
            catch (Exception)
            {
                return default(decimal);

                //log error

            }

		}


    }
}
