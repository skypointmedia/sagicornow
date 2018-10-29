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
            get{
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

		public String health
		{
			get
			{
                return _health;
			}
			set
			{
                _health = value;
                this.healthInfo = GetHealthInfoFromCode(_health);
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

        public Decimal CoverageAmount { get; set; }
        public DateTime? birthday { get; set; }
        public StateInfo stateInfo { get; set; }
        public AccordOlifeValue genderInfo { get; set; }
        public AccordOlifeValue healthInfo { get; set; }
        public AccordOlifeValue smoketStatusInfo { get; set; }

        public bool? ReplacementPolicy { get; set; }
        public bool CheckReplacementPolicy { get; set; }
        public bool replacementPolicyVisible { get; set; }

        private List<string> _viewMessages = new List<string>();
        public List<string> ViewMessages { get{ return _viewMessages; } set{ _viewMessages = value; } }

        public List<State> States = QuoteModel.States();
        public List<Gender> Genders = QuoteModel.Genders();
        public List<SmokerStatus> SmokerStatuses = QuoteModel.SmokerStatuses();
        public List<HealthStatus> HealthStatuses = QuoteModel.HealthStatuses();


        //get the state info from the given state code
        private StateInfo GetStateInfoFromCode(string stateCode)
        {
            switch(stateCode)
            {
                case "AZ":
                    return new StateInfo() { Code = stateCode, Name = "Arizona", TC = 4, Value = "OLI_USA_AZ" };
                case "FL":
                    return new StateInfo() { Code = stateCode, Name = "Florida", TC = 12, Value = "OLI_USA_FL" };
				case "TX":
					return new StateInfo() { Code = stateCode, Name = "Texas", TC = 51, Value = "OLI_USA_TX" };
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

		private AccordOlifeValue GetHealthInfoFromCode(string h)
		{
			switch (h)
			{
				case "OLI_UNWRITE_STANDARD":
                    return new AccordOlifeValue() { TC = 3, Value = h };
				case "OLI_UNWRITE_PREFERRED":
                    return new AccordOlifeValue() { TC = 1, Value = h };
				case "OLI_UNWRITE_RATED":
					return new AccordOlifeValue() { TC = 6, Value = h };
				default:
					return null;
			}
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
