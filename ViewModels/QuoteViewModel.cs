using System;
using System.Collections.Generic;
using SagicorNow.Extensions;
using SagicorNow.Models;
using System.Linq;
using SagicorNow.Business.Models;

namespace SagicorNow.ViewModels
{
    public class QuoteViewModel
    {

        public QuoteViewModel()
        {
            DisplayFormat = DisplayInfo.FULLPAGE;
        }

        public QuoteViewModel(DisplayInfo display)
        {
            DisplayFormat = display;
        }

        //properties
        private string _state;
        private string _health;
        private string _gender;
        private string _tobacco;

        public int Age => birthday?.GetAge() ?? 0;

        public string state
        {
            get => _state;
            set
            {
                _state = value;
                this.stateInfo = GetStateInfoFromCode(_state);
            }
        }

        public string gender
        {
            get => _gender;
            set
            {
                _gender = value;
                this.genderInfo = GetGenderInfoFromCode(_gender);
            }
        }

        public string tobacco
        {
            get => _tobacco;
            set
            {
                _tobacco = value;
                this.smokerStatusInfo = GetSmokerStatInfoFromCode(_tobacco);
            }
        }

        public string health
        {
            get => _health;
            set
            {
                _health = value;
                this.riskClass = QuoteViewModel.GetRiskClass(_health, _tobacco);
            }
        }

        public enum DisplayInfo
        {
            FULLPAGE,
            SINGLE_COLUMN_BARE
        }

        public DisplayInfo DisplayFormat { get; set; }

        public decimal CoverageAmount { get; set; }
        public DateTime? birthday { get; set; }
        public StateInfo stateInfo { get; set; }
        public AccordOlifeValue genderInfo { get; set; }
        public AccordOlifeValue riskClass { get; set; }
        public AccordOlifeValue smokerStatusInfo { get; set; }

        public bool? ReplacementPolicy { get; set; }
        public bool CheckReplacementPolicy { get; set; }
        public bool replacementPolicyVisible { get; set; }

        public List<string> ViewMessages { get; set; } = new List<string>();

        public string SocialSecurityNumber { get; set; }
        public bool IsNewProposal { get; set; } = true;

        public List<State> States = QuoteModel.States();
        public List<Gender> Genders = QuoteModel.Genders();
        public List<SmokerStatus> SmokerStatuses = QuoteModel.SmokerStatuses();
        public List<HealthStatus> HealthStatuses = QuoteModel.HealthStatuses();


        //get the state info from the given state code
        private StateInfo GetStateInfoFromCode(string stateCode)
        {
            var states = QuoteModel.States();

            var state1 = states.FirstOrDefault(s => s.Value == stateCode);
            return null != state1 ? new StateInfo { Code = state1.Value, Name = state1.Text, TC = state1.TC, Value = state1.TCValue } : null;
        }

        //get the state info from the given tc value
        public static StateInfo GetStateInfoFromTC(int tc)
        {
            var states = QuoteModel.States();

            var state = states.FirstOrDefault(s => s.TC == tc);
            return null != state ? new StateInfo { Code = state.Value, Name = state.Text, TC = state.TC, Value = state.TCValue } : null;
        }

        private AccordOlifeValue GetGenderInfoFromCode(string g)
        {
            switch (g)
            {
                case "OLI_GENDER_MALE":
                    return new AccordOlifeValue() { TC = 1, Value = g };
                case "OLI_GENDER_FEMALE":
                    return new AccordOlifeValue() { TC = 2, Value = g };
                default:
                    return null;
            }
        }

        public static AccordOlifeValue GetRiskClass(string h, string smokerStat)
        {
            if (smokerStat == "OLI_TOBACCO_CURRENT")
            {
                //tobacco
                switch (h)
                {
                    case "OLI_UNWRITE_PREFERRED":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.PREFERRED_TOBACCO, Value = h };
                    case "OLI_UNWRITE_STANDARD":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.STANDARD_TOBACCO, Value = h };
                    case "OLI_UNWRITE_RATED":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.RATED_TOBACCO, Value = h };
                    case "OLI_UNWRITE_SUPERB":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.PREFERREDPLUS_TOBACCO, Value = h };
                    case "OLI_UNWRITE_POOR":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.RATED2_TOBACCO, Value = h };
                    default:
                        return null;
                }
            }
            else
            {
                //non-tobacco
                switch (h)
                {
                    case "OLI_UNWRITE_PREFERRED":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.PREFERRED_NONTOBACCO, Value = h };
                    case "OLI_UNWRITE_STANDARD":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.STANDARD_NONTOBACCO, Value = h };
                    case "OLI_UNWRITE_RATED":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.RATED_NONTOBACCO, Value = h };
                    case "OLI_UNWRITE_SUPERB":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.PREFERREDPLUS_NONTOBACCO, Value = h };
                    case "OLI_UNWRITE_POOR":
                        return new AccordOlifeValue() { TC = (int)RiskClasses.RATED2_NONTOBACCO, Value = h };
                    default:
                        return null;
                }
            }

        }

        /// <summary>
        /// all the new risk classes and their values as enumerations
        /// </summary>
        public enum RiskClasses
        {
            PREFERRED_NONTOBACCO = 1,
            PREFERRED_TOBACCO = 2,
            STANDARD_NONTOBACCO = 3,
            STANDARD_TOBACCO = 4,
            RATED_NONTOBACCO = 5,
            RATED_TOBACCO = 6,
            PREFERREDPLUS_NONTOBACCO = 7,
            PREFERREDPLUS_TOBACCO = 8,
            RATED2_NONTOBACCO = 9,
            RATED2_TOBACCO = 10
        }

        /// <summary>
        /// return the max coverage based on age
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        public static decimal GetMaxCoverageBasedOnAge(int age)
        {
            decimal maxCoverage = 0M;
            if (age <= 45)
                maxCoverage = 1000000M;
            else if (age <= 55)
                maxCoverage = 750000M;
            else if (age <= 65)
                maxCoverage = 500000M;

            return maxCoverage;
        }

        private AccordOlifeValue GetSmokerStatInfoFromCode(string h)
        {
            switch (h)
            {
                case "OLI_TOBACCO_NEVER":
                    return new AccordOlifeValue() { TC = 1, Value = h };
                case "OLI_TOBACCO_CURRENT":
                    return new AccordOlifeValue() { TC = 3, Value = h };
                default:
                    return null;
            }
        }

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

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public decimal AccidentalDeathRiderAmount { get; set; }

        public decimal ChildrenCoverageRiderAmount { get; set; }

        public int AgeOfYoungest { get; set; }
    }
}
