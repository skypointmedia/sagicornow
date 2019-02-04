using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SagicorNow.Models
{
    public class FirelightActivityDataItem
    {
        public string DataItemId { get; set; }
        public string Value { get; set; }
        public string OriginalValue { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool CustomActionClicked { get; set; }
    }
}