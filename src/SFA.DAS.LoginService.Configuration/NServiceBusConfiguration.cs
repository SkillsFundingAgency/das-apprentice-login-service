namespace SFA.DAS.LoginService.Configuration
{
    public class NServiceBusConfiguration
    {
        public string SharedServiceBusEndpointUrl { get; set; }

        public string NServiceBusLicense
        {
            get => _nServiceBusLicense;
            set => _nServiceBusLicense = System.Net.WebUtility.HtmlDecode(value);
        }

        private string _nServiceBusLicense;
    }
}