using System;
using System.Collections.Generic;
namespace SagicorNow.Models
{
    public class QuoteModel
    {
        public QuoteModel()
        {
        }

        //get all states
        public static List<State> States()
        {
            return new List<State>() {
                new State() { Text = "", Value = "" },
                new State() { Text = "Arizona", Value = "AZ" },
                new State() { Text = "Florida", Value = "FL" },
                new State() { Text = "Texas", Value = "TX" }
            };
        }

		//get all Genders
		public static List<Gender> Genders()
		{
			return new List<Gender>() {
                new Gender() { Text = "", Value = "" },
				new Gender() { Text = "Male", Value = "OLI_GENDER_MALE" },
				new Gender() { Text = "Female", Value = "OLI_GENDER_FEMALE" }
			};
		}


		//get all Smoker Statuses
        public static List<SmokerStatus> SmokerStatuses()
		{
			return new List<SmokerStatus>() {
                new SmokerStatus() { Text = "", Value = "" },
				new SmokerStatus() { Text = "No", Value = "OLI_TOBACCO_NEVER" },
				//new SmokerStatus() { Text = "Prior", Value = "OLI_TOBACCO_PRIOR" },
				new SmokerStatus() { Text = "Yes", Value = "OLI_TOBACCO_CURRENT" }
			};
		}

		//get all Health Statuses
        public static List<HealthStatus> HealthStatuses()
		{
			return new List<HealthStatus>() {
                new HealthStatus() { Text = "", Value = "" },
				new HealthStatus() { Text = "Excellent", Value = "OLI_UNWRITE_PREFERRED" },
				new HealthStatus() { Text = "Good", Value = "OLI_UNWRITE_STANDARD" },
				new HealthStatus() { Text = "Fair", Value = "OLI_UNWRITE_RATED" }
			};
		}

        public static List<FamilyMakeup> FamilyMakeupOptions()
		{
            return new List<FamilyMakeup>() {
				new FamilyMakeup() { Text = "", Value = "" },
				new FamilyMakeup() { Text = "Me", Value = "ME" },
				new FamilyMakeup() { Text = "Me, My Partner", Value = "ME_PARTNER" },
				new FamilyMakeup() { Text = "Me, My Kids", Value = "ME_MYKIDS" },
                new FamilyMakeup() { Text = "Me, My Partner and Kids", Value = "ME_PARTNER_MYKIDS" }
			};
		}

		public static List<SchoolType> SchoolTypes()
		{
			return new List<SchoolType>() {
				new SchoolType() { Text = "", Value = "" },
				new SchoolType() { Text = "Public", Value = "PUBLIC" },
				new SchoolType() { Text = "Private", Value = "PRIVATE" }
			};
		}
    }

    public class State
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

	public class Gender
	{
		public string Text { get; set; }
		public string Value { get; set; }
	}

	public class SmokerStatus
	{
		public string Text { get; set; }
		public string Value { get; set; }
	}

	public class HealthStatus
	{
		public string Text { get; set; }
		public string Value { get; set; }
	}
    
    public class FamilyMakeup
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

	public class SchoolType
	{
		public string Text { get; set; }
		public string Value { get; set; }
	}
}
