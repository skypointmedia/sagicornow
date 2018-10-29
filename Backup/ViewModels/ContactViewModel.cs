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
    }
}
