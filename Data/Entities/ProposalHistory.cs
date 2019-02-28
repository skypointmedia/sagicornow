using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SagicorNow.Data.Entities
{
    public class ProposalHistory
    {
        public ProposalHistory()
        {
            CreatedDateTime = DateTime.Now;
            LastActiveDateTime = DateTime.Now;
        }

        [Key,StringLength(9)]
        public string SSN { get; set; }
        public string ActivityId { get; set; }
        public DateTime  CreatedDateTime { get; set; }
        public DateTime LastActiveDateTime { get; set; }
    }
}