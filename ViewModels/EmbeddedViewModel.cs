namespace SagicorNow.ViewModels
{
    public class EmbeddedViewModel
    {
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

    }
}