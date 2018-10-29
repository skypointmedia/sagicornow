using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SagicorNow.Extensions;
using SagicorNow.Models;

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

        public int Age
        {
            get
            {
                return birthday == null ? 0 : birthday.Value.GetAge();
            }
        }

        public String state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                this.stateInfo = GetStateInfoFromCode(_state);
            }
        }

        public String gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
                this.genderInfo = GetGenderInfoFromCode(_gender);
            }
        }

        public String tobacco
        {
            get
            {
                return _tobacco;
            }
            set
            {
                _tobacco = value;
                this.smoketStatusInfo = GetSmokerStatInfoFromCode(_tobacco);
            }
        }

        public String health
        {
            get
            {
                return _health;
            }
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

        public Decimal CoverageAmount { get; set; }
        public DateTime? birthday { get; set; }
        public StateInfo stateInfo { get; set; }
        public AccordOlifeValue genderInfo { get; set; }
        public AccordOlifeValue riskClass { get; set; }
        public AccordOlifeValue smoketStatusInfo { get; set; }

        public bool? ReplacementPolicy { get; set; }
        public bool CheckReplacementPolicy { get; set; }
        public bool replacementPolicyVisible { get; set; }

        private List<string> _viewMessages = new List<string>();
        public List<string> ViewMessages { get { return _viewMessages; } set { _viewMessages = value; } }

        public List<State> States = QuoteModel.States();
        public List<Gender> Genders = QuoteModel.Genders();
        public List<SmokerStatus> SmokerStatuses = QuoteModel.SmokerStatuses();
        public List<HealthStatus> HealthStatuses = QuoteModel.HealthStatuses();


        //get the state info from the given state code
        private StateInfo GetStateInfoFromCode(string stateCode)
        {
            switch (stateCode)
            {
                case "AZ":
                    return new StateInfo() { Code = stateCode, Name = "Arizona", TC = 4, Value = "OLI_USA_AZ" };
                case "FL":
                    return new StateInfo() { Code = stateCode, Name = "Florida", TC = 12, Value = "OLI_USA_FL" };
                case "TX":
                    return new StateInfo() { Code = stateCode, Name = "Texas", TC = 51, Value = "OLI_USA_TX" };

                case "AL":
                    return new StateInfo() { Code = stateCode, Name = "Alabama", TC = 1, Value = "OLI_USA_AL" };
                case "AR":
                    return new StateInfo() { Code = stateCode, Name = "Arkansas", TC = 5, Value = "OLI_USA_AR" };
                case "CA":
                    return new StateInfo() { Code = stateCode, Name = "California", TC = 6, Value = "OLI_USA_CA" };
                case "CO":
                    return new StateInfo() { Code = stateCode, Name = "Colorado", TC = 7, Value = "OLI_USA_CO" };
                case "GA":
                    return new StateInfo() { Code = stateCode, Name = "Georgia", TC = 13, Value = "OLI_USA_GA" };
                case "HI":
                    return new StateInfo() { Code = stateCode, Name = "Hawaii", TC = 15, Value = "OLI_USA_HI" };

                case "IA":
                    return new StateInfo() { Code = stateCode, Name = "Iowa", TC = 19, Value = "OLI_USA_IA" };
                case "ID":
                    return new StateInfo() { Code = stateCode, Name = "Idaho", TC = 16, Value = "OLI_USA_ID" };
                case "IL":
                    return new StateInfo() { Code = stateCode, Name = "Illinois", TC = 17, Value = "OLI_USA_IL" };

                case "IN":
                    return new StateInfo() { Code = stateCode, Name = "Indiana", TC = 18, Value = "OLI_USA_IN" };
                case "KS":
                    return new StateInfo() { Code = stateCode, Name = "Kansas", TC = 20, Value = "OLI_USA_KS" };
                case "KY":
                    return new StateInfo() { Code = stateCode, Name = "Kentucky", TC = 21, Value = "OLI_USA_KY" };

                case "LA":
                    return new StateInfo() { Code = stateCode, Name = "Louisiana", TC = 22, Value = "OLI_USA_LA" };
                case "MA":
                    return new StateInfo() { Code = stateCode, Name = "Massachusetts", TC = 26, Value = "OLI_USA_MA" };
                case "MD":
                    return new StateInfo() { Code = stateCode, Name = "Maryland", TC = 25, Value = "OLI_USA_MD" };

                case "MI":
                    return new StateInfo() { Code = stateCode, Name = "Michigan", TC = 27, Value = "OLI_USA_MI" };
                case "MN":
                    return new StateInfo() { Code = stateCode, Name = "Minnesota", TC = 28, Value = "OLI_USA_MN" };
                case "MO":
                    return new StateInfo() { Code = stateCode, Name = "Missouri", TC = 30, Value = "OLI_USA_MO" };

                case "MS":
                    return new StateInfo() { Code = stateCode, Name = "Mississippi", TC = 29, Value = "OLI_USA_MS" };
                case "NC":
                    return new StateInfo() { Code = stateCode, Name = "North Carolina", TC = 38, Value = "OLI_USA_NC" };
                case "NE":
                    return new StateInfo() { Code = stateCode, Name = "Nebraska", TC = 32, Value = "OLI_USA_NE" };

                case "NH":
                    return new StateInfo() { Code = stateCode, Name = "New Hampshire", TC = 34, Value = "OLI_USA_NH" };
                case "NJ":
                    return new StateInfo() { Code = stateCode, Name = "New Jersey", TC = 35, Value = "OLI_USA_NJ" };
                case "NM":
                    return new StateInfo() { Code = stateCode, Name = "New Mexico", TC = 36, Value = "OLI_USA_NM" };

                case "NV":
                    return new StateInfo() { Code = stateCode, Name = "Nevada", TC = 33, Value = "OLI_USA_NV" };
                case "OH":
                    return new StateInfo() { Code = stateCode, Name = "Ohio", TC = 41, Value = "OLI_USA_OH" };
                case "OK":
                    return new StateInfo() { Code = stateCode, Name = "Oklahoma", TC = 42, Value = "OLI_USA_OK" };

                case "OR":
                    return new StateInfo() { Code = stateCode, Name = "Oregon", TC = 43, Value = "OLI_USA_OR" };
                case "PA":
                    return new StateInfo() { Code = stateCode, Name = "Pennsylvania", TC = 45, Value = "OLI_USA_PA" };
                case "RI":
                    return new StateInfo() { Code = stateCode, Name = "Rhode Island", TC = 47, Value = "OLI_USA_RI" };

                case "SC":
                    return new StateInfo() { Code = stateCode, Name = "South Carolina", TC = 48, Value = "OLI_USA_SC" };
                case "TN":
                    return new StateInfo() { Code = stateCode, Name = "Tennessee", TC = 50, Value = "OLI_USA_TN" };

                case "UT":
                    return new StateInfo() { Code = stateCode, Name = "Utah", TC = 52, Value = "OLI_USA_UT" };
                case "VA":
                    return new StateInfo() { Code = stateCode, Name = "Virginia", TC = 55, Value = "OLI_USA_VA" };
                case "WA":
                    return new StateInfo() { Code = stateCode, Name = "Washington", TC = 56, Value = "OLI_USA_WA" };

                case "WI":
                    return new StateInfo() { Code = stateCode, Name = "Wisconsin", TC = 58, Value = "OLI_USA_WI" };
                case "WV":
                    return new StateInfo() { Code = stateCode, Name = "West Virginia", TC = 57, Value = "OLI_USA_WV" };
                case "WY":
                    return new StateInfo() { Code = stateCode, Name = "Wyoming", TC = 59, Value = "OLI_USA_WY" };

                default:
                    return null;
            }
        }

        //get the state info from the given tc value
        public static StateInfo GetStateInfoFromTC(int tc)
        {
            switch (tc)
            {
                case 4:
                    return new StateInfo() { Code = "AZ", Name = "Arizona", TC = tc, Value = "OLI_USA_AZ" };
                case 12:
                    return new StateInfo() { Code = "FL", Name = "Florida", TC = tc, Value = "OLI_USA_FL" };
                case 51:
                    return new StateInfo() { Code = "TX", Name = "Texas", TC = tc, Value = "OLI_USA_TX" };

                case 1:
                    return new StateInfo() { Code = "AL", Name = "Alabama", TC = tc, Value = "OLI_USA_AL" };
                case 5:
                    return new StateInfo() { Code = "AR", Name = "Arkansas", TC = tc, Value = "OLI_USA_AR" };
                case 7:
                    return new StateInfo() { Code = "CO", Name = "Colorado", TC = tc, Value = "OLI_USA_CO" };

                case 6:
                    return new StateInfo() { Code = "CA", Name = "California", TC = tc, Value = "OLI_USA_CA" };
               
                case 13:
                    return new StateInfo() { Code = "GA", Name = "Georgia", TC = tc, Value = "OLI_USA_GA" };
                case 15:
                    return new StateInfo() { Code = "HI", Name = "Hawaii", TC = tc, Value = "OLI_USA_HI" };

                case 19:
                    return new StateInfo() { Code = "IA", Name = "Iowa", TC = tc, Value = "OLI_USA_IA" };
                case 16:
                    return new StateInfo() { Code = "ID", Name = "Idaho", TC = tc, Value = "OLI_USA_ID" };
                case 17:
                    return new StateInfo() { Code = "IL", Name = "Illinois", TC = tc, Value = "OLI_USA_IL" };

                case 18:
                    return new StateInfo() { Code = "IN", Name = "Indiana", TC = tc, Value = "OLI_USA_IN" };
                case 20:
                    return new StateInfo() { Code = "KS", Name = "Kansas", TC = tc, Value = "OLI_USA_KS" };
                case 21:
                    return new StateInfo() { Code = "KY", Name = "Kentucky", TC = tc, Value = "OLI_USA_KY" };

                case 22:
                    return new StateInfo() { Code = "LA", Name = "Louisiana", TC = tc, Value = "OLI_USA_LA" };
                case 26:
                    return new StateInfo() { Code = "MA", Name = "Massachusetts", TC = tc, Value = "OLI_USA_MA" };
                case 25:
                    return new StateInfo() { Code = "MD", Name = "Maryland", TC = tc, Value = "OLI_USA_MD" };

                case 27:
                    return new StateInfo() { Code = "MI", Name = "Michigan", TC = tc, Value = "OLI_USA_MI" };
                case 28:
                    return new StateInfo() { Code = "MN", Name = "Minnesota", TC = tc, Value = "OLI_USA_MN" };
                case 30:
                    return new StateInfo() { Code = "MO", Name = "Missouri", TC = tc, Value = "OLI_USA_MO" };

                case 29:
                    return new StateInfo() { Code = "MS", Name = "Mississippi", TC = tc, Value = "OLI_USA_TX" };
                case 38:
                    return new StateInfo() { Code = "NC", Name = "North Carolina", TC = tc, Value = "OLI_USA_AZ" };
                case 32:
                    return new StateInfo() { Code = "NE", Name = "Nebraska", TC = tc, Value = "OLI_USA_FL" };

                case 34:
                    return new StateInfo() { Code = "NH", Name = "New Hampshire", TC = tc, Value = "OLI_USA_NH" };
                case 35:
                    return new StateInfo() { Code = "NJ", Name = "New Jersey", TC = tc, Value = "OLI_USA_NJ" };
                case 36:
                    return new StateInfo() { Code = "NM", Name = "New Mexico", TC = tc, Value = "OLI_USA_NM" };

                case 33:
                    return new StateInfo() { Code = "NV", Name = "Nevada", TC = tc, Value = "OLI_USA_NV" };
                case 41:
                    return new StateInfo() { Code = "OH", Name = "Ohio", TC = tc, Value = "OLI_USA_OH" };
                case 42:
                    return new StateInfo() { Code = "OK", Name = "Oklahoma", TC = tc, Value = "OLI_USA_OK" };

                case 43:
                    return new StateInfo() { Code = "OR", Name = "Oregon", TC = tc, Value = "OLI_USA_OR" };
                case 45:
                    return new StateInfo() { Code = "PA", Name = "Pennsylvania", TC = tc, Value = "OLI_USA_PA" };
                case 47:
                    return new StateInfo() { Code = "RI", Name = "Rhode Island", TC = tc, Value = "OLI_USA_RI" };

                case 48:
                    return new StateInfo() { Code = "SC", Name = "South Carolina", TC = tc, Value = "OLI_USA_SC" };
                case 50:
                    return new StateInfo() { Code = "TN", Name = "Tennessee", TC = tc, Value = "OLI_USA_TN" };

                case 52:
                    return new StateInfo() { Code = "UT", Name = "Utah", TC = tc, Value = "OLI_USA_UT" };
                case 55:
                    return new StateInfo() { Code = "VA", Name = "Virginia", TC = tc, Value = "OLI_USA_VA" };
                case 56:
                    return new StateInfo() { Code = "WA", Name = "Washington", TC = tc, Value = "OLI_USA_WA" };

                case 58:
                    return new StateInfo() { Code = "WI", Name = "Wisconsin", TC = tc, Value = "OLI_USA_WI" };
                case 57:
                    return new StateInfo() { Code = "WV", Name = "West Virginia", TC = tc, Value = "OLI_USA_WV" };
                case 59:
                    return new StateInfo() { Code = "WY", Name = "Wyoming", TC = tc, Value = "OLI_USA_WY" };

                default:
                    return null;
            }
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
                    return new AccordOlifeValue() { TC = 2, Value = h };
                default:
                    return null;
            }
        }



    }

    public class StateInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int TC { get; set; }
        public string Value { get; set; }
    }

    public class AccordOlifeValue
    {
        public int TC { get; set; }
        public string Value { get; set; }
    }

}
