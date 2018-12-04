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
            new State() { Text = "Alabama", Value = "AL",  TC = 1, TCValue = "OLI_USA_AL" },
            new State() { Text = "Arizona", Value = "AZ",TC = 4, TCValue = "OLI_USA_AZ" },
            new State() { Text = "Arkansas", Value = "AR", TC = 5, TCValue = "OLI_USA_AR" },

            new State() { Text = "California", Value = "CA", TC = 6, TCValue = "OLI_USA_CA" },
            new State() { Text = "Colorado", Value = "CO", TC = 7, TCValue = "OLI_USA_CO" },

            new State() { Text = "Delaware", Value = "DE", TC = 9, TCValue = "OLI_USA_DE" },
            new State() { Text = "District of Columbia", Value = "DC", TC = 10, TCValue = "OLI_USA_DC" },

            new State() { Text = "Florida", Value = "FL", TC = 12, TCValue = "OLI_USA_FL" },

            new State() { Text = "Georgia", Value = "GA", TC = 13, TCValue = "OLI_USA_GA" },
            new State() { Text = "Hawaii", Value = "HI", TC = 15, TCValue = "OLI_USA_HI" },

            new State() { Text = "Idaho", Value = "ID", TC = 16, TCValue = "OLI_USA_ID" },
            new State() { Text = "Illinois", Value = "IL", TC = 17, TCValue = "OLI_USA_IL" },
            new State() { Text = "Indiana", Value = "IN", TC = 18, TCValue = "OLI_USA_IN" },
            new State() { Text = "Iowa", Value = "IA", TC = 19, TCValue = "OLI_USA_IA" },

            //new State() { Text = "Kansas", Value = "KS" },

            new State() { Text = "Kentucky", Value = "KY", TC = 21, TCValue = "OLI_USA_KY" },
            new State() { Text = "Louisiana", Value = "LA", TC = 22, TCValue = "OLI_USA_LA" },

            new State() { Text = "Maryland", Value = "MD", TC = 25, TCValue = "OLI_USA_MD" },
            new State() { Text = "Massachusetts", Value = "MA", TC = 26, TCValue = "OLI_USA_MA" },
            new State() { Text = "Michigan", Value = "MI", TC = 27, TCValue = "OLI_USA_MI" },
            new State() { Text = "Minnesota", Value = "MN", TC = 28, TCValue = "OLI_USA_MN" },

            new State() { Text = "Mississippi", Value = "MS", TC = 29, TCValue = "OLI_USA_MS" },
            new State() { Text = "Missouri", Value = "MO", TC = 30, TCValue = "OLI_USA_MO" },
            new State() { Text = "Montana", Value = "MT", TC = 31, TCValue = "OLI_USA_MT" },
            new State() { Text = "Nebraska", Value = "NE", TC = 32, TCValue = "OLI_USA_NE" },

            new State() { Text = "Nevada", Value = "NV", TC = 33, TCValue = "OLI_USA_NV" },
            new State() { Text = "New Hampshire", Value = "NH", TC = 34, TCValue = "OLI_USA_NH" },
            new State() { Text = "New Jersey", Value = "NJ", TC = 35, TCValue = "OLI_USA_NJ" },

            new State() { Text = "New Mexico", Value = "NM", TC = 36, TCValue = "OLI_USA_NM" },
            new State() { Text = "North Carolina", Value = "NC", TC = 38, TCValue = "OLI_USA_NC" },
            new State() { Text = "North Dakota", Value = "ND", TC = 39, TCValue = "OLI_USA_ND" },

            new State() { Text = "Ohio", Value = "OH", TC = 41, TCValue = "OLI_USA_OH" },
            new State() { Text = "Oklahoma", Value = "OK", TC = 42, TCValue = "OLI_USA_OK" },
            new State() { Text = "Oregon", Value = "OR", TC = 43, TCValue = "OLI_USA_OR" },

            new State() { Text = "Pennsylvania", Value = "PA", TC = 45, TCValue = "OLI_USA_PA" },
            new State() { Text = "Rhode Island", Value = "RI", TC = 47, TCValue = "OLI_USA_RI" },
            new State() { Text = "South Carolina", Value = "SC", TC = 48, TCValue = "OLI_USA_SC" },
            new State() { Text = "South Dakota", Value = "SD", TC = 49, TCValue = "OLI_USA_SD" },

            new State() { Text = "Tennessee", Value = "TN", TC = 50, TCValue = "OLI_USA_TN" },
            new State() { Text = "Texas", Value = "TX", TC = 51, TCValue = "OLI_USA_TX" },
            new State() { Text = "Utah", Value = "UT", TC = 52, TCValue = "OLI_USA_UT" },

            new State() { Text = "Virginia", Value = "VA", TC = 55, TCValue = "OLI_USA_VA" },

            new State() { Text = "Washington", Value = "WA", TC = 56, TCValue = "OLI_USA_WA" },
            new State() { Text = "West Virginia", Value = "WV", TC = 57, TCValue = "OLI_USA_WV" },
            new State() { Text = "Wisconsin", Value = "WI", TC = 58, TCValue = "OLI_USA_WI" },
            new State() { Text = "Wyoming", Value = "WY", TC = 59, TCValue = "OLI_USA_WY" }

            };
        }

        //List states to restrict regarding replacement policy
        public static List<string> GetReplacementPolicyStates()
        {

            return new List<string>
            {
                "FL",
                "IL",
                "IN",
                "NV",
                "WA",
                "WY"
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
                new SmokerStatus() { Text = "No (not in the last 24 months)", Value = "OLI_TOBACCO_NEVER" },
                //new SmokerStatus() { Text = "Prior", Value = "OLI_TOBACCO_PRIOR" },
                new SmokerStatus() { Text = "Yes (in the last 24 months)", Value = "OLI_TOBACCO_CURRENT" }
            };
        }

        //get all Health Statuses
        public static List<HealthStatus> HealthStatuses()
        {
            return new List<HealthStatus>() {
                new HealthStatus() { Text = "", Value = "" },
                new HealthStatus() { Text = "Superb", Value = "OLI_UNWRITE_SUPERB" },
                new HealthStatus() { Text = "Excellent", Value = "OLI_UNWRITE_PREFERRED" },
                new HealthStatus() { Text = "Good", Value = "OLI_UNWRITE_STANDARD" },
                new HealthStatus() { Text = "Fair", Value = "OLI_UNWRITE_RATED" },
                new HealthStatus() { Text = "Poor", Value = "OLI_UNWRITE_POOR" },
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
        public int TC { get; set; }
        public string TCValue { get; set; }
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