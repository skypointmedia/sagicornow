using System;
namespace SagicorNow.Models
{
    public class EligibilityInfo
    {
        public EligibilityInfo()
        {
            
        }

        //properties
		public bool IsEligible { get; set; }
		public string EligibilityMessage { get; set; }
        public Boolean IsReplacememtReject { get; set; }
        public string State { get; set; }
        public int Age { get; set; }
    }
}
