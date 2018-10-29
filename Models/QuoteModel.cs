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
            //new State() { Text = "Alabama", Value = "AL" },
            new State() { Text = "Arizona", Value = "AZ" },
            //new State() { Text = "Arkansas", Value = "AR" },

            //new State() { Text = "California", Value = "CA" },
            new State() { Text = "Colorado", Value = "CO" },
            new State() { Text = "Florida", Value = "FL" },

            //new State() { Text = "Georgia", Value = "GA" },
            new State() { Text = "Hawaii", Value = "HI" },
            //new State() { Text = "Idaho", Value = "ID" },

            //new State() { Text = "Illinois", Value = "IL" },

            //new State() { Text = "Indiana", Value = "IN" },
            new State() { Text = "Iowa", Value = "IA" },

            //new State() { Text = "Kansas", Value = "KS" },

            new State() { Text = "Kentucky", Value = "KY" },
            new State() { Text = "Louisiana", Value = "LA" },
            new State() { Text = "Maryland", Value = "MD" },

            //new State() { Text = "Massachusetts", Value = "MA" },

            //new State() { Text = "Michigan", Value = "MI" },
            //new State() { Text = "Minnesota", Value = "MN" },

            new State() { Text = "Mississippi", Value = "MS" },
            new State() { Text = "Missouri", Value = "MO" },

            new State() { Text = "Nebraska", Value = "NE" },

            //new State() { Text = "Nevada", Value = "NV" },
            new State() { Text = "New Hampshire", Value = "NH" },
            new State() { Text = "New Jersey", Value = "NJ" },

            new State() { Text = "New Mexico", Value = "NM" },
            new State() { Text = "North Carolina", Value = "NC" },

            new State() { Text = "Ohio", Value = "OH" },
            //new State() { Text = "Oklahoma", Value = "OK" },
            new State() { Text = "Oregon", Value = "OR" },

            //new State() { Text = "Pennsylvania", Value = "PA" },
            //new State() { Text = "Rhode Island", Value = "RI" },
            new State() { Text = "South Carolina", Value = "SC" },

            //new State() { Text = "Tennessee", Value = "TN" },
            new State() { Text = "Texas", Value = "TX" },
            new State() { Text = "Utah", Value = "UT" },

            new State() { Text = "Virginia", Value = "VA" },
            //new State() { Text = "Washington", Value = "WA" },
    
            new State() { Text = "West Virginia", Value = "WV" },       
            new State() { Text = "Wisconsin", Value = "WI" },
       
            new State() { Text = "Wyoming", Value = "WY" }

            };
        }

        //List states to restrict regarding replacement policy
        public static List<string> GetReplacementPolicyStates() {

            return new List<string>
            {
                "FL",
                "IL",
                "IA",
                "MA",
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