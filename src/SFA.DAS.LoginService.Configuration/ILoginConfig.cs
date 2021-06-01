namespace SFA.DAS.LoginService.Configuration
{
    public interface ILoginConfig
    {
        string BaseUrl { get; set; }
        string SqlConnectionString { get; set; }
        int PasswordResetExpiryInHours { get; set; }
        string CertificateThumbprint { get; set; }
        NotificationsApiConfiguration NotificationsApiClientConfiguration { get; set; }
        int MaxFailedAccessAttempts { get; set; }
        int DaysInvitationIsValidFor { get; set; }
        NServiceBusConfiguration NServiceBusConfiguration { get; set; }
        ApprenticeLoginApiConfiguration ApprenticeLoginApi { get; set; }
    }
}