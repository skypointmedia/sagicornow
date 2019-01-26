using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SagicorNow.Models
{
    public class FirelightActivityReturn
    {
        public string ActivityId { get; set; }
        public string Id { get; set; }

        public int ResultCode { get; set; }
        public string ExceptionError { get; set; }
            
    }
}