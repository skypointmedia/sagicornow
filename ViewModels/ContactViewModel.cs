using System;
namespace SagicorNow.ViewModels
{
    public class ContactViewModel
    {
        //constructor
        public ContactViewModel()
        {
            
        }

        //properties
        public string DenialMessage { get; set; }
        public bool IsReplacementReject { get; set; }
        public string State { get; set; }

        public string email { get; set; }
        public string firstName { get; set; }
        public string phone { get; set; }
    }
}
