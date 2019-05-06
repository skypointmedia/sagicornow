using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SagicorNow.ViewModels
{
    public class RetrievePreviousQuoteViewModel
    {
        public QuoteViewModel.DisplayInfo DisplayFormat { get; set; }
        public List<string> ViewMessages { get; set; } = new List<string>();
        public string Email { get; set; }
        public string Password { get; set; }
    }
}