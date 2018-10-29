using System;
namespace SagicorNow.Models
{
    public class ContactModel
    {
        public ContactModel()
        {
        }

        public string firstName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string state { get; set; }
        public string denialMessage { get; set; }
    }
}
