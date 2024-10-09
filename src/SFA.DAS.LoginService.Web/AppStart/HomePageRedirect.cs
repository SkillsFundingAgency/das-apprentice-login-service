namespace SFA.DAS.LoginService.Web.AppStart
{
    public class HomePageRedirect
    {
        private readonly string _environment;

        public HomePageRedirect(string environment)
        {
            _environment = environment;
        }
        public string HomePage()
        {
            return _environment.ToLower() switch
            {
                "local" => "",
                "prd" => "https://my.apprenticeships.education.gov.uk",
                _ => $"https://{_environment.ToLower()}-aas.apprenticeships.education.gov.uk"
            };
        }
    }   
}