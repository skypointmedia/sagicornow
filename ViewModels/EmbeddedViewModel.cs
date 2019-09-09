using System.Collections.Generic;

namespace SagicorNow.ViewModels
{
    public class EmbeddedViewModel
    {
        public EmbeddedViewModel()
        {
            ViewMessages = new List<string>();
        }
        public string AccessToken { get; set; }

        public bool IsNew { get; set; }

        public string ActivityId { get; set; }

        public string FirelightBaseUrl { get; set; }

        public string EAppScriptsUrl
        {
            get
            {
                return $"{FirelightBaseUrl}scripts/eApp";
            }
        }

        public List<string> ViewMessages { get; set; }
        public decimal FaceAmount { get; set; }
        public decimal Quote { get; set; }
    }
}