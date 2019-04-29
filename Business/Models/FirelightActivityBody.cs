using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SagicorNow.Models
{
    public class FirelightActivityBody
    {
        public string Id { get; set; }
        public string CUSIP { get; set; }
        public int Jurisdiction { get; set; }
        public int TransactionType { get; set; }
        public string CarrierCode { get; set; }

        public List<FirelightActivityDataItem> DataItems { get; set; }
    }
}