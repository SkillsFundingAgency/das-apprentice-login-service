using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Steps
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute()
            : base(() => CreateFixture())
        {
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
            return fixture;
        }
    }
}