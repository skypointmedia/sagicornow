using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SagicorNow.Data.Entities;

namespace SagicorNow.Data
{
    public class SageNowContext: DbContext{
        public SageNowContext() : base("SagicorNow")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SageNowContext>());
        }
        public IDbSet<ProposalHistory> ProposalHistories { get; set; }
    }
}